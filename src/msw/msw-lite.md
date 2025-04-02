# 🧪 Mock API Layer for Blazor Server using Express.js

## Overview

This document outlines how to simulate **MSW-style API mocking** for a **Blazor Server** application by running a local **Express.js** server that acts as a controllable mock API layer.

This approach decouples frontend development from live backend dependencies, enabling rapid prototyping, isolated testing, and flexible development workflows.

---

## 🎯 Goals

- Simulate real API behavior without requiring a live backend
- Allow dynamic mocking of endpoints (success, error, latency, etc.)
- Provide easy toggling between mock and real APIs
- Enable sharing of mock logic between developers and automated tests

---

## 🏗️ Architecture

```
Blazor Server App
       |
       |  HttpClient -> http://localhost:5001/api/...
       |
   [Mock API Layer]
     Express.js Server
        ↳ Handles and mocks API routes
        ↳ (Optional) Proxies to real API
```

---

## ⚙️ Development Setup

### 1. Blazor Server App

#### Configure `HttpClient`

In `Program.cs`:

```csharp
builder.Services.AddHttpClient("MockApi", client =>
{
    client.BaseAddress = new Uri("http://localhost:5001/");
});
```

Use it in components/services:

```csharp
var client = HttpClientFactory.CreateClient("MockApi");
var data = await client.GetFromJsonAsync<User>("/api/user");
```

---

### 2. Express Mock API Server

Create a new `mock-server.js` file:

```javascript
const express = require('express');
const cors = require('cors');
const app = express();
const port = 5001;

app.use(cors());
app.use(express.json());

app.get('/api/user', (req, res) => {
  res.json({ id: 1, name: 'Mocked User' });
});

app.post('/api/login', (req, res) => {
  const { username } = req.body;
  res.json({ token: `mock-token-for-${username}` });
});

app.listen(port, () => {
  console.log(`✅ Mock API running at http://localhost:${port}`);
});
```

Install dependencies:

```bash
npm init -y
npm install express cors
node mock-server.js
```

---

## 🔄 Optional: Proxy to Real API

You can proxy requests that aren't mocked:

```javascript
const { createProxyMiddleware } = require('http-proxy-middleware');

app.use('/api', createProxyMiddleware({
  target: 'https://real-api.example.com',
  changeOrigin: true,
  onProxyReq: (proxyReq, req, res) => {
    console.log(`Proxying: ${req.method} ${req.originalUrl}`);
  }
}));
```

Install:
```bash
npm install http-proxy-middleware
```

---

## 🧰 Dev Workflow

- Run the mock server: `node mock-server.js`
- Run your Blazor app
- All `HttpClient` requests go to the mock API instead of the real backend
- You can simulate errors, delays, or switch logic live

---

## 🧪 Advanced Mock Features (Optional)

- Add `setTimeout` to simulate latency
- Use a config file or query param to toggle mock behaviors
- Persist data in-memory or use `lowdb` for lightweight mock storage
- Integrate Faker.js for dynamic content

---

## 🧼 Clean Separation

Encapsulate API logic behind interfaces:

```csharp
public interface IUserService
{
    Task<User> GetCurrentUserAsync();
}
```

Then use dependency injection to swap implementations for:
- Mock (`MockUserService`)
- Real (`HttpUserService` using `HttpClient`)

---

## 📦 Bonus: Integration with Unit Tests

You can reuse the Express mock server for integration tests by:
- Spawning it in the test setup (`child_process`)
- Killing it after tests
- Using a consistent mock port

---

## 📍 Summary

| Feature                   | Status     |
|--------------------------|------------|
| Realistic API simulation | ✅          |
| Easy toggle/mock control | ✅          |
| Reusable across dev/test | ✅          |
| MSW-like behavior        | ✅ (server-side) |
| Browser-level interception | ❌ (Blazor Server limitation) |

---

## 🚀 Future Enhancements

- UI to toggle mocks at runtime
- WebSocket mocking support
- Record-and-replay mode
- Use Swagger/OpenAPI to auto-generate mock routes
