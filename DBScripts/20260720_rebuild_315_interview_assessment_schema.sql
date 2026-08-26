/*
    MODULE 315 - FULL INTERVIEW ASSESSMENT REBUILD
    SQL Server 2014 / compatibility level 120

    DESTRUCTIVE SCRIPT:
    - Drops all old and new interview-assessment tables.
    - Deletes all existing interview reports, assignments, PDFs metadata and scores.
    - Creates a clean normalized schema. There is no legacy-data migration.

    Set @ConfirmDestructiveRebuild = 1 only after database/file backup and review.
*/

SET NOCOUNT ON;
SET XACT_ABORT ON;

DECLARE @ConfirmDestructiveRebuild bit = 0;
DECLARE @CurrentDatabase sysname = DB_NAME();

IF @CurrentDatabase <> N'DBDocumentManagementSystem'
BEGIN
    RAISERROR(N'Wrong database. Expected DBDocumentManagementSystem, current database is %s.', 16, 1, @CurrentDatabase);
    RETURN;
END;

IF @ConfirmDestructiveRebuild <> 1
BEGIN
    RAISERROR(N'Destructive rebuild is locked. Set @ConfirmDestructiveRebuild = 1 after backup and review.', 16, 1);
    RETURN;
END;

BEGIN TRY
    BEGIN TRANSACTION;

    /* Drop every FK connected to the target tables, independent of FK name. */
    DECLARE @DropForeignKeys nvarchar(max) = N'';

    SELECT @DropForeignKeys = @DropForeignKeys
        + N'ALTER TABLE ' + QUOTENAME(OBJECT_SCHEMA_NAME(fk.parent_object_id))
        + N'.' + QUOTENAME(OBJECT_NAME(fk.parent_object_id))
        + N' DROP CONSTRAINT ' + QUOTENAME(fk.name) + N';' + CHAR(13)
    FROM sys.foreign_keys AS fk
    WHERE OBJECT_NAME(fk.parent_object_id) IN
          (
              N'dt315_InterviewReport',
              N'dt315_InterviewCandidate',
              N'dt315_InterviewDefaultInterviewer',
              N'dt315_InterviewAssignment',
              N'dt315_InterviewScore',
              N'dt315_InterviewScoreAudit',
              N'dt315_InterviewAssignmentAudit',
              N'dt307_InterviewReport',
              N'dt307_InterviewCandidate',
              N'dt307_InterviewDefaultInterviewer',
              N'dt307_InterviewAssignment',
              N'dt307_InterviewScore',
              N'dt307_InterviewScoreAudit',
              N'dt307_InterviewAssignmentAudit'
          )
       OR OBJECT_NAME(fk.referenced_object_id) IN
          (
              N'dt315_InterviewReport',
              N'dt315_InterviewCandidate',
              N'dt315_InterviewDefaultInterviewer',
              N'dt315_InterviewAssignment',
              N'dt315_InterviewScore',
              N'dt315_InterviewScoreAudit',
              N'dt315_InterviewAssignmentAudit',
              N'dt307_InterviewReport',
              N'dt307_InterviewCandidate',
              N'dt307_InterviewDefaultInterviewer',
              N'dt307_InterviewAssignment',
              N'dt307_InterviewScore',
              N'dt307_InterviewScoreAudit',
              N'dt307_InterviewAssignmentAudit'
          );

    IF LEN(@DropForeignKeys) > 0
        EXEC sys.sp_executesql @DropForeignKeys;

    /* Remove the obsolete empty interview schema from module 307. */
    IF OBJECT_ID(N'dbo.dt307_InterviewScoreAudit', N'U') IS NOT NULL
        DROP TABLE dbo.dt307_InterviewScoreAudit;

    IF OBJECT_ID(N'dbo.dt307_InterviewAssignmentAudit', N'U') IS NOT NULL
        DROP TABLE dbo.dt307_InterviewAssignmentAudit;

    IF OBJECT_ID(N'dbo.dt307_InterviewScore', N'U') IS NOT NULL
        DROP TABLE dbo.dt307_InterviewScore;

    IF OBJECT_ID(N'dbo.dt307_InterviewAssignment', N'U') IS NOT NULL
        DROP TABLE dbo.dt307_InterviewAssignment;

    IF OBJECT_ID(N'dbo.dt307_InterviewDefaultInterviewer', N'U') IS NOT NULL
        DROP TABLE dbo.dt307_InterviewDefaultInterviewer;

    IF OBJECT_ID(N'dbo.dt307_InterviewCandidate', N'U') IS NOT NULL
        DROP TABLE dbo.dt307_InterviewCandidate;

    IF OBJECT_ID(N'dbo.dt307_InterviewReport', N'U') IS NOT NULL
        DROP TABLE dbo.dt307_InterviewReport;

    IF OBJECT_ID(N'dbo.dt315_InterviewScoreAudit', N'U') IS NOT NULL
        DROP TABLE dbo.dt315_InterviewScoreAudit;

    IF OBJECT_ID(N'dbo.dt315_InterviewAssignmentAudit', N'U') IS NOT NULL
        DROP TABLE dbo.dt315_InterviewAssignmentAudit;

    IF OBJECT_ID(N'dbo.dt315_InterviewScore', N'U') IS NOT NULL
        DROP TABLE dbo.dt315_InterviewScore;

    IF OBJECT_ID(N'dbo.dt315_InterviewAssignment', N'U') IS NOT NULL
        DROP TABLE dbo.dt315_InterviewAssignment;

    IF OBJECT_ID(N'dbo.dt315_InterviewDefaultInterviewer', N'U') IS NOT NULL
        DROP TABLE dbo.dt315_InterviewDefaultInterviewer;

    IF OBJECT_ID(N'dbo.dt315_InterviewCandidate', N'U') IS NOT NULL
        DROP TABLE dbo.dt315_InterviewCandidate;

    IF OBJECT_ID(N'dbo.dt315_InterviewReport', N'U') IS NOT NULL
        DROP TABLE dbo.dt315_InterviewReport;

    /* Assessment period. */
    CREATE TABLE dbo.dt315_InterviewReport
    (
        Id           varchar(8) NOT NULL,
        DisplayName  nvarchar(256) NOT NULL,
        StartAt      datetime2(0) NOT NULL,
        EndAt        datetime2(0) NOT NULL,
        Status       varchar(20) NOT NULL
            CONSTRAINT DF_dt315_InterviewReport_Status DEFAULT ('Draft'),
        Notes        nvarchar(1000) NULL,
        CreatedBy    varchar(10) NOT NULL,
        CreatedAt    datetime2(0) NOT NULL
            CONSTRAINT DF_dt315_InterviewReport_CreatedAt DEFAULT (sysdatetime()),
        UpdatedBy    varchar(10) NULL,
        UpdatedAt    datetime2(0) NULL,
        OpenedAt     datetime2(0) NULL,
        CompletedAt  datetime2(0) NULL,
        RowVersion   rowversion NOT NULL,

        CONSTRAINT PK_dt315_InterviewReport PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT CK_dt315_InterviewReport_Status
            CHECK (Status IN ('Draft', 'Open', 'Completed', 'Closed', 'Archived')),
        CONSTRAINT CK_dt315_InterviewReport_TimeRange CHECK (EndAt > StartAt)
    );

    CREATE NONCLUSTERED INDEX IX_dt315_InterviewReport_StatusTime
        ON dbo.dt315_InterviewReport(Status, StartAt, EndAt);

    /* One candidate and one current PDF per assessment period. */
    CREATE TABLE dbo.dt315_InterviewCandidate
    (
        Id                      bigint IDENTITY(1,1) NOT NULL,
        ReportId                varchar(8) NOT NULL,
        CandidateId             varchar(10) NOT NULL,
        UsesDefaultInterviewers bit NOT NULL
            CONSTRAINT DF_dt315_InterviewCandidate_UsesDefault DEFAULT (1),
        OriginalFileName        nvarchar(260) NULL,
        PhysicalFileName        varchar(50) NULL,
        RelativePath            nvarchar(500) NULL,
        FileSize                bigint NULL,
        Sha256                  char(64) NULL,
        UploadedBy              varchar(10) NULL,
        UploadedAt              datetime2(0) NULL,
        CreatedAt               datetime2(0) NOT NULL
            CONSTRAINT DF_dt315_InterviewCandidate_CreatedAt DEFAULT (sysdatetime()),
        UpdatedAt               datetime2(0) NOT NULL
            CONSTRAINT DF_dt315_InterviewCandidate_UpdatedAt DEFAULT (sysdatetime()),
        RowVersion              rowversion NOT NULL,

        CONSTRAINT PK_dt315_InterviewCandidate PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT UQ_dt315_InterviewCandidate_ReportCandidate UNIQUE (ReportId, CandidateId),
        CONSTRAINT CK_dt315_InterviewCandidate_FileSize
            CHECK (FileSize IS NULL OR FileSize BETWEEN 1 AND 20971520),
        CONSTRAINT CK_dt315_InterviewCandidate_FileMetadata CHECK
        (
            (PhysicalFileName IS NULL AND RelativePath IS NULL AND FileSize IS NULL
             AND Sha256 IS NULL AND UploadedBy IS NULL AND UploadedAt IS NULL)
            OR
            (PhysicalFileName IS NOT NULL AND RelativePath IS NOT NULL AND FileSize IS NOT NULL
             AND Sha256 IS NOT NULL AND UploadedBy IS NOT NULL AND UploadedAt IS NOT NULL)
        ),
        CONSTRAINT FK_dt315_InterviewCandidate_Report
            FOREIGN KEY (ReportId) REFERENCES dbo.dt315_InterviewReport(Id)
    );

    CREATE NONCLUSTERED INDEX IX_dt315_InterviewCandidate_Report
        ON dbo.dt315_InterviewCandidate(ReportId)
        INCLUDE (CandidateId, UsesDefaultInterviewers, UploadedAt);

    /* Interviewers selected once and applied by default to the entire period. */
    CREATE TABLE dbo.dt315_InterviewDefaultInterviewer
    (
        ReportId       varchar(8) NOT NULL,
        InterviewerId  varchar(10) NOT NULL,
        AssignedBy     varchar(10) NOT NULL,
        AssignedAt     datetime2(0) NOT NULL
            CONSTRAINT DF_dt315_InterviewDefaultInterviewer_AssignedAt DEFAULT (sysdatetime()),

        CONSTRAINT PK_dt315_InterviewDefaultInterviewer PRIMARY KEY CLUSTERED (ReportId, InterviewerId),
        CONSTRAINT FK_dt315_InterviewDefaultInterviewer_Report
            FOREIGN KEY (ReportId) REFERENCES dbo.dt315_InterviewReport(Id)
    );

    CREATE NONCLUSTERED INDEX IX_dt315_InterviewDefaultInterviewer_User
        ON dbo.dt315_InterviewDefaultInterviewer(InterviewerId, ReportId);

    /* Actual per-candidate assignment used for authorization and scoring. */
    CREATE TABLE dbo.dt315_InterviewAssignment
    (
        Id                  bigint IDENTITY(1,1) NOT NULL,
        CandidateProfileId  bigint NOT NULL,
        InterviewerId       varchar(10) NOT NULL,
        Source              varchar(10) NOT NULL,
        IsActive            bit NOT NULL
            CONSTRAINT DF_dt315_InterviewAssignment_IsActive DEFAULT (1),
        AssignedBy          varchar(10) NOT NULL,
        AssignedAt          datetime2(0) NOT NULL
            CONSTRAINT DF_dt315_InterviewAssignment_AssignedAt DEFAULT (sysdatetime()),
        RemovedBy           varchar(10) NULL,
        RemovedAt           datetime2(0) NULL,
        RowVersion          rowversion NOT NULL,

        CONSTRAINT PK_dt315_InterviewAssignment PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT CK_dt315_InterviewAssignment_Source
            CHECK (Source IN ('Default', 'Custom')),
        CONSTRAINT CK_dt315_InterviewAssignment_Removal CHECK
        (
            (IsActive = 1 AND RemovedBy IS NULL AND RemovedAt IS NULL)
            OR
            (IsActive = 0 AND RemovedBy IS NOT NULL AND RemovedAt IS NOT NULL)
        ),
        CONSTRAINT FK_dt315_InterviewAssignment_Candidate
            FOREIGN KEY (CandidateProfileId) REFERENCES dbo.dt315_InterviewCandidate(Id)
    );

    CREATE UNIQUE NONCLUSTERED INDEX UX_dt315_InterviewAssignment_Active
        ON dbo.dt315_InterviewAssignment(CandidateProfileId, InterviewerId)
        WHERE IsActive = 1;

    CREATE NONCLUSTERED INDEX IX_dt315_InterviewAssignment_Interviewer
        ON dbo.dt315_InterviewAssignment(InterviewerId, IsActive)
        INCLUDE (CandidateProfileId, AssignedAt);

    /*
        All four criteria use a 0-100 scale.
        Total = ProfessionalSkill 40% + Responsiveness 30%
              + Communication 20% + ReportQuality 10%.
    */
    CREATE TABLE dbo.dt315_InterviewScore
    (
        Id                      bigint IDENTITY(1,1) NOT NULL,
        AssignmentId            bigint NOT NULL,
        ProfessionalSkill       int NOT NULL,
        ProfessionalSkillNote   nvarchar(500) NULL,
        Responsiveness          int NOT NULL,
        ResponsivenessNote      nvarchar(500) NULL,
        Communication           int NOT NULL,
        CommunicationNote       nvarchar(500) NULL,
        ReportQuality           int NOT NULL,
        ReportQualityNote       nvarchar(500) NULL,
        Total                   numeric(5,1) NOT NULL,
        SubmittedAt             datetime2(0) NOT NULL,
        UpdatedAt               datetime2(0) NOT NULL
            CONSTRAINT DF_dt315_InterviewScore_UpdatedAt DEFAULT (sysdatetime()),
        ReopenedAt              datetime2(0) NULL,
        ReopenedBy              varchar(10) NULL,
        RowVersion              rowversion NOT NULL,

        CONSTRAINT PK_dt315_InterviewScore PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT UQ_dt315_InterviewScore_Assignment UNIQUE (AssignmentId),
        CONSTRAINT CK_dt315_InterviewScore_ProfessionalSkill CHECK (ProfessionalSkill BETWEEN 0 AND 100),
        CONSTRAINT CK_dt315_InterviewScore_Responsiveness CHECK (Responsiveness BETWEEN 0 AND 100),
        CONSTRAINT CK_dt315_InterviewScore_Communication CHECK (Communication BETWEEN 0 AND 100),
        CONSTRAINT CK_dt315_InterviewScore_ReportQuality CHECK (ReportQuality BETWEEN 0 AND 100),
        CONSTRAINT CK_dt315_InterviewScore_Total CHECK
        (
            Total = CAST
            (
                (ProfessionalSkill * 4
                 + Responsiveness * 3
                 + Communication * 2
                 + ReportQuality) / 10.0
                AS numeric(5,1)
            )
        ),
        CONSTRAINT CK_dt315_InterviewScore_Reopened CHECK
        (
            (ReopenedAt IS NULL AND ReopenedBy IS NULL)
            OR (ReopenedAt IS NOT NULL AND ReopenedBy IS NOT NULL)
        ),
        CONSTRAINT FK_dt315_InterviewScore_Assignment
            FOREIGN KEY (AssignmentId) REFERENCES dbo.dt315_InterviewAssignment(Id)
    );

    CREATE NONCLUSTERED INDEX IX_dt315_InterviewScore_SubmittedAt
        ON dbo.dt315_InterviewScore(SubmittedAt)
        INCLUDE (AssignmentId, Total);

    /* Full score snapshot on submit, reopen and resubmit. */
    CREATE TABLE dbo.dt315_InterviewScoreAudit
    (
        Id                      bigint IDENTITY(1,1) NOT NULL,
        ScoreId                 bigint NOT NULL,
        Action                  varchar(20) NOT NULL,
        ActorId                 varchar(10) NOT NULL,
        ActionAt                datetime2(0) NOT NULL
            CONSTRAINT DF_dt315_InterviewScoreAudit_ActionAt DEFAULT (sysdatetime()),
        Reason                  nvarchar(500) NULL,
        ProfessionalSkill       int NOT NULL,
        ProfessionalSkillNote   nvarchar(500) NULL,
        Responsiveness          int NOT NULL,
        ResponsivenessNote      nvarchar(500) NULL,
        Communication           int NOT NULL,
        CommunicationNote       nvarchar(500) NULL,
        ReportQuality           int NOT NULL,
        ReportQualityNote       nvarchar(500) NULL,
        Total                   numeric(5,1) NOT NULL,
        SubmittedAt             datetime2(0) NOT NULL,

        CONSTRAINT PK_dt315_InterviewScoreAudit PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT CK_dt315_InterviewScoreAudit_Action
            CHECK (Action IN ('Submitted', 'Reopened', 'Resubmitted')),
        CONSTRAINT FK_dt315_InterviewScoreAudit_Score
            FOREIGN KEY (ScoreId) REFERENCES dbo.dt315_InterviewScore(Id)
    );

    CREATE NONCLUSTERED INDEX IX_dt315_InterviewScoreAudit_Score
        ON dbo.dt315_InterviewScoreAudit(ScoreId, ActionAt);

    /* Assignment history supports later investigation of custom exceptions. */
    CREATE TABLE dbo.dt315_InterviewAssignmentAudit
    (
        Id                  bigint IDENTITY(1,1) NOT NULL,
        AssignmentId        bigint NOT NULL,
        Action              varchar(20) NOT NULL,
        ActorId             varchar(10) NOT NULL,
        ActionAt            datetime2(0) NOT NULL
            CONSTRAINT DF_dt315_InterviewAssignmentAudit_ActionAt DEFAULT (sysdatetime()),
        Reason              nvarchar(500) NULL,
        CandidateProfileId  bigint NOT NULL,
        InterviewerId       varchar(10) NOT NULL,
        Source              varchar(10) NOT NULL,

        CONSTRAINT PK_dt315_InterviewAssignmentAudit PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT CK_dt315_InterviewAssignmentAudit_Action
            CHECK (Action IN ('Assigned', 'Removed', 'Restored')),
        CONSTRAINT FK_dt315_InterviewAssignmentAudit_Assignment
            FOREIGN KEY (AssignmentId) REFERENCES dbo.dt315_InterviewAssignment(Id)
    );

    CREATE NONCLUSTERED INDEX IX_dt315_InterviewAssignmentAudit_Assignment
        ON dbo.dt315_InterviewAssignmentAudit(AssignmentId, ActionAt);

    COMMIT TRANSACTION;

    PRINT N'Full interview-assessment schema rebuild completed successfully.';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    DECLARE @ErrorMessage nvarchar(4000) = ERROR_MESSAGE();
    DECLARE @ErrorNumber int = ERROR_NUMBER();
    DECLARE @ErrorLine int = ERROR_LINE();

    RAISERROR(N'Rebuild failed. Error %d at line %d: %s', 16, 1, @ErrorNumber, @ErrorLine, @ErrorMessage);
END CATCH;
