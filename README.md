# Cirreum.Runtime.Wasm.Msal

[![NuGet Version](https://img.shields.io/nuget/v/Cirreum.Runtime.Wasm.Msal.svg?style=flat-square&labelColor=1F1F1F&color=003D8F)](https://www.nuget.org/packages/Cirreum.Runtime.Wasm.Msal/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Cirreum.Runtime.Wasm.Msal.svg?style=flat-square&labelColor=1F1F1F&color=003D8F)](https://www.nuget.org/packages/Cirreum.Runtime.Wasm.Msal/)
[![GitHub Release](https://img.shields.io/github/v/release/cirreum/Cirreum.Runtime.Wasm.Msal?style=flat-square&labelColor=1F1F1F&color=FF3B2E)](https://github.com/cirreum/Cirreum.Runtime.Wasm.Masl/releases)
[![License](https://img.shields.io/github/license/cirreum/Cirreum.Runtime.Wasm.Msal?style=flat-square&labelColor=1F1F1F&color=F2F2F2)](https://github.com/cirreum/Cirreum.Runtime.Wasm.Msal/blob/main/LICENSE)
[![.NET](https://img.shields.io/badge/.NET-10.0-003D8F?style=flat-square&labelColor=1F1F1F)](https://dotnet.microsoft.com/)

**Seamless Azure Entra ID authentication for Blazor WebAssembly applications**

## Overview

**Cirreum.Runtime.Wasm.Msal** provides a fluent API for integrating Microsoft Authentication Library (MSAL) with Blazor WebAssembly applications in the Cirreum Framework. It simplifies Azure Entra ID authentication setup for both workforce (internal) and external identity (CIAM) scenarios, with optional Microsoft Graph integration for user profile enrichment.

## Features

- **Workforce & External Identity** - First-class support for Entra ID workforce tenants and Entra External ID (CIAM)
- **Dynamic Authentication** - Runtime tenant resolution for multi-tenant SaaS applications
- **Microsoft Graph Integration** - Profile enrichment with configurable depth (minimal, extended, or custom)
- **Fluent Builder API** - Chainable configuration methods with IntelliSense support
- **Session Monitoring** - Track user activity and API calls with configurable timeouts
- **Claims Extensibility** - Custom claims transformations via `IClaimsExtender`
- **Authorization Policies** - Pre-configured role-based policies with easy customization
- **WASM Optimized** - Source-generated logging for minimal bundle size

## Installation

```bash
dotnet add package Cirreum.Runtime.Wasm.Msal
```

## Quick Start

### Workforce Authentication

```csharp
var builder = DomainApplication.CreateBuilder(args);

builder.AddEntraAuth(
    tenantId: "your-tenant-id",  // or "common" for multi-tenant
    clientId: "your-client-id"
);

await builder.BuildAndRunAsync<App>();
```

### External Identity (CIAM)

```csharp
builder.AddEntraExternalAuth(
    domain: "your-tenant",       // e.g., "contoso" for contoso.ciamlogin.com
    clientId: "your-client-id"
);
```

## Microsoft Graph Integration

### Minimal Enrichment

Basic user profile data with `User.Read` scope only:

```csharp
builder.AddEntraAuth("tenant-id", "client-id")
    .AddGraphServices()
    .WithMinimalGraphEnrichment();
```

### Extended Enrichment

Comprehensive profile including mailbox settings, organization, and directory memberships:

```csharp
builder.AddEntraAuth("tenant-id", "client-id")
    .AddGraphServices()
    .WithExtendedGraphEnrichment();
```

### External Tenant Enrichment

Optimized for Entra External ID with limited Graph API access:

```csharp
builder.AddEntraExternalAuth("domain", "client-id")
    .AddGraphServices()
    .WithGraphEnrichment();
```

### Custom Graph Scopes

```csharp
builder.AddEntraAuth("tenant-id", "client-id")
    .AddGraphServices()
    .WithExtendedGraphEnrichment(graphScopes: [
        "User.Read",
        "MailboxSettings.Read",
        "Directory.Read.All"
    ]);
```

## Advanced Configuration

### Custom Authorization Policies

Default policies (`Standard`, `StandardInternal`, `StandardAgent`, `StandardManager`, `StandardAdmin`) are included automatically. Add custom policies:

```csharp
builder.AddEntraAuth("tenant-id", "client-id", authorization: options =>
{
    options.AddPolicy("Engineering", policy =>
        policy.RequireClaim("department", "Engineering"));
});
```

### Session Monitoring

```csharp
builder.AddEntraAuth("tenant-id", "client-id")
    .AddSessionMonitoring(options =>
    {
        options.TrackApiCalls = true;
        options.IdleTimeout = TimeSpan.FromMinutes(30);
    });
```

### Custom Claims Extender

```csharp
builder.AddEntraAuth("tenant-id", "client-id")
    .AddClaimsExtender<MyClaimsExtender>();
```

### Application User Integration

```csharp
builder.AddEntraAuth("tenant-id", "client-id")
    .AddApplicationUser<AppUser, AppUserLoader>();
```

### Dynamic Multi-Tenant Authentication

For SaaS applications where tenant configuration is resolved at runtime:

```csharp
builder.AddDynamicAuth(
    configureWorkforce: auth => auth
        .AddGraphServices()
        .WithExtendedGraphEnrichment(),
    configureExternal: auth => auth
        .AddGraphServices()
        .WithGraphEnrichment()
);
```

Dynamic auth retrieves configuration via `cirreum.tenant.getConfig()`, which is populated by the loader script. Custom scopes are specified in the tenant configuration endpoint response, not in code.

### Custom OAuth Scopes

```csharp
builder.AddEntraAuth(
    tenantId: "tenant-id",
    clientId: "client-id",
    defaultScopes: ["api://my-api/read", "api://my-api/write"]
);
```

## Default OAuth Scopes

The following scopes are included by default:
- `openid`
- `profile`
- `email`
- `offline_access`
- `User.Read`

Additional scopes passed via `defaultScopes` are merged with these defaults.

## Architecture

```
Cirreum.Runtime.Wasm.Msal
├── HostingExtensions           # Entry points: AddEntraAuth, AddEntraExternalAuth, AddDynamicAuth
├── Authentication/
│   ├── Builders/               # Fluent builder implementations
│   ├── Enrichment/             # Graph profile enrichers (Minimal, Extended, External)
│   ├── Providers/              # GraphServiceClient provider
│   ├── EntraUserAccount        # User account with roles support
│   └── MsalClaimsPrincipalFactory
└── Extensions                  # Claims, Graph, Session configuration
```

## Requirements

- .NET 10.0+
- Blazor WebAssembly
- Azure Entra ID tenant (workforce or external)

## Dependencies

- `Microsoft.Authentication.WebAssembly.Msal`
- `Cirreum.Runtime.Wasm`
- `Cirreum.Graph.Provider`

## License

MIT License - see [LICENSE](LICENSE) for details.

---

**Cirreum Foundation Framework**
*Layered simplicity for modern .NET*
