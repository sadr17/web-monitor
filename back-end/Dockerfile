FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /back-end
COPY *.sln ./
COPY ./src/api/WebMonitorApi/WebMonitorApi.csproj ./src/api/WebMonitorApi/
COPY ./src/monitor/Monitor/Monitor.csproj ./src/monitor/Monitor/
COPY ./tests/WebMonitor.Tests/WebMonitor.Tests.csproj ./tests/WebMonitor.Tests/WebMonitor.Tests.csproj
RUN dotnet restore
COPY . .
RUN dotnet build WebMonitor.sln -c Release -o /app

FROM build AS publish
RUN dotnet publish WebMonitor.sln -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "WebMonitorApi.dll"]
