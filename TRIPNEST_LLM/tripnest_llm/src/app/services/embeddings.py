# src/app/services/embeddings.py
import os
import google.generativeai as genai

# configure (reads GOOGLE_API_KEY from env)
genai.configure(api_key=os.getenv("GOOGLE_API_KEY"))

# default model; override with env var if needed
EMBED_MODEL = os.getenv("EMBED_MODEL", "models/text-embedding-004")

def embed_text(text: str):
    """
    Returns list[float] embedding using google-generativeai v0.8.5 embed_content.
    """
    # embed_content expects content (string or list)
    resp = genai.embed_content(model=EMBED_MODEL, content=text)
    # resp shape in 0.8.5: resp is dict-like; embeddings usually at resp["embedding"] or resp.data[0].embedding
    # try common shapes:
    if isinstance(resp, dict) and "embedding" in resp:
        return resp["embedding"]
    # fallback: resp.data[0].embedding
    try:
        return resp.data[0].embedding
    except Exception:
        # inspect resp if unexpected
        raise RuntimeError(f"Unexpected embed response shape: {resp}")
