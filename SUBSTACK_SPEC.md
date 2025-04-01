# 📚 Meritocious Substacks: Semantic Communities of Discourse

## 🌱 What is a Substack?

A **Substack** is a user-created, topic-driven micro-community—similar in spirit to Reddit's subreddits, but more aligned with knowledge, exploration, and AI-enhanced discourse.

Each Substack is:
- **Topic-based**: e.g. `AI Alignment`, `Open Source Philosophy`, `Modern Stoicism`
- **Taggable**: Metadata tags for filtering and discovery
- **Forkable**: Any Substack can be forked to explore an alternate perspective
- **Star-able / Watchable**: Users can follow a Substack and receive relevant updates

---

## 🧩 Core Features

### ✅ Create a Substack
- Define a **name**, **description**, **tags**, and **topic area**
- Optionally define **default post structure** (e.g. discussion, essay, question)
- System prevents duplicate names with fuzzy match checks

### ✅ Post to a Substack
- Posts are written in **Markdown**
- Each post is scored on merit (clarity, originality, insight, tone) by AI
- Posts appear in reverse chronological order or sorted by merit
- Users can **fork a post** to continue the thought in a new direction

### ✅ Forking

- **Fork a post**: Create a new post inspired by another, maintaining a reference link
- **Fork a Substack**: Clone an entire Substack to create a variant focused on a new angle or philosophy

Forking promotes pluralism and exploration rather than flame wars.

---

## 🌌 Feed Personalization & Thresholding

Users can **watch** Substacks and configure **thresholds** for what appears in their feed:

### Threshold Categories (AI-Scored)
- `Clarity`
- `Novelty`
- `Insight`
- `Respectfulness`
- `Cohesion`

### Example Filter Config
> Show posts from watched Substacks only if:
> - Clarity ≥ 4
> - Insight ≥ 3
> - Respectfulness ≥ 5

This keeps the feed **signal-rich**, personalized, and adaptive to intent.

---

## ⭐ Engagement Actions

- **Star**: Bookmark a Substack or post you value
- **Watch**: Follow a Substack for feed updates
- **Fork**: Extend a post or Substack in a new direction
- **Comment**: (TBD) Limited to merit-eligible users?

---

## 👤 User Profiles

Each user has a profile showing:
- Public posts and forks
- Watched and starred Substacks
- Optional bio + social metadata

Privacy settings:
- `Public`
- `Friends-only`
- `Private`

Friend status enables access to restricted content (posts, forks).

---

## 📈 Merit-Based Discovery

The platform supports deep discovery:
- Search by tags, topics, authors
- Filter by AI score ranges
- View “rising Substacks” by momentum
- Weekly digest of top posts in watched Substacks

---

## 🛠 Tech Design Hints

- Substack = top-level entity (like a group)
- Posts reference SubstackId
- ForkedSubstackId and ForkedPostId reference origins
- AI scoring stored per post with 5-point dimensions
- Feed service filters posts by watch list + thresholds

---

## 🧪 Future Features

- Substack-level moderation and forking policies
- Invite-only Substacks
- Reputation systems per Substack
- AI-generated Substack summaries
- Cross-linking Substacks (“you may also like...”)

---

## ✨ Example User Flow

1. User visits `meritocious.com`
2. Clicks "Explore Substacks"
3. Filters by `AI`, `Debate`, `Science`
4. Stars `Modern Rationalism`
5. Sets thresholds (clarity ≥ 4, insight ≥ 3)
6. Watches Substack
7. Reads top post, forks it with a new idea
8. Friend views user's fork in their own feed

Merit rises. Knowledge flows. Echo chambers fall away.
