# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar arquivos de projeto e restaurar dependências
COPY ["src/SensorDataIngestion.Domain/SensorDataIngestion.Domain.csproj", "SensorDataIngestion.Domain/"]
COPY ["src/SensorDataIngestion.Application/SensorDataIngestion.Application.csproj", "SensorDataIngestion.Application/"]
COPY ["src/SensorDataIngestion.Infrastructure/SensorDataIngestion.Infrastructure.csproj", "SensorDataIngestion.Infrastructure/"]
COPY ["src/SensorDataIngestion.API/SensorDataIngestion.API.csproj", "SensorDataIngestion.API/"]

RUN dotnet restore "SensorDataIngestion.API/SensorDataIngestion.API.csproj"

# Copiar código fonte
COPY src/ .

# Build da aplicação
WORKDIR "/src/SensorDataIngestion.API"
RUN dotnet build "SensorDataIngestion.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "SensorDataIngestion.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Criar usuário não-root
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

COPY --from=publish /app/publish .

# Expor porta
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "SensorDataIngestion.API.dll"]
