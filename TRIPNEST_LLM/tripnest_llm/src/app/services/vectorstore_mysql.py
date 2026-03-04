# src/app/services/vectorstore_mysql.py
from dotenv import load_dotenv
load_dotenv()   # load .env in project root

import os
import json
import mysql.connector
import numpy as np
import faiss
from datetime import datetime

# --- Config from env ---
MYSQL_HOST = os.getenv("MYSQL_HOST", "localhost")
MYSQL_PORT = int(os.getenv("MYSQL_PORT", 3306))
MYSQL_USER = os.getenv("MYSQL_USER", "root")
MYSQL_PASSWORD = os.getenv("MYSQL_PASSWORD", "")   # must be non-empty
MYSQL_DB = os.getenv("MYSQL_DB", "tripnest")

# embed dim from env (fallback to 768 if unspecified)
EMBED_DIM = int(os.getenv("EMBED_DIM", 768))

# FAISS index and id order cached in memory
_FAISS_INDEX = None
_ID_ORDER = []  # list of item_id in same order as index vectors

# --- Helpers ---
def get_conn():
    """
    Returns a new mysql.connector connection.
    Raises helpful error if password missing or connection fails.
    """
    if not MYSQL_PASSWORD:
        raise RuntimeError("MYSQL_PASSWORD is empty — please set it in your environment or .env file.")
    try:
        conn = mysql.connector.connect(
            host=MYSQL_HOST, port=MYSQL_PORT, user=MYSQL_USER, password=MYSQL_PASSWORD, database=MYSQL_DB
        )
        return conn
    except mysql.connector.Error as e:
        raise RuntimeError(f"MySQL connection error: {e}")

def upsert(items):
    """
    items: list of dicts: {item_type, item_id, snippet, metadata, embedding(list[float]), vector_ref(optional)}
    Stores embedding binary into vector_blob, dim into dim column.
    """
    if not items:
        return
    conn = get_conn()
    cur = conn.cursor()
    try:
        for it in items:
            item_type = it.get("item_type", "default")
            item_id = it["item_id"]
            snippet = it.get("snippet", "")
            metadata = json.dumps(it.get("metadata", {}), ensure_ascii=False)
            emb = np.array(it["embedding"], dtype=np.float32)
            blob = emb.tobytes()
            dim = emb.shape[0]
            now = datetime.utcnow().strftime("%Y-%m-%d %H:%M:%S")
            cur.execute(
                """
                INSERT INTO embeddings (item_type, item_id, vector_ref, vector_blob, dim, updated_at, snippet, metadata)
                VALUES (%s, %s, %s, %s, %s, %s, %s, %s)
                ON DUPLICATE KEY UPDATE vector_ref=VALUES(vector_ref), vector_blob=VALUES(vector_blob), dim=VALUES(dim),
                                        updated_at=VALUES(updated_at), snippet=VALUES(snippet), metadata=VALUES(metadata)
                """,
                (item_type, item_id, it.get("vector_ref"), blob, dim, now, snippet, metadata)
            )
        conn.commit()
    finally:
        cur.close()
        conn.close()

    # after upsert, rebuild FAISS index (or schedule incremental rebuild in production)
    rebuild_index()

def load_all_embeddings_from_db():
    """
    Loads all embeddings from DB.
    Returns (ids_list, numpy_array_of_vectors) or ([], None) if empty.
    """
    conn = get_conn()
    cur = conn.cursor()
    try:
        cur.execute("SELECT item_id, vector_blob, dim FROM embeddings")
        rows = cur.fetchall()
    finally:
        cur.close()
        conn.close()

    ids = []
    vecs = []
    for item_id, blob, dim in rows:
        if blob is None:
            continue
        arr = np.frombuffer(blob, dtype=np.float32)
        if dim is not None and arr.size != int(dim):
            # skip inconsistent vector and log
            print(f"[vectorstore_mysql] dimension mismatch for {item_id}: db dim={dim} arr_size={arr.size}. Skipping.")
            continue
        ids.append(item_id)
        vecs.append(arr)
    if not vecs:
        return [], None
    xb = np.vstack(vecs).astype('float32')
    return ids, xb

def rebuild_index():
    """
    Rebuilds FAISS index from DB embeddings. Called after upserts or on startup.
    """
    global _FAISS_INDEX, _ID_ORDER
    ids, xb = load_all_embeddings_from_db()
    if xb is None or xb.size == 0:
        _FAISS_INDEX = None
        _ID_ORDER = []
        print("[vectorstore_mysql] No vectors found to build FAISS index.")
        return
    # Normalize for cosine similarity
    faiss.normalize_L2(xb)
    dim = xb.shape[1]
    # Use IndexFlatIP (inner product on normalized vectors -> cosine)
    index = faiss.IndexFlatIP(dim)
    index.add(xb)
    _FAISS_INDEX = index
    _ID_ORDER = ids
    print(f"[vectorstore_mysql] FAISS index built: {len(_ID_ORDER)} vectors dim={dim}")

def query(vector, top_k=5):
    """
    Query with vector (list[float]) -> returns list of {item_id, score, snippet, metadata}
    """
    global _FAISS_INDEX, _ID_ORDER
    if _FAISS_INDEX is None:
        rebuild_index()
        if _FAISS_INDEX is None:
            return []
    q = np.array([vector], dtype=np.float32)
    faiss.normalize_L2(q)
    D, I = _FAISS_INDEX.search(q, top_k)
    idxs = I[0].tolist()
    scores = D[0].tolist()
    results = []
    # map indices -> ids
    selected_ids = [ _ID_ORDER[i] for i in idxs if i != -1 ]
    if not selected_ids:
        return []
    # fetch snippet/metadata for selected ids
    conn = get_conn()
    cur = conn.cursor()
    try:
        format_strings = ",".join(["%s"] * len(selected_ids))
        cur.execute(f"SELECT item_id, snippet, metadata FROM embeddings WHERE item_id IN ({format_strings})", tuple(selected_ids))
        rows = cur.fetchall()
    finally:
        cur.close()
        conn.close()

    row_map = { r[0]: (r[1], json.loads(r[2]) if r[2] else {}) for r in rows }
    # preserve order of selected_ids and map scores
    for i, idx in enumerate(idxs):
        if idx == -1:
            continue
        _id = _ID_ORDER[idx]
        snippet, metadata = row_map.get(_id, ("", {}))
        results.append({"item_id": _id, "score": float(scores[i]), "snippet": snippet, "metadata": metadata})
    return results
