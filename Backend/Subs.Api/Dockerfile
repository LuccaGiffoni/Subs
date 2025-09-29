# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution + project files (so restore can run)
COPY Subs.sln ./
COPY Subs.Api/Subs.Api.csproj Subs.Api/
COPY Subs.Worker/Subs.Worker.csproj Subs.Worker/
COPY Subs.Core/Subs.Core.csproj Subs.Core/
COPY Subs.Domain/Subs.Domain.csproj Subs.Domain/
COPY Subs.Utils/Subs.Utils.csproj Subs.Utils/
# ⛔ Optional: if you don’t want UnitTests in Docker, skip this line
COPY Subs.UnitTests/Subs.UnitTests.csproj Subs.UnitTests/

# Restore packages
RUN dotnet restore Subs.sln

# Copy the rest of the source code
COPY . .

# Publish only the API
RUN dotnet publish Subs.Api/Subs.Api.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "Subs.Api.dll"]
