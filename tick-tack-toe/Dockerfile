#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["tick-tack-toe/tick-tack-toe.csproj", "tick-tack-toe/"]
RUN dotnet restore "tick-tack-toe/tick-tack-toe.csproj"
COPY . .
WORKDIR "/src/tick-tack-toe"
RUN dotnet build "tick-tack-toe.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "tick-tack-toe.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "tick-tack-toe.dll"]