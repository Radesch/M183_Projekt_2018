CREATE TABLE [dbo].[Comment] (
    [Id]        INT      IDENTITY (1, 1) NOT NULL,
    [PostId]    INT      NULL,
    [UserId]    INT      NULL,
    [Commet]    TEXT     NULL,
    [CreatedOn] DATETIME NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[Post] (
    [Id]          INT           IDENTITY (1, 1) NOT NULL,
    [UserId]      INT           NOT NULL,
    [Title]       VARCHAR (50)  NOT NULL,
    [Description] VARCHAR (500) NOT NULL,
    [Content]     TEXT          NOT NULL,
    [CreatedOn]   DATETIME      NULL,
    [ModifiedOn]  DATETIME      NULL,
    [DeletedOn]   DATETIME      NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[Token] (
    [Id]        INT          IDENTITY (1, 1) NOT NULL,
    [Token]     VARCHAR (50) NOT NULL,
    [UserId]    INT          NOT NULL,
    [Expiry]    DATETIME     NOT NULL,
    [DeletedOn] DATETIME     NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);
CREATE TABLE [dbo].[User] (
    [Id]                INT          IDENTITY (1, 1) NOT NULL,
    [Username]          VARCHAR (50) NOT NULL,
    [Password]          VARCHAR (50) NOT NULL,
    [Firstname]         VARCHAR (50) NOT NULL,
    [Familyname]        VARCHAR (50) NOT NULL,
    [Mobilephonenumber] VARCHAR (50) NULL,
    [Role]              VARCHAR (20) NULL,
    [Status]            VARCHAR (20) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[UserLog] (
    [Id]     INT           IDENTITY (1, 1) NOT NULL,
    [UserId] INT           NULL,
    [Action] VARCHAR (500) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[Userlogin] (
    [Id]         INT          IDENTITY (1, 1) NOT NULL,
    [UserId]     INT          NOT NULL,
    [IP]         VARCHAR (50) NULL,
    [SessionId]  VARCHAR (50) NULL,
    [CreatedOn]  DATETIME     NULL,
    [ModifiedOn] DATETIME     NULL,
    [DeletedOn]  DATETIME     NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

SET IDENTITY_INSERT [dbo].[User] ON
INSERT INTO [dbo].[User] ([Id], [Username], [Password], [Firstname], [Familyname], [Mobilephonenumber], [Role], [Status]) VALUES (1, N'hans@muster.ch', N'Must3R', N'Hans', N'Muster', NULL, N'user', N'active')
INSERT INTO [dbo].[User] ([Id], [Username], [Password], [Firstname], [Familyname], [Mobilephonenumber], [Role], [Status]) VALUES (2, N'eva@muster.ch', N'MustER', N'Eva', N'Muster', NULL, N'admin', N'active')
SET IDENTITY_INSERT [dbo].[User] OFF

SET IDENTITY_INSERT [dbo].[Comment] ON
INSERT INTO [dbo].[Comment] ([Id], [PostId], [UserId], [Commet], [CreatedOn]) VALUES (1, 1, 1, N'Test Comment', NULL)
INSERT INTO [dbo].[Comment] ([Id], [PostId], [UserId], [Commet], [CreatedOn]) VALUES (2, 2, 1, N'Test Commet 2', NULL)
SET IDENTITY_INSERT [dbo].[Comment] OFF

SET IDENTITY_INSERT [dbo].[Post] ON
INSERT INTO [dbo].[Post] ([Id], [UserId], [Title], [Description], [Content], [CreatedOn], [ModifiedOn], [DeletedOn]) VALUES (1, 1, N'Lorem Ipsum', N'Dolor Sit amiet', N'Testcontent', NULL, NULL, NULL)
INSERT INTO [dbo].[Post] ([Id], [UserId], [Title], [Description], [Content], [CreatedOn], [ModifiedOn], [DeletedOn]) VALUES (2, 2, N'Lorem Ipsum', N'Dolor Sit amiet', N'Testcontent 2', NULL, NULL, NULL)
SET IDENTITY_INSERT [dbo].[Post] OFF
