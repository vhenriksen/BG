
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 11/27/2013 17:00:09
-- Generated from EDMX file: C:\Users\Kent\Documents\Forsøg med BG\BG\BG.Model\Model.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [bganlaegsteknikapp_dk_db];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_UserDailyReport]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[DailyReportSet] DROP CONSTRAINT [FK_UserDailyReport];
GO
IF OBJECT_ID(N'[dbo].[FK_DailyReportTask]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TaskSet] DROP CONSTRAINT [FK_DailyReportTask];
GO
IF OBJECT_ID(N'[dbo].[FK_EconomicCacheEconomicProject]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[EconomicProjectSet] DROP CONSTRAINT [FK_EconomicCacheEconomicProject];
GO
IF OBJECT_ID(N'[dbo].[FK_EconomicProjectEconomicTask_EconomicProject]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[EconomicProjectEconomicTask] DROP CONSTRAINT [FK_EconomicProjectEconomicTask_EconomicProject];
GO
IF OBJECT_ID(N'[dbo].[FK_EconomicProjectEconomicTask_EconomicTask]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[EconomicProjectEconomicTask] DROP CONSTRAINT [FK_EconomicProjectEconomicTask_EconomicTask];
GO
IF OBJECT_ID(N'[dbo].[FK_EconomicTaskTask]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TaskSet] DROP CONSTRAINT [FK_EconomicTaskTask];
GO
IF OBJECT_ID(N'[dbo].[FK_EconomicProjectTask]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TaskSet] DROP CONSTRAINT [FK_EconomicProjectTask];
GO
IF OBJECT_ID(N'[dbo].[FK_EconomicProjectGroup]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[EconomicProjectSet] DROP CONSTRAINT [FK_EconomicProjectGroup];
GO
IF OBJECT_ID(N'[dbo].[FK_GroupUser_Group]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[GroupUser] DROP CONSTRAINT [FK_GroupUser_Group];
GO
IF OBJECT_ID(N'[dbo].[FK_GroupUser_User]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[GroupUser] DROP CONSTRAINT [FK_GroupUser_User];
GO
IF OBJECT_ID(N'[dbo].[FK_ActivityEconomicProject]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ActivitySet] DROP CONSTRAINT [FK_ActivityEconomicProject];
GO
IF OBJECT_ID(N'[dbo].[FK_ActivityRoutine]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ActivitySet] DROP CONSTRAINT [FK_ActivityRoutine];
GO
IF OBJECT_ID(N'[dbo].[FK_ActivityEconomicTaskType]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ActivitySet] DROP CONSTRAINT [FK_ActivityEconomicTaskType];
GO
IF OBJECT_ID(N'[dbo].[FK_BreakDailyReport]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[BreakSet] DROP CONSTRAINT [FK_BreakDailyReport];
GO
IF OBJECT_ID(N'[dbo].[FK_RouteRoutine]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RoutineSet] DROP CONSTRAINT [FK_RouteRoutine];
GO
IF OBJECT_ID(N'[dbo].[FK_DateRoutine_Date]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[DateRoutine] DROP CONSTRAINT [FK_DateRoutine_Date];
GO
IF OBJECT_ID(N'[dbo].[FK_DateRoutine_Routine]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[DateRoutine] DROP CONSTRAINT [FK_DateRoutine_Routine];
GO
IF OBJECT_ID(N'[dbo].[FK_ActivityTask]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TaskSet] DROP CONSTRAINT [FK_ActivityTask];
GO
IF OBJECT_ID(N'[dbo].[FK_UserRoutine_User]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserRoutine] DROP CONSTRAINT [FK_UserRoutine_User];
GO
IF OBJECT_ID(N'[dbo].[FK_UserRoutine_Routine]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserRoutine] DROP CONSTRAINT [FK_UserRoutine_Routine];
GO
IF OBJECT_ID(N'[dbo].[FK_ActivityActivityUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ActivityUserSet] DROP CONSTRAINT [FK_ActivityActivityUser];
GO
IF OBJECT_ID(N'[dbo].[FK_ActivityUserActivity]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ActivitySet] DROP CONSTRAINT [FK_ActivityUserActivity];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[TaskSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TaskSet];
GO
IF OBJECT_ID(N'[dbo].[DailyReportSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DailyReportSet];
GO
IF OBJECT_ID(N'[dbo].[EconomicCacheSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[EconomicCacheSet];
GO
IF OBJECT_ID(N'[dbo].[EconomicProjectSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[EconomicProjectSet];
GO
IF OBJECT_ID(N'[dbo].[EconomicTaskTypeSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[EconomicTaskTypeSet];
GO
IF OBJECT_ID(N'[dbo].[UserSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserSet];
GO
IF OBJECT_ID(N'[dbo].[GroupSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[GroupSet];
GO
IF OBJECT_ID(N'[dbo].[RoutineSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RoutineSet];
GO
IF OBJECT_ID(N'[dbo].[ActivitySet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ActivitySet];
GO
IF OBJECT_ID(N'[dbo].[BreakSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[BreakSet];
GO
IF OBJECT_ID(N'[dbo].[RouteSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RouteSet];
GO
IF OBJECT_ID(N'[dbo].[DateSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DateSet];
GO
IF OBJECT_ID(N'[dbo].[ActivityUserSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ActivityUserSet];
GO
IF OBJECT_ID(N'[dbo].[EconomicProjectEconomicTask]', 'U') IS NOT NULL
    DROP TABLE [dbo].[EconomicProjectEconomicTask];
GO
IF OBJECT_ID(N'[dbo].[GroupUser]', 'U') IS NOT NULL
    DROP TABLE [dbo].[GroupUser];
GO
IF OBJECT_ID(N'[dbo].[DateRoutine]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DateRoutine];
GO
IF OBJECT_ID(N'[dbo].[UserRoutine]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserRoutine];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'TaskSet'
CREATE TABLE [dbo].[TaskSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [TimeEnded] datetime  NULL,
    [TimeStarted] datetime  NULL,
    [isEmailed] bit  NOT NULL,
    [DailyReport_Id] int  NOT NULL,
    [EconomicTask_Id] int  NOT NULL,
    [EconomicTask_EconomicTaskTypeId] int  NOT NULL,
    [EconomicProject_Id] int  NOT NULL,
    [Activity_Id] int  NULL
);
GO

-- Creating table 'DailyReportSet'
CREATE TABLE [dbo].[DailyReportSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [DayEnded] datetime  NULL,
    [DayStarted] datetime  NULL,
    [User_Id] int  NOT NULL
);
GO

-- Creating table 'EconomicCacheSet'
CREATE TABLE [dbo].[EconomicCacheSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [LastUpdated] datetime  NOT NULL
);
GO

-- Creating table 'EconomicProjectSet'
CREATE TABLE [dbo].[EconomicProjectSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [EconomicProjectId] int  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [EconomicCache_Id] int  NOT NULL,
    [Group_Id] int  NOT NULL
);
GO

-- Creating table 'EconomicTaskTypeSet'
CREATE TABLE [dbo].[EconomicTaskTypeSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [EconomicTaskTypeId] int  NOT NULL,
    [Name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'UserSet'
CREATE TABLE [dbo].[UserSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [EconomicUserId] int  NOT NULL,
    [Username] nvarchar(max)  NOT NULL,
    [Password] nvarchar(max)  NOT NULL,
    [IsAdmin] bit  NOT NULL
);
GO

-- Creating table 'GroupSet'
CREATE TABLE [dbo].[GroupSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [EconomicGroupNumber] int  NOT NULL
);
GO

-- Creating table 'RoutineSet'
CREATE TABLE [dbo].[RoutineSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [RouteId] int  NOT NULL
);
GO

-- Creating table 'ActivitySet'
CREATE TABLE [dbo].[ActivitySet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Minutes] float  NOT NULL,
    [ActivityTaken] datetime  NULL,
    [IsUsed] bit  NOT NULL,
    [EconomicProject_Id] int  NOT NULL,
    [Routine_Id] int  NULL,
    [EconomicTaskType_Id] int  NOT NULL,
    [EconomicTaskType_EconomicTaskTypeId] int  NOT NULL,
    [TakenBy_Id] int  NULL
);
GO

-- Creating table 'BreakSet'
CREATE TABLE [dbo].[BreakSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [BreakStarted] datetime  NULL,
    [BreakEnded] datetime  NULL,
    [DailyReport_Id] int  NOT NULL
);
GO

-- Creating table 'RouteSet'
CREATE TABLE [dbo].[RouteSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'DateSet'
CREATE TABLE [dbo].[DateSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [TheDate] datetime  NOT NULL
);
GO

-- Creating table 'ActivityUserSet'
CREATE TABLE [dbo].[ActivityUserSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ActivityId] int  NOT NULL,
    [UserId] int  NOT NULL
);
GO

-- Creating table 'EconomicProjectEconomicTask'
CREATE TABLE [dbo].[EconomicProjectEconomicTask] (
    [EconomicProject_Id] int  NOT NULL,
    [EconomicTaskTypes_Id] int  NOT NULL,
    [EconomicTaskTypes_EconomicTaskTypeId] int  NOT NULL
);
GO

-- Creating table 'GroupUser'
CREATE TABLE [dbo].[GroupUser] (
    [Group_Id] int  NOT NULL,
    [User_Id] int  NOT NULL
);
GO

-- Creating table 'DateRoutine'
CREATE TABLE [dbo].[DateRoutine] (
    [Dates_Id] int  NOT NULL,
    [Routines_Id] int  NOT NULL
);
GO

-- Creating table 'UserRoutine'
CREATE TABLE [dbo].[UserRoutine] (
    [Users_Id] int  NOT NULL,
    [Routines_Id] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'TaskSet'
ALTER TABLE [dbo].[TaskSet]
ADD CONSTRAINT [PK_TaskSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'DailyReportSet'
ALTER TABLE [dbo].[DailyReportSet]
ADD CONSTRAINT [PK_DailyReportSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'EconomicCacheSet'
ALTER TABLE [dbo].[EconomicCacheSet]
ADD CONSTRAINT [PK_EconomicCacheSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'EconomicProjectSet'
ALTER TABLE [dbo].[EconomicProjectSet]
ADD CONSTRAINT [PK_EconomicProjectSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id], [EconomicTaskTypeId] in table 'EconomicTaskTypeSet'
ALTER TABLE [dbo].[EconomicTaskTypeSet]
ADD CONSTRAINT [PK_EconomicTaskTypeSet]
    PRIMARY KEY CLUSTERED ([Id], [EconomicTaskTypeId] ASC);
GO

-- Creating primary key on [Id] in table 'UserSet'
ALTER TABLE [dbo].[UserSet]
ADD CONSTRAINT [PK_UserSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'GroupSet'
ALTER TABLE [dbo].[GroupSet]
ADD CONSTRAINT [PK_GroupSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'RoutineSet'
ALTER TABLE [dbo].[RoutineSet]
ADD CONSTRAINT [PK_RoutineSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ActivitySet'
ALTER TABLE [dbo].[ActivitySet]
ADD CONSTRAINT [PK_ActivitySet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'BreakSet'
ALTER TABLE [dbo].[BreakSet]
ADD CONSTRAINT [PK_BreakSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'RouteSet'
ALTER TABLE [dbo].[RouteSet]
ADD CONSTRAINT [PK_RouteSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'DateSet'
ALTER TABLE [dbo].[DateSet]
ADD CONSTRAINT [PK_DateSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ActivityUserSet'
ALTER TABLE [dbo].[ActivityUserSet]
ADD CONSTRAINT [PK_ActivityUserSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [EconomicProject_Id], [EconomicTaskTypes_Id], [EconomicTaskTypes_EconomicTaskTypeId] in table 'EconomicProjectEconomicTask'
ALTER TABLE [dbo].[EconomicProjectEconomicTask]
ADD CONSTRAINT [PK_EconomicProjectEconomicTask]
    PRIMARY KEY CLUSTERED ([EconomicProject_Id], [EconomicTaskTypes_Id], [EconomicTaskTypes_EconomicTaskTypeId] ASC);
GO

-- Creating primary key on [Group_Id], [User_Id] in table 'GroupUser'
ALTER TABLE [dbo].[GroupUser]
ADD CONSTRAINT [PK_GroupUser]
    PRIMARY KEY CLUSTERED ([Group_Id], [User_Id] ASC);
GO

-- Creating primary key on [Dates_Id], [Routines_Id] in table 'DateRoutine'
ALTER TABLE [dbo].[DateRoutine]
ADD CONSTRAINT [PK_DateRoutine]
    PRIMARY KEY CLUSTERED ([Dates_Id], [Routines_Id] ASC);
GO

-- Creating primary key on [Users_Id], [Routines_Id] in table 'UserRoutine'
ALTER TABLE [dbo].[UserRoutine]
ADD CONSTRAINT [PK_UserRoutine]
    PRIMARY KEY CLUSTERED ([Users_Id], [Routines_Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [User_Id] in table 'DailyReportSet'
ALTER TABLE [dbo].[DailyReportSet]
ADD CONSTRAINT [FK_UserDailyReport]
    FOREIGN KEY ([User_Id])
    REFERENCES [dbo].[UserSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserDailyReport'
CREATE INDEX [IX_FK_UserDailyReport]
ON [dbo].[DailyReportSet]
    ([User_Id]);
GO

-- Creating foreign key on [DailyReport_Id] in table 'TaskSet'
ALTER TABLE [dbo].[TaskSet]
ADD CONSTRAINT [FK_DailyReportTask]
    FOREIGN KEY ([DailyReport_Id])
    REFERENCES [dbo].[DailyReportSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_DailyReportTask'
CREATE INDEX [IX_FK_DailyReportTask]
ON [dbo].[TaskSet]
    ([DailyReport_Id]);
GO

-- Creating foreign key on [EconomicCache_Id] in table 'EconomicProjectSet'
ALTER TABLE [dbo].[EconomicProjectSet]
ADD CONSTRAINT [FK_EconomicCacheEconomicProject]
    FOREIGN KEY ([EconomicCache_Id])
    REFERENCES [dbo].[EconomicCacheSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_EconomicCacheEconomicProject'
CREATE INDEX [IX_FK_EconomicCacheEconomicProject]
ON [dbo].[EconomicProjectSet]
    ([EconomicCache_Id]);
GO

-- Creating foreign key on [EconomicProject_Id] in table 'EconomicProjectEconomicTask'
ALTER TABLE [dbo].[EconomicProjectEconomicTask]
ADD CONSTRAINT [FK_EconomicProjectEconomicTask_EconomicProject]
    FOREIGN KEY ([EconomicProject_Id])
    REFERENCES [dbo].[EconomicProjectSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [EconomicTaskTypes_Id], [EconomicTaskTypes_EconomicTaskTypeId] in table 'EconomicProjectEconomicTask'
ALTER TABLE [dbo].[EconomicProjectEconomicTask]
ADD CONSTRAINT [FK_EconomicProjectEconomicTask_EconomicTask]
    FOREIGN KEY ([EconomicTaskTypes_Id], [EconomicTaskTypes_EconomicTaskTypeId])
    REFERENCES [dbo].[EconomicTaskTypeSet]
        ([Id], [EconomicTaskTypeId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_EconomicProjectEconomicTask_EconomicTask'
CREATE INDEX [IX_FK_EconomicProjectEconomicTask_EconomicTask]
ON [dbo].[EconomicProjectEconomicTask]
    ([EconomicTaskTypes_Id], [EconomicTaskTypes_EconomicTaskTypeId]);
GO

-- Creating foreign key on [EconomicTask_Id], [EconomicTask_EconomicTaskTypeId] in table 'TaskSet'
ALTER TABLE [dbo].[TaskSet]
ADD CONSTRAINT [FK_EconomicTaskTask]
    FOREIGN KEY ([EconomicTask_Id], [EconomicTask_EconomicTaskTypeId])
    REFERENCES [dbo].[EconomicTaskTypeSet]
        ([Id], [EconomicTaskTypeId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_EconomicTaskTask'
CREATE INDEX [IX_FK_EconomicTaskTask]
ON [dbo].[TaskSet]
    ([EconomicTask_Id], [EconomicTask_EconomicTaskTypeId]);
GO

-- Creating foreign key on [EconomicProject_Id] in table 'TaskSet'
ALTER TABLE [dbo].[TaskSet]
ADD CONSTRAINT [FK_EconomicProjectTask]
    FOREIGN KEY ([EconomicProject_Id])
    REFERENCES [dbo].[EconomicProjectSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_EconomicProjectTask'
CREATE INDEX [IX_FK_EconomicProjectTask]
ON [dbo].[TaskSet]
    ([EconomicProject_Id]);
GO

-- Creating foreign key on [Group_Id] in table 'EconomicProjectSet'
ALTER TABLE [dbo].[EconomicProjectSet]
ADD CONSTRAINT [FK_EconomicProjectGroup]
    FOREIGN KEY ([Group_Id])
    REFERENCES [dbo].[GroupSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_EconomicProjectGroup'
CREATE INDEX [IX_FK_EconomicProjectGroup]
ON [dbo].[EconomicProjectSet]
    ([Group_Id]);
GO

-- Creating foreign key on [Group_Id] in table 'GroupUser'
ALTER TABLE [dbo].[GroupUser]
ADD CONSTRAINT [FK_GroupUser_Group]
    FOREIGN KEY ([Group_Id])
    REFERENCES [dbo].[GroupSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [User_Id] in table 'GroupUser'
ALTER TABLE [dbo].[GroupUser]
ADD CONSTRAINT [FK_GroupUser_User]
    FOREIGN KEY ([User_Id])
    REFERENCES [dbo].[UserSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_GroupUser_User'
CREATE INDEX [IX_FK_GroupUser_User]
ON [dbo].[GroupUser]
    ([User_Id]);
GO

-- Creating foreign key on [EconomicProject_Id] in table 'ActivitySet'
ALTER TABLE [dbo].[ActivitySet]
ADD CONSTRAINT [FK_ActivityEconomicProject]
    FOREIGN KEY ([EconomicProject_Id])
    REFERENCES [dbo].[EconomicProjectSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ActivityEconomicProject'
CREATE INDEX [IX_FK_ActivityEconomicProject]
ON [dbo].[ActivitySet]
    ([EconomicProject_Id]);
GO

-- Creating foreign key on [Routine_Id] in table 'ActivitySet'
ALTER TABLE [dbo].[ActivitySet]
ADD CONSTRAINT [FK_ActivityRoutine]
    FOREIGN KEY ([Routine_Id])
    REFERENCES [dbo].[RoutineSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ActivityRoutine'
CREATE INDEX [IX_FK_ActivityRoutine]
ON [dbo].[ActivitySet]
    ([Routine_Id]);
GO

-- Creating foreign key on [EconomicTaskType_Id], [EconomicTaskType_EconomicTaskTypeId] in table 'ActivitySet'
ALTER TABLE [dbo].[ActivitySet]
ADD CONSTRAINT [FK_ActivityEconomicTaskType]
    FOREIGN KEY ([EconomicTaskType_Id], [EconomicTaskType_EconomicTaskTypeId])
    REFERENCES [dbo].[EconomicTaskTypeSet]
        ([Id], [EconomicTaskTypeId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ActivityEconomicTaskType'
CREATE INDEX [IX_FK_ActivityEconomicTaskType]
ON [dbo].[ActivitySet]
    ([EconomicTaskType_Id], [EconomicTaskType_EconomicTaskTypeId]);
GO

-- Creating foreign key on [DailyReport_Id] in table 'BreakSet'
ALTER TABLE [dbo].[BreakSet]
ADD CONSTRAINT [FK_BreakDailyReport]
    FOREIGN KEY ([DailyReport_Id])
    REFERENCES [dbo].[DailyReportSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_BreakDailyReport'
CREATE INDEX [IX_FK_BreakDailyReport]
ON [dbo].[BreakSet]
    ([DailyReport_Id]);
GO

-- Creating foreign key on [RouteId] in table 'RoutineSet'
ALTER TABLE [dbo].[RoutineSet]
ADD CONSTRAINT [FK_RouteRoutine]
    FOREIGN KEY ([RouteId])
    REFERENCES [dbo].[RouteSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_RouteRoutine'
CREATE INDEX [IX_FK_RouteRoutine]
ON [dbo].[RoutineSet]
    ([RouteId]);
GO

-- Creating foreign key on [Dates_Id] in table 'DateRoutine'
ALTER TABLE [dbo].[DateRoutine]
ADD CONSTRAINT [FK_DateRoutine_Date]
    FOREIGN KEY ([Dates_Id])
    REFERENCES [dbo].[DateSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Routines_Id] in table 'DateRoutine'
ALTER TABLE [dbo].[DateRoutine]
ADD CONSTRAINT [FK_DateRoutine_Routine]
    FOREIGN KEY ([Routines_Id])
    REFERENCES [dbo].[RoutineSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_DateRoutine_Routine'
CREATE INDEX [IX_FK_DateRoutine_Routine]
ON [dbo].[DateRoutine]
    ([Routines_Id]);
GO

-- Creating foreign key on [Activity_Id] in table 'TaskSet'
ALTER TABLE [dbo].[TaskSet]
ADD CONSTRAINT [FK_ActivityTask]
    FOREIGN KEY ([Activity_Id])
    REFERENCES [dbo].[ActivitySet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ActivityTask'
CREATE INDEX [IX_FK_ActivityTask]
ON [dbo].[TaskSet]
    ([Activity_Id]);
GO

-- Creating foreign key on [Users_Id] in table 'UserRoutine'
ALTER TABLE [dbo].[UserRoutine]
ADD CONSTRAINT [FK_UserRoutine_User]
    FOREIGN KEY ([Users_Id])
    REFERENCES [dbo].[UserSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Routines_Id] in table 'UserRoutine'
ALTER TABLE [dbo].[UserRoutine]
ADD CONSTRAINT [FK_UserRoutine_Routine]
    FOREIGN KEY ([Routines_Id])
    REFERENCES [dbo].[RoutineSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserRoutine_Routine'
CREATE INDEX [IX_FK_UserRoutine_Routine]
ON [dbo].[UserRoutine]
    ([Routines_Id]);
GO

-- Creating foreign key on [ActivityId] in table 'ActivityUserSet'
ALTER TABLE [dbo].[ActivityUserSet]
ADD CONSTRAINT [FK_ActivityActivityUser]
    FOREIGN KEY ([ActivityId])
    REFERENCES [dbo].[ActivitySet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ActivityActivityUser'
CREATE INDEX [IX_FK_ActivityActivityUser]
ON [dbo].[ActivityUserSet]
    ([ActivityId]);
GO

-- Creating foreign key on [TakenBy_Id] in table 'ActivitySet'
ALTER TABLE [dbo].[ActivitySet]
ADD CONSTRAINT [FK_ActivityUserActivity]
    FOREIGN KEY ([TakenBy_Id])
    REFERENCES [dbo].[ActivityUserSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ActivityUserActivity'
CREATE INDEX [IX_FK_ActivityUserActivity]
ON [dbo].[ActivitySet]
    ([TakenBy_Id]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------