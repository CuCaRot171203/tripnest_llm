import os
from fastapi import FastAPI
from .api import ai, health
from dotenv import load_dotenv

load_dotenv()  # loads .env

app = FastAPI(title="LLM Directions API")

app.include_router(health.router)
app.include_router(ai.router)

@app.get("/")
def root():
    return {"status": "ok", "service": "llm-directions"}