FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Subs.Worker.dll"]