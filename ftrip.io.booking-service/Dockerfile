FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build
WORKDIR /build

ARG CONFIGURATION=Release

# Restore dependencies
COPY ["ftrip.io.booking-service.contracts/*.csproj", "ftrip.io.booking-service.contracts/package*.json", "./ftrip.io.booking-service.contracts/"]
RUN dotnet restore /p:Configuration=$CONFIGURATION ftrip.io.booking-service.contracts/ftrip.io.booking-service.contracts.csproj

COPY ["ftrip.io.booking-service/*.csproj", "ftrip.io.booking-service/package*.json", "./ftrip.io.booking-service/"]
RUN dotnet restore /p:Configuration=$CONFIGURATION ftrip.io.booking-service/ftrip.io.booking-service.csproj

# Publish
COPY . .

RUN dotnet publish -c $CONFIGURATION -o out ftrip.io.booking-service/ftrip.io.booking-service.csproj

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine AS final
WORKDIR /app

RUN addgroup --system ftrip-io && adduser --system booking-service

COPY --from=build --chown=booking-service:ftrip-io --chmod=777 /build/out .

EXPOSE 80

USER booking-service

ENTRYPOINT ["dotnet", "ftrip.io.booking-service.dll"]