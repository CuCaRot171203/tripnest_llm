# test_generate_run.py
from src.app.services.llm import generate_guidance
import os

print("LLM_MODEL:", os.getenv("LLM_MODEL"))
out = generate_guidance("Write one friendly sentence greeting the user.")
print("LLM output:\n", out)
