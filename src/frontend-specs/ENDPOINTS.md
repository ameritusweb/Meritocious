# 🌟 Meritocious API Endpoints - Complete Reference

## 🔐 Authentication & Identity
```
POST   /api/auth/login                # Login with email/password
POST   /api/auth/google               # Google OAuth login
POST   /api/auth/register             # Register new user
POST   /api/auth/verify-email         # Submit verification token
POST   /api/auth/resend-verification  # Resend verification email
POST   /api/auth/logout               # Logout user
GET    /api/auth/me                   # Get current user identity & status
POST   /api/auth/refresh              # Refresh JWT token
POST   /api/auth/revoke               # Revoke refresh token
POST   /api/auth/google/link          # Link Google account
POST   /api/auth/google/unlink        # Unlink Google account
```

## 👤 User Profiles & Management
```
GET    /api/users/{username}          # Get public profile
PUT    /api/users/me                  # Update own profile
GET    /api/users/me/forks            # User's forks
GET    /api/users/me/substacks        # User's Substacks
GET    /api/users/me/settings         # Feed + merit thresholds
PUT    /api/users/me/settings         # Update feed + visibility settings
GET    /api/users/top-contributors    # Get top contributors
GET    /api/users/{id}/posts          # Get user's posts
GET    /api/users/{id}/comments       # Get user's comments
GET    /api/users/{id}/merit-log      # Get user's merit history
```

## 📚 Substacks
```
GET    /api/substacks                 # Get list (with tag/merit filters)
POST   /api/substacks                 # Create new Substack
GET    /api/substacks/{id}            # Get single Substack
PUT    /api/substacks/{id}            # Edit Substack
DELETE /api/substacks/{id}            # Delete Substack
POST   /api/substacks/{id}/watch      # Follow a Substack
POST   /api/substacks/{id}/star       # Star/bookmark a Substack
GET    /api/substacks/{id}/posts      # Posts in the Substack
POST   /api/substacks/{id}/fork       # Fork a Substack
GET    /api/substacks/suggested       # AI-suggested Substacks
GET    /api/substacks/trending        # Trending Substacks
GET    /api/substacks/recommended     # Personalized recommendations
```

## 📝 Posts
```
GET    /api/posts                     # Get all posts (with filters)
POST   /api/posts                     # Create new post
GET    /api/posts/{id}               # Get full post
PUT    /api/posts/{id}               # Edit post
DELETE /api/posts/{id}               # Delete post
POST   /api/posts/{id}/fork          # Fork post
GET    /api/posts/{id}/forks         # Get post forks
GET    /api/posts/{id}/lineage       # Get ancestry tree of forks
GET    /api/posts/{id}/merit         # Get merit scores
POST   /api/posts/{id}/star          # Star/bookmark post
GET    /api/posts/trending           # Get trending posts
GET    /api/posts/recommended        # Get personalized recommendations
GET    /api/posts/search             # Search posts
GET    /api/posts/{id}/history       # Get edit history
```

## 💬 Comments
```
GET    /api/posts/{id}/comments      # Get post comments
POST   /api/posts/{id}/comments      # Create comment
PUT    /api/comments/{id}            # Update comment
DELETE /api/comments/{id}            # Delete comment
GET    /api/comments/{id}/replies    # Get comment replies
POST   /api/comments/{id}/fork       # Fork comment thread
```

## 🏷️ Tags & Topics
```
GET    /api/tags                     # Get all tags
GET    /api/tags/popular            # Get popular tags
GET    /api/tags/search             # Search tags
GET    /api/tags/{name}             # Get tag details
POST   /api/tags                    # Create new tag
GET    /api/tags/{name}/posts       # Get posts by tag
POST   /api/tags/{name}/posts/{id}  # Add tag to post
DELETE /api/tags/{name}/posts/{id}  # Remove tag from post
GET    /api/tags/trending           # Get trending topics
GET    /api/tags/{name}/related     # Get related tags
```

## 🎯 Merit & AI Engine
```
POST   /api/merit/evaluate          # Evaluate text pre-submission
GET    /api/merit/scores/{id}       # Get merit score details
POST   /api/merit/recalculate       # Recalculate merit score
GET    /api/merit/thresholds        # Get merit thresholds
PUT    /api/merit/thresholds        # Update merit thresholds
POST   /api/merit/thresholds/test   # Simulate feed filter preview
GET    /api/merit/badges/{userId}   # Get merit-based badges
```

## 🔍 Search & Discovery
```
GET    /api/search                  # Global search
GET    /api/search/users            # Search users
GET    /api/search/posts            # Search posts
GET    /api/search/comments         # Search comments
GET    /api/search/tags             # Search tags
GET    /api/search/substacks        # Search substacks
GET    /api/search/topics           # Get semantic topic groups
GET    /api/search/related-posts/{id} # Get related posts
```

## 🔔 Notifications
```
GET    /api/users/{id}/notifications     # Get user notifications
POST   /api/notifications/mark-read      # Mark as read
GET    /api/notifications/unread-count   # Get badge count
GET    /api/notifications/settings       # Get settings
PUT    /api/notifications/settings       # Update settings
```

## 📊 Analytics
```
GET    /api/analytics/users/{id}/impact  # Get user impact metrics
GET    /api/analytics/content/{id}/reach # Get content reach metrics
GET    /api/analytics/trends            # Get platform trends
GET    /api/analytics/topics            # Get topic analytics
GET    /api/analytics/graph/{id}        # Get idea remix graph
```

## 🛡️ Moderation
```
POST   /api/moderation/reports          # Report content
GET    /api/moderation/reports          # Get reports (for mods)
PUT    /api/moderation/reports/{id}     # Update report status
POST   /api/moderation/moderate         # Moderate content
GET    /api/moderation/reports/stats    # Get moderation stats
POST   /api/mod/posts/{id}/hide         # Hide from feed
POST   /api/mod/posts/{id}/feature      # Feature content
GET    /api/mod/reports                 # View flagged content
```

## 🔄 User Interactions
```
POST   /api/interactions/star/{id}      # Star content
DELETE /api/interactions/star/{id}      # Unstar content
GET    /api/interactions/starred        # Get starred content
POST   /api/interactions/watch/{id}     # Watch content
DELETE /api/interactions/watch/{id}     # Unwatch content
GET    /api/interactions/watched        # Get watched content
```

## Common Query Parameters
Most GET endpoints support:
```
page          # Page number
pageSize      # Items per page
sortBy        # Sorting field (date, merit, relevance)
sortOrder     # asc or desc
timeFrame     # all, day, week, month, year
include       # Additional fields to include
filters       # Content filters
```

## Response Format
All endpoints use a consistent format:
```json
{
  "success": boolean,
  "data": object | array,
  "error": string | null,
  "metadata": {
    "page": number,
    "pageSize": number,
    "totalItems": number,
    "totalPages": number
  }
}
```
