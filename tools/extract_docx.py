import sys
from pathlib import Path

try:
    from docx import Document
except Exception as e:
    print("MISSING_LIB", e)
    sys.exit(2)

docx_path = Path(r"c:/Users/oswal/Downloads/AgendaComunicacion/formas base/transcript.docx")

if not docx_path.exists():
    print(f"NOT_FOUND: {docx_path}")
    sys.exit(1)

doc = Document(docx_path)
texts = []
for para in doc.paragraphs:
    texts.append(para.text)

full = "\n".join(texts)
print(full)
