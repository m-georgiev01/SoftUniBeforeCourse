--- Problem 01: DDL
CREATE TABLE Countries(
	Id INT PRIMARY KEY IDENTITY
	, [Name] VARCHAR(10) NOT NULL
);

CREATE TABLE Addresses(
	Id INT PRIMARY KEY IDENTITY
	, StreetName NVARCHAR(20) NOT NULL
	, StreetNumber INT 
	, PostCode INT NOT NULL
	, City VARCHAR(25) NOT NULL
	, CountryId INT NOT NULL
	, CONSTRAINT FK_Addresses_Countries FOREIGN KEY (CountryId) REFERENCES Countries(Id)
);

CREATE TABLE Vendors(
	Id INT PRIMARY KEY IDENTITY
	, [Name] NVARCHAR(25) NOT NULL
	, NumberVAT NVARCHAR(15) NOT NULL
	, AddressId INT NOT NULL
	, CONSTRAINT FK_Vendors_Addresses FOREIGN KEY (AddressId) REFERENCES Addresses(Id)
);

CREATE TABLE Clients(
	Id INT PRIMARY KEY IDENTITY
	, [Name] NVARCHAR(25) NOT NULL
	, NumberVAT NVARCHAR(15) NOT NULL
	, AddressId INT NOT NULL
	, CONSTRAINT FK_Clients_Addresses FOREIGN KEY (AddressId) REFERENCES Addresses(Id)
);

CREATE TABLE Categories(
	Id INT PRIMARY KEY IDENTITY
	, [Name] VARCHAR(10) NOT NULL
);

CREATE TABLE Products(
	Id INT PRIMARY KEY IDENTITY
	, [Name] NVARCHAR(35) NOT NULL
	, Price DECIMAL(18,2) NOT NULL
	, CategoryId INT NOT NULL
	, VendorId INT NOT NULL
	, CONSTRAINT FK_Products_Categories FOREIGN KEY (CategoryId) REFERENCES Categories(Id)
	, CONSTRAINT FK_Products_Vendors FOREIGN KEY (VendorId) REFERENCES Vendors(Id)
);

CREATE TABLE Invoices(
	Id INT PRIMARY KEY IDENTITY
	, Number INT NOT NULL
	, IssueDate DATETIME2 NOT NULL
	, DueDate DATETIME2 NOT NULL
	, Amount DECIMAL(18,2) NOT NULL
	, Currency VARCHAR(5) NOT NULL
	, ClientId INT NOT NULL
	, CONSTRAINT FK_Invoices_Clients FOREIGN KEY (ClientId) REFERENCES Clients(Id)
);

CREATE TABLE ProductsClients(
	ProductId INT NOT NULL
	, ClientId INT NOT NULL
	, CONSTRAINT PK_ProductsClients PRIMARY KEY (ProductId, ClientId)
	, CONSTRAINT FK_ProductsClients_Products  FOREIGN KEY (ProductId) REFERENCES Products(Id)
	, CONSTRAINT FK_ProductsClients_Clients  FOREIGN KEY (ClientId) REFERENCES Clients(Id)
);

--2
--First Execute Dataset.sql
INSERT INTO Products 
	([Name], Price, CategoryId, VendorId) 
VALUES
	('SCANIA Oil Filter XD01', 78.69, 1, 1),
	('MAN Air Filter XD01', 97.38, 1, 5),
	('DAF Light Bulb 05FG87', 55.00, 2, 13),
	('ADR Shoes 47-47.5', 49.85, 3, 5),
	('Anti-slip pads S', 5.87, 5, 7)

INSERT INTO Invoices 
	(Number, Amount, IssueDate, DueDate, Currency, ClientId) 
VALUES
	(1219992181, 180.96, '2023-03-01', '2023-04-30', 'BGN', 3),
	(1729252340, 158.18, '2022-11-06', '2023-01-04', 'EUR', 13),
	(1950101013, 615.15, '2023-02-17', '2023-04-18', 'USD', 19)



--3
UPDATE Invoices
SET DueDate = '2023-04-01'
WHERE Year(IssueDate) = 2022 AND Month(IssueDate) = 11

UPDATE Clients
SET AddressId = 3
WHERE [Name] LIKE '%CO%'

--4
DELETE FROM ProductsClients WHERE ClientId = 11
DELETE FROM Invoices WHERE ClientId = 11
DELETE FROM Clients WHERE SUBSTRING(NumberVat, 1, 2) = 'IT'

--5
SELECT Number, Currency
FROM Invoices
ORDER BY Amount DESC, DueDate;