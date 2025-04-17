# SEO Checker UI

An Angular 19 standalone application that consumes the SEO Checker API via a generated OpenAPI client and Angular Material for styling.

## Overview

This standalone Angular 19 app lets you:

- Enter search keywords and a target URL  
- View SEO ranking results (e.g. positions in Google or Bing)  
- Switch between configured search engines  

The client library is generated at build time from your API’s Swagger/OpenAPI spec, and uses Angular Material for UI components.

## Testing Credentials

Use these credentials to obtain a JWT and test protected endpoints:

- **Email:** `test-user@gmail.com`  
- **Password:** `Password123!`

<br>

# SEO Checker API

This is a Clean‑Architecture‑based .NET 9 Minimal API that powers an SEO‑Checker service. It exposes endpoints to submit keywords and URLs, fetch ranking positions from Google, Bing (and additional engines via a plug‑in provider model), and caches each distinct search for one hour to limit outbound calls. Access is secured with JWT Bearer tokens, while a fixed‑window rate limiter guards against abuse. Internally, business operations are organized as MediatR commands/queries, data is persisted via EF Core (Code‑First with migrations/seeding), and every endpoint is documented through Swagger/OpenAPI (Swashbuckle).

## What You Can Do in Swagger UI
- **Explore Endpoints:**  
  Browse and view details about each API endpoint, including HTTP methods, URL paths, and parameter definitions.

  To open the interactive Swagger UI, navigate to https://localhost:{PORT}/swagger.

- **Request/Response Examples:**  
  See sample requests and responses for endpoints such as `api/token` and `api/bing/search-ranking`.

- **Authorize Using JWT:**  
  Use the "Authorize" button to input your JWT token. This allows you to test secured endpoints directly from the Swagger UI.

- **Interactive Testing:**  
  Try out endpoints directly in the browser by sending test requests and viewing the responses.

## Testing Credential
```bash
  curl --location 'https://localhost:7274/api/token' \
    --header 'accept: */*' \
    --header 'Content-Type: application/json' \
    --data-raw '{
      "email": "test-user@gmail.com",
      "password": "Password123!"
    }'
```