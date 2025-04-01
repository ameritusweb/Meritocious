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
| AntDesignBlazor | UI components                    |
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

# 🧱 Meritocious Frontend Layout Vision

## ✨ Overview

This document outlines the desired frontend layout and interaction design for **Meritocious**, taking inspiration from **Ant Design Pro**, but tailored to support the platform's values: focus, merit, clarity, and intelligent discussion.

---

## 🎯 Key UX Goals

- ✅ Clean, responsive layout
- ✅ Sidebar that collapses intelligently
- ✅ Icon-only mode on small screens
- ✅ Hover-to-expand sidebar behavior
- ✅ Tabbed navigation docked to routes
- ✅ Closable, persistent tabs
- ✅ Smooth multi-page user flow

---

## 🧭 Sidebar Design

### Features:
- **Responsive collapse**: At narrow widths, the sidebar should collapse to icon-only mode.
- **Hover-to-expand**: On desktop, hovering over the collapsed sidebar reveals the menu temporarily.
- **Icons with tooltips**: Provide context when collapsed.
- **Persistent navigation state**: Sidebar should remember the last state (collapsed/expanded).

### Components:
- `Layout.Sider`
- `Menu`, `MenuItem`, `SubMenu`
- Tooltips for collapsed mode

---

## 🧩 Tabbed Navigation Workspace

Inspired by IDEs and Ant Design Pro:

### Behavior:
- Tabs represent **active views**, each tied to a route
- Clicking sidebar items **opens a new tab**
- Users can **switch between open tabs**
- Tabs can be **closed manually**
- Tabs are **route-aware** — clicking a tab navigates to that route

### Advanced:
- Restore tabs from session or local storage
- Indicate unsaved changes
- Allow middle-click to close tabs

### Component Concept:
```razor
<Tabs Type="EditableCard">
    <TabPane Key="home" Tab="Home" Closable="false" />
    <TabPane Key="t-123" Tab="Thread: AGI Risks" Closable="true" />
</Tabs>
```

---

## 📱 Responsiveness

- Sidebar collapses below `lg` breakpoint
- Content adjusts with flexbox/grid layouts
- Tab layout adapts to available width (with overflow scroll)
- Mobile-friendly toggles or drawers when necessary

---

## 🧠 Navigation Flow

| Action                  | Behavior                           |
|-------------------------|------------------------------------|
| Click Sidebar Item      | Opens or focuses corresponding tab |
| Click Tab               | Navigates to that view             |
| Close Tab               | Removes from view, switches focus  |
| Refresh page            | Tabs and routes are restored       |

---

## 🧪 Future Enhancements

- Drag-to-reorder tabs
- Detachable pop-out tabs (multi-view)
- Multi-account/multi-context session separation
- Tab group saving for power users

---

## ✅ Summary

The layout should support:
- Merit-focused browsing
- Multitasking without clutter
- Context-rich navigation
- Responsive and intuitive user flow

The goal is to give users a sense of **workspace for ideas**, not just a feed.
