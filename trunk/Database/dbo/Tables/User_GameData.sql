CREATE TABLE [dbo].[User_GameData] (
    [Id]                 INT           IDENTITY (1, 1) NOT NULL,
    [CreatorUserId]      INT           NOT NULL,
    [OpponentUserId]     INT           NOT NULL,
    [GameDataId]         INT           NULL,
    [CreatorPlayerColor] NVARCHAR (10) NOT NULL,
    [ActiveGame]         BIT           NOT NULL,
    [CreatedDate]        DATETIME      NOT NULL,
    [UpdatedDate]        DATETIME      NULL,
    CONSTRAINT [PK_UserGameData] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UserGameData_CreatorUser] FOREIGN KEY ([CreatorUserId]) REFERENCES [dbo].[User] ([Id]),
    CONSTRAINT [FK_UserGameData_GameData] FOREIGN KEY ([GameDataId]) REFERENCES [dbo].[GameData] ([Id]),
    CONSTRAINT [FK_UserGameData_OpponentUser] FOREIGN KEY ([OpponentUserId]) REFERENCES [dbo].[User] ([Id])
);

