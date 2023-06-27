--1
CREATE TABLE Categories(
	Id INT PRIMARY KEY IDENTITY
	,[Name] VARCHAR(50) NOT NULL
	);

CREATE TABLE Addresses(
	Id INT PRIMARY KEY IDENTITY
	,StreetName NVARCHAR(100) NOT NULL
	,StreetNumber INT NOT NULL
	,Town VARCHAR(30) NOT NULL
	,Country VARCHAR(50) NOT NULL
	,ZIP INT NOT NULL
	);

CREATE TABLE Publishers(
	Id INT PRIMARY KEY IDENTITY
	,[Name] VARCHAR(30) UNIQUE NOT NULL
	,AddressId INT NOT NULL
	,Website NVARCHAR(50)
	,Phone NVARCHAR(20)
	,CONSTRAINT FK_Publishers_Addresses FOREIGN KEY (AddressId) REFERENCES Addresses(Id)
);

CREATE TABLE PlayersRanges(
	Id INT PRIMARY KEY IDENTITY
	,PlayersMin INT NOT NULL
	,PlayersMax INT NOT NULL
);

CREATE TABLE Boardgames(
	Id INT PRIMARY KEY IDENTITY
	,[Name] NVARCHAR(30) NOT NULL
	,YearPublished INT NOT NULL
	,Rating DECIMAL(3,2) NOT NULL
	,CategoryId INT NOT NULL
	,PublisherId INT NOT NULL
	,PlayersRangeId INT NOT NULL
	,CONSTRAINT FK_Boardgames_Categories FOREIGN KEY (CategoryId) REFERENCES Categories(Id)
	,CONSTRAINT FK_Boardgames_Publishers FOREIGN KEY (PublisherId) REFERENCES Publishers(Id)
	,CONSTRAINT FK_Boardgames_PlayersRanges FOREIGN KEY (PlayersRangeId) REFERENCES PlayersRanges(Id)
);

CREATE TABLE Creators(
	Id INT PRIMARY KEY IDENTITY
	,FirstName NVARCHAR(30) NOT NULL
	,LastName NVARCHAR(30) NOT NULL
	,Email NVARCHAR(30) NOT NULL
);

CREATE TABLE CreatorsBoardgames(
	CreatorId INT NOT NULL
	,BoardgameId INT NOT NULL
	,CONSTRAINT PK_CreatorsBoardgames PRIMARY KEY (CreatorId,BoardgameID)
	,CONSTRAINT FK_CreatorsBoardgames_Creators  FOREIGN KEY (CreatorId) REFERENCES Creators(Id)
	,CONSTRAINT FK_CreatorsBoardgames_Cigars  FOREIGN KEY (BoardgameId) REFERENCES Boardgames(Id)
);

--2
INSERT INTO Boardgames ([Name], YearPublished, Rating, CategoryId, PublisherId, PlayersRangeId) VALUES
('Deep Blue', 2019, 5.67, 1, 15, 7),
('Paris', 2016, 9.78, 7, 1, 5),
('Catan: Starfarers', 2021, 9.87, 7, 13, 6),
('Bleeding Kansas', 2020, 3.25, 3, 7, 4),
('One Small Step', 2019, 5.75, 5, 9, 2)

INSERT INTO Publishers ([Name], AddressId, Website, Phone) VALUES
('Agman Games', 5, 'www.agmangames.com', '+16546135542'),
('Amethyst Games', 7, 'www.amethystgames.com', '+15558889992'),
('BattleBooks', 13, 'www.battlebooks.com', '+12345678907')

--3
UPDATE PlayersRanges 
SET PlayersMax +=1 
WHERE Id=1 
UPDATE Boardgames 
SET [Name]=[Name] + 'V2'
WHERE YearPublished>=2020

--4
DELETE FROM CreatorsBoardgames WHERE BoardgameId IN (1,16,31,47)
DELETE FROM Boardgames WHERE PublisherId IN (1,16)
DELETE FROM Publishers WHERE AddressId IN (5)
DELETE FROM Addresses WHERE SUBSTRING(Town, 1, 1) = 'L'

--5
SELECT [Name], Rating
FROM Boardgames
ORDER BY YearPublished, [Name] DESC