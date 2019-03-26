CREATE TABLE [dbo].[User] (
    [Id]          BIGINT         IDENTITY (1, 1) NOT NULL,
    [Login]       NVARCHAR (150) NOT NULL,
    [IsActive]    BIT            CONSTRAINT [DF_User_IsActive] DEFAULT ((1)) NOT NULL,
    [CreatedById] BIGINT         NULL,
    [CreatedDate] DATETIME       NULL,
    [UpdatedById] BIGINT         NULL,
    [UpdatedDate] DATETIME       NULL,
    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([Id] ASC)
);





