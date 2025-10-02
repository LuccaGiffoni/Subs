# Subs

**Subs** is a subscription order management system for **infoproducts, digital courses, and recurring plans**.

It is designed as a reference project for a modern architecture based on **.NET, messaging, PostgreSQL, Docker, and React**.

---

## Purpose

- Manage subscription orders reliably and at scale.
- Allow creation, querying, and status tracking of orders.
- Integrate asynchronous processing via messaging (Azure Service Bus).
- Provide a well-defined REST API and an intuitive Frontend.

---

## How to Run

### 1. Apply Initial Database Migration

Before starting the system, apply the initial migration to set up the database schema:

```sh
docker compose up -d postgres
docker compose exec backend sh
dotnet ef database update
exit
```

*(If migrations are applied automatically by the backend container, you can skip this step.)*

---

### 2. Start All Services

Start the backend, frontend, worker, and database containers:

```sh
docker compose up --build
```

---

### 3. Access the Application

- **Frontend:**  
  [http://localhost:3000](http://localhost:3000)

- **API Endpoints:**  
  [http://localhost:5000/](http://localhost:5000/swagger)

- **Healthcheck:**  
  [http://localhost:5000/health](http://localhost:5000/health)

---

### 4. Add a Client

You can add a client via the API Swagger UI or by sending a POST request to:

```
POST http://localhost:5000/api/clients
```

**Example JSON:**
```json
{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com"
}
```

---

### 5. Add a Subscription

After creating a client, create a subscription via the API or frontend:

```
POST http://localhost:5000/api/subscriptions
```

**Example JSON:**
```json
{
  "clientId": "<client-id>",
  "productId": "course-001",
  "payment": {
    "method": "Credit",
    "frequency": "Monthly",
    "amount": 99.90,
    "currency": { "code": "BRL" }
  }
}
```

Or use the frontend UI at [http://localhost:3000](http://localhost:3000) to add subscriptions interactively.

---

## How to Use the Application Features

### 1. Creating a Subscription

- On the main page of the frontend ([http://localhost:3000](http://localhost:3000)), click the **"Create Subscription"** button.
- Fill in the required fields (Client, Product, Payment details).
- Submit the form.  
  The subscription will be created and sent for processing.  
  You can view the new subscription in the list after creation.

---

### 2. Viewing Subscription Details

- In the subscription list, each row has a **"Details"** button (often represented by an icon or link).
- Click the **Details** button to open a sidebar or modal with all information about the selected subscription.
- In the details view, you can see:
  - Subscription status and metadata
  - Payment information
  - **Event History**: All operations/events related to this subscription, including creation, processing, and rollbacks.

---

### 3. Monitoring the Bus Queue

- On the frontend, look for the **"Bus Monitor"** button (usually in the navigation bar or header).
- Click **Bus Monitor** to open the Subscription Bus Queue Monitor.
- This view shows all messages currently in the processing queue:
  - Subscription ID
  - Status (Processed, Failed, Processing, Received)
  - Created At
  - Operation (Create, Delete, etc.)
- You can filter by Subscription ID and refresh the queue to see real-time updates.

---

**Tip:**  
If you don't see new subscriptions or events, use the **Refresh** button in the Bus Monitor or Details sidebar to reload the latest data.

---

## Architecture

The project uses a **layered and service-oriented architecture**:

```mermaid
    FE[Frontend (React+Tailwind)]
    API[Subs.Api (ASP.NET Core Web API)]
    DB[(PostgreSQL)]
    WK[Subs.Worker (.NET Worker Service)]
    SB[(Azure Service Bus)]

    FE <--> API
    API <--> DB
    API -- "Publishes events" --> SB
    WK -- "Consumes events" --> SB
    WK <--> DB
```

### Components

- **Subs.Api**
  - Exposes the REST API
  - Persistence via `SubsDbContext` (EF Core + PostgreSQL)
  - Publishes events to Azure Service Bus (`SubscriptionMessage`)
  - Healthchecks and Swagger

- **Subs.Worker**
  - Azure Service Bus message consumer
  - Processes order creation events
  - Updates subscription status
  - Idempotency guaranteed by unique processing key

- **Subs.Core**
  - Implementations

- **Subs.Domain**
  - Interfaces
  - Models
  - Enums and domain rules

- **Frontend**
  - React + TailwindCSS
  - UI for creating and listing orders
  - Integration with REST API

---

## Technologies

- **Backend**
  - [.NET 9.0](https://dotnet.microsoft.com/)
  - [ASP.NET Core Web API](https://learn.microsoft.com/aspnet/core/web-api)
  - [Entity Framework Core](https://learn.microsoft.com/ef/core/)
  - [Npgsql (PostgreSQL Provider)](https://www.npgsql.org/efcore/index.html)
  - [Azure Service Bus SDK](https://learn.microsoft.com/azure/service-bus-messaging/service-bus-dotnet-get-started-with-queues)
  - [Serilog](https://serilog.net/)

- **Infrastructure**
  - [Docker](https://www.docker.com/)
  - [Docker Compose](https://docs.docker.com/compose/)
  - [PostgreSQL](https://www.postgresql.org/)
  - [PgAdmin](https://www.pgadmin.org/)

- **Frontend**
  - [React](https://react.dev/)
  - [Vite](https://vitejs.dev/)
  - [TailwindCSS](https://tailwindcss.com/)

---

## Repository Structure

```
Subs/
 ├── Backend/
 │   ├── Subs.Api/         # ASP.NET Core API
 │   ├── Subs.Worker/      # Worker for message consumption
 │   ├── Subs.Core/        # Implementations
 │   ├── Subs.Domain/      # Entities, enums, interfaces
 │   └── Subs.UnitTests/   # Unit tests
 ├── Frontend/
 │   └── Subs.Frontend/    # React Frontend
 ├── Deployment/
      └── docker-compose.yml # Container 
```

---

## How to Run Locally

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download)
- [Docker + Docker Compose](https://www.docker.com/)
- [Node.js 18+ (for frontend)](https://nodejs.org/)

### Steps

1. **Clone the repository:**
   ```bash
   git clone https://github.com/your-username/subs.git
   cd subs
   ```

2. **Configure environment variables:**
   ```bash
   cp .env.example .env
   ```

3. **Start all services:**
   ```bash
   docker compose up --build
   ```

   - This will build and start the API, Worker, PostgreSQL, and Frontend (if configured).
   - Initial database migrations are applied automatically by the backend container.

4. **Access:**
   - API Swagger → [http://localhost:5000/swagger](http://localhost:5000/swagger)
   - Healthcheck → [http://localhost:5000/health](http://localhost:5000/health)
   - Frontend → [http://localhost:3000](http://localhost:3000) (if running)

---

## Useful Know-Hows

- **Manual Migrations:**  
  If you need to apply migrations manually:
  ```sh
  docker compose exec backend sh
  dotnet ef database update
  ```
  *(Adjust for your ORM if different)*

- **Frontend Development:**  
  To run the frontend separately:
  ```sh
  cd Subs.Frontend
  npm install
  npm run dev
  ```
  The frontend will be available at [http://localhost:5173](http://localhost:5173) by default.

- **Stopping Containers:**
  ```sh
  docker compose down
  ```

- **Viewing Logs:**
  ```sh
  docker compose logs -f
  ```

- **Environment Variables:**  
  Always check and update your `.env` file for connection strings, secrets, and service bus settings.

- **PgAdmin Access:**  
  PgAdmin is available for database management. Default credentials are set in `.env.example`.

- **Testing:**  
  Run backend unit tests with:
  ```sh
  dotnet test
  ```

---

## Backend Project Creation

The backend solution was created with:

```sh
git init
dotnet new sln -n Subs

dotnet new webapi  -o Subs.Api
dotnet new worker  -o src/Subs.Worker
dotnet new classlib -o src/Subs.Core
dotnet new classlib -o src/Subs.Domain
dotnet new xunit -o tests/Subs.UnitTests

dotnet sln add src/Subs.Api/Subs.Api.csproj
dotnet sln add src/Subs.Worker/Subs.Worker.csproj
dotnet sln add src/Subs.Core/Subs.Core.csproj
dotnet sln add src/Subs.Domain/Subs.Domain.csproj
dotnet sln add tests/Subs.UnitTests/Subs.UnitTests.csproj

dotnet add src/Subs.Api reference src/Subs.Core
dotnet add src/Subs.Api reference src/Subs.Domain
dotnet add src/Subs.Worker reference src/Subs.Core
dotnet add src/Subs.Worker reference src/Subs.Domain
dotnet add tests/Subs.UnitTests reference src/Subs.Core
dotnet add tests/Subs.UnitTests reference src/Subs.Domain
```

Additional projects like `Subs.Utils` may be added as needed.

---

## Frontend Project Creation

The frontend was initialized with:

```sh
npm create vite@latest Subs.Frontend -- --template react
cd Subs.Frontend
npm install
npm install axios react-router-dom@6 @headlessui/react @heroicons/react
npm install -D tailwindcss postcss autoprefixer
npx tailwindcss init -p
```

---

## Troubleshooting

- **Database connection issues:**  
  Check your `.env` and Docker logs for errors.
- **Service Bus issues:**  
  Ensure your Azure Service Bus credentials are correct and the service is reachable.
- **Frontend not connecting to API:**  
  Confirm API URL in frontend `.env` or config files.
- **Migrations not running:**  
  Run migrations manually inside the backend container.

---

## Questions?

Check the configuration files (`docker-compose.yml`, `.env`, etc) or contact the project maintainer.