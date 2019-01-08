using System.IO;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Apteco.TfsDump.Console
{
  public class WorkItemManager
  {
    #region private constants
    private const string TitleFieldName = "System.Title";
    private const string TeamProjectFieldName = "System.TeamProject";
    private const string AreaPathFieldName = "System.AreaPath";
    private const string IterationPathFieldName = "System.IterationPath";
    private const string WorkItemTypeFieldName = "System.WorkItemType";
    private const string StateFieldName = "System.State";
    private const string ReasonFieldName = "System.Reason";
    private const string AssignedToFieldName = "System.AssignedTo";
    private const string CreatedDateFieldName = "System.CreatedDate";
    private const string CreatedByFieldName = "System.CreatedBy";
    private const string ChangedDateFieldName = "System.ChangedDate";
    private const string ChangedByFieldName = "System.ChangedBy";
    #endregion

    #region private fields
    private WorkItemTrackingHttpClient witClient;
    #endregion

    #region public constructor
    public WorkItemManager(WorkItemTrackingHttpClient witClient)
    {
      this.witClient = witClient;
    }
    #endregion

    #region public methods
    public async Task WriteWorkItemDetails(TextWriter writer)
    {
      WorkItemQueryResult queryResult = await witClient.QueryByWiqlAsync(new Wiql()
      {
        Query = $"SELECT {TitleFieldName}, "+
                $"       {TeamProjectFieldName}, "+
                $"       {AreaPathFieldName}, "+
                $"       {IterationPathFieldName}, "+
                $"       {WorkItemTypeFieldName}, "+
                $"       {StateFieldName}, "+
                $"       {ReasonFieldName}, "+
                $"       {AssignedToFieldName}, "+
                $"       {CreatedDateFieldName}, "+
                $"       {CreatedByFieldName}, "+
                $"       {ChangedDateFieldName}, "+
                $"       {ChangedByFieldName} "+
                "FROM workitems"
      });

      await WriteHeader(writer);
      foreach (WorkItemReference workItemReference in queryResult.WorkItems)
      {
        await WriteWorkItemDetails(witClient, workItemReference.Id, writer);
      }
    }
    #endregion

    #region private methods
    private async Task WriteWorkItemDetails(WorkItemTrackingHttpClient witClient, int id, TextWriter writer)
    {
      WorkItem workitem = await witClient.GetWorkItemAsync(id, null, null, WorkItemExpand.All);
      await WriteWorkItemDetails(workitem, writer);
    }

    private async Task WriteHeader(TextWriter writer)
    {
      await writer.WriteLineAsync("Id\tProject\tTitle\tAreaPath\tIterationPath\tWorkItemType\tState\tReason\tAssignedTo\tCreatedDate\tCreatedBy\tChangedDate\tChangedBy");
    }

    private async Task WriteWorkItemDetails(WorkItem workitem, TextWriter writer)
    {
      string title = GetField(workitem, TitleFieldName).SanitiseForTabDelimitedString();
      string project = GetField(workitem, TeamProjectFieldName).SanitiseForTabDelimitedString();
      string areaPath = GetField(workitem, AreaPathFieldName).SanitiseForTabDelimitedString();
      string iterationPath = GetField(workitem, IterationPathFieldName).SanitiseForTabDelimitedString();
      string workItemType = GetField(workitem, WorkItemTypeFieldName).SanitiseForTabDelimitedString();
      string state = GetField(workitem, StateFieldName).SanitiseForTabDelimitedString();
      string reason = GetField(workitem, ReasonFieldName).SanitiseForTabDelimitedString();
      string assignedTo = GetField(workitem, AssignedToFieldName).SanitiseForTabDelimitedString();
      string createdDate = GetField(workitem, CreatedDateFieldName).SanitiseForTabDelimitedString();
      string createdBy = GetField(workitem, CreatedByFieldName).SanitiseForTabDelimitedString();
      string changedDate = GetField(workitem, ChangedDateFieldName).SanitiseForTabDelimitedString();
      string changedBy = GetField(workitem, ChangedByFieldName).SanitiseForTabDelimitedString();

      await writer.WriteLineAsync($"{workitem.Id}\t{project}\t{title}\t{areaPath}\t{iterationPath}\t{workItemType}\t{state}\t{reason}\t{assignedTo}\t{createdDate}\t{createdBy}\t{changedDate}\t{changedBy}");
    }

    private string GetField(WorkItem workitem, string key)
    {
      if (!workitem.Fields.TryGetValue(key, out object fieldValue))
        return null;

      return fieldValue?.ToString();
    }
    #endregion
  }
}
