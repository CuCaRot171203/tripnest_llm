# src/app/services/maps_client.py
from dotenv import load_dotenv
load_dotenv()

import os
import httpx
from typing import Dict, Any, Optional

NOMINATIM_URL = os.getenv("NOMINATIM_URL", "https://nominatim.openstreetmap.org")
OVERPASS_URL = os.getenv("OVERPASS_URL", "https://overpass-api.de/api/interpreter")
ORS_API_KEY = os.getenv("ORS_API_KEY")  # set in .env

# --- Nominatim geocode (dev use only) ---
async def geocode_nominatim(address: str) -> Dict[str, Any]:
    params = {"q": address, "format": "jsonv2", "limit": 1}
    headers = {"User-Agent": "tripnest-llm/1.0"}
    async with httpx.AsyncClient(timeout=15.0) as client:
        r = await client.get(f"{NOMINATIM_URL}/search", params=params, headers=headers)
        r.raise_for_status()
        data = r.json()
    if not data:
        raise RuntimeError("Nominatim: no geocode result")
    first = data[0]
    return {"lat": float(first["lat"]), "lng": float(first["lon"]), "display_name": first.get("display_name")}

# --- Directions via OpenRouteService ---
async def directions_ors(origin: str, destination: str, profile: str = "foot-walking") -> Dict[str, Any]:
    """
    origin/destination: can be "lat,lng" or address string.
    profile options: driving-car, cycling-regular, foot-walking, etc.
    """
    if ORS_API_KEY is None:
        return {"status": "NO_KEY", "error_message": "ORS_API_KEY not set", "routes": []}

    def is_coord(s: str):
        parts = [p.strip() for p in s.split(",")]
        if len(parts) < 2:
            return False
        try:
            float(parts[0]); float(parts[1])
            return True
        except:
            return False

    # geocode if needed (Nominatim)
    if not is_coord(origin):
        g = await geocode_nominatim(origin)
        origin_coord = [g["lng"], g["lat"]]
    else:
        lng, lat = origin.split(",", 1)
        origin_coord = [float(lng.strip()), float(lat.strip())]

    if not is_coord(destination):
        g = await geocode_nominatim(destination)
        dest_coord = [g["lng"], g["lat"]]
    else:
        lng, lat = destination.split(",", 1)
        dest_coord = [float(lng.strip()), float(lat.strip())]

    payload = {
        "coordinates": [origin_coord, dest_coord]
    }
    headers = {"Authorization": ORS_API_KEY, "Content-Type": "application/json"}
    url = f"https://api.openrouteservice.org/v2/directions/{profile}/geojson"

    async with httpx.AsyncClient(timeout=30.0) as client:
        r = await client.post(url, headers=headers, json=payload)
        # return structured error instead of raising so API can include it
        try:
            data = r.json()
        except Exception:
            return {"status": "ERROR", "error_message": f"ORS returned non-json: {r.text}", "routes": []}

    if r.status_code != 200 or "features" not in data:
        return {"status": "ERROR", "error_message": data, "routes": []}

    feat = data["features"][0]
    props = feat.get("properties", {})
    # extract summary
    seg = props.get("segments", [])
    distance_m = seg[0].get("distance") if seg else None
    duration_s = seg[0].get("duration") if seg else None
    distance_km = round(distance_m / 1000.0, 2) if distance_m else None
    duration_min = round(duration_s / 60.0) if duration_s else None

    return {"status": "OK", "routes": [feat], "distance_km": distance_km, "duration_min": duration_min}
