
USE [TalariusChess]
GO

SET IDENTITY_INSERT [dbo].[GameData] ON
GO

INSERT [dbo].[GameData] ([Id], [FEN], [PGN], [Result], [CreatedDate], [UpdatedDate]) VALUES (5, N'rnbqk2r/pppp1ppp/5n2/8/1bP1PB2/1QN5/PP1K1PPP/R4BNR b kq - 4 7', N'1. e4 e5 2. d4 exd4 3. c4 d3 4. Bf4 d2 5. Kxd2 Bb4 6. Nc3 Nf6 7. Qb3 ', NULL, NULL, NULL)
GO

SET IDENTITY_INSERT [dbo].[GameData] OFF
GO

SET IDENTITY_INSERT [dbo].[User] ON
GO

INSERT [dbo].[User] ([Id], [DisplayName], [Email], [Salt], [Password], [Rating], [CreatedDate], [UpdatedDate]) VALUES (1, N'Don', N'donrolling@hotmail.com', N'Ub28wJs8EZg=', N'�]� �B�V�R����a�2��܎)�G���', NULL, NULL, NULL)
GO

INSERT [dbo].[User] ([Id], [DisplayName], [Email], [Salt], [Password], [Rating], [CreatedDate], [UpdatedDate]) VALUES (2, N'Ivan', N'markov.ivan@gmail.com', N'GPzDseSW+FU=', N'; ���s^]��X3M���''�c�"n�~1��x�', NULL, NULL, NULL)
GO

INSERT [dbo].[User] ([Id], [DisplayName], [Email], [Salt], [Password], [Rating], [CreatedDate], [UpdatedDate]) VALUES (1002, N'Donnie G', N'donrolling@gmail.com', N'45xuHCiqZ5Y=', N'�jZ>!<��1*��-�9@���.@`�&�ی,��?', NULL, NULL, NULL)
GO

SET IDENTITY_INSERT [dbo].[User] OFF
GO

SET IDENTITY_INSERT [dbo].[User_GameData] ON
GO

INSERT [dbo].[User_GameData] ([Id], [CreatorUserId], [OpponentUserId], [GameDataId], [CreatorPlayerColor], [ActiveGame], [CreatedDate], [UpdatedDate]) VALUES (1, 1, 2, 5, N'White', 1, CAST(0x0000A1A4009AF5FF AS DateTime), NULL)
GO

INSERT [dbo].[User_GameData] ([Id], [CreatorUserId], [OpponentUserId], [GameDataId], [CreatorPlayerColor], [ActiveGame], [CreatedDate], [UpdatedDate]) VALUES (1002, 1002, 2, NULL, N'White', 0, CAST(0x0000A1AD00E29121 AS DateTime), NULL)
GO

INSERT [dbo].[User_GameData] ([Id], [CreatorUserId], [OpponentUserId], [GameDataId], [CreatorPlayerColor], [ActiveGame], [CreatedDate], [UpdatedDate]) VALUES (1003, 1002, 2, NULL, N'White', 0, CAST(0x0000A1AD00E32B77 AS DateTime), NULL)
GO

INSERT [dbo].[User_GameData] ([Id], [CreatorUserId], [OpponentUserId], [GameDataId], [CreatorPlayerColor], [ActiveGame], [CreatedDate], [UpdatedDate]) VALUES (1004, 1002, 2, NULL, N'White', 0, CAST(0x0000A1AD00E407C8 AS DateTime), NULL)
GO

INSERT [dbo].[User_GameData] ([Id], [CreatorUserId], [OpponentUserId], [GameDataId], [CreatorPlayerColor], [ActiveGame], [CreatedDate], [UpdatedDate]) VALUES (1005, 1, 2, NULL, N'White', 0, CAST(0x0000A1AD01048959 AS DateTime), NULL)
GO

SET IDENTITY_INSERT [dbo].[User_GameData] OFF
GO
