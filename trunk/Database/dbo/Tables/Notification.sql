CREATE TABLE [dbo].[Notification] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [UserId]      INT            NOT NULL,
    [Subject]     NVARCHAR (200) NOT NULL,
    [Message]     NVARCHAR (200) NOT NULL,
    [Read]        BIT            CONSTRAINT [DF_Notification_Read] DEFAULT ((0)) NOT NULL,
    [Deleted]     BIT            CONSTRAINT [DF_Notification_Deleted] DEFAULT ((0)) NOT NULL,
    [CreatedDate] DATETIME       NULL,
    [UpdatedDate] DATETIME       NULL,
    CONSTRAINT [PK_Notification] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Notification_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[User] ([Id])
);

