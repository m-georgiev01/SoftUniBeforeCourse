--1
CREATE DATABASE	Minions;

USE Minions;

--2
CREATE TABLE Minions (
	Id INT,
	[Name] VARCHAR(250),
	Age INT,
	CONSTRAINT PK_Minions PRIMARY KEY (Id)
);

CREATE TABLE Towns (
	Id INT,
	[Name] VARCHAR (250)
	CONSTRAINT PK_Towns PRIMARY KEY (Id)
);

--3
ALTER TABLE Minions
ADD TownId INT;

ALTER TABLE Minions
ADD CONSTRAINT FK_Minions_Towns FOREIGN KEY (TownId) REFERENCES Towns(Id);


--4
INSERT INTO Towns 
	(Id, [Name])
VALUES
	(1,'Sofia'),
	(2,'Plovdiv'),
	(3,'Varna');

INSERT INTO Minions
	(Id, [Name], Age, TownId)
VALUES
	(1, 'Kevin', 22, 1),
	(2, 'Bob', 15, 3),
	(3, 'Steward', NULL, 2);

--5
TRUNCATE TABLE Minions;

--6
DROP TABLE Minions;
DROP TABLE Towns;

--7
--Using SQL query, create table People with the following columns:
--Id - unique number. For every person there will be no more than 231-1 people (auto incremented).
--Name - full name of the person. There will be no more than 200 Unicode characters (not null).
--Picture - image with size up to 2 MB (allow nulls).
--Height - in meters. Real number precise up to 2 digits after floating point (allow nulls).
--Weight - in kilograms. Real number precise up to 2 digits after floating point (allow nulls).
--Gender - possible states are m or f (not null).
--Birthdate - (not null).
--Biography - detailed biography of the person. It can contain max allowed Unicode characters (allow nulls).
--Make the Id a primary key. Populate the table with only 5 records. Submit your CREATE and INSERT statements as Run queries & check DB.

CREATE TABLE People(
	Id INT IDENTITY(1,1),
	[Name] NVARCHAR(200) NOT NULL,
	Picture VARBINARY(MAX),
	Height DECIMAL(3,2),
	[Weight] DECIMAL(5,2),
	Gender CHAR(1) NOT NULL,
	Birthdate DATE NOT NULL,
	Biography NVARCHAR(MAX),
	CONSTRAINT PK_People PRIMARY KEY (Id)
);

INSERT INTO People 
	([Name], Picture, Height, [Weight], Gender, Birthdate, Biography) 
VALUES
	('P1', NULL, 1.75, 83.2, 'm', '1999-07-07', 'B1'),
	('P2', NULL, 2.75, 33.2, 'f', '1999-07-09', 'B2'),
	('P3', NULL, 1.55, 673.2, 'm', '1999-07-05', 'B3'),
	('P4', NULL, 1.75, 43.2, 'm', '1999-07-03', 'B4'),
	('P5', NULL, 1.75, 13.2, 'f', '1999-07-02', 'B5');

--8
--Using SQL query create table Users with columns:
--Id - unique number for every user. There will be no more than 263-1 users (auto incremented).
--Username - unique identifier of the user. It will be no more than 30 characters (non Unicode)  (required).
--Password - password will be no longer than 26 characters (non Unicode) (required).
--ProfilePicture - image with size up to 900 KB. 
--LastLoginTime
--IsDeleted - shows if the user deleted his/her profile. Possible states are true or false.
--Make the Id a primary key. Populate the table with exactly 5 records. Submit your CREATE and INSERT statements as Run queries & check DB.

CREATE TABLE Users (
	Id BIGINT IDENTITY(1,1),
	Username VARCHAR(30) NOT NULL,
	[Password] VARCHAR(26) NOT NULL,
	ProfilePicture VARBINARY(MAX),
	LastLoginTime DATETIME2,
	IsDeleted BIT,
	CONSTRAINT PK_Users PRIMARY KEY (Id)
);

INSERT INTO	Users
	(Username, [Password], ProfilePicture, LastLoginTime, IsDeleted)
VALUES
	('U1', 'Pass1', NULL, GETDATE(), 0),
	('U2', 'Pass2', NULL, GETDATE(), 0),
	('U3', 'Pass3', NULL, GETDATE(), 0),
	('U4', 'Pass4', NULL, GETDATE(), 0),
	('U5', 'Pass5', NULL, GETDATE(), 0);


--9
ALTER TABLE Users
DROP CONSTRAINT PK_Users;  

ALTER TABLE Users
ADD CONSTRAINT PK_Users PRIMARY KEY(Id);

--10
ALTER TABLE Users
ADD CONSTRAINT CHK_Users CHECK (LEN(Password) > 4);

--11
ALTER TABLE Users
ADD CONSTRAINT DF_Users_LastLoginTime
DEFAULT GETDATE() FOR LastLoginTime; 

--12
ALTER TABLE Users
DROP CONSTRAINT PK_Users

ALTER TABLE Users
ADD CONSTRAINT PK_Users PRIMARY KEY (Id)

ALTER TABLE Users
ADD CONSTRAINT CH_Users_Username CHECK (LEN(Username) > 2)