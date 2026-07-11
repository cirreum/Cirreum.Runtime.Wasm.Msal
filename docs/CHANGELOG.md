# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

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

