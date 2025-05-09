# Contributing to this project

## How to contribute

Government employees, public and members of the private sector are encouraged to contribute to the repository by **forking and submitting a pull request**.

(If you are new to GitHub, you might start with a [basic tutorial](https://help.github.com/articles/set-up-git) and check out a more detailed guide to [pull requests](https://help.github.com/articles/using-pull-requests/).)

Pull requests will be evaluated by the repository guardians on a schedule and if deemed beneficial will be committed to the master.

All contributors retain the original copyright to their stuff, but by contributing to this project, you grant a world-wide, royalty-free, perpetual, irrevocable, non-exclusive, transferable license to all users **under the terms of the license under which this project is distributed.**

Below are instructions and guidelines how to set up a development environment, and some principles and guidelines around contributing to the repository.

## Developer setup

Required toolset:

- Microsoft Visual Studio 2022+, JetBrains Rider 2023.3+, or VS.Code with C# Dev Kit with .net 8 SDK
- VS.Code with Volar, Prettier, and ESLint plugins
- [XrmToolBox](https://www.xrmtoolbox.com/) with [Early Bound Generator V2](https://github.com/daryllabar/DLaB.Xrm.XrmToolBoxTools/wiki/Early-Bound-Generator) plugin
- node.js 20.10+ + npm

## Directory structure

All the functional code is located in [src](./src). The `ECER.sln` is the main Visual Studio solution (will aslo be used by Rider and C# Dev Kit). this folder also contains the shared files for the dotnet compiler (\*.props) and style.

the [tools](./tools) directory contains various tools to support this repository:

- [helm](./tools/helm): contains helm charts to deploy the project into OpenShift
  - [ecer](./tools/helm/ecer): the main chart for the application and services
  - [tools](./tools/helm/tools): supporting chart for various artifacts required by the main chart

Consult the relevant readme for more information

## Build and test

The build process is designed to enable an equivalent developer experience using the installed .net 8 sdk as well as dockerfile build. There is a specific dockerfile for each deployable unit which also contains the

- Open the solution in your favourite IDE
  - run the unit and integration tests
  - run a docker build to run all code quality tests and build scripts

## Running locally

### Registry Portal

- override the missing secrets in `ECER.Clients.RegistryPortal.Server` project's secrets.json
- set `ECER.Clients.RegistryPortal.Server` as the startup project and run
- open `http://localhost:5121` in your favourite web browser

## Code Quality

Code quality is enforced automatically at build time. Each PR will trigger a build that will ensure the below rules are enforced, and will fail on any errors. It is recommended to build the C# projects locally, fix all warnings, and also run `npm run lintfix` before requesting a PR.

### C# services

All projects must use the global `global.json` and will inherit from `Directory.Build.props` their shared properties and settings.

`.editorconfig` file contains ale the code analysis and formatting rules for the projects.

Nuget packages are managed centraly and the version is set in `Directory.Packages.props`.

### Vue apps

All Vue projects use `prettier` and `eslint`. `.eslintrc.js` contains the project specific settings and extends the recommended Vue lint settings.

## Versioning

Please refer to our confluence page for information on versioning and deployment https://eccbc.atlassian.net/wiki/spaces/ECER/pages/596902589/Versioning+and+Deployment

## Contribute code changes

This project follows [github flow](https://docs.github.com/en/get-started/quickstart/github-flow#following-github-flow). To contribute code changes, submit a PR for the code owners' review.

All commits should be descriptive of the change and reference the original JIRA ticket. The branch name should also reflect the type of the ticket:

- tasks: `tasks/ECER-[###]`
- stories: `stories/ECER-[###]`
- bugs: `bugs/ECER-[###]`

When merging a PR, the code owners will prefer to squash the commits into a single merge, and will use GitHub automated commit message generation, so be mindful of your commit messages. In case of smaller changes, the code owners may decide not to squash.

### C# services tests

It is the contributing developers responsibility to ensure that modified, added or removed endpoint(s) have corresponding integration tests.

**NOTE**: This is _not_ included in the code quality checks at build time. Developers are expected to run tests locally to ensure all tests are passing before PR.

### Vue apps tests

TBD

## CI/CD

Each synced PR will trigger checks by building the relevant component's dockerfile. Dockerfiles contain the build scripts and invoke various code quality functions such as running tests, lint and similar. This means that in order to produce a deployable component, the docker build process much complete successfully. It also means the build process as it runs in GH Actions can be replicated locally by a developer using Docker/Podman engine.

A successful build after merging to the master branch will tag the new image with `latest` tag. This tag will automatically trigger a deployment to the development environment. To promote to higher environments, please see GitOps documentation.

## GitOps

The repository contains 2 helm charts: `ecer` for the main services and components, and `tools` for supporting CRDs.

There are 3 environments in OpenShift - dev, test and prod. Deployment is done by using ImageStream tags and DeploymentConfig triggers. The Tools namespace contains Tekton workflows to automatically backup and promote image tags between environments. The `tools` chart installs them in OpenShift.
