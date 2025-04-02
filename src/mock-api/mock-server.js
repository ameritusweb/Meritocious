const express = require('express');
const cors = require('cors');
const fs = require('fs');
const path = require('path');
const { createProxyMiddleware } = require('http-proxy-middleware');

const app = express();
const port = 5001;

// Helper functions for simulating real-world conditions
const simulateDelay = () => new Promise(resolve => setTimeout(resolve, Math.random() * 1000));
const simulateError = (chance = 0.1) => Math.random() < chance;

// Configuration management
let currentConfig = 'default';
let configs = new Map();

// Load all configuration files
function loadConfigs() {
  const configsDir = path.join(__dirname, 'configs');
  const configFiles = fs.readdirSync(configsDir)
    .filter(file => file.endsWith('.json'));

  configs.clear();
  for (const file of configFiles) {
    const config = JSON.parse(fs.readFileSync(path.join(configsDir, file)));
    configs.set(config.id, config);
  }
  
  console.log('📝 Loaded configurations:', Array.from(configs.keys()));
}

// Initialize configs
loadConfigs();

// Helper to get endpoint key
function getEndpointKey(method, path) {
  return `${method}:${path}`;
}

// Helper to process dynamic values in response
function processDynamicValues(response, req) {
  if (typeof response === 'object') {
    return JSON.parse(
      JSON.stringify(response, (key, value) => {
        if (typeof value === 'string' && value.startsWith('param:')) {
          const param = value.split(':')[1];
          return req.params[param];
        }
        return value;
      })
    );
  }
  return response;
}

// Middleware to handle mock responses
const mockMiddleware = async (req, res, next) => {
  const config = configs.get(currentConfig);
  if (!config) {
    return res.status(500).json({ error: `Configuration '${currentConfig}' not found` });
  }

  // Build endpoint key
  const routePath = req.route.path;
  const endpointKey = getEndpointKey(req.method, routePath);
  
  // Get response from config
  const mockResponse = config.responses[endpointKey];
  if (!mockResponse) {
    return next();
  }

  // Simulate network conditions
  await simulateDelay();
  if (simulateError()) {
    return res.status(500).json({
      error: 'Mock server error',
      message: 'This is a simulated error from the mock server'
    });
  }

  // Handle conditional responses
  if (mockResponse.condition) {
    const condition = new Function('body', `return ${mockResponse.condition}`);
    const result = condition(req.body);
    
    if (result) {
      return res.json(processDynamicValues(mockResponse.success, req));
    } else {
      const { status, body } = mockResponse.error;
      return res.status(status).json(body);
    }
  }

  // Return regular response
  res.json(processDynamicValues(mockResponse, req));
};

app.use(cors());
app.use(express.json());

// Configuration management endpoints
app.get('/mock-api/configs', (req, res) => {
  res.json({
    current: currentConfig,
    available: Array.from(configs.keys())
  });
});

app.post('/mock-api/config/:id', (req, res) => {
  const { id } = req.params;
  if (!configs.has(id)) {
    return res.status(404).json({ error: `Configuration '${id}' not found` });
  }
  currentConfig = id;
  res.json({ message: `Switched to configuration: ${id}` });
});

app.post('/mock-api/reload', (req, res) => {
  loadConfigs();
  res.json({ message: 'Configurations reloaded' });
});

// Mock endpoints
// Note: Each route uses the mockMiddleware to handle responses from config

// User & Auth endpoints
app.get('/api/user/profile', mockMiddleware);
app.post('/api/auth/login', mockMiddleware);
app.get('/api/users/:userId/preferences', mockMiddleware);
app.put('/api/users/:userId/preferences', mockMiddleware);

// Posts endpoints
app.get('/api/posts/recommended', mockMiddleware);
app.get('/api/posts/:id', mockMiddleware);
app.post('/api/posts', mockMiddleware);
app.get('/api/posts/:postId/comments', mockMiddleware);

// Comments endpoints
app.get('/api/comments/:commentId', mockMiddleware);
app.get('/api/comments/:commentId/replies', mockMiddleware);
app.post('/api/comments', mockMiddleware);
app.post('/api/comments/:commentId/like', mockMiddleware);

// Tags endpoints
app.get('/api/tags/popular', mockMiddleware);
app.get('/api/tags/search', mockMiddleware);
app.get('/api/tags/:name/posts', mockMiddleware);
app.get('/api/tags/:name/wiki', mockMiddleware);
app.get('/api/tags/:name/relationships', mockMiddleware);
app.get('/api/tags/trending', mockMiddleware);
app.get('/api/tags/:name/moderation-history', mockMiddleware);
app.get('/api/tags/following', mockMiddleware);
app.post('/api/tags/:name/follow', mockMiddleware);
app.delete('/api/tags/:name/follow', mockMiddleware);

// Substack endpoints
app.get('/api/substack/trending', mockMiddleware);
app.get('/api/substack/recommended', mockMiddleware);
app.get('/api/substack/:slug', mockMiddleware);
app.get('/api/substack/:slug/metrics', mockMiddleware);
app.post('/api/substack/import', mockMiddleware);
app.get('/api/substack/validate', mockMiddleware);

// Remix endpoints
app.get('/api/remix/:id', mockMiddleware);
app.post('/api/remix', mockMiddleware);
app.get('/api/remix/:id/analytics', mockMiddleware);
app.get('/api/remix/:id/suggestions', mockMiddleware);
app.get('/api/remix/:id/score', mockMiddleware);
app.get('/api/remix/trending', mockMiddleware);
app.post('/api/remix/:id/insights', mockMiddleware);

// Moderation endpoints
app.post('/api/moderation/report', mockMiddleware);
app.get('/api/moderation/history/:userId', mockMiddleware);
app.get('/api/moderation/queue', mockMiddleware);

// Analytics & Merit endpoints
app.get('/api/analytics/user/:userId', mockMiddleware);
app.get('/api/merit/score/:userId', mockMiddleware);
app.get('/api/merit/breakdown/:userId', mockMiddleware);

// Search endpoint
app.get('/api/search', mockMiddleware);

// Notification endpoints
app.get('/api/notifications', mockMiddleware);
app.post('/api/notifications/read', mockMiddleware);

// Optional: Proxy unhandled requests to real API
/* 
app.use('/api', createProxyMiddleware({
  target: 'https://api.meritocious.com',
  changeOrigin: true,
  onProxyReq: (proxyReq, req, res) => {
    console.log(`Proxying: ${req.method} ${req.originalUrl}`);
  }
}));
*/

app.listen(port, () => {
  console.log(`✅ Mock API running at http://localhost:${port}`);
  console.log(`📝 Current configuration: ${currentConfig}`);
  console.log(`💡 Available configurations: ${Array.from(configs.keys()).join(', ')}`);
  console.log(`
  Commands:
  - List configs:   curl http://localhost:${port}/mock-api/configs
  - Switch config:  curl -X POST http://localhost:${port}/mock-api/config/admin
  - Reload configs: curl -X POST http://localhost:${port}/mock-api/reload
  `);
});
    name: 'Mock User',
    email: 'mock@example.com',
    meritScore: 85,
    contributions: 42
  });
});

app.post('/api/auth/login', (req, res) => {
  const { username, password } = req.body;
  if (username === 'test' && password === 'test') {
    res.json({
      token: 'mock-jwt-token',
      user: { id: 1, name: 'Test User' }
    });
  } else {
    res.status(401).json({ error: 'Invalid credentials' });
  }
});

// Posts API
app.get('/api/posts/recommended', (req, res) => {
  res.json([
    {
      id: 1,
      title: 'Understanding Merit Scores',
      author: 'Mock Author',
      meritScore: 92,
      tags: ['tutorial', 'merit']
    },
    {
      id: 2,
      title: 'Building Better Communities',
      author: 'Another Author',
      meritScore: 88,
      tags: ['community', 'growth']
    }
  ]);
});

app.get('/api/posts/:id', (req, res) => {
  res.json({
    id: parseInt(req.params.id),
    title: 'Mock Post',
    content: 'This is a mock post content with *markdown* support.',
    author: 'Mock Author',
    meritScore: 90,
    tags: ['mock', 'test'],
    createdAt: new Date().toISOString()
  });
});

app.post('/api/posts', (req, res) => {
  res.json({
    id: 999,
    ...req.body,
    meritScore: 80,
    createdAt: new Date().toISOString()
  });
});

// Comments API
app.get('/api/posts/:postId/comments', (req, res) => {
  res.json([
    {
      id: 1,
      content: 'Great post!',
      author: 'Commenter1',
      meritScore: 75,
      createdAt: new Date().toISOString()
    },
    {
      id: 2,
      content: 'Interesting perspective.',
      author: 'Commenter2',
      meritScore: 82,
      createdAt: new Date().toISOString()
    }
  ]);
});

app.post('/api/comments', (req, res) => {
  res.json({
    id: 888,
    ...req.body,
    meritScore: 70,
    createdAt: new Date().toISOString()
  });
});

// Notifications API
app.get('/api/notifications', (req, res) => {
  res.json([
    {
      id: 1,
      type: 'comment',
      message: 'Someone commented on your post',
      isRead: false,
      createdAt: new Date().toISOString()
    },
    {
      id: 2,
      type: 'merit',
      message: 'Your merit score increased',
      isRead: true,
      createdAt: new Date().toISOString()
    }
  ]);
});

app.post('/api/notifications/read', (req, res) => {
  res.json({ success: true });
});

// Tags API
app.get('/api/tags/trending', (req, res) => {
  res.json([
    { id: 1, name: 'technology', count: 150 },
    { id: 2, name: 'community', count: 120 },
    { id: 3, name: 'merit', count: 100 }
  ]);
});

// Substack API
app.get('/api/substacks/recommended', (req, res) => {
  res.json([
    {
      id: 1,
      name: 'Tech Insights',
      description: 'Daily tech analysis',
      followers: 1200,
      meritScore: 88
    },
    {
      id: 2,
      name: 'Community Building',
      description: 'Building online communities',
      followers: 800,
      meritScore: 92
    }
  ]);
});

// Moderation API
// Merit Score API
app.get('/api/merit/score/:userId', (req, res) => {
  res.json({
    userId: parseInt(req.params.userId),
    totalScore: 85,
    components: {
      contentQuality: 90,
      communityEngagement: 85,
      contributionFrequency: 80,
      peerRecognition: 85
    },
    history: [
      { date: '2024-03-01', score: 82 },
      { date: '2024-03-15', score: 84 },
      { date: '2024-04-01', score: 85 }
    ]
  });
});

app.get('/api/merit/breakdown/:userId', (req, res) => {
  res.json({
    postQuality: [
      { metric: 'Clarity', score: 90 },
      { metric: 'Originality', score: 85 },
      { metric: 'Research', score: 88 }
    ],
    engagement: [
      { metric: 'Comments', count: 150 },
      { metric: 'Responses', count: 45 },
      { metric: 'Citations', count: 12 }
    ],
    impact: [
      { metric: 'Views', count: 1200 },
      { metric: 'Saves', count: 80 },
      { metric: 'Shares', count: 25 }
    ]
  });
});

