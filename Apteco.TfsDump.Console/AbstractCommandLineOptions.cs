using CommandLine;

namespace Apteco.TfsDump.Console
{
  public abstract class AbstractCommandLineOptions
  {
    [Option('c', Required = true, HelpText = "The URL of TFS Collection (i.e. https://tfs.example.com/DefaultCollection)")]
    public string CollectionUrl { get; set; }

    [Option('u', Required = false, HelpText = "TFS Username.  If omitted then the logged in user is used")]
    public string Username { get; set; }

    [Option('p', Required = false, HelpText = "TFS Password")]
    public string Password { get; set; }
  }
}
