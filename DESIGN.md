# 🎨 Meritocious Frontend Design Document

## ✨ Vision Summary

Meritocious is a platform for thoughtful conversation, not shallow engagement. Its frontend design should:
- Encourage reading over reacting
- Promote clarity and signal over noise
- Empower users to explore, contribute, and reflect
- Visually embody **merit, nuance, and trust**

The visual theme should feel:
- **Minimal**, but not sterile  
- **Thoughtful**, but not academic  
- **Vibrant**, but not attention-seeking  
- **Alive**, but not noisy

## 🎨 Theme: _“Digital Agora”_

Imagine a modern-day forum — not in the Reddit or Twitter sense, but a digital **agora**: a clean, light-filled space for discussion and debate.

## 🌈 Color Palette

| Purpose         | Color         | Notes                                  |
|-----------------|---------------|----------------------------------------|
| Primary         | #00A38C       | Signature teal — represents “merit”    |
| Accent          | #FFD700       | Golden signal — used sparingly         |
| Background      | #121212       | Dark mode default                      |
| Surface         | #1E1E1E       | Cards, components                      |
| Text Primary    | #FFFFFF       | Readability is key                     |
| Text Muted      | #B0B0B0       | For secondary information              |
| Danger/Moderation | #FF4C4C     | For flagged or problematic content     |

## 🔤 Typography

Font: `'Inter', system-ui, sans-serif`

| Level       | Style                                 |
|-------------|----------------------------------------|
| H1          | 32px, bold, used for thread titles     |
| H2          | 24px, semi-bold, used for section headers |
| Body        | 16px, regular, longform content        |
| Meta        | 14px, muted, for usernames, dates      |
| Code/Quote  | Monospace or styled blocks             |

## 🧩 Components

### Thread Card
- Title
- Summary/snippet
- Merit Score
- Metadata
- Actions

### Comment Thread
- Recursive UI
- Hover merit reasoning
- Fork thread option

### Moderation Dialog
- AI analysis
- Rephrase, remove, allow
- Constitution principle reference

### Merit Badge
- Merit grade: High, Moderate, Low, Flagged
- Tooltip explains reasoning

### Sort Tabs
- New, Top, Insightful, Underrated, Dissent

## 🛠 UI Patterns

### Navigation
- Top nav: logo, new thread, user menu, theme toggle
- Sidebar (optional): filters, tags

### Layout
- Home: vertical threads
- Thread: left = discussion, right = context
- Profile: contribution timeline
- Mod dashboard: flagged posts

### Animation & Motion
- Fade in, pulse, slide for modals and updates

## 🧠 Interaction Design

### Contribution Flow
1. Write post
2. AI analysis
3. Feedback
4. Revise or submit
5. Score + log

### Moderation
- Suggest rephrasing
- Transparent reasoning
- Appeal/revise path

## 📚 Docs & Help
- /constitution
- /merit
- /faq

## 🧩 Tech Stack

| Tool            | Role                             |
|-----------------|----------------------------------|
| Blazor Server   | SPA + SSR                        |
| MudBlazor       | UI components                    |
| ASP.NET Core    | Backend + API                    |
| Semantic Kernel | AI integration                   |
| Tailwind (opt)  | Custom styles                    |
| Markdown/Razor  | Content                          |

## 🧪 Future Ideas
- Merit visualizations
- Thread maps
- AI digests
- Voice input
- Anonymity gradients

## ✅ Summary

Design rules:
- Minimalist, readable interface
- Merit > metrics
- Clarity > noise
- Transparent moderation
- Guidance > punishment
