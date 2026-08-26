# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [2.0.4] - 2026-08-26

### Updated

- Updated NuGet packages.

## [2.0.3] - 2026-08-25

### Updated

- Updated NuGet packages.

## [2.0.2] - 2026-08-20

### Updated

- Updated NuGet packages.

## [2.0.1] - 2026-08-04

### Updated

- Re-pinned `Cirreum.Runtime.Wasm` `2.0.0` → `2.0.1` — **take this immediately if you took
  2.0.0**: it makes `AppRouteView` probe the framework bootstrap client (in 2.0.0 the
  `NotProvisioned`/`Disabled` states were unreachable under `AddApplicationUser`), and renames
  its pass-through fragments `NotAuthorizedContent`/`AuthorizingContent` →
  `NotAuthorized`/`Authorizing` (compile-time markup error where the old names were used).

## [2.0.0] - 2026-08-04

### Breaking

- **The four `AddApplicationUserResolver` wrapper verbs removed** (type + factory overloads on
  both `IEntraAuthenticationBuilder` and `IEntraExternalBuilder`), following
  `Cirreum.Runtime.Wasm` 2.0.0's removal of the client-side resolver. Replaced by
  **`AddApplicationUser<TUser>(Uri)`** on both builders: the app supplies its user type and
  its server's base URI, and the framework fetches the caller's own record from the server's
  bootstrap endpoint during initialization — reaching disabled callers, whom the old
  resolver-through-operations path could never serve. See `MIGRATION-v2.md` and the
  `Cirreum.Runtime.Wasm` 2.0.0 migration guide.

### Updated

- Re-pinned `Cirreum.Runtime.Wasm` `1.2.4` → `2.0.0`, `Cirreum.Graph.Provider` `1.0.62` →
  `1.0.63` (Cirreum spine 4.2.0 wave).

## [1.0.52] - 2026-07-31

### Updated

- Updated NuGet packages (Cirreum spine 4.0.1 wave: records-only grant semantics via `Cirreum.Domain` 4.0.1 / `Cirreum.Contracts` 4.0.1; Infrastructure and Runtime repins).

## [1.0.51] - 2026-07-30

### Updated

- Updated NuGet packages — picks up the `Cirreum.Domain` 3.0.0 authorization-enforcement wave
  (fail-open operation-authorization fix + `IPolicyAuthorizer` rename) through the re-pinned
  lower-layer packages; see Cirreum.Domain `MIGRATION-v3.md`.

## [1.0.50] - 2026-07-29

### Updated

- Updated NuGet packages.

## [1.0.49] - 2026-07-28

### Updated

- Updated NuGet packages.

## [1.0.48] - 2026-07-27

### Updated

- Updated NuGet packages.

## [1.0.47] - 2026-07-24

### Fixed

- Graph profile enrichment no longer overwrites `UserProfile.Nickname` with the directory
  `mailNickname`. `Nickname` is the OIDC `nickname` claim — a casual name, resolved by the
  claims enrichment — while `mailNickname` is Entra's mail alias (e.g. `jane.smith`), a
  different concept; the old mapping stomped a genuine `nickname` claim value (or nulled it
  when Graph returned no alias) and surfaced the alias as if it were a nickname. The
  `mailNickname` field is also no longer requested in any enricher's Graph `$select`.
- Graph enrichment left `UserProfile.DisplayName` null when the directory returned no display
  name (sparse directory data, or a failed Graph query). The directory display name still wins
  when present, but it no longer clears a value the claims pass already resolved — the claims
  enrichment in `Cirreum.Domain` now consolidates `DisplayName` from claims alone (`Nickname`,
  then the `name` claim, then a given/family composite), so a UI consolidated on `DisplayName`
  doesn't render blank.

### Updated

- `Cirreum.Runtime.Wasm` 1.1.0 → 1.1.1, which brings that claims-level `DisplayName`
  consolidation (`Cirreum.Domain` 1.3.x) through to Entra clients.

## [1.0.46] - 2026-07-24

### Updated

- `Cirreum.Runtime.Wasm` 1.0.52 → 1.1.0. `MsalClaimsPrincipalFactory` inherits the new built-in
  claim processing: provisioned `custom*` claims (`customRoles`, `customName`, …) are
  canonicalized to their native names automatically (JSON-array values split for `IsInRole`), and
  the authentication-state publication fixes ship transitively — claim transforms always run,
  publication dedupes on user id + claims-content fingerprint. No API change in this package; an
  `IClaimsExtender` is now needed only for app-specific transformations (e.g. native-vs-minted
  precedence), not for `custom*` remapping.

### Fixed

- README no longer implies an `IClaimsExtender` is required for `custom*` claim remapping — the
  canonicalization is built into the authentication pipeline; extender guidance now covers only
  the advanced cases.

## [1.0.45] - 2026-07-20

### Updated

- Updated NuGet packages.

## [1.0.44] - 2026-07-19

### Updated

- Updated NuGet packages.

## [1.0.43] - 2026-07-11

### Updated

- Updated NuGet packages (`Cirreum.Runtime.Wasm` 1.0.47 → 1.0.50).

## [1.0.42] - 2026-07-09

### Updated

- Updated NuGet packages.

## [1.0.41] - 2026-07-04

### Fixed

- **Completed this repo's transitive foundation cutover off legacy `Cirreum.Core`.** `Cirreum.Graph.Provider` (re-pinned to `1.0.53`) and `Cirreum.Runtime.Wasm` (re-pinned to `1.0.45`) both dropped `Cirreum.Core` for the `Cirreum.Kernel`/`Cirreum.Contracts`/`Cirreum.Domain` spine — this repo's `IEntraAuthenticationBuilder`, `GraphEnabledBuilder`, and `ExternalGraphEnabledBuilder` had always resolved `IUserProfileEnrichmentBuilder`/`IGraphEnabledBuilder`/`IApplicationUserResolver`/etc. transitively through Core, with no direct reference of its own. Once Core left the graph these types became ambiguous, then missing. No source changes needed beyond removing the transient `Cirreum.AuthenticationProvider` reference added mid-migration — `IUserProfileEnrichmentBuilder`/`IGraphEnabledBuilder`/`IExternalGraphEnabledBuilder` relocated to `Cirreum.Contracts`/`Cirreum.Domain` (host-agnostic profile enrichment, not an Authentication-track concern) and now flow in transitively through the existing `Cirreum.Graph.Provider`/`Cirreum.Runtime.Wasm` references.

## [1.0.40] - 2026-05-10

### Updated

- Updated NuGet packages.

## [1.0.39] - 2026-05-10

### Updated

- Updated NuGet packages.

## [1.0.38] - 2026-05-01

### Updated
- Updated NuGet packages.

