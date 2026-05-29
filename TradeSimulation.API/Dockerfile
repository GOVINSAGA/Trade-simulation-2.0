FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ["TradeSimulation.API.csproj", "./"]
RUN dotnet restore "TradeSimulation.API.csproj"

# Copy everything else and build
COPY . .
RUN dotnet publish "TradeSimulation.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine
WORKDIR /app
COPY --from=build /app/publish .

# Expose port
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "TradeSimulation.API.dll"]
