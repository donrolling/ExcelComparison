CREATE TABLE [dbo].[RolePermission] (
    [RoleId]       BIGINT   NOT NULL,
    [PermissionId] BIGINT   NOT NULL,
    [IsActive]     BIT      CONSTRAINT [DF_RolePermission_IsActive] DEFAULT ((1)) NOT NULL,
    [CreatedById]  BIGINT   NULL,
    [CreatedDate]  DATETIME NULL,
    [UpdatedById]  BIGINT   NULL,
    [UpdatedDate]  DATETIME NULL,
    CONSTRAINT [PK_RolePermission] PRIMARY KEY CLUSTERED ([RoleId] ASC, [PermissionId] ASC),
    CONSTRAINT [FK_RolePermission_Permission] FOREIGN KEY ([PermissionId]) REFERENCES [dbo].[Permission] ([Id]),
    CONSTRAINT [FK_RolePermission_Role] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Role] ([Id])
);

