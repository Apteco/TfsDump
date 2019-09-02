using System.Collections.Generic;
using System.Threading.Tasks;
using Apteco.TfsDump.Core.Sinks;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi;

namespace Apteco.TfsDump.Core.TfsManagers
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
    public async Task WriteCommitDetails(bool duplicateCommitsForMultipleWorkitems, ISink sink)
    {
      List<GitRepository> repositories = await gitClient.GetRepositoriesAsync();

      await InitialiseSink(sink);
      foreach (GitRepository repository in repositories)
      {
        await WriteCommitDetailsForRepository(repository, duplicateCommitsForMultipleWorkitems, sink);
      }
    }
    #endregion

    #region private methods
    private async Task WriteCommitDetailsForRepository(GitRepository repository, bool duplicateCommitsForMultipleWorkitems, ISink sink)
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
            await WriteCommitDetails(repository, commit, null, sink);
          }
          else
          {
            if (duplicateCommitsForMultipleWorkitems)
            {
              foreach (ResourceRef workItemRef in commit.WorkItems)
              {
                await WriteCommitDetails(repository, commit, workItemRef.Id, sink);
              }
            }
            else
            {
              await WriteCommitDetails(repository, commit, commit.WorkItems[0]?.Id, sink);
            }
          }
        }
      }
    }

    private async Task InitialiseSink(ISink sink)
    {
      await sink.InitialiseSink(
        new string[]
        {
          "RepositoryName",
          "CommitId",
          "CommitDate",
          "CommitAuthor",
          "Comment",
          "WorkItemId"
        },
        "CommitId");
    }

    private async Task WriteCommitDetails(GitRepository repository, GitCommitRef commit, string workItemId, ISink sink)
    {
      await sink.Write(
        new string[]
        {
          repository.Name,
          commit.CommitId,
          commit.Author.Date.ToString("s"),
          commit.Author.Name,
          commit.Comment,
          workItemId
        });
    }
    #endregion
  }
}
