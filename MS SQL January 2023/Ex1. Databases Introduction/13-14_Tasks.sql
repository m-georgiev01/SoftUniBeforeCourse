--13
--Using SQL queries create Movies database with the following entities:
--Directors (Id, DirectorName, Notes)
--Genres (Id, GenreName, Notes)
--Categories (Id, CategoryName, Notes)
--Movies (Id, Title, DirectorId, CopyrightYear, Length, GenreId, CategoryId, Rating, Notes)
--Set the most appropriate data types for each column. Set a primary key to each table. Populate each table with exactly 5 records. 
--Make sure the columns that are present in 2 tables would be of the same data type. Consider which fields are always required
-- and which are optional. Submit your CREATE TABLE and INSERT statements as Run queries & check DB.

CREATE TABLE Directors
(
	Id INT PRIMARY KEY IDENTITY,
	DirectorName VARCHAR(205) NOT NULL,
	Notes VARCHAR(205)
)

INSERT INTO Directors 
	(DirectorName, Notes) 
VALUES
	('Dimitar', 'Pod Prikritie'),
	('Vankov', 'Sudbi'),
	('Ivanov', 'Nad Prikritie'),
	('Dimitrov', 'Do Prikritie'),
	('Ivanov', 'Zad Prikritie')


CREATE TABLE Genres
(
	Id INT PRIMARY KEY IDENTITY,
	GenreName VARCHAR(205) NOT NULL,
	Notes VARCHAR(205)
)

INSERT INTO Genres 
VALUES
	('Action','Action movies...'),
	('Comedy','Comedy movies...'),
	('Horror','Horror movies...'),
	('Thriller','Thriller movies...'),
	('Romantic','Romantic movies...')

CREATE TABLE Categories
(
	Id INT PRIMARY KEY IDENTITY,
	CategoryName VARCHAR(205) NOT NULL,
	Notes VARCHAR(205)
)

INSERT INTO Categories 
VALUES
	('Funny',NULL),
	('Scarry',NULL),
	('Interesting',NULL),
	('Boring',NULL),
	('Kids',NULL)

CREATE TABLE Movies
(
	Id INT PRIMARY KEY IDENTITY,
	Title VARCHAR(205) UNIQUE NOT NULL,
	DirectorId INT FOREIGN KEY REFERENCES Directors(Id),
	CopyRightYear DATE,
	Length TIME NOT NULL,
	GenreId INT FOREIGN KEY REFERENCES Genres(Id),
	CategoryId INT FOREIGN KEY REFERENCES Categories(Id),
	Rating DECIMAL NOT NULL,
	Notes VARCHAR(205)
)

INSERT INTO Movies 
VALUES
	('Pod Prikritie',1,'2011','00:45:00',1,1,10,NULL),
	('Nad Prikritie',2,'2012','00:45:00',2,2,9.5,NULL),
	('Zad Prikritie',3,'2015','00:50:00',4,4,8,NULL),
	('Do Prikritie',4,'2014','00:44:00',3,3,4,NULL),
	('Vuv Prikritie',5,'2016','00:48:00',5,5,5,NULL)

--14
--Using SQL queries create CarRental database with the following entities:
--Categories (Id, CategoryName, DailyRate, WeeklyRate, MonthlyRate, WeekendRate)
--Cars (Id, PlateNumber, Manufacturer, Model, CarYear, CategoryId, Doors, Picture, Condition, Available)
--Employees (Id, FirstName, LastName, Title, Notes)
--Customers (Id, DriverLicenceNumber, FullName, Address, City, ZIPCode, Notes)
--RentalOrders (Id, EmployeeId, CustomerId, CarId, TankLevel, KilometrageStart, KilometrageEnd, TotalKilometrage, StartDate, EndDate, TotalDays, RateApplied, TaxRate, OrderStatus, Notes)
--Set the most appropriate data types for each column. Set a primary key to each table.
-- Populate each table with only 3 records. Make sure the columns that are present in 2 tables would be of the same data type. 
--Consider which fields are always required and which are optional.

