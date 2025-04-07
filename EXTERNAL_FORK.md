
# 🔗 External Forks: Connecting Ideas Across the Web

Meritocious isn't just a platform—it's an ecosystem of evolving ideas. With **External Forks**, users can extend the dialogue beyond our walls, forking thoughts from anywhere on the internet: Reddit, Hacker News, Substack, blogs, podcasts, research papers, and more.

---

## 🌱 What is an External Fork?

An **External Fork** is a special post type that begins as a response, riff, or evolution of a piece of content **outside of Meritocious**.

### Examples:
- 🧠 Forking a Reddit post that asked, "Can AGI grow food?"
- ✍️ Remixing a Substack article about decentralized governance
- 🎧 Responding to a podcast clip on cognitive science
- 📜 Interpreting a specific claim in a research paper

> It’s not just a reference. It’s a **continuation**—an act of thought evolution.

---

## 🛠 Key Features

| Feature              | Description |
|----------------------|-------------|
| **Source Linking**   | Users attach a URL to the original content |
| **Platform Metadata**| Displays platform name (e.g. Reddit, HN, etc.) |
| **Fork Type Badge**  | e.g. `External Fork – Hacker News` |
| **Origin Info**      | Optionally extracts title, author, date |
| **Fork Threads**     | Shows other Meritocious posts that forked from the same source |
| **Reverse Lookup (future)** | "This Reddit post has 3 forks on Meritocious" |

---

## ✨ Fork UI Example

> 🔗 **Forked from**: [Reddit – r/Futurology – "What if AGI managed farms?"](https://reddit.com/example)  
>  
> **Post Title:** *Beyond Scarcity: AGI and Regenerative Autonomy*  
>  
> _In a recent thread, someone asked whether AGI could manage farms. That stuck with me—not because of the tech, but because of the framing. What if we stopped optimizing for yield and started optimizing for renewal? Here's where that idea could go..._

---

## 🔍 Fork Request: Help Great Ideas Grow

Sometimes a user finds a gem in the wild, but doesn't have time to explore it themselves. With **Fork Requests**, they can nudge the community to pick it up.

### How It Works:
- Submit a Fork Request with:
  - 🔗 Link to source content
  - 🧭 Suggested focus or question
  - 📚 Optional context or tags
- Fork Requests appear in a public queue under “🪄 Ideas Waiting to Grow”
- Other users can **claim a request** and fork it into a full Meritocious post

> "This HN comment has gold buried in it. Someone, please dig it up."

---

## 🧩 Schema Suggestion (JSON)

```json
{
  "post_id": 1059,
  "title": "The Gardening Intelligence of AGI",
  "fork_type": "external",
  "external_source": {
    "platform": "Reddit",
    "url": "https://reddit.com/r/Futurology/abc123",
    "title": "Can AGI manage farms?",
    "timestamp": "2025-03-30T12:04:00Z"
  },
  "origin_tag": "AI in Agriculture"
}
```

---

## 🧠 Why This Matters

- ✅ Bridges the gap between platforms and preserves idea lineage
- ✅ Encourages remixing, not reposting
- ✅ Makes Meritocious the home of **idea version control**
- ✅ Enables a web-wide graph of **intellectual provenance**
- ✅ Democratizes contribution—anyone can surface great material

---

> *The best ideas don’t live in one place. Now, neither does your thinking.*
