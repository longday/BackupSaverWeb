FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS Build


COPY bin/Release/netcoreapp3.1/publish/ /App
WORKDIR /App

COPY ./BackupSaver.csproj ./

RUN dotnet restore

COPY . .

RUN dotnet build ./BackupSaver.csproj --output /build

FROM mcr.microsoft.com/dotnet/core/runtime:3.1

RUN apt update && \
    apt -y install gnupg2 bzip2 wget && \
    wget --quiet -O - https://www.postgresql.org/media/keys/ACCC4CF8.asc | apt-key add - && \
    echo "deb http://apt.postgresql.org/pub/repos/apt/ buster-pgdg main" | tee /etc/apt/sources.list.d/pgdg.list && \
    apt update && \
    apt install postgresql-client-12 -y
    
WORKDIR /App
COPY --from=Build /build ./ 

ENTRYPOINT ["dotnet", "BackupSaver.dll"]