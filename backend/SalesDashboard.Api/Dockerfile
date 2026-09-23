FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем csproj относительно контекста сборки (context = ./backend)
COPY SalesDashboard.Api/SalesDashboard.Api.csproj SalesDashboard.Api/
RUN dotnet restore SalesDashboard.Api/SalesDashboard.Api.csproj

# Копируем весь код backend-а
COPY . .

# Публикуем в Release
RUN dotnet publish SalesDashboard.Api/SalesDashboard.Api.csproj -c Release -o /app --no-restore

# Финальный образ
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app ./

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "SalesDashboard.Api.dll"]