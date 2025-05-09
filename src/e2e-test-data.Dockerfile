FROM mcr.microsoft.com/dotnet/sdk:8.0 AS net-builder

# install diagnostics tools
RUN mkdir /tools && \
    dotnet tool install --tool-path /tools dotnet-trace && \
    dotnet tool install --tool-path /tools dotnet-counters && \
    dotnet tool install --tool-path /tools dotnet-dump

# install node.js
ARG NODE_MAJOR=20
RUN curl -SLO https://deb.nodesource.com/nsolid_setup_deb.sh \
    && chmod 500 nsolid_setup_deb.sh \
    && ./nsolid_setup_deb.sh ${NODE_MAJOR} \
    && apt-get install nodejs -y --no-install-recommends \
    && apt-get clean

WORKDIR /src
# Copy the main source project files
COPY ["ECER.sln", ".editorconfig", "Directory.Build.props", "Directory.Packages.props", "global.json", "./"]
COPY */*.csproj ./
COPY */*/*.csproj ./
COPY */*/*.esproj ./
RUN cat ECER.sln \
| grep "\.*sproj" \
| awk '{print $4}' \
| sed -e 's/[",]//g' \
| sed 's#\\#/#g' \
| xargs -I % sh -c 'mkdir -p $(dirname %) && mv $(basename %) $(dirname %)/'

# restore nuget packages
RUN dotnet restore "ECER.Clients.E2ETestData/ECER.Clients.E2ETestData.csproj" -r linux-x64 -p:PublishReadyToRun=true
RUN dotnet restore "ECER.Tests/ECER.Tests.csproj" -r linux-x64 -p:PublishReadyToRun=true

COPY . .

# run unit tests
RUN dotnet test "ECER.Tests/ECER.Tests.csproj" --filter "Category!=IntegrationTest" -r linux-x64 -p:PublishReadyToRun=true

# build publish
RUN dotnet publish "ECER.Clients.E2ETestData/ECER.Clients.E2ETestData.csproj" -c Release -o /app/publish --no-restore --self-contained -r linux-x64 -p:PublishReadyToRun=true

FROM registry.access.redhat.com/ubi8/dotnet-80-runtime:8.0 AS final
ARG VERSION
ENV VERSION=$VERSION
ARG TIMESTAMP
ENV TIMESTAMP=$TIMESTAMP
ARG COMMIT
ENV COMMIT=$COMMIT

WORKDIR /app
# copy diagnostics tools
WORKDIR /tools
COPY --from=net-builder /tools .
# copy app
WORKDIR /app
COPY --from=net-builder /app/publish .
ENTRYPOINT ["dotnet", "ECER.Clients.E2ETestData.dll"]
