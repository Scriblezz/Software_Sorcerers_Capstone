# This fork highlights production support readiness: logging, error handling, and troubleshooting documentation.

# Movies Made Easy

Movies Made Easy is an ASP.NET Core application that lets users search for streaming-ready titles, capture recently viewed items, and review subscription insights.

## Setup & Run Instructions
1. Install the .NET 8 SDK and optionally the Azure Cosmos DB/SQL prerequisites used by the original project.
2. Clone this fork and navigate to `MoviesMadeEasyProject/MoviesMadeEasy`.
3. Provide the required connection strings in `appsettings.Development.json` (local SQL Server/MySQL) and any API keys used by the external movie service.
4. Restore and build:  
   `dotnet restore`  
   `dotnet build`
5. Run the site:  
   `dotnet run`  
   The app listens on the ports printed in the console (HTTPS by default).

## Logging
- Uses ASP.NET Core’s built-in `ILogger` pipeline (console by default, Application Insights if configured in the host).
- Search workflow emits structured `INFO`, `WARN`, and `ERROR` logs:
  - `INFO`: request start, raw result counts, completion.
  - `WARN`: rejected input or null responses from the provider.
  - `ERROR`: exceptions from upstream services or unexpected failures.
- Each entry carries a UTC timestamp, `requestId` (`HttpContext.TraceIdentifier`), and key inputs such as `query`, `sortBy`, `minYear`, `maxYear`.

## Troubleshooting Example
**Scenario:** Upstream movie catalog API timeout caused the UI spinner to hang with no error.

**Sample log:**  
`ERROR 2026-01-15T20:04:11Z: SearchMovies external provider failure | requestId=0HMRHO1... | query='Inception'`

**Investigation:**
1. Collected the `requestId` from the client’s network tab.
2. Queried logs for that identifier and confirmed the ERROR entry occurred directly after the `_movieService.SearchMoviesAsync` call.
3. Reviewed hosting metrics and downstream diagnostics, finding repeated timeouts from the provider during peak traffic.

**Fix:** Wrapped the external call in exception handling that logs the failure and returns HTTP 503 with a safe JSON payload (`error: movie_service_unavailable`, `requestId`). Added INFO logs around the workflow so it is clear where processing stopped.

**Verification:** Replayed the search, observed the client receive the 503 JSON instantly, and confirmed the log sequence shows start → provider failure → controlled ERROR → response sent.

## How I Debug Issues
1. Reproduce or gather the failing request’s identifier from the UI or logs.
2. Correlate the `requestId` across INFO/WARN/ERROR entries to pinpoint the failing layer.
3. Inspect inputs (query, filters) to rule out validation issues.
4. Check downstream dependencies (database, external APIs) for latency or availability anomalies.
5. Implement targeted fixes (validation, logging, graceful responses) and retest the exact scenario.
6. Leave documentation in the README when the scenario illustrates support readiness.

## Project Notes
- Forked from the Western Oregon University “Software Sorcerers” senior capstone.
- Original maintainers: Alden Roy (maintainer), Kaitlyn Woodard, Kira Morgan, Brady Parksion.
- Team followed Agile/Scrum ceremonies; see `Planning_Documents/` for historical artifacts.