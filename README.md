# Subs

O **Subs** é um sistema de gestão de pedidos de assinatura para **infoprodutos, cursos digitais e planos de recorrência**.

Ele foi desenvolvido como um projeto de referência para uma arquitetura moderna baseada em **.NET, mensageria, PostgreSQL e Docker**.

---

## Objetivo
- Gerenciar ordens de assinatura de forma confiável e escalável.
- Permitir criação, consulta e acompanhamento do status dos pedidos.
- Integrar processamento assíncrono via mensageria (Azure Service Bus).
- Fornecer uma API REST bem definida e um Frontend intuitivo.

---

## Arquitetura

O projeto adota uma **arquitetura em camadas e serviços**:

```mermaid
    FE[Frontend (React+Tailwind)]
    API[Subs.Api (ASP.NET Core Web API)]
    DB[(PostgreSQL)]
    WK[Subs.Worker (.NET Worker Service)]
    SB[(Azure Service Bus)]

    FE <--> API
    API <--> DB
    API -- "Publica eventos" --> SB
    WK -- "Consome eventos" --> SB
    WK <--> DB
```

### Componentes
- **Subs.Api**
  - Exposição da API REST
  - Persistência via `SubsDbContext` (EF Core + PostgreSQL)
  - Publicação de eventos no Azure Service Bus (`OrderCreated`)
  - Healthchecks e Swagger

- **Subs.Worker**
  - Consumidor de mensagens do Azure Service Bus
  - Processa eventos de criação de pedidos
  - Atualiza status da assinatura (`Pendente → Ativo → Finalizado`)
  - Idempotência garantida por chave única no processamento

- **Subs.Core**
  - Implementações

  **Subs.Domain**
  - Interfaces
  - Modelos
  - Enums e regras de domínio

- **Frontend (futuro)**
  - Interface para criação e listagem de pedidos
  - Integração com API REST

---

## Tecnologias
- **Backend**
  - [.NET 9.0](https://dotnet.microsoft.com/)
  - [ASP.NET Core Web API](https://learn.microsoft.com/aspnet/core/web-api)
  - [Entity Framework Core](https://learn.microsoft.com/ef/core/)
  - [Npgsql (PostgreSQL Provider)](https://www.npgsql.org/efcore/index.html)
  - [Azure Service Bus SDK](https://learn.microsoft.com/azure/service-bus-messaging/service-bus-dotnet-get-started-with-queues)
  - [Serilog](https://serilog.net/)

- **Infraestrutura**
  - [Docker](https://www.docker.com/)
  - [Docker Compose](https://docs.docker.com/compose/)
  - [PostgreSQL](https://www.postgresql.org/)
  - [PgAdmin](https://www.pgadmin.org/)

- **Frontend (em desenvolvimento)**
  - [React](https://react.dev/)
  - [Vite](https://vitejs.dev/)
  - [TailwindCSS](https://tailwindcss.com/)

---

## Estrutura do Repositório

```
subs/
 ├── src/
 │   ├── Subs.Api/         # API ASP.NET Core
 │   ├── Subs.Worker/      # Worker para consumo de mensagens
 │   ├── Subs.Core/        # Immplementações
 │   ├── Subs.Domain/      # Entidades, enums e interfaces
 │   └── Subs.Frontend/    # (futuro) Frontend React
 ├── tests/
 │   └── Subs.UnitTests/   # Testes unitários
 ├── docker-compose.yml    # Orquestração de containers
 ├── .env.example          # Variáveis de ambiente
 ├── README.md             # Este arquivo
 └── Subs.sln              # Solution .NET
```

---

## Como rodar localmente

### Pré-requisitos
- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download)
- [Docker + Docker Compose](https://www.docker.com/)
- [Node.js 18+ (para o frontend futuramente)](https://nodejs.org/)

### Passos
1. Clone o repositório:
   ```bash
   git clone https://github.com/seu-usuario/subs.git
   cd subs
   ```

2. Configure as variáveis de ambiente:
   ```bash
   cp .env.example .env
   ```

3. Suba os serviços:
   ```bash
   docker compose up --build
   ```

4. Acesse:
   - API Swagger → [http://localhost:5000/swagger](http://localhost:5000/swagger)
   - Healthcheck → [http://localhost:5000/health](http://localhost:5000/health)

---

## Processo de criação do projeto
### Backend
A solução de backend foi criada com os seguintes comands:

```
git init
dotnet new sln -n Subs

dotnet new webapi  -o Order.Api
dotnet new worker  -o src/Order.Worker
dotnet new classlib -o src/Order.Core
dotnet new xunit -o tests/Order.UnitTests

dotnet sln add src/Order.Api/src/Order.Api.csproj
dotnet sln add src/Order.Worker/src/Order.Worker.csproj
dotnet sln add src/Order.Core/src/Order.Core.csproj
dotnet sln add tests/Order.UnitTests/tests/Order.UnitTests.csproj

dotnet add src/Order.Api reference src/Order.Core
dotnet add src/Order.Worker reference src/Order.Core
dotnet add tests/Order.UnitTests reference src/Order.Core
```

É importante notar que a Solução recebeu algumas atualizações e adições de Projetos ao longo do desenvolvimento, como o `Subs.Utils`.

### Frontend
A criação inicial do projeto foi feita com os seguintes comandos:
```
npm create vite@latest Subs.Frontend -- --template react
cd Subs.Frontend
npm install
npm install axios react-router-dom@6 @headlessui/react @heroicons/react
npm install -D tailwindcss postcss autoprefixer
npx tailwindcss init -p
```
