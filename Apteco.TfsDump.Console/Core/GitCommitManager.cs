using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi;

namespace Apteco.TfsDump.Console.Core
{
  public class GitCommitManager
  {
    #region private fields
    private GitHttpClient gitClient;
    #endregion

    #region public constructor
    public GitCommitManager(GitHttpClient gitClient)
    {
      this.gitClient = gitClient;
    }
    #endregion

    #region public methods
    public async Task WriteCommitDetails(bool duplicateCommitsForMultipleWorkitems, TextWriter writer)
    {
      List<GitRepository> repositories = await gitClient.GetRepositoriesAsync();

      await WriteHeader(writer);
      foreach (GitRepository repository in repositories)
      {
        await WriteCommitDetailsForRepository(repository, duplicateCommitsForMultipleWorkitems, writer);
      }
    }
    #endregion

    #region private methods
    private async Task WriteCommitDetailsForRepository(GitRepository repository, bool duplicateCommitsForMultipleWorkitems, TextWriter writer)
    {
      int skip = 0;
      while (true)
      {
        List<GitCommitRef> commits = await gitClient.GetCommitsAsync(repository.Id, new GitQueryCommitsCriteria()
        {
          IncludeWorkItems = true
        }, skip, 1000);

        if (commits?.Count == 0)
          return;

        skip += commits.Count;

        foreach (GitCommitRef commit in commits)
        {
          if (commit.WorkItems == null || commit.WorkItems.Count == 0)
          {
            await WriteCommitDetails(repository, commit, null, writer);
          }
          else
          {
            if (duplicateCommitsForMultipleWorkitems)
            {
              foreach (ResourceRef workItemRef in commit.WorkItems)
              {
                await WriteCommitDetails(repository, commit, workItemRef.Id, writer);
              }
            }
            else
            {
              await WriteCommitDetails(repository, commit, commit.WorkItems[0]?.Id, writer);
            }
          }
        }
      }
    }

    private async Task WriteHeader(TextWriter writer)
    {
      await writer.WriteLineAsync("RepositoryName\tCommitId\tCommitDate\tCommitAuthor\tComment\tWorkItemId");
    }

    private async Task WriteCommitDetails(GitRepository repository, GitCommitRef commit, string workItemId, TextWriter writer)
    {
      await writer.WriteLineAsync($"{repository.Name}\t{commit.CommitId}\t{commit.Author.Date:s}\t{commit.Author.Name}\t{commit.Comment.SanitiseForTabDelimitedString()}\t{workItemId}");
    }
    #endregion
  }
}
