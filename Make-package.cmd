@echo off
dotnet build -c Release src/PgpSharp/PgpSharp.csproj
dotnet pack -c Release /p:ContinuousIntegrationBuild=true -o ./build src/PgpSharp/PgpSharp.csproj
