FROM mcr.microsoft.com/dotnet/sdk:8.0 AS net-builder

# path to secrets json file for tests
ENV SECRETS_FILE_PATH=

# install node.js
ARG NODE_MAJOR=22
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
RUN dotnet restore "ECER.Tests/ECER.Tests.csproj"
COPY . .
# build the project
RUN dotnet build "ECER.Tests/ECER.Tests.csproj"

ENTRYPOINT ["dotnet", "test", "--filter", "Category!=Internal", "ECER.Tests/ECER.Tests.csproj"]
