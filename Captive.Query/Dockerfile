#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Development
WORKDIR /src
COPY ["Captive.Query/Captive.Queries.csproj", "Captive.Query/"]
COPY ["Captive.Applications/Captive.Applications.csproj", "Captive.Applications/"]
COPY ["Captive.Data/Captive.Data.csproj", "Captive.Data/"]
COPY ["Captive.Processing/Captive.Processing.csproj", "Captive.Processing/"]
COPY ["Captive.Reports/Captive.Reports.csproj", "Captive.Reports/"]
RUN dotnet restore "./Captive.Query/Captive.Queries.csproj"
COPY . .
WORKDIR "/src/Captive.Query"
RUN dotnet build "./Captive.Queries.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Development
RUN dotnet publish "./Captive.Queries.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Captive.Queries.dll"]