# src/app/services/llm.py
import os
import google.generativeai as genai

# đọc API key từ env
genai.configure(api_key=os.getenv("GOOGLE_API_KEY"))

# REPLACE_ME: thay model theo list_models output nếu muốn
MODEL_NAME = os.getenv("LLM_MODEL", "models/gemini-2.5-pro")

# instantiate model once
_model = genai.GenerativeModel(MODEL_NAME)

def generate_guidance(prompt: str):
    """
    Call Gemini's generate_content using google-generativeai v0.8.5 API.
    Returns text string (resp.text) or raw resp if text not present.
    """
    resp = _model.generate_content(prompt)   # NOTE: do NOT pass max_output_tokens here on v0.8.5
    # preferred extraction
    if hasattr(resp, "text") and resp.text:
        return resp.text
    # fallback to common nested shapes
    try:
        return resp.output[0].content[0].text
    except Exception:
        return str(resp)
