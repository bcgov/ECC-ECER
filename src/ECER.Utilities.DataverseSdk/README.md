# Dataverse entities

This project is for hosting the generated early bound entities as defined in ECER Dataverse schema. It uses Early Bound Generator V2 plugin for XrmToolKit to automatically identity and create entities, option sets, commands and service context entities to be used by other projects to access and manipulate data in Dataverse.

## Generating early bound Dataverse C# code

- Make sure the plugin is installed in XrmToolKit
- Open the tool and connect to ECER dev Dataverse instance - the connection string is `AuthType=Office365;Url=https://***.dynamics.com;Username=****@gov.bc.ca;Password=********`
- from the tool, open `Model\ecer.xml` and click 'Generate'
- when the code generation is completed, the Model directory will contain all the entities that are included:
  - all entities and messages that are prefixed with ecer
  - any other whitelisted entities and messages
- click 'Save' to persist any changes to `Model\ecer.xml` and override the existing file
