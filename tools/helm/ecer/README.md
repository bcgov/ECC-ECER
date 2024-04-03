# ECER Helm Chart

This directory contains a Helm chart to deploy ECER

## Usage

To update or install the chart's dependencies run the following command:

```sh
helm dep update
```

To install a new environment, ensure the [env]_values.yaml matches the environment, then run the following command:

```sh
helm -n [namespace] install [env name] .
```

To upgrade an existing environment, run the following command:

```sh
helm -n [namespace] upgrade [env name] .
```
