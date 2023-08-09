IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628092337_init')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230628092337_init', N'7.0.8');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628092849_dbset-fix')
BEGIN
    CREATE TABLE [ImageCollections] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(80) NOT NULL,
        CONSTRAINT [PK_ImageCollections] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628092849_dbset-fix')
BEGIN
    CREATE TABLE [Images] (
        [Id] int NOT NULL IDENTITY,
        [Title] nvarchar(80) NOT NULL,
        [Description] nvarchar(80) NOT NULL,
        [ImageCollectionId] int NULL,
        [ImageName] nvarchar(max) NULL,
        CONSTRAINT [PK_Images] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Images_ImageCollections_ImageCollectionId] FOREIGN KEY ([ImageCollectionId]) REFERENCES [ImageCollections] ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628092849_dbset-fix')
BEGIN
    CREATE INDEX [IX_Images_ImageCollectionId] ON [Images] ([ImageCollectionId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628092849_dbset-fix')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230628092849_dbset-fix', N'7.0.8');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628110603_seed-data')
BEGIN
    ALTER TABLE [Images] DROP CONSTRAINT [FK_Images_ImageCollections_ImageCollectionId];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628110603_seed-data')
BEGIN
    DROP INDEX [IX_Images_ImageCollectionId] ON [Images];
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Images]') AND [c].[name] = N'ImageCollectionId');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Images] DROP CONSTRAINT [' + @var0 + '];');
    EXEC(N'UPDATE [Images] SET [ImageCollectionId] = 0 WHERE [ImageCollectionId] IS NULL');
    ALTER TABLE [Images] ALTER COLUMN [ImageCollectionId] int NOT NULL;
    ALTER TABLE [Images] ADD DEFAULT 0 FOR [ImageCollectionId];
    CREATE INDEX [IX_Images_ImageCollectionId] ON [Images] ([ImageCollectionId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628110603_seed-data')
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[ImageCollections]'))
        SET IDENTITY_INSERT [ImageCollections] ON;
    EXEC(N'INSERT INTO [ImageCollections] ([Id], [Name])
    VALUES (1, N''collection 1'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[ImageCollections]'))
        SET IDENTITY_INSERT [ImageCollections] OFF;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628110603_seed-data')
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'ImageCollectionId', N'ImageName', N'Title') AND [object_id] = OBJECT_ID(N'[Images]'))
        SET IDENTITY_INSERT [Images] ON;
    EXEC(N'INSERT INTO [Images] ([Id], [Description], [ImageCollectionId], [ImageName], [Title])
    VALUES (1, N''desc 1'', 1, N''test.png'', N''image 1'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'ImageCollectionId', N'ImageName', N'Title') AND [object_id] = OBJECT_ID(N'[Images]'))
        SET IDENTITY_INSERT [Images] OFF;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628110603_seed-data')
BEGIN
    ALTER TABLE [Images] ADD CONSTRAINT [FK_Images_ImageCollections_ImageCollectionId] FOREIGN KEY ([ImageCollectionId]) REFERENCES [ImageCollections] ([Id]) ON DELETE CASCADE;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628110603_seed-data')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230628110603_seed-data', N'7.0.8');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230630122323_AddedNonMappedImageFile')
BEGIN
    DECLARE @var1 sysname;
    SELECT @var1 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Images]') AND [c].[name] = N'ImageName');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Images] DROP CONSTRAINT [' + @var1 + '];');
    EXEC(N'UPDATE [Images] SET [ImageName] = N'''' WHERE [ImageName] IS NULL');
    ALTER TABLE [Images] ALTER COLUMN [ImageName] nvarchar(80) NOT NULL;
    ALTER TABLE [Images] ADD DEFAULT N'' FOR [ImageName];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230630122323_AddedNonMappedImageFile')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230630122323_AddedNonMappedImageFile', N'7.0.8');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230630122427_ImageNameAdjustment')
BEGIN
    DECLARE @var2 sysname;
    SELECT @var2 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Images]') AND [c].[name] = N'ImageName');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [Images] DROP CONSTRAINT [' + @var2 + '];');
    ALTER TABLE [Images] ALTER COLUMN [ImageName] nvarchar(100) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230630122427_ImageNameAdjustment')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230630122427_ImageNameAdjustment', N'7.0.8');
END;
GO

COMMIT;
GO

