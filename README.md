![Banner](Images/Banner.png)

# SmartMigrations

[![SmartMigrations NuGet Package](https://img.shields.io/nuget/v/SmartMigrations.svg)](https://www.nuget.org/packages/SmartMigrations/) [![SmartMigrations NuGet Package Downloads](https://img.shields.io/nuget/dt/SmartMigrations)](https://www.nuget.org/packages/SmartMigrations) [![GitHub Actions Status](https://github.com/Xkonti/smart-migrations-net/workflows/Build/badge.svg?branch=main)](https://github.com/Xkonti/smart-migrations-net/actions)

[![GitHub Actions Build History](https://buildstats.info/github/chart/Xkonti/smart-migrations-net?branch=main&includeBuildsFromPullRequest=false)](https://github.com/Xkonti/smart-migrations-net/actions)


A database-agnostic migration planning and execution library for .NET that intelligently finds the optimal path between database schema versions.

## Overview

SmartMigrations provides a flexible, class-based approach to database schema migrations with intelligent path planning. Unlike traditional linear migration systems, SmartMigrations allows you to define multiple migration paths and automatically calculates the most efficient route to your target schema version.

## Key Features

### 🎯 **Class-per-Migration Architecture**
Each migration is defined as a separate class with two key attributes:
- **From**: The source version this migration can upgrade from
- **To**: The target version this migration upgrades to

### 🔄 **Flexible Migration Direction**
Each migration class can implement:
- `Apply()` - Forward migration logic
- `Revert()` - Rollback migration logic
- One-way migrations (implement only `Apply()` or only `Revert()`)

### 🚀 **Intelligent Path Planning**
- Uses **Dijkstra's algorithm** to find the shortest migration path
- Supports migration consolidation for performance optimization
- Handles complex migration scenarios with multiple possible routes
- Can mark problematic migrations as "avoid" while maintaining fallback options

### 🗃️ **Database Agnostic**
- You provide the actual migration implementation code
- SmartMigrations handles the planning and orchestration
- Works with any database system (SQL Server, PostgreSQL, SurrealDB, MongoDB, etc.)

### 📈 **Migration Consolidation**
Create consolidated migrations that combine multiple smaller ones:
- Correct mistakes from previous migrations
- Improve migration performance
- Simplify complex migration chains
- Avoid problematic migration paths

## How It Works

1. **Define your migrations** as classes with `From` and `To` version attributes
2. **Implement Apply/Revert methods** with your database-specific logic
3. **Let SmartMigrations plan** the optimal path to your target version
4. **Execute the planned migration** sequence automatically

Whether you're migrating up to the latest version or rolling back to a previous state, SmartMigrations finds the most efficient path through your migration graph.