CREATE TABLE Categories
(
	Id INT PRIMARY KEY IDENTITY,
	CategoryName VARCHAR(205) UNIQUE NOT NULL,
	DailyRate DECIMAL(4,1) NOT NULL,
	WeeklyRate DECIMAL(4,1) NOT NULL,
	MonthlyRate DECIMAL(4,1) NOT NULL,
	WeekendRate DECIMAL(4,2) NOT NULL
);

INSERT INTO Categories 
VALUES
	('Category1', 1.0, 1.1, 1.1, 1.1),
	('Category2', 1.0, 1.2, 1.2, 1.2),
	('Category3', 1.0, 1.3, 1.3, 1.3);

CREATE TABLE Cars
(
	Id INT PRIMARY KEY IDENTITY,
	PlateNumber VARCHAR(205) NOT NULL,
	Manufacturer VARCHAR(205) NOT NULL,
	Model VARCHAR(205) NOT NULL,
	CarYear DATE,
	CategoryId INT FOREIGN KEY REFERENCES Categories(Id),
	Doors INT NOT NULL,
	Picture VARBINARY(MAX),
	Condition VARCHAR(205) NOT NULL,
	Available BIT NOT NULL
);

INSERT INTO Cars 
VALUES
	('12345','Mercedes','C180','01-01-1997',1,4,NULL,'Excellent',1),
	('1236','Mercedes','C220','01-01-1998',2,4,NULL,'Excellent',1),
	('12344','Mercedes','AMG','01-01-1999',3,4,NULL,'Good',0);

CREATE TABLE Employees
(
	Id INT PRIMARY KEY IDENTITY,
	FirstName VARCHAR(205) NOT NULL,
	LastName VARCHAR(205) NOT NULL,
	Title VARCHAR(205),
	Notes VARCHAR(MAX)
);

INSERT INTO Employees 
VALUES
	('I','M','CEO',NULL),
	('B','B','Invest',NULL),
	('B','G',NULL,NULL);

CREATE TABLE Customers
(
	Id INT PRIMARY KEY IDENTITY,
	DriverLicenceNumber VARCHAR(205) UNIQUE NOT NULL,
	FullName VARCHAR(205) NOT NULL,
	Address VARCHAR(205) NOT NULL,
	City VARCHAR(205) NOT NULL,
	ZIPCode VARCHAR(8),
	Notes VARCHAR(MAX)
);

INSERT INTO Customers 
VALUES
	('1234567890','I C M', 'Pld', 'Plovdiv','4004', NULL),
	('1234567891','B M B', 'Sofia', 'Sofia','1000', NULL),
	('1234567892','I C C', 'Varna', 'Varna','2500', NULL);

CREATE TABLE RentalOrders
(
	Id INT PRIMARY KEY IDENTITY,
	EmployeeId INT FOREIGN KEY REFERENCES Employees(Id),
	CustomerId INT FOREIGN KEY REFERENCES Customers(Id),
	CarId INT FOREIGN KEY REFERENCES Cars(Id),
	TankLevel DECIMAL(4,2) NOT NULL,
	KilometrageStart BIGINT,
	KilometrageEnd BIGINT,
	TotalKilometrage BIGINT,
	StartDate DATE NOT NULL,
	EndDate DATE NOT NULL,
	TotalDays INT,
	RateApplied DECIMAL(4,1),
	TaxRate DECIMAL(4,1),
	OrderStatus BIT,
	Notes VARCHAR(MAX)
);

INSERT INTO RentalOrders 
VALUES
	(1,1,1,50,0,320,18000,'05-10-2020','05-11-2020',6, NULL, NULL, NULL, NULL),
	(2,2,2,50,0,420,18000,'05-10-2020','05-10-2020',5, NULL, NULL, NULL, NULL),
	(3,3,3,50,0,520,18000,'05-10-2020','05-12-2020',7, NULL, NULL, NULL, NULL);