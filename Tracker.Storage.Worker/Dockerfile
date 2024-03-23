#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Tracker.Storage.Worker/Tracker.Storage.Worker.csproj", "Tracker.Storage.Worker/"]
RUN dotnet restore "Tracker.Storage.Worker/Tracker.Storage.Worker.csproj"
COPY . .
WORKDIR "/src/Tracker.Storage.Worker"
RUN dotnet build "Tracker.Storage.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Tracker.Storage.Worker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tracker.Storage.Worker.dll"]