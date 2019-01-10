using CommandLine;

namespace Apteco.TfsDump.Console
{
  [Verb("git", HelpText = "Output git commits")]
  public class GitCommandLineOptions : AbstractCommandLineOptions
  {
    [Option('d', "duplicate", Default = false, HelpText = "Whether to include duplicate commit rows for each associated workitem if there are more than one.")]
    public bool DuplicateCommitsForMultipleWorkitems { get; set; }
  }
}
