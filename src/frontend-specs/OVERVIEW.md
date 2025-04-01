📦 Component Architecture Outline (Blazor + Ant Design)
We'll break this into sections based on functionality:

1. 🌐 Layout & Shell
These are your top-level containers that persist across navigation.

MainLayout.razor
Sidebar navigation (Sider)

Header (user avatar, notifications, search bar)

Content wrapper

Dark/light theme toggle

AppHeader.razor
Logo + site title

Search input (global post/Substack search)

User avatar w/ dropdown: profile, settings, logout

“Create New” button

SidebarMenu.razor
Navigation:

Home

Explore Substacks

My Feed

My Forks

Notifications

Settings

2. 🧵 Substack System
Handles creation, exploration, and interaction with topic-based communities.

SubstackList.razor
Grid or list of Substacks

Tags filter (CheckBoxGroup)

Sort: by merit, popularity, newness

Watch/Star buttons

SubstackCard.razor
Display info for a single Substack (title, tags, brief, stats)

Buttons: Watch, Star, Fork

SubstackDetail.razor
Title, description, owner info

Posts list (sortable by merit or date)

Tags filter

Fork Substack button

Watch/Star toggle

CreateSubstackModal.razor
Name

Description

Tags (AntDesign Select w/ tag creation)

Post structure template (optional)

Fuzzy match duplicate prevention

3. 📝 Posts & Threading
Core knowledge contribution system: post creation, threading, forking.

PostList.razor
List of posts (title, summary)

Sorting: merit, date, popularity

Filtering by AI-score dimensions (clarity, insight, etc.)

PostCard.razor
Title

Excerpt or rendered Markdown preview

Author, merit score(s), fork count

Buttons: View, Star, Fork

PostDetail.razor
Full content (Markdown renderer)

AI merit breakdown (clarity, novelty, etc.)

Fork post button

Comment section (if enabled)

Related forks/remixes panel

CreatePost.razor
Markdown editor (TextArea + live preview)

AI merit scoring (pre-submit preview)

Select Substack

Submit button

4. 🌱 Forking & Remixing
Promote idea evolution over debate.

ForkPostModal.razor
Select source post

Editor prefilled with reference/context

Option to “Remix” — synthesize multiple sources

ForkSubstackModal.razor
Prefill with original Substack metadata

Edit title, description

Link back to origin

Optionally invite collaborators

5. 🧠 Merit System UI
Shows and configures AI-generated quality assessments.

MeritScorePanel.razor
Displays AI scores across:

Clarity

Novelty

Insight

Respectfulness

Cohesion

Radar chart or score bars

Explanation tooltips (e.g., “What does Clarity 4 mean?”)

MeritThresholdSelector.razor
Sliders or dropdowns for each metric

Save to feed preferences

Applied when browsing content

6. 📚 User Profiles
Track merit logs, contributions, and preferences.

UserProfile.razor
Bio + social links

Public posts & forks

Watched/Starred Substacks

Privacy settings

UserCard.razor
Small version used in comments or post previews

Username, avatar, merit stats preview

SettingsPage.razor
Feed thresholds

Notification preferences

Account + privacy controls

7. 🔔 Notifications / Activity Feed
Track forks, comments, mentions, invites.

NotificationPanel.razor
List of recent activity

Mark as read

Filters: Mentions, Forks, New Posts

8. 🔎 Search & Discovery
Semantic and tag-based search across content.

SearchBar.razor
Autosuggest for posts, users, tags, Substacks

SearchResults.razor
Results grouped by content type (Posts, Substacks, People)

Tabs or vertical layout

9. 📊 Analytics & Graphs (future)
Optional visualizations of discourse networks and post evolution.

SemanticRemixGraph.razor
Force-directed or flow diagram

Shows forking/remixing lineage

Highlights idea evolution

TrendingSubstacks.razor
“Rising Stars” view by growth in merit score or forks

10. 🛠 Shared Components
Reusable UI building blocks.

TagSelector.razor

MarkdownViewer.razor

MeritBadge.razor (little icon or badge for top-tier content)

ForkIcon.razor

StarButton.razor

WatchToggle.razor

🧪 Bonus Dev Tools (optional)
For testing and visualization of AI scores.

MeritSimulator.razor
Enter sample content → see AI scores

Tweak thresholds and see effect on visibility

---

🧬 Idea Lineage & Provenance
ForkLineageViewer.razor
Shows a “breadcrumb” or tree-like view:

Original post → Fork 1 → Fork 2

Helps users trace intellectual ancestry

Can appear inline or in a modal

InspiredByPanel.razor
Inline widget showing “Sources of inspiration”

Links to 1+ referenced posts if part of a synthesis fork

Optional inline diff view or summary of reused content

🗨️ Discussions & Micro-Comments
(If you want comments at all, or a constrained version of them)

CommentThread.razor
Threaded, limited-depth comments

Sort by merit or recency

Optional AI-enhanced “Summarize this thread”

CommentComposer.razor
Markdown input

Optional “reason for comment” selector: Clarification / Challenge / Extension

🧰 Community Tools & Admin Controls
SubstackModerationPanel.razor
For Substack creators/mods:

Hide post

Promote post

Set thresholds or access level

Show moderation actions publicly (for transparency)

InviteUserModal.razor
Invite-only Substack creation

Select access level: Viewer / Poster / Forker

🧠 AI-Assisted Features
AISuggestedForks.razor
“You might fork this into…” prompt

Suggests angles, counterpoints, or related Substacks to start

AISummarizePost.razor
Generates a TL;DR or key takeaways

Optional: “Summarize as tweet / outline / bullet points”

AIAssistedTagger.razor
Suggests tags during post creation

Learnable from user overrides (fine-tuning optional later)

📌 Knowledge Navigation & Mapping
IdeaMap.razor
Interactive map view of forks, syntheses, and clusters

Useful for knowledge graph-like visualization

TagExplorer.razor
Visual or hierarchical tag exploration

Discover Substacks/posts via tag relationships

TopicClusterBrowser.razor
AI-powered: Groups of related posts by meaning

Think “semantic subreddit clustering”

📖 Digest & Personalization
WeeklyDigestBuilder.razor
Lets user create/share a personalized digest

Pulls from watched Substacks, starred posts, high-merit content

FeedPreview.razor
User-adjustable preview showing how current thresholds affect visibility

Good for onboarding new users to merit tuning

🎨 Content Presentation Enhancements
MeritBadgeList.razor
Little icons or highlights: “Clarity Champ”, “Most Insightful”, etc.

Optionally AI-awarded with thresholds

PostTypeIcon.razor
Icon indicator for: Discussion / Essay / Question / Synthesis / Response

PostSummaryCard.razor
Condensed format for post feed (like Twitter/X cards)

Shows title, preview, AI summary, merit scores

🛠️ Meta/Utility Components
MarkdownDiffViewer.razor
Visual diff between original and forked post

Highlight what was kept, added, removed

PrivacySelector.razor
For post or profile visibility:

Public / Friends-only / Private

Could be used at the Substack level too

ThresholdPresetSelector.razor
"Curious", "Critical", "Strict" modes for AI filters

One-click setup for new users

🧭 Optional Niche/Advanced Components
CivicTag.razor
Special badge or namespace tag for posts of public relevance (e.g. philosophy, politics, science policy)

PeerInviteToken.razor
Mechanism to invite high-merit users to exclusive Substacks

Could gatekeep forks or discussions without being elitist

CitationManager.razor
Add references, links, external sources to post

Good for long-form or essay-type threads
