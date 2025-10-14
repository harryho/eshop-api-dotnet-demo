## E-Shop API Demo

A demo api built on the top of .net core 9 and minimal api

### Architecture

```mermaid
flowchart TB
    subgraph ClientLayer["Client Layer"]
        C1[REST Client]
        C2[Swagger UI]
        C3[HTTP Client]
    end

    subgraph APIGateway["API Gateway"]
        API["Minimal API"]
        V1[API Version 1.0]
        V2[API Version 2.0]
    end

    subgraph AuthLayer["Authentication and Authorization"]
        JWT[JWT Bearer Authentication]
        AUTH["Authorization Policies"]
    end

    subgraph AppLayer["Application Layer"]
        ENDPOINTS["Products Endpoints"]
        MIDDLEWARE["Middleware Pipeline"]
        E1["Exception Handler"]
        E2["HTTP Logging"]
        E3["CORS"]
        E4["Parameter Validation"]
    end

    subgraph BusinessLayer["Business Layer"]
        REPO["Repositories"]
        ENTITIES["Domain Entities"]
        DTOS["DTOs"]
    end

    subgraph DataLayer["Data Layer"]
        EF[Entity Framework Core]
        CONTEXT["EshopContext<br/>DbContext"]
        MIGRATIONS["EF Migrations"]
    end

    subgraph Infrastructure["Infrastructure"]
        DOCKER["Docker Container<br/>SQL Server"]
        CONFIG["Configuration"]
    end

    Database[("MS Sql Database")]

    %% Client connections
    C1 --> API
    C2 --> API
    C3 --> API

    %% API flow
    API --> JWT
    API -. use .-> V1
    API -. use .-> V2
    API -. invoke .->ENDPOINTS
    V1 -. use .-> DTOS
    V2 -. use .-> DTOS


    %% Authentication flow
    JWT --> AUTH


    %% Application flow
    MIDDLEWARE --> ENDPOINTS
    MIDDLEWARE --> E1
    MIDDLEWARE --> E2
    MIDDLEWARE --> E3
    MIDDLEWARE --> E4
    ENDPOINTS --> REPO
    REPO --> ENTITIES
    ENTITIES --> DTOS

    %% Data flow
    REPO --> EF
    EF --> CONTEXT
    MIGRATIONS --> CONTEXT
    CONTEXT --> Database

    %% Infrastructure
    DOCKER --> Database
    CONFIG --> API

    %% Styling
    style AppLayer fill:#d3d3d3
    style BusinessLayer fill:#d3d3d3
    style Infrastructure fill:#d3d3d3
    style APIGateway fill:#d3d3d3
    style DataLayer fill:#d3d3d3
    style AuthLayer fill:#d3d3d3
    style ClientLayer fill:#d3d3d3


```

#### Key Architectural Components

**🔐 Security Features**
- JWT Bearer token authentication
- Role-based authorization (Staff/Admin roles)
- Scope-based permissions (read/write access)
- CORS configuration for cross-origin requests

**📊 API Features**
- RESTful API design with CRUD operations
- API versioning (v1.0 and v2.0)
- Swagger/OpenAPI documentation
- Request/response logging
- Global exception handling
- Parameter validation

**💾 Data Management**
- Entity Framework Core ORM
- Code-first migrations
- Repository pattern implementation



### Prerequisite

- Dotnet Core 8
- Visual Studio Code
  - C# Extension
  - Rest Client Extension
- Docker


### Starting SQL Server with Docker

```powershell
docker compose up -d mssql
```


### Create user jwts for local test via script (Recommended)

```powershell
.\generate-jwt-tokens.ps1
```

- The scripts will overwrite any existing token values in local-test.http
- If you need to regenerate tokens, simply run the script again
- The tokens are specific to your local development environment

### Create user jwts for local test (Manual)

```powershell
# Copying template file to local-test.http
copy -Force local-test-template.http local-test.http 

# create jwts for user with different role and permission

# Staff with read permission
dotnet user-jwts create --role Staff --scope "products:read"
# Update the token in the file `local-test.http`
# @STAFF_READ_TOKEN=<Staff Read Token>

# e.g.
# @STAFF_READ_TOKEN=eyJhbGciOiJIUxxxxxx.yyyyyy.zzzzz

# Staff with write permission
dotnet user-jwts create --role Staff --scope "products:write"
# Update the token in the file `local-test.http`
# @STAFF_WRITE_TOKEN=<Staff Write Token>

# Admin with read permission
dotnet user-jwts create --role Admin --scope "products:read"
# Update the token in the file `local-test.http`
# @ADMIN_READ_TOKEN=<Admin Read Token>

# Admin with write permission
dotnet user-jwts create --role Admin --scope "products:write"
# Update the token in the file `local-test.http`
# @ADMIN_WRITE_TOKEN=<Admin Write Token>

```

### Token Expiration

The generated JWT tokens have an expiration of approximately 90 days, making them suitable for extended testing periods.

### Starting E-Shop  API

> Eshop.Api project will create the database Eshop if it doesn't exist.

- Option 1: Run & Debug the E-Shop API from Visual Studio Code (F5)

- Option 2: Use command

    ```powershell
    cd <repository_location>
    dotnet run -p Eshop.Api --launch-profile https

    # info: Microsoft.EntityFrameworkCore.Migrations[20411]
    #   Acquiring an exclusive lock for migration application. 
    # See https://aka.ms/efcore-docs-migrations-lock 
    # for more information if this takes too long.
    # info: Microsoft.EntityFrameworkCore.Migrations[20405]
    #     No migrations were applied. The database is already up to date.
    # info: DB Initializer[9]
    #     DB is ready!
    # info: Microsoft.Hosting.Lifetime[14]
    #     Now listening on: https://localhost:7236
    # info: Microsoft.Hosting.Lifetime[14]
    #     Now listening on: http://localhost:5117
    # info: Microsoft.Hosting.Lifetime[0]
    #     Application started. Press Ctrl+C to shut down.
    # info: Microsoft.Hosting.Lifetime[0]
    #    Hosting environment: Development
    ```



### Test E-Shop API via Rest Client

- Update the port in file `local-test.http` if you have a different port.
- Make sure tokens updated in file `local-test.http` as instructed above.
- Execute the POST requests first.
- Execute the GET requests. You will see some results as below.
  
```http
HTTP/1.1 200 OK
Connection: close
Content-Type: application/json; charset=utf-8
Date: Wed, 12 Mar 2025 14:23:11 GMT
Server: Kestrel
Transfer-Encoding: chunked

[
  {
    "id": 4,
    "name": "Anta Waffle Racer Crater",
    "genre": "Basket Shoes",
    "unitPrice": 22.88,
    "unitInStock": 12,
    "releaseDate": "2020-09-30T00:00:00",
    "imageUri": "https://dummyimage.com/200x200/eee/000"
  },
  {
    "id": 5,
    "name": "XTEP AntaCourt Royale",
    "genre": "Tennis Shoes",
    "unitPrice": 39.99,
    "unitInStock": 11,
    "releaseDate": "2021-09-30T00:00:00",
    "imageUri": "https://dummyimage.com/200x200/eee/000"
  }
]
```
 
### Swagger 

Access Swagger on browser
`https://localhost:<YOUR_API_PORT>/swagger/index.html`

![version1](screenshots/version1-screenshot.png)

<!-- ![version2](screenshots/version2-screenshot.png) -->

### Drop the database

```powershell
.\drop-db.ps1
```