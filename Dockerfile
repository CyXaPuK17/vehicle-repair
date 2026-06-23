FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/VehicleRepair.Domain/VehicleRepair.Domain.csproj             src/VehicleRepair.Domain/
COPY src/VehicleRepair.Application/VehicleRepair.Application.csproj   src/VehicleRepair.Application/
COPY src/VehicleRepair.Infrastructure/VehicleRepair.Infrastructure.csproj src/VehicleRepair.Infrastructure/
COPY src/VehicleRepair.API/VehicleRepair.API.csproj                   src/VehicleRepair.API/

RUN dotnet restore src/VehicleRepair.API/VehicleRepair.API.csproj

COPY src/ src/

RUN dotnet publish src/VehicleRepair.API/VehicleRepair.API.csproj \
    -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "VehicleRepair.API.dll"]
