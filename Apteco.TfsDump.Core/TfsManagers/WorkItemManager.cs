using System.Threading.Tasks;
using Apteco.TfsDump.Core.Sinks;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Apteco.TfsDump.Core.TfsManagers
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
    public async Task WriteWorkItemDetails(ISink sink)
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

      await InitialiseSink(sink);
      foreach (WorkItemReference workItemReference in queryResult.WorkItems)
      {
        await WriteWorkItemDetails(witClient, workItemReference.Id, sink);
      }
    }
    #endregion

    #region private methods
    private async Task WriteWorkItemDetails(WorkItemTrackingHttpClient witClient, int id, ISink sink)
    {
      WorkItem workitem = await witClient.GetWorkItemAsync(id, null, null, WorkItemExpand.All);
      await WriteWorkItemDetails(workitem, sink);
    }

    private async Task InitialiseSink(ISink sink)
    {
      await sink.InitialiseSink(
        new string[]
        {
          "Id",
          "Project",
          "Title",
          "AreaPath",
          "IterationPath",
          "WorkItemType",
          "State",
          "Reason",
          "AssignedTo",
          "CreatedDate",
          "CreatedBy",
          "ChangedDate",
          "ChangedBy"
        },
        "Id");
    }

    private async Task WriteWorkItemDetails(WorkItem workitem, ISink sink)
    {
      await sink.Write(
        new string[]
        {
          workitem.Id.ToString(),
          GetField(workitem, TeamProjectFieldName),
          GetField(workitem, TitleFieldName),
          GetField(workitem, AreaPathFieldName),
          GetField(workitem, IterationPathFieldName),
          GetField(workitem, WorkItemTypeFieldName),
          GetField(workitem, StateFieldName),
          GetField(workitem, ReasonFieldName),
          GetField(workitem, AssignedToFieldName),
          GetField(workitem, CreatedDateFieldName),
          GetField(workitem, CreatedByFieldName),
          GetField(workitem, ChangedDateFieldName),
          GetField(workitem, ChangedByFieldName)
        });
    }

    private string GetField(WorkItem workitem, string key)
    {
      if (!workitem.Fields.TryGetValue(key, out object fieldValue))
        return null;

      if (fieldValue is IdentityRef)
        return ((IdentityRef)fieldValue).DisplayName;

      return fieldValue?.ToString();
    }
    #endregion
  }
}
