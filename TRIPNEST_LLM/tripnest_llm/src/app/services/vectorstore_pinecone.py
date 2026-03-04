# vectorstore_pinecone.py
import os
# CHOOSE: install pinecone-client if using Pinecone
# pip install pinecone-client

# REPLACE_ME: set PINECONE_API_KEY & env in .env
from pinecone import PineconeClient
PINECONE_API_KEY = os.getenv("PINECONE_API_KEY")
PINECONE_ENV = os.getenv("PINECONE_ENVIRONMENT")
INDEX_NAME = os.getenv("PINECONE_INDEX_NAME", "routes-index")

pinecone = PineconeClient(api_key=PINECONE_API_KEY, environment=PINECONE_ENV)

def ensure_index(dim: int = 1536):
    existing = [i.name for i in pinecone.list_indexes()]
    if INDEX_NAME not in existing:
        pinecone.create_index(INDEX_NAME, dimension=dim)
    return pinecone.Index(INDEX_NAME)

def upsert(items: list):
    idx = ensure_index(len(items[0]["embedding"]))
    idx.upsert(vectors=[{"id": it["id"], "values": it["embedding"], "metadata": {"snippet": it["snippet"], **it.get("metadata",{})}} for it in items])

def query(vector: list, top_k: int = 5):
    idx = ensure_index(len(vector))
    res = idx.query(vector=vector, top_k=top_k, include_metadata=True)
    matches = []
    for m in res.matches:
        matches.append({
            "id": m.id,
            "snippet": m.metadata.get("snippet"),
            "metadata": m.metadata,
            "score": m.score
        })
    return matches