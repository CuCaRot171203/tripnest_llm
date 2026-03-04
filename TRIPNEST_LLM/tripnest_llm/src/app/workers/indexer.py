# src/app/workers/indexer.py
from dotenv import load_dotenv
load_dotenv()

import os
import json
from pathlib import Path
from ..services.embeddings import embed_text
from ..services import vectorstore_mysql

BASE = Path(__file__).resolve().parents[2]
DATA_FILE = BASE / "data" / "seed_docs.json"  # REPLACE_ME: path to your seed data

def load_docs():
    if not DATA_FILE.exists():
        raise FileNotFoundError(f"Seed data file not found: {DATA_FILE}. Create a JSON list of docs.")
    with open(DATA_FILE, "r", encoding="utf-8") as f:
        return json.load(f)  # expecting list of {"id": "...", "text": "...", "metadata": {...}, "type": "poi"}

def index_docs():
    docs = load_docs()
    items = []
    for d in docs:
        text = d.get("text") or d.get("snippet") or d.get("description","")
        if not text:
            print(f"[indexer] skipping doc id={d.get('id')} due to empty text")
            continue
        try:
            emb = embed_text(text)
        except Exception as e:
            print(f"[indexer] embed error for id={d.get('id')}: {e}")
            continue
        items.append({
            "item_type": d.get("type","default"),
            "item_id": d["id"],
            "snippet": text[:800],
            "metadata": d.get("metadata", {}),
            "embedding": emb,
            "vector_ref": d.get("vector_ref")
        })
    if not items:
        print("[indexer] No items to index.")
        return
    # upsert into MySQL embeddings table and rebuild FAISS index
    try:
        vectorstore_mysql.upsert(items)
        print(f"[indexer] Indexed {len(items)} docs into MySQL & FAISS")
    except Exception as e:
        print(f"[indexer] upsert error: {e}")
        raise

if __name__ == "__main__":
    index_docs()
