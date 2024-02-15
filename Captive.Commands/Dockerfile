#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Debug
WORKDIR /src
COPY ["Captive.Commands/Captive.Commands.csproj", "Captive.Commands/"]
COPY ["Captive.Applications/Captive.Applications.csproj", "Captive.Applications/"]
COPY ["Captive.Data/Captive.Data.csproj", "Captive.Data/"]
COPY ["Captive.Processing/Captive.Processing.csproj", "Captive.Processing/"]
RUN dotnet restore "./Captive.Commands/./Captive.Commands.csproj"
COPY . .
WORKDIR "/src/Captive.Commands"
RUN dotnet build "./Captive.Commands.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Captive.Commands.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Captive.Commands.dll"]