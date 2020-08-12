FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS Build

COPY bin/Release/netcoreapp3.1/publish/ /App
WORKDIR /App

COPY ./WebUI.csproj ./

RUN dotnet restore

COPY . .

RUN dotnet build ./WebUI.csproj --output /build

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1

RUN apt update && \
    apt -y install gnupg2 bzip2 wget nodejs npm && \
    wget --quiet -O - https://www.postgresql.org/media/keys/ACCC4CF8.asc | apt-key add - && \
    echo "deb http://apt.postgresql.org/pub/repos/apt/ buster-pgdg main" | tee /etc/apt/sources.list.d/pgdg.list && \
    apt update && \
    apt install postgresql-client-12 -y
    
RUN npm -y install typescript @types/react @types/react-dom 
    
WORKDIR /App
COPY --from=Build /build ./ 

ENTRYPOINT ["dotnet", "WebUI.dll"]