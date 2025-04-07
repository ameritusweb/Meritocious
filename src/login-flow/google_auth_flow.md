# 🔐 Authentication Flow: Email + Google-Linked Accounts (Blazor Server App)

## Overview

This system allows users to:

- Sign up or log in with **email and password**
- Sign in directly with **Google OAuth**
- ✅ All accounts **must be linked to a Google account** (enforced post-login)
- Email/password login is allowed, but **must be verified** by linking it to the user’s Google identity

---

## ✅ Supported Login Methods

### 1. Sign in with Google
- If it's the first time:
  - A new user is created using the Google email.
  - The Google account is linked automatically.
- If already linked:
  - User is signed in immediately.

### 2. Sign up or sign in with Email/Password
- If credentials are valid:
  - The user is logged in.
  - The system checks if their account is linked to a Google account.
    - ❌ If not linked: user is redirected to link a Google account.
    - ✅ If linked: access is granted.

---

## 🔁 Account Linking Logic

### Step 1: User logs in with Email/Password
- `SignInManager.PasswordSignInAsync(...)` succeeds.
- System checks for linked logins via `UserManager.GetLoginsAsync(user)`
- If no Google login found:
  - Redirect to `/LinkGoogle` endpoint to initiate Google OAuth

### Step 2: User links Google account
- `LinkGoogle` starts the OAuth flow:
  - Calls `ConfigureExternalAuthenticationProperties("Google", callbackUrl)`
- After successful OAuth, `LinkGoogleCallback`:
  - Retrieves `ExternalLoginInfo`
  - Calls `UserManager.AddLoginAsync(user, info)`
  - Optionally stores `info.ProviderKey` (Google ID) into `ApplicationUser.GoogleId`
- User is now fully authenticated and linked.

---

## 📦 Data Stored in Identity

- `AspNetUsers`:
  - Email
  - PasswordHash
  - Optional: `GoogleId` (manual copy of ProviderKey)

- `AspNetUserLogins`:
  - `LoginProvider`: "Google"
  - `ProviderKey`: Google subject ID
  - `UserId`: FK to `AspNetUsers`

---

## 🔐 Security Notes

- Google’s `ProviderKey` is a stable, unique identifier for a user across your app.
- Don’t rely solely on email for identity—emails can change, `ProviderKey` cannot.
- Avoid exposing `ProviderKey` to clients; use it server-side for linking/validation.

---

## 📜 Summary

| Action | Behavior |
|--------|----------|
| Google Sign-In | Log in or create account and auto-link |
| Email/Password Login | Require Google account to be linked before proceeding |
| New User (Email/Password) | Must go through Google linking post-sign-up |
| User Not Linked to Google | Redirect to `/LinkGoogle` |
| User Linked to Google | Access granted |


---

## 🧠 Enforcing Google Link Using Default [Authorize] Attribute

Instead of creating a custom attribute like `[RequireGoogleLinked]`, we can enforce that a user is both authenticated **and** has a Google account linked by extending the default `[Authorize]` policy.

### ✅ Goal

- Use `[Authorize]` as usual
- Automatically check that the logged-in user has a Google login linked
- If not, redirect them to `/Account/LinkGoogle` to complete the linking

---

### 1. Create the Requirement

```csharp
public class GoogleLinkedRequirement : IAuthorizationRequirement { }
```

---

### 2. Create the Authorization Handler

```csharp
public class GoogleLinkedHandler : AuthorizationHandler<GoogleLinkedRequirement>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GoogleLinkedHandler(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, GoogleLinkedRequirement requirement)
    {
        if (!context.User.Identity.IsAuthenticated)
            return;

        var httpContext = _httpContextAccessor.HttpContext;
        var user = await _userManager.GetUserAsync(context.User);
        if (user == null)
            return;

        var logins = await _userManager.GetLoginsAsync(user);
        bool isGoogleLinked = logins.Any(l => l.LoginProvider == "Google");

        if (isGoogleLinked)
        {
            context.Succeed(requirement);
        }
        else
        {
            // Redirect to Google linking page
            httpContext.Response.Redirect("/Account/LinkGoogle");
            context.Fail();
        }
    }
}
```

---

### 3. Register the Policy in `Program.cs`

```csharp
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .AddRequirements(new GoogleLinkedRequirement())
        .Build();
});

builder.Services.AddScoped<IAuthorizationHandler, GoogleLinkedHandler>();
```

---

### 4. Usage

```csharp
[Authorize]
public IActionResult Dashboard()
{
    return View();
}
```

No changes needed—any `[Authorize]` check will now also validate that the user has a Google login.

---

### ❗ Opting Out for Specific Pages

If you want to skip the Google requirement for login, register, or linking pages:

```csharp
options.AddPolicy("NoGoogleRequired", policy =>
{
    policy.RequireAuthenticatedUser(); // or allow anonymous
});
```

Then:

```csharp
[Authorize(Policy = "NoGoogleRequired")]
public IActionResult LinkGoogle() { ... }
```
