# TfsDump

A utility to dump work item information and git commits from Azure DevOps (either a local install of Azure DevOps Server or the Azure hosted one). The utility will output one row per item with tab delimiters.

The name of the project relates to the historic name for Azure DevOps (Team Foundation Server - TFS).

## Usage

`dotnet TfsDump.dll git|workitems|workitemrevisions|help -c <collection url> [-u <username> -p <password>]|[-t <personal acccess token>]`

Connection example:

- `dotnet TfsDump.dll git -c https://tfs.example.com/DefaultCollection` will output data from the given Azure DevOps Server using the logged in user's credentials.

- `dotnet TfsDump.dll git -c https://tfs.example.com/DefaultCollection -u TfsUser -p password` will output data from the given Azure DevOps Server using the given username and password.

- `dotnet TfsDump.dll git -c https://dev.azure.com/some-organisation-name/some-project-name -t abc123` will output data from the given organisation and project in Azure DevOps using the given [personal access token](https://docs.microsoft.com/en-us/azure/devops/organizations/accounts/use-personal-access-tokens-to-authenticate?view=vsts).

Usage examples:

- `dotnet TfsDump.dll git -c https://dev.azure.com/some-organisation-name/some-project-name -t abc123` will output all git commits from the given organisation and project in Azure DevOps.

- `dotnet TfsDump.dll workitems -c https://dev.azure.com/some-organisation-name/some-project-name -t abc123` will output all work items from the given organisation and project in Azure DevOps.

- `dotnet TfsDump.dll workitemrevisions -c https://dev.azure.com/some-organisation-name/some-project-name -t abc123` will output all work item history/revision changes from the given organisation and project in Azure DevOps.

- `dotnet TfsDump.dll help` will output a help string.
