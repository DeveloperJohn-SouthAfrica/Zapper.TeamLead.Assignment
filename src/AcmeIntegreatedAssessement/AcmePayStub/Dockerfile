FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["AcmePayStub/AcmePayStub.csproj", "AcmePayStub/"]
RUN dotnet restore "AcmePayStub/AcmePayStub.csproj"
COPY AcmePayStub/ "AcmePayStub/"
WORKDIR /src/AcmePayStub
RUN dotnet publish "AcmePayStub.csproj" -c Release -o /app/publish --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet", "AcmePayStub.dll"]
