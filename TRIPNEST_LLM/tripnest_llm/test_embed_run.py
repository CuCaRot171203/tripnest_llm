# test_embed_run.py
from src.app.services.embeddings import embed_text
import os

print("EMBED_MODEL:", os.getenv("EMBED_MODEL"))
v = embed_text("Hello world from Tripnest test")
print("embedding len:", len(v))
print("first 5 values:", v[:5])
