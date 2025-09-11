# ECER Tools Helm Chart

This chart contains tools specific resources to support ECER deployments.

## Usage

To install or upgrade, run the following command :

To upgrade/install a new environment, ensure the values.yaml matches the environment, navigate to the values.yaml file in this folder then run the following command:

helm upgrade --install -f values.yaml <<helm-name>> . -n <<name-space>>

Example:

```sh
helm upgrade --install -f values.yaml imagestreams . -n -tools
```
