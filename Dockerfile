FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build

RUN curl --silent --location https://deb.nodesource.com/setup_10.x | bash -
RUN apt-get install --yes nodejs

WORKDIR /BackupSaverWeb
COPY . .

RUN dotnet restore "./WebUI.csproj"

RUN dotnet publish "WebUI.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1

RUN apt update && \
    apt -y install gnupg2 bzip2 wget && \
    wget --quiet -O - https://www.postgresql.org/media/keys/ACCC4CF8.asc | apt-key add - && \
    echo "deb http://apt.postgresql.org/pub/repos/apt/ buster-pgdg main" | tee /etc/apt/sources.list.d/pgdg.list && \
    apt update && \
    apt install postgresql-client-12 -y

COPY --from=build /app/publish .

WORKDIR /app

ENTRYPOINT ["dotnet", "WebUI.dll"]