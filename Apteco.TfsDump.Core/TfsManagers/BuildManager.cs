using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apteco.TfsDump.Core.Sinks;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;

namespace Apteco.TfsDump.Core.TfsManagers
{
  public class BuildManager
  {
    #region private constants
    private const string LinesCoverageStatsLabel = "Lines";
    #endregion

    #region private fields
    private ProjectHttpClient projectHttpClient;
    private BuildHttpClient buildClient;
    private TestManagementHttpClient testManagementClient;
    #endregion

    #region public constructor
    public BuildManager(ProjectHttpClient projectHttpClient, BuildHttpClient buildClient, TestManagementHttpClient testManagementClient)
    {
      this.projectHttpClient = projectHttpClient;
      this.buildClient = buildClient;
      this.testManagementClient = testManagementClient;
    }
    #endregion

    #region public methods
    public async Task WriteBuildDetails(ISink sink)
    {
      IPagedList<TeamProjectReference> projects = await projectHttpClient.GetProjects(null, 10000, 0);

      await InitialiseSink(sink);
      foreach (TeamProjectReference project in projects)
      {
        await WriteBuildDetailsForRepository(project.Name, sink);
      }
    }
    #endregion

    #region private methods
    private async Task WriteBuildDetailsForRepository(string projectName, ISink sink)
    {
      IPagedList<Build> builds = await buildClient.GetBuildsAsync2(projectName, null, null, null, null, null, null, null, BuildStatus.Completed);
      foreach (Build build in builds)
      {
        CodeCoverageSummary coverage = await testManagementClient.GetCodeCoverageSummaryAsync(projectName, build.Id);
        List<TestRun> testRuns = await testManagementClient.GetTestRunsAsync(projectName, build.Uri.ToString());

        await WriteBuildDetails(build, testRuns, coverage, sink);
      }
    }

    private async Task InitialiseSink(ISink sink)
    {
      await sink.InitialiseSink(
        new string[]
        {
          "Id",
          "BuildNumber",
          "Definition",
          "RequestedBy",
          "RequestedFor",
          "Repository",
          "QueueTime",
          "StartTime",
          "FinishTime",
          "SourceBranch",
          "SourceVersion",
          "Result",
          "TotalTests",
          "TestsPassed",
          "TestsIgnored",
          "CoveredLines",
          "TotalLines"
        },
        "Id");
    }

    private async Task WriteBuildDetails(Build build, List<TestRun> testRuns, CodeCoverageSummary coverage, ISink sink)
    {
      CodeCoverageStatistics linesCovered = coverage?.CoverageData?.FirstOrDefault()?.CoverageStats?.FirstOrDefault(s => s.Label == LinesCoverageStatsLabel);

      int totalTests = testRuns?.Sum(r => r.TotalTests) ?? 0;
      int passedTests = testRuns?.Sum(r => r.PassedTests) ?? 0;
      int ignoredTests = testRuns?.Sum(r => r.IncompleteTests + r.NotApplicableTests + r.UnanalyzedTests) ?? 0;

      await sink.Write(
        new string[]
        {
          build.Id.ToString(),
          build.BuildNumber,
          build.Definition?.Name,
          build.RequestedBy?.DisplayName,
          build.RequestedFor?.DisplayName,
          build.Repository?.Name,
          build.QueueTime?.ToString("s"),
          build.StartTime?.ToString("s"),
          build.FinishTime?.ToString("s"),
          build.SourceBranch,
          build.SourceVersion,
          build.Result.ToString(),
          totalTests.ToString(),
          passedTests.ToString(),
          ignoredTests.ToString(),
          linesCovered?.Covered.ToString(),
          linesCovered?.Total.ToString()
        });
    }
    #endregion
  }
}
