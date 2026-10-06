FROM mcr.microsoft.com/dotnet/sdk:10.0

RUN dotnet tool install --tool-path /tools Microsoft.SqlPackage

ENV PATH="/tools:${PATH}"

ENTRYPOINT ["sqlpackage"]
