CREATE TABLE [dbo].[User] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [DisplayName] NVARCHAR (200) NOT NULL,
    [Email]       NVARCHAR (200) NOT NULL,
    [Salt]        NVARCHAR (200) NOT NULL,
    [Password]    NVARCHAR (200) NOT NULL,
    [Rating]      INT            NULL,
    [CreatedDate] DATETIME       NULL,
    [UpdatedDate] DATETIME       NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([Id] ASC)
);

