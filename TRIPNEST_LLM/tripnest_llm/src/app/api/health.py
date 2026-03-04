# health.py
from fastapi import APIRouter

router = APIRouter(prefix="/api/health")

@router.get("/ping")
def ping():
    return {"ping": "pong"}