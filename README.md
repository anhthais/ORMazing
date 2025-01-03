# ORMazing

ORMazing is a lightweight ORM framework designed to handle database operations in .NET using ADO.NET. It supports basic CRUD operations, connection management, and entity mapping without relying on external ORM libraries.

## Features

## Prerequisites

- `.NET 6` or later
- Docker (for running SQL Server in a container)

## Project Structure

- ORMazing (Class Library): Contains the core ORM logic including database connections, query building, entity mapping, and repository patterns.
- DemoProgram (Console Application): A demo program that demonstrates how to use ORMazing with SQL Server and a sample database.

## Run DemoProgram

1. Run Docker Container for SQL Server

```
cd .\DemoProgram
docker-compose up -d
```

This will pull the latest SQL Server image from Docker Hub, create a container, expose it on port 1433 and initialize `DemoDB`.

2. Build and run the solution
