# Migration Guide: Cirreum.Runtime.Wasm.Msal 1.x → 2.0

## Why v2

`Cirreum.Runtime.Wasm` 2.0.0 removed the client-side application-user resolver: the framework
now fetches the caller's own record from the server's bootstrap endpoint
(`GET /_cirreum/application-user`), which requires authentication and nothing else — so the
record arrives even for a disabled caller and `ViewState.Disabled` renders for the first time.
This package's wrapper verbs follow that surface. The full rationale and walkthrough live in
the `Cirreum.Runtime.Wasm` `MIGRATION-v2.md`; this guide covers only the wrapper delta.

## Breaking Changes — Find/Replace Table

| v1.x | v2.0 |
|------|------|
| `auth.AddApplicationUserResolver<MyResolver>()` (on `IEntraAuthenticationBuilder`) | `auth.AddApplicationUser<MyUser>(serviceUri)` |
| `auth.AddApplicationUserResolver(sp => …)` (on `IEntraAuthenticationBuilder`) | `auth.AddApplicationUser<MyUser>(serviceUri)` |
| `external.AddApplicationUserResolver<MyResolver>()` (on `IEntraExternalBuilder`) | `external.AddApplicationUser<MyUser>(serviceUri)` |
| `external.AddApplicationUserResolver(sp => …)` (on `IEntraExternalBuilder`) | `external.AddApplicationUser<MyUser>(serviceUri)` |

`MyUser` is your `IApplicationUser` implementation — the type your **server-side** resolver
returns. `serviceUri` is your Cirreum server's base URI (the same one your remote clients use).

## Migration Walkthrough

1. Update `Cirreum.Runtime.Wasm.Msal` to 2.0.0 (brings `Cirreum.Runtime.Wasm` 2.0.0).
2. Replace the wrapper call per the table:

   ```csharp
   builder.AddEntraAuth("tenant-id", "client-id")
       .AddApplicationUser<AppUser>(new Uri("https://api.example.com/"));
   ```

3. Delete your client-side resolver class and anything that existed only to serve it.
4. Verify the server runs `Cirreum.Runtime.Server` 1.2.0+ with its server-side
   `AddApplicationUserResolver<T>()` registration (unchanged).

## What Didn't Change

Everything else in this package: `AddEntraAuth` / `AddEntraExternalAuth` composition, claims
factories and extenders, Graph enrichment, and the server-side registration surface.

## Downstream Package Impact

None below this package — it is a leaf. `Cirreum.Runtime.Wasm.Oidc` 2.0.0 carries the same
change for OIDC apps.
