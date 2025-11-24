# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution file
COPY ["ElectroHuila.sln", "./"]
COPY ["Directory.Build.props", "./"]

# Copy project files
COPY ["src/1. Core/ElectroHuila.Domain/ElectroHuila.Domain.csproj", "src/1. Core/ElectroHuila.Domain/"]
COPY ["src/1. Core/ElectroHuila.Application/ElectroHuila.Application.csproj", "src/1. Core/ElectroHuila.Application/"]
COPY ["src/2. Infrastructure/ElectroHuila.Infrastructure/ElectroHuila.Infrastructure.csproj", "src/2. Infrastructure/ElectroHuila.Infrastructure/"]
COPY ["src/2. Infrastructure/ElectroHuila.Infrastructure.External/ElectroHuila.Infrastructure.External.csproj", "src/2. Infrastructure/ElectroHuila.Infrastructure.External/"]
COPY ["src/3. Presentation/ElectroHuila.WebApi/ElectroHuila.WebApi.csproj", "src/3. Presentation/ElectroHuila.WebApi/"]

# Restore dependencies
RUN dotnet restore "src/3. Presentation/ElectroHuila.WebApi/ElectroHuila.WebApi.csproj"

# Copy the rest of the source code
COPY . .

# Build the application
WORKDIR "/src/src/3. Presentation/ElectroHuila.WebApi"
RUN dotnet build "ElectroHuila.WebApi.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "ElectroHuila.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Install curl for health checks
RUN apt-get update && \
    apt-get install -y curl && \
    rm -rf /var/lib/apt/lists/*

# Copy published application
COPY --from=publish /app/publish .

# Create directories for logs
RUN mkdir -p /app/logs

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=QA
ENV ASPNETCORE_URLS=http://+:5000

# Expose port
EXPOSE 5000

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \
    CMD curl -f http://localhost:5000/health || exit 1

ENTRYPOINT ["dotnet", "ElectroHuila.WebApi.dll"]
