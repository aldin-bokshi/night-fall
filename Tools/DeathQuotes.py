import json
from pathlib import Path

BASE = Path(__file__).parent

source = BASE / "DeathQuotes.cs"
output = BASE / "DeathQuotes.json"

text = source.read_text(encoding="utf-8")

# Find the actual array assignment, not the DeathQuotes class name.
assignment = text.find("Quotes =")

if assignment == -1:
    raise Exception("Could not find 'Quotes =' in DeathQuotes.cs")

# Find the [ that comes after "Quotes ="
array_start = text.find("[", assignment)

if array_start == -1:
    raise Exception("Could not find the opening '[' of the Quotes array")

# Find the matching ]
depth = 0
array_end = None

for i in range(array_start, len(text)):
    if text[i] == "[":
        depth += 1

    elif text[i] == "]":
        depth -= 1

        if depth == 0:
            array_end = i
            break

if array_end is None:
    raise Exception("Could not find the closing ']' of the Quotes array")

array_text = text[array_start + 1:array_end]

# Extract every quoted string.
quotes = []
i = 0

while i < len(array_text):

    if array_text[i] == '"':
        i += 1
        quote = ""

        while i < len(array_text):

            if array_text[i] == "\\" and i + 1 < len(array_text):
                quote += array_text[i]
                quote += array_text[i + 1]
                i += 2
                continue

            if array_text[i] == '"':
                i += 1
                break

            quote += array_text[i]
            i += 1

        quotes.append(quote)

    else:
        i += 1

if not quotes:
    raise Exception("Found the Quotes array, but extracted 0 quotes.")

output.write_text(
    json.dumps(
        {"quotes": quotes},
        ensure_ascii=False,
        indent=4
    ),
    encoding="utf-8"
)

print(f"Successfully converted {len(quotes)} quotes.")
print(f"Created: {output}")