#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
EXPOSE 4430

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["DiplomaMarketBackend/DiplomaMarketBackend.csproj", "DiplomaMarketBackend/"]
RUN dotnet restore "DiplomaMarketBackend/DiplomaMarketBackend.csproj"
COPY . .
WORKDIR "/src/DiplomaMarketBackend"
RUN dotnet build "DiplomaMarketBackend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DiplomaMarketBackend.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DiplomaMarketBackend.dll"]