CREATE TABLE [dbo].[UserRole] (
    [UserId]      BIGINT   NOT NULL,
    [RoleId]      BIGINT   NOT NULL,
    [IsActive]    BIT      CONSTRAINT [DF_UserRole_IsActive] DEFAULT ((1)) NOT NULL,
    [CreatedById] BIGINT   NULL,
    [CreatedDate] DATETIME NULL,
    [UpdatedById] BIGINT   NULL,
    [UpdatedDate] DATETIME NULL,
    CONSTRAINT [PK_UserRole] PRIMARY KEY CLUSTERED ([UserId] ASC, [RoleId] ASC),
    CONSTRAINT [FK_UserRole_Role] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Role] ([Id]),
    CONSTRAINT [FK_UserRole_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[User] ([Id])
);

