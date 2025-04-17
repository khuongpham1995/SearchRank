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

This is a Clean Architecture-based Minimal API project that demonstrates secure JWT authentication, user authentication, and robust rate limiting to prevent abuse. This API has been upgraded to .NET 9, taking advantage of the latest performance improvements, language features, and enhanced runtime capabilities. The project uses MediatR for command/query handling, EF Core for data persistence, and Swagger (via Swashbuckle) for OpenAPI documentation.

## What You Can Do in Swagger UI
- **Explore Endpoints:**  
  Browse and view details about each API endpoint, including HTTP methods, URL paths, and parameter definitions.

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