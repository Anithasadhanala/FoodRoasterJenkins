# Use the official .NET SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the FoodRoasterServer.csproj and restore dependencies
COPY FoodRoasterServer.csproj .
RUN dotnet restore

# Copy the rest of the application code
COPY . .

# Build the application
RUN dotnet build "FoodRoasterServer.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "FoodRoasterServer.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Use the official ASP.NET Core runtime image for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Set the ASP.NET Core environment to Production
ENV ASPNETCORE_ENVIRONMENT=Production

# Expose the port the application listens on
EXPOSE 8080

# Create the Logs directory for Serilog
RUN mkdir -p Logs

# Copy the published application from the build stage
COPY --from=publish /app/publish .

# Define the entry point for the application
ENTRYPOINT ["bash", "-c", "dotnet FoodRoasterServer.dll seed && dotnet FoodRoasterServer.dll"]