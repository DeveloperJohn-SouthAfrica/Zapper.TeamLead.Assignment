# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["AcmeIntegreatedAssessement.Api/AcmeIntegreatedAssessement.Api.csproj", "AcmeIntegreatedAssessement.Api/"]
COPY ["AcmeIntegreatedAssessement.Application/AcmeIntegreatedAssessement.Application.csproj", "AcmeIntegreatedAssessement.Application/"]
COPY ["AcmeIntegreatedAssessement.Infrastructure/AcmeIntegreatedAssessement.Infrastructure.csproj", "AcmeIntegreatedAssessement.Infrastructure/"]
COPY ["AcmeIntegreatedAssessement.Domain/AcmeIntegreatedAssessement.Domain.csproj", "AcmeIntegreatedAssessement.Domain/"]
RUN dotnet restore "./AcmeIntegreatedAssessement.Api/AcmeIntegreatedAssessement.Api.csproj"
COPY . .
WORKDIR "/src/AcmeIntegreatedAssessement.Api"
RUN dotnet build "./AcmeIntegreatedAssessement.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AcmeIntegreatedAssessement.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
USER root
WORKDIR /app
RUN apt-get update \
    && apt-get install -y --no-install-recommends openssh-server procps \
    && rm -rf /var/lib/apt/lists/* \
    && mkdir -p /var/run/sshd \
    && ssh-keygen -A
COPY --from=publish /app/publish .
COPY AcmeIntegreatedAssessement.Api/scripts/start-dotnet.sh /scripts/start-dotnet.sh
RUN chmod +x /scripts/start-dotnet.sh
ENV SSH_ROOT_PASSWORD=ZapperSSH! \
    ASPNETCORE_URLS=http://+:8080 \
    DOTNET_USE_POLLING_FILE_WATCHER=1
EXPOSE 8080
EXPOSE 8081
EXPOSE 2222
ENTRYPOINT ["/scripts/start-dotnet.sh"]
