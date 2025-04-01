🧾 API Endpoint List for Meritocious
🔐 Authentication & Identity
Method	Endpoint	Purpose
POST	/api/auth/login	Log user in with email/password
POST	/api/auth/google	Google OAuth login
POST	/api/auth/register	Register new user
POST	/api/auth/verify-email	Submit verification token
POST	/api/auth/resend-verification	Resend verification email
POST	/api/auth/logout	Log user out
GET	/api/auth/me	Get current user identity & status
👤 User Profiles
Method	Endpoint	Purpose
GET	/api/users/{username}	Get public profile
PUT	/api/users/me	Update profile
GET	/api/users/me/forks	User’s forks
GET	/api/users/me/substacks	User’s Substacks
GET	/api/users/me/settings	Feed + merit thresholds
PUT	/api/users/me/settings	Update feed + visibility settings
📚 Substacks
Method	Endpoint	Purpose
GET	/api/substacks	Get list (with tag/merit filters)
POST	/api/substacks	Create a new Substack
GET	/api/substacks/{id}	Get a single Substack by ID
PUT	/api/substacks/{id}	Edit Substack (name, tags, rules)
POST	/api/substacks/{id}/watch	Follow a Substack
POST	/api/substacks/{id}/star	Star/bookmark a Substack
GET	/api/substacks/{id}/posts	Posts in the Substack
POST	/api/substacks/{id}/fork	Fork a Substack
GET	/api/substacks/suggested	AI-suggested Substacks
GET	/api/substacks/trending	Trending Substacks by merit momentum
📝 Posts
Method	Endpoint	Purpose
GET	/api/posts	Get all posts (with filters, tags, Substack)
POST	/api/posts	Create new post
GET	/api/posts/{id}	Get full post by ID
PUT	/api/posts/{id}	Edit post
DELETE	/api/posts/{id}	Delete (with soft-delete optional)
POST	/api/posts/{id}/fork	Fork an existing post
GET	/api/posts/{id}/forks	Get all forks of a post
GET	/api/posts/{id}/lineage	Return ancestry tree of forks
GET	/api/posts/{id}/merit	Get AI merit scores
POST	/api/posts/{id}/star	Star/bookmark a post
GET	/api/posts/{id}/comments	List comments (if implemented)
POST	/api/posts/{id}/comments	Add a comment
🎯 AI/Merit Engine
Method	Endpoint	Purpose
POST	/api/merit/evaluate	Evaluate text (used during post creation)
GET	/api/merit/posts/{id}	Get AI merit score breakdown
POST	/api/merit/thresholds/test	Simulate feed filter preview
GET	/api/merit/badges/{userId}	Merit-based badges (e.g. “Insight Leader”)
🔎 Search & Discovery
Method	Endpoint	Purpose
GET	/api/search?q=...	Global search: posts, users, substacks
GET	/api/search/tags	Tag cloud / suggestions
GET	/api/search/topics	Semantic topic groups
GET	/api/search/related-posts/{id}	Related posts by semantic meaning
GET	/api/posts/suggestions/{id}	AI-generated “you might also fork” suggestions
🔔 Notifications (Optional)
Method	Endpoint	Purpose
GET	/api/notifications	List user notifications
POST	/api/notifications/mark-read	Mark as read
GET	/api/notifications/unread-count	Notification badge count
📈 Analytics & Trending
Method	Endpoint	Purpose
GET	/api/analytics/posts/trending	Posts with rising merit
GET	/api/analytics/substacks/trending	Same for Substacks
GET	/api/analytics/users/top-contributors	Based on contribution score
GET	/api/analytics/graph/{id}	Idea remix graph for a post or Substack
🛠 Admin / Moderation (If Needed)
Method	Endpoint	Purpose
POST	/api/mod/posts/{id}/hide	Temporarily remove from feed
POST	/api/mod/posts/{id}/feature	Highlight for visibility
GET	/api/mod/reports	View flagged content
POST	/api/mod/flags	Flag a post/comment for review
