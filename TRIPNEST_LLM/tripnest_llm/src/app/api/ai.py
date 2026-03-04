from fastapi import APIRouter, HTTPException
from pydantic import BaseModel
from ..services.embeddings import embed_text
from ..services import llm
from ..services import vectorstore_mysql
from ..services import maps_client  # using OpenRouteService + Nominatim client
import os
import asyncio

router = APIRouter(prefix="/api/ai", tags=["ai"])

class RouteRequest(BaseModel):
    origin: str | None = None
    destination: str | None = None
    user_query: str

@router.post("/route")
async def route(req: RouteRequest):
    """
    Endpoint flow:
    1) optionally call maps (ORS) to get route summary
    2) embed user query (Gemini embedding)
    3) semantic search via MySQL+FAISS
    4) build prompt using retrieved context + maps info
    5) call LLM to generate guidance
    """

    # 1) Try calling maps (OpenRouteService). Use directions_ors (async).
    maps_info = {"status": "NO_REQUEST", "routes": []}
    if req.origin and req.destination:
        try:
            # NOTE: maps_client.directions_ors returns a dict like:
            # {"status":"OK", "routes":[...], "distance_km":..., "duration_min":...}
            maps_info = await maps_client.directions_ors(req.origin, req.destination, profile="foot-walking")
        except Exception as e:
            # don't fail the whole endpoint for maps errors; include error message for visibility
            maps_info = {"status": "ERROR", "error_message": str(e), "routes": []}

    # 2) embed user query using your embeddings service (Gemini embedding)
    try:
        q_vec = embed_text(req.user_query)
    except Exception as e:
        # embedding failed (API key / model issue), return helpful error
        raise HTTPException(status_code=500, detail=f"Embedding error: {e}")

    # 3) semantic search via MySQL + FAISS
    try:
        matches = vectorstore_mysql.query(q_vec, top_k=5)
    except Exception as e:
        # if vectorstore has problem, continue but set matches empty
        matches = []
        # you may want to log exception in real app
        # print("vectorstore query error:", e)

    # Build context snippets (dedupe and limit)
    seen = set()
    context_snippets = []
    for m in matches:
        s = m.get("snippet", "")
        if not s:
            continue
        if s in seen:
            continue
        seen.add(s)
        context_snippets.append(s)
        if len(context_snippets) >= 6:
            break

    # 4) build prompt for LLM
    context_text = "\n".join([f"- {s}" for s in context_snippets]) if context_snippets else "No retrieved context available."
    maps_text = maps_info if isinstance(maps_info, dict) else {"value": str(maps_info)}

    prompt = f"""You are a helpful travel assistant that gives safe walking/driving guidance.

User query:
{req.user_query}

Retrieved context (top {len(context_snippets)}):
{context_text}

Maps info (from routing service):
{maps_text}

Please produce:
1) Step-by-step guidance/directions (concise, numbered).
2) Safety tips specific to the route (lighting, stairs, busy intersections).
3) Estimated travel time/distance (use maps info if available).
4) One short alternative route if applicable.
Keep the answer in Vietnamese if the user query is Vietnamese; otherwise use user's language.
"""

    # 5) call LLM (wrap in try/except)
    try:
        guidance = llm.generate_guidance(prompt)
    except Exception as e:
        # LLM failure: return helpful debug info
        raise HTTPException(status_code=500, detail=f"LLM error: {e}")

    # final response
    return {"guidance": guidance, "retrieved": matches, "maps": maps_info}
