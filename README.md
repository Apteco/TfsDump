# TfsDump
A utility to dump work item information and git commits from a TFS server.  The utility will output one row per item with tab delimiters.

## Usage

`dotnet TfsDump.dll git|workitems|help -c <collection url> [-u <username> -p <password>]`

For example: 
* `dotnet TfsDump.dll git -c https://tfs.example.com/DefaultCollection` will output all git commits from the given TFS server using the logged in user's credentials.

* `dotnet TfsDump.dll git -c https://tfs.example.com/DefaultCollection -u TfsUser -p password` will output all git commits from the given TFS server using the given username and password.

* `dotnet TfsDump.dll workitems -c https://tfs.example.com/DefaultCollection` will output all TFS work items from the given TFS server using the logged in user's credentials.

* `dotnet TfsDump.dll help` will output a help string.
