using System;
using System.Net;
using System.Threading.Tasks;
using Apteco.TfsDump.Core.Sinks;
using Apteco.TfsDump.Core.TfsManagers;
using CommandLine;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace Apteco.TfsDump.Console
{
  public class Program
  {
    public static int Main(string[] args)
    {
      return Parser.Default.ParseArguments<GitCommandLineOptions, WorkItemsCommandLineOptions>(args)
        .MapResult(
          (GitCommandLineOptions opts) => RunGit(opts),
          (WorkItemsCommandLineOptions opts) => RunWorkItems(opts),
          errs => 1
        );
    }

    private static int RunGit(GitCommandLineOptions options)
    {
      VssConnection connection = CreateConnection(options);
      ISink sink = CreateSink("git", options);

      GitHttpClient gitClient = connection.GetClient<GitHttpClient>();

      Task task = new GitCommitManager(gitClient).WriteCommitDetails(options.DuplicateCommitsForMultipleWorkitems, sink);
      task.Wait();
      return 0;
    }

    private static int RunWorkItems(WorkItemsCommandLineOptions options)
    {
      VssConnection connection = CreateConnection(options);
      ISink sink = CreateSink("workitems", options);
      WorkItemTrackingHttpClient witClient = connection.GetClient<WorkItemTrackingHttpClient>();

      Task task = new WorkItemManager(witClient).WriteWorkItemDetails(sink);
      task.Wait();
      return 0;
    }

    private static VssConnection CreateConnection(AbstractCommandLineOptions options)
    {
      VssCredentials creds;
      if (!string.IsNullOrEmpty(options.PersonalAccessToken))
        creds = new VssBasicCredential(string.Empty, options.PersonalAccessToken);
      else if (!string.IsNullOrEmpty(options.Username))
        creds = new VssCredentials(new WindowsCredential(new NetworkCredential(options.Username, options.Password)));
      else
        creds = new VssCredentials(new WindowsCredential(true));

      return new VssConnection(new Uri(options.CollectionUrl), creds);
    }

    private static ISink CreateSink(string commandName, AbstractCommandLineOptions options)
    {
      if (!string.IsNullOrEmpty(options.ConnectionString))
      {
        string tableName = string.IsNullOrEmpty(options.DatabaseTableName) ? commandName : options.DatabaseTableName;
        return new DatabaseSink(options.ConnectionString, tableName);
      }

      return new TextWriterSink(System.Console.Out);
    }

  }
}
