FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Shop/Shop.csproj", "Shop/"]
RUN dotnet restore "Shop/Shop.csproj"
COPY . .
WORKDIR "/src/Shop"
RUN dotnet build "Shop.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Shop.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Shop.dll"]
