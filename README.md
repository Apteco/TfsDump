# TfsDump
A utility to dump work item information and git commits from a TFS server.  The utility will output one row per item with tab delimiters.

## Usage

`TfsDump.exe <collection url> <username> <password> [git|workitems]`

For example: 
* `TfsDump.exe https://tfs.example.com/DefaultCollection TfsUser password` will output all git commits from the given TFS server.

* `TfsDump.exe https://tfs.example.com/DefaultCollection TfsUser password workitems` will output all TFS work items from the given TFS server.
