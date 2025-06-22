#!/bin/bash

# Exit on any error
set -e

# Check if required environment variables are set
if [ -z "$DB_PASSWORD" ]; then
    echo "ERROR: DB_PASSWORD environment variable is not set"
    exit 1
fi

if [ -z "$DB_NAME" ]; then
    echo "ERROR: DB_NAME environment variable is not set"
    exit 1
fi

echo "Running migrations (will create database if it doesn't exist)..."
dotnet ef database update --project Nanny.Admin.Infrastructure --startup-project Nanny.Admin.Api --connection "Server=sqlserver;Database=${DB_NAME};User=sa;Password=${DB_PASSWORD};TrustServerCertificate=True;"
echo "Database setup and migrations completed successfully!" 