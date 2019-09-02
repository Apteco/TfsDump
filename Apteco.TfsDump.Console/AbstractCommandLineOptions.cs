using CommandLine;

namespace Apteco.TfsDump.Console
{
  public abstract class AbstractCommandLineOptions
  {
    [Option('c', "collection-url", Required = true, HelpText = "The URL of TFS Collection (i.e. https://tfs.example.com/DefaultCollection)")]
    public string CollectionUrl { get; set; }

    [Option('u', "username", Required = false, HelpText = "TFS Username.  If omitted then the logged in user is used")]
    public string Username { get; set; }

    [Option('p', "password", Required = false, HelpText = "TFS Password")]
    public string Password { get; set; }

    [Option('t', "access-token", Required = false, HelpText = "TFS Personal Access Token")]
    public string PersonalAccessToken { get; set; }

    [Option('s', "connection-string", Required = false, HelpText = "Database connection string (otherwise will output to console)")]
    public string ConnectionString { get; set; }

    [Option('n', "table-name", Required = false, HelpText = "Database table name (otherwise will default to command name)")]
    public string DatabaseTableName { get; set; }
  }
}