// Remix API
app.get('/api/posts/:id/remixes', (req, res) => {
  res.json([
    {
      id: 1,
      title: 'Remix of Original Post',
      originalPostId: parseInt(req.params.id),
      author: 'Remixer1',
      meritScore: 82,
      changes: ['Added new perspective', 'Updated data'],
      createdAt: new Date().toISOString()
    },
    {
      id: 2,
      title: 'Alternative Take',
      originalPostId: parseInt(req.params.id),
      author: 'Remixer2',
      meritScore: 78,
      changes: ['Different approach', 'New examples'],
      createdAt: new Date().toISOString()
    }
  ]);
});

app.post('/api/posts/:id/remix', (req, res) => {
  res.json({
    id: 555,
    originalPostId: parseInt(req.params.id),
    ...req.body,
    meritScore: 75,
    createdAt: new Date().toISOString()
  });
});

// Search API
app.get('/api/search', (req, res) => {
  const query = req.query.q;
  res.json({
    posts: [
      {
        id: 1,
        title: `Post about ${query}`,
        excerpt: `This post discusses ${query} in detail...`,
        meritScore: 88
      },
      {
        id: 2,
        title: `Another post mentioning ${query}`,
        excerpt: `Related discussion about ${query}...`,
        meritScore: 82
      }
    ],
    tags: [
      { id: 1, name: query, count: 150 },
      { id: 2, name: `${query}-advanced`, count: 75 }
    ],
    users: [
      {
        id: 1,
        name: 'Expert User',
        bio: `Specialist in ${query}`,
        meritScore: 90
      }
    ]
  });
});

// Moderation API
app.post('/api/moderation/report', (req, res) => {
  res.json({
    id: 777,
    status: 'pending',
    createdAt: new Date().toISOString()
  });
});

app.get('/api/moderation/history/:userId', (req, res) => {
  res.json([
    {
      id: 1,
      type: 'warning',
      reason: 'Content quality',
      actionTaken: 'Warning issued',
      date: new Date().toISOString()
    },
    {
      id: 2,
      type: 'review',
      reason: 'Community guidelines',
      actionTaken: 'Content reviewed',
      date: new Date().toISOString()
    }
  ]);
});

// Analytics API
app.get('/api/analytics/user/:userId', (req, res) => {
  res.json({
    contentMetrics: {
      totalPosts: 25,
      totalComments: 150,
      avgMeritScore: 85,
      topTags: ['technology', 'community', 'merit']
    },
    engagementMetrics: {
      views: 5000,
      reactions: 750,
      comments: 250,
      shares: 100
    },
    timelineMetrics: [
      { date: '2024-03-01', posts: 5, comments: 20, merit: 82 },
      { date: '2024-03-15', posts: 8, comments: 35, merit: 84 },
      { date: '2024-04-01', posts: 12, comments: 45, merit: 85 }
    ],
    topContent: [
      {
        id: 1,
        title: 'Best Post',
        meritScore: 92,
        views: 1200
      },
      {
        id: 2,
        title: 'Second Best',
        meritScore: 88,
        views: 800
      }
    ]
  });
});

// User Preferences API
app.get('/api/users/:userId/preferences', (req, res) => {
  res.json({
    theme: 'dark',
    emailNotifications: {
      comments: true,
      mentions: true,
      meritUpdates: true
    },
    contentPreferences: {
      preferredTags: ['technology', 'community'],
      excludedTags: ['politics'],
      contentLanguages: ['en', 'es']
    },
    privacySettings: {
      profileVisibility: 'public',
      activityVisibility: 'followers',
      allowMentions: true
    }
  });
});

app.put('/api/users/:userId/preferences', (req, res) => {
  res.json({
    ...req.body,
    updatedAt: new Date().toISOString()
  });
});

// Optional: Proxy unhandled requests to real API
/* 
app.use('/api', createProxyMiddleware({
  target: 'https://api.meritocious.com',
  changeOrigin: true,
  onProxyReq: (proxyReq, req, res) => {
    console.log(`Proxying: ${req.method} ${req.originalUrl}`);
  }
}));
*/

app.listen(port, () => {
  console.log(`✅ Mock API running at http://localhost:${port}`);
});