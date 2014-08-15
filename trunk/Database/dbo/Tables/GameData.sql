CREATE TABLE [dbo].[GameData] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [FEN]         NVARCHAR (100) NOT NULL,
    [PGN]         NVARCHAR (MAX) NULL,
    [Result]      NVARCHAR (50)  NULL,
    [CreatedDate] DATETIME       NULL,
    [UpdatedDate] DATETIME       NULL,
    CONSTRAINT [PK_GameData] PRIMARY KEY CLUSTERED ([Id] ASC)
);

