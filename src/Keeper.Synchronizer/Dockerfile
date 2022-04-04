FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "src/Keeper.Synchronizer/Keeper.Synchronizer.csproj"
WORKDIR "/src/src/Keeper.Synchronizer"
RUN dotnet build "Keeper.Synchronizer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Keeper.Synchronizer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Keeper.Synchronizer.dll"]
