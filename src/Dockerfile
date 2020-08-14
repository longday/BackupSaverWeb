FROM longday/asp-alpine:3.1.401-build AS build-env
ARG APP_VERSION
WORKDIR /app
COPY ./Infrastructure/Infrastructure.csproj ./Infrastructure/Infrastructure.csproj
COPY ./Server/Server.csproj ./Server/Server.csproj
RUN dotnet restore Server
COPY . .
RUN dotnet publish Server --no-restore -c Release --no-self-contained -r linux-musl-x64 -o /app/out /p:Version=${APP_VERSION}

FROM longday/asp-alpine:3.1.401-run
WORKDIR /app
COPY --from=build-env /app/out .
EXPOSE 80
CMD ["dotnet", "Server.dll"]

