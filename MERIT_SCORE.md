# 🎯 MERIT_SCORE.md  
### How Content is Evaluated on Meritocious

## 🧠 What Is a Merit Score?

The **Merit Score** is a composite evaluation assigned to every post, comment, or thread on the Meritocious platform.  
It reflects how much a contribution **advances the conversation** based on clarity, originality, relevance, civility, and constructive value.

It is **not** a popularity metric.  
It is **not** based on karma, engagement volume, or user reputation.  
It is **not** fixed—it evolves as the conversation evolves.

---

## 🧮 Merit Score Formula (v1.0 - Alpha)

Each post is analyzed by AI models and scored across 5 weighted dimensions:

| Component        | Weight  | Description                                                                 |
|------------------|---------|-----------------------------------------------------------------------------|
| `ClarityScore`   | 0.25    | Semantic coherence, grammar, readability, and structure                     |
| `NoveltyScore`   | 0.25    | Degree of semantic divergence from nearby posts (avoids repetition/echo)   |
| `ContributionScore` | 0.20 | Does it move the discussion forward? Refines, challenges, or expands ideas |
| `CivilityScore`  | 0.15    | Tone, respectfulness, empathy, and non-toxic language                      |
| `RelevanceScore` | 0.15    | How well it connects to the thread, question, or topic at hand             |

Each component returns a score between `0.00` and `1.00`.  
The final `MeritScore` is the **weighted average**:

MeritScore = (ClarityScore * 0.25) + (NoveltyScore * 0.25) + (ContributionScore * 0.20) + (CivilityScore * 0.15) + (RelevanceScore * 0.15)


---

## 🔍 Component Breakdown

### 🧠 ClarityScore
- Uses LLM embeddings + grammar scoring models
- Penalizes confusing phrasing, contradictions, excessive jargon
- Favors clean, direct expression—even when complex

### 💡 NoveltyScore
- Measures vector distance between your post and surrounding ones
- Higher score = your contribution brings new concepts or frames
- Reduces reward for reworded agreement or recycled takes

### 🔄 ContributionScore
- Based on discourse tree analysis + semantic linking
- Scores higher if your post:
  - Adds missing information
  - Corrects a flaw in the argument
  - Introduces a new lens on the topic
  - Bridges two ideas together

### 🧘 CivilityScore
- Uses a toxicity classifier + sentiment model
- Penalizes:
  - Personal attacks
  - Sarcasm masking hostility
  - Dismissiveness or elitism
- Elevates:
  - Respectful disagreement
  - Good-faith questions
  - “Steel-manning” an opposing view

### 📌 RelevanceScore
- Checks semantic alignment with:
  - The original post or question
  - The specific parent comment (in nested threads)
- Encourages staying on-topic and thread-aware contribution

---

## 📈 Merit Score Ranges (Suggested Use)

| Score Range | Interpretation                        | System Behavior                  |
|-------------|----------------------------------------|----------------------------------|
| `0.00–0.29` | Low signal / possible noise            | Downranked, mod-review if extreme|
| `0.30–0.59` | Limited merit, possible surface-level  | Neutral or lightly visible       |
| `0.60–0.79` | Solid contribution                     | Promoted to core thread          |
| `0.80–0.89` | High-value insight                     | Highlighted and summarized       |
| `0.90–1.00` | Exceptional idea or framing            | Featured, added to merit logs    |

---

## 🔄 Dynamic Scoring & Re-Evaluation

Merit scores are **not fixed forever**.

- If a comment gains meaningful replies, its **ContributionScore** may increase
- If a post is forked into deeper discussion, its **RelevanceScore** may rise
- If LLMs improve or moderation models update, scores can be re-evaluated
- Score changes are **versioned and timestamped** for transparency

---

## 📂 Score Transparency

Each post includes:
- A public **Merit Score breakdown**
- Option to view **"Why this post is ranked here"**
- History of **score changes with context explanations**

This allows users to:
- Learn what makes good contributions
- Improve their own posts over time
- Trust the system’s decisions

---

## 🧪 What About Bias?

To reduce AI bias:
- Use **open-source moderation models** where possible
- Include **feedback loops** for users to flag misrankings
- Log **edge cases** for human + model retraining
- Run **cross-model evaluations** to check consistency

We believe in **AI-assisted governance**, not AI-as-overlord.

---

## 🧬 Final Thought

The Merit Score is **not a score of your worth**—it’s a reflection of how your idea contributes to the conversation.

The goal isn’t to chase numbers.  
The goal is to **surface insight**—so we can all think better, together.

---

👁‍🗨 For algorithm details, model architectures, and feedback, visit:  
`/ai/moderation-engine` | `/discussions/scoring-feedback` | `/roadmap/voting-system`

