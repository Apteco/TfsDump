using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace Apteco.TfsDump.Console
{
  public class Program
  {
    private const string WorkItemsDumpType = "workitems";
    private const string GitDumpType = "git";

    public static void Main(string[] args)
    {
      if (args.Length < 3)
      {
        System.Console.WriteLine("Usage: TfsDump.exe <collection url> <username> <password> [git|workitems]");
        return;
      }

      Task task = Run(args[0], args[1], args[2], args.Length >= 4? args[3] : GitDumpType);
      task.Wait();
    }

    private static async Task Run(string collectionUrl, string username, string password, string type)
    {
      VssCredentials creds = new VssCredentials(new WindowsCredential(new NetworkCredential(username, password)));
      VssConnection connection = new VssConnection(new Uri(collectionUrl), creds);

      WorkItemTrackingHttpClient witClient = connection.GetClient<WorkItemTrackingHttpClient>();
      GitHttpClient gitClient = connection.GetClient<GitHttpClient>();

      switch (type)
      {
        case GitDumpType:
          await new GitCommitManager(gitClient).WriteCommitDetails(System.Console.Out);
          break;

        case WorkItemsDumpType:
          await new WorkItemManager(witClient).WriteWorkItemDetails(System.Console.Out);
          break;

        default:
          System.Console.Error.WriteLine("Bad dump type: "+type);
          break;
      }
    }
  }
}
