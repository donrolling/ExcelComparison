CREATE TABLE [dbo].[Permission] (
    [Id]          BIGINT         IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (150) NOT NULL,
    [Action]      NVARCHAR (150) NULL,
    [IsActive]    BIT            CONSTRAINT [DF_Permission_IsActive] DEFAULT ((1)) NOT NULL,
    [CreatedById] BIGINT         NULL,
    [CreatedDate] DATETIME       NULL,
    [UpdatedById] BIGINT         NULL,
    [UpdatedDate] DATETIME       NULL,
    CONSTRAINT [PK_Permission] PRIMARY KEY CLUSTERED ([Id] ASC)
);

