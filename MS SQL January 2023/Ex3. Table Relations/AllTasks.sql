--1
CREATE TABLE Passports(
    PassportID INT PRIMARY KEY IDENTITY(101,1),
    PassportNumber CHAR(8)
);

CREATE TABLE Persons(
    PersonID INT IDENTITY PRIMARY KEY,
    FirstName VARCHAR(50) NOT NULL,
    Salary DECIMAL(9,2) NOT NULL,
    PassportID INT UNIQUE FOREIGN KEY REFERENCES Passports(PassportID)
);

INSERT INTO Passports 
VALUES
	('N34FG21B'),
	('K65LO4R7'),
	('ZE657QP2');

INSERT INTO Persons 
	(FirstName, Salary, PassportID) 
VALUES
	('Roberto', 43300.00, 102), 
	('Tom', 56100.00, 103), 
	('Yana', 60200.00, 101);

--2
CREATE TABLE Manufacturers(
    ManufacturerID INT IDENTITY PRIMARY KEY,
    [Name] VARCHAR(50) NOT NULL,
    EstablishedOn DATE NOT NULL 
);

CREATE TABLE Models(
    ModelID INT IDENTITY(101,1) PRIMARY KEY,
    [Name] VARCHAR(50) NOT NULL,
    ManufacturerID INT FOREIGN KEY REFERENCES Manufacturers(ManufacturerID)
);

INSERT INTO Manufacturers 
	([Name], EstablishedOn) 
VALUES
	('BMW', '07/03/1916'),
	('Tesla', '01/01/2003'),
	('Lada', '01/05/1966');

INSERT INTO Models 
	([Name], ManufacturerID) 
VALUES
	('X1', 1),  
	('i6', 1 ), 
	('Model S', 2),
	('Model X', 2),
	('Model 3', 2),
	('Nova 3',3);

--3
CREATE TABLE Students(
    StudentID INT IDENTITY PRIMARY KEY,
    [Name] VARCHAR(50) NOT NULL
);

CREATE TABLE Exams(
    ExamID INT IDENTITY(101,1) PRIMARY KEY,
    [Name] VARCHAR(50) NOT NULL
);

CREATE TABLE StudentsExams(
    StudentID INT FOREIGN KEY REFERENCES Students(StudentID),
    ExamID INT FOREIGN KEY REFERENCES Exams(ExamID)
	CONSTRAINT PK_Students_Exams PRIMARY KEY (StudentID,ExamID)
);

INSERT INTO Students 
VALUES
	('Mila'),
	('Toni'),
	('Ron');

INSERT INTO Exams
VALUES
	('SpringMVC'),
	('Neo4j'),
	('Oracle 11g');

INSERT INTO StudentsExams
	(StudentID,ExamID)
VALUES
	(1,101),
	(1,102),
	(2,101),
	(3,103),
	(2,102),
	(2,103);

--4
	CREATE TABLE Teachers(
		TeacherID INT IDENTITY(101,1) PRIMARY KEY,
		[Name] VARCHAR(50) NOT NULL,
		ManagerID INT FOREIGN KEY REFERENCES Teachers(TeacherID)
	);

	INSERT INTO Teachers
		([Name],ManagerID) 
	VALUES
		('John', NULL),
		('Maya', 106),
		('Silvia', 106),
		('Ted', 105),
		('Mark', 101),
		('Greta', 101);

--5
CREATE TABLE ItemTypes(
	ItemTypeID INT IDENTITY PRIMARY KEY,
	[Name] NVARCHAR(150) NOT NULL
);

CREATE TABLE Items(
	ItemID INT IDENTITY PRIMARY KEY,
	[Name] NVARCHAR(150) NOT NULL,
	ItemTypeID INT FOREIGN KEY REFERENCES ItemTypes(ItemTypeID)
);

CREATE TABLE Cities(
	CityID INT IDENTITY PRIMARY KEY,
	[Name] NVARCHAR(150) NOT NULL
);

CREATE TABLE Customers(
	CustomerID INT IDENTITY PRIMARY KEY,
	[Name] NVARCHAR(150) NOT NULL,
	Birthday DATE NOT NULL,
	CityID INT FOREIGN KEY REFERENCES Cities(CityID)
);

CREATE TABLE Orders(
	OrderID INT IDENTITY PRIMARY KEY,
	CustomerID INT FOREIGN KEY REFERENCES Customers(CustomerID)
);

CREATE TABLE OrderItems(
	OrderID INT FOREIGN KEY REFERENCES Orders(OrderID),
	ItemID INT FOREIGN KEY REFERENCES Items(ItemID),
	CONSTRAINT PK_OrderItems PRIMARY KEY (OrderID, ItemID)
);

--6
CREATE TABLE Majors(
    MajorID INT IDENTITY PRIMARY KEY,
    [Name] NVARCHAR(50) NOT NULL
);

CREATE TABLE Students(
    StudentID INT IDENTITY PRIMARY KEY,
    StudentNumber CHAR(10) NOT NULL,
    StudentName NVARCHAR(50) NOT NULL,
    MajorID INT FOREIGN KEY REFERENCES Majors(MajorID)
);

CREATE TABLE Payments(
    PaymentID INT IDENTITY PRIMARY KEY,
    PaymentDate DATE NOT NULL,
    PaymentAmount DECIMAL(13,2),
    StudentID INT FOREIGN KEY REFERENCES Students(StudentID)
);

CREATE TABLE Subjects(
    SubjectID INT IDENTITY PRIMARY KEY,
    SubjectName NVARCHAR(50) NOT NULL
);

CREATE TABLE Agenda(
    StudentID INT FOREIGN KEY REFERENCES Students(StudentID),
    SubjectID INT FOREIGN KEY REFERENCES Subjects(SubjectID)
    CONSTRAINT PK_Agenda PRIMARY KEY (StudentID, SubjectID)
);

--9
SELECT m.MountainRange, p.PeakName, p.Elevation
FROM Peaks AS p
JOIN Mountains AS m
ON p.MountainId = m.Id
WHERE m.MountainRange = 'Rila'
ORDER BY p.Elevation DESC;