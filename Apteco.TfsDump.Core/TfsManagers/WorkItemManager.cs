﻿using System;
using System.Threading.Tasks;
using Apteco.TfsDump.Core.Sinks;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System.Collections.Generic;
using System.Linq;

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
    private const string ResolvedDateFieldName = "Microsoft.VSTS.Common.ResolvedDate";
    private const string ResolvedByFieldName = "Microsoft.VSTS.Common.ResolvedBy";
    private const string ClosedDateFieldName = "Microsoft.VSTS.Common.ClosedDate";
    private const string ClosedByFieldName = "Microsoft.VSTS.Common.ClosedBy";
    private const string PriorityFieldName = "Microsoft.VSTS.Common.Priority";
    private const string SeverityFieldName = "Microsoft.VSTS.Common.Severity";
    private const string TagsFieldName = "System.Tags";

    private readonly string[] CommonWorkItemFields = new string[]
    {
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
      "ChangedBy",
      "ResolvedDate",
      "ResolvedBy",
      "ClosedDate",
      "ClosedBy",
      "Priority",
      "Severity",
      "Tags"
    };
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
                $"       {ChangedByFieldName}, "+
                $"       {ResolvedDateFieldName}, " +
                $"       {ResolvedByFieldName}, " +
                $"       {ClosedDateFieldName}, " +
                $"       {ClosedByFieldName}, " +
                $"       {PriorityFieldName}, " +
                $"       {SeverityFieldName}, " +
                $"       {TagsFieldName} " +
                "FROM workitems"
      });

      await InitialiseWorkItemSink(sink);
      foreach (WorkItemReference workItemReference in queryResult.WorkItems)
      {
        await WriteWorkItemDetails(witClient, workItemReference.Id, sink);
      }
    }

    public async Task WriteWorkItemRevisionDetails(ISink sink)
    {
      WorkItemQueryResult queryResult = await witClient.QueryByWiqlAsync(new Wiql()
      {
        Query = $"SELECT {TitleFieldName}, " +
                $"       {TeamProjectFieldName} " +
                "FROM workitems"
      });

      await InitialiseWorkItemRevisionSink(sink);
      foreach (WorkItemReference workItemReference in queryResult.WorkItems)
      {
        await WriteWorkItemRevisions(witClient, workItemReference.Id, sink);
      }
    }
    #endregion

    #region private methods
    private async Task WriteWorkItemDetails(WorkItemTrackingHttpClient witClient, int id, ISink sink)
    {
      WorkItem workitem = await witClient.GetWorkItemAsync(id, null, null, WorkItemExpand.All);
      await WriteWorkItemDetails(workitem, sink);
    }

    private async Task InitialiseWorkItemSink(ISink sink)
    {
      await sink.InitialiseSink(
        new string[]
        {
          "Id",
        }.Concat(CommonWorkItemFields).ToArray(),
        "Id");
    }

    private async Task WriteWorkItemDetails(WorkItem workitem, ISink sink)
    {
      await sink.Write(
        new string[]
        {
          workitem.Id.ToString(),
        }.Concat(GetCommonFields(workitem)).ToArray());
    }

    private async Task WriteWorkItemRevisions(WorkItemTrackingHttpClient witClient, int id, ISink sink)
    {
      List<WorkItem> revisions = await witClient.GetRevisionsAsync(id, null, null, WorkItemExpand.Fields);
      foreach (WorkItem revision in revisions)
        await WriteWorkItemRevision(revision, sink);
    }

    private async Task InitialiseWorkItemRevisionSink(ISink sink)
    {
      await sink.InitialiseSink(
        new string[]
        {
          "Id",
          "Revision"
        }.Concat(CommonWorkItemFields).ToArray(),
        new string[] { "Id", "Revision" });
    }

    private async Task WriteWorkItemRevision(WorkItem workitem, ISink sink)
    {
      await sink.Write(
        new string[]
        {
          workitem.Id.ToString(),
          workitem.Rev.ToString(),
        }.Concat(GetCommonFields(workitem)).ToArray());
    }

    private string[] GetCommonFields(WorkItem workitem)
    {
      return new string[]
      {
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
        GetField(workitem, ChangedByFieldName),
        GetField(workitem, ResolvedDateFieldName),
        GetField(workitem, ResolvedByFieldName),
        GetField(workitem, ClosedDateFieldName),
        GetField(workitem, ClosedByFieldName),
        GetField(workitem, PriorityFieldName),
        GetField(workitem, SeverityFieldName),
        GetField(workitem, TagsFieldName),
      };
    }

    private string GetField(WorkItem workitem, string key)
    {
      if (!workitem.Fields.TryGetValue(key, out object fieldValue))
        return null;

      if (fieldValue is IdentityRef)
        return ((IdentityRef)fieldValue).DisplayName;

      if (fieldValue is DateTime)
        return ((DateTime)fieldValue).ToString("s");

      return fieldValue?.ToString();
    }
    #endregion
  }
}
