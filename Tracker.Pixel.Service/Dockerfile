#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Tracker.Pixel.Service/Tracker.Pixel.Service.csproj", "Tracker.Pixel.Service/"]
RUN dotnet restore "Tracker.Pixel.Service/Tracker.Pixel.Service.csproj"
COPY . .
WORKDIR "/src/Tracker.Pixel.Service"
RUN dotnet build "Tracker.Pixel.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Tracker.Pixel.Service.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tracker.Pixel.Service.dll"]