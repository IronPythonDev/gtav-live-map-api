#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["GTAVLiveMap.TelegramBot/GTAVLiveMap.TelegramBot.csproj", "GTAVLiveMap.TelegramBot/"]
COPY ["GTAVLiveMap.Core.Infrastructure/GTAVLiveMap.Core.Infrastructure.csproj", "GTAVLiveMap.Core.Infrastructure/"]
COPY ["GTAVLiveMap.Domain/GTAVLiveMap.Domain.csproj", "GTAVLiveMap.Domain/"]
RUN dotnet restore "GTAVLiveMap.TelegramBot/GTAVLiveMap.TelegramBot.csproj"
COPY . .
WORKDIR "/src/GTAVLiveMap.TelegramBot"
RUN dotnet build "GTAVLiveMap.TelegramBot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GTAVLiveMap.TelegramBot.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GTAVLiveMap.TelegramBot.dll"]