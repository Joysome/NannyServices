# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files first for better layer caching
COPY ["Nanny.Admin.Api/Nanny.Admin.Api.csproj", "Nanny.Admin.Api/"]
COPY ["Nanny.Admin.Application/Nanny.Admin.Application.csproj", "Nanny.Admin.Application/"]
COPY ["Nanny.Admin.Domain/Nanny.Admin.Domain.csproj", "Nanny.Admin.Domain/"]
COPY ["Nanny.Admin.Infrastructure/Nanny.Admin.Infrastructure.csproj", "Nanny.Admin.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "Nanny.Admin.Api/Nanny.Admin.Api.csproj"

# Copy source code
COPY . .

# Build the application
RUN dotnet build "Nanny.Admin.Api/Nanny.Admin.Api.csproj" -c Release --no-restore

# Publish stage
FROM build AS publish
RUN dotnet publish "Nanny.Admin.Api/Nanny.Admin.Api.csproj" -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && \
    rm -rf /var/lib/apt/lists/* && \
    apt-get clean

# Create non-root user for security
RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

# Copy published application
COPY --from=publish --chown=appuser:appuser /app/publish .

# Expose ports
EXPOSE 8080
EXPOSE 8081

# Start the application
ENTRYPOINT ["dotnet", "Nanny.Admin.Api.dll"]
