CREATE TABLE [git](
	[RepositoryName] [nvarchar](50) NULL,
	[CommitId] [nvarchar](50),
	[CommitDate] [nvarchar](50) NULL,
	[CommitAuthor] [nvarchar](50) NULL,
	[Comment] [nvarchar](max) NULL,
	[WorkItemId] [nvarchar](10) NULL,
	[CollectionUrl] [nvarchar](100)
	CONSTRAINT [PK_git] PRIMARY KEY CLUSTERED 
	(
		[CommitId] ASC,
		[CollectionUrl] ASC
	)
) 

CREATE TABLE [workitems](
	[Id] [nvarchar](10),
	[Project] [nvarchar](50) NULL,
	[Title] [nvarchar](max) NULL,
	[AreaPath] [nvarchar](100) NULL,
	[IterationPath] [nvarchar](100) NULL,
	[WorkItemType] [nvarchar](20) NULL,
	[State] [nvarchar](50) NULL,
	[Reason] [nvarchar](50) NULL,
	[AssignedTo] [nvarchar](100) NULL,
	[CreatedDate] [nvarchar](50) NULL,
	[CreatedBy] [nvarchar](100) NULL,
	[ChangedDate] [nvarchar](50) NULL,
	[ChangedBy] [nvarchar](100) NULL,
	[ResolvedDate] [nvarchar](50) NULL,
	[ResolvedBy] [nvarchar](100) NULL,
	[ClosedDate] [nvarchar](50) NULL,
	[ClosedBy] [nvarchar](100) NULL,
	[CollectionUrl] [nvarchar](100)
	CONSTRAINT [PK_workitems] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC,
		[CollectionUrl] ASC
	)
)

CREATE TABLE [builds](
	[Id] [nvarchar](10),
	[BuildNumber] [nvarchar](50) NULL,
	[Definition] [nvarchar](50) NULL,
	[RequestedBy] [nvarchar](50) NULL,
	[RequestedFor] [nvarchar](50) NULL,
	[Repository] [nvarchar](50) NULL,
	[QueueTime] [nvarchar](20) NULL,
	[StartTime] [nvarchar](20) NULL,
	[FinishTime] [nvarchar](20) NULL,
	[SourceBranch] [nvarchar](100) NULL,
	[SourceVersion] [nvarchar](50) NULL,
	[Result] [nvarchar](50) NULL,
	[TotalTests] [nvarchar](10) NULL,
	[TestsPassed] [nvarchar](10) NULL,
	[TestsIgnored] [nvarchar](10) NULL,
	[CoveredLines] [nvarchar](10) NULL,
	[TotalLines] [nvarchar](10) NULL,
	[CollectionUrl] [nvarchar](100)
	CONSTRAINT [PK_builds] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC,
		[CollectionUrl] ASC
	)
)