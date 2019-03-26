CREATE TABLE [dbo].[Role] (
    [Id]          BIGINT        IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (50) NOT NULL,
    [IsActive]    BIT           CONSTRAINT [DF_Role_IsActive] DEFAULT ((1)) NOT NULL,
    [CreatedById] BIGINT        NULL,
    [CreatedDate] DATETIME      NULL,
    [UpdatedById] BIGINT        NULL,
    [UpdatedDate] DATETIME      NULL,
    CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED ([Id] ASC)
);

