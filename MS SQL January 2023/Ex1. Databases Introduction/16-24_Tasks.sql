--16
--Now create bigger database called SoftUni. It should hold information about
--Towns (Id, Name)
--Addresses (Id, AddressText, TownId)
--Departments (Id, Name)
--Employees (Id, FirstName, MiddleName, LastName, JobTitle, DepartmentId, HireDate, Salary, AddressId)
--The Id columns are auto incremented, starting from 1 and increased by 1 (1, 2, 3, 4). Make sure you use appropriate data types
--for each column. Add primary and foreign keys as constraints for each table. Use only SQL queries. Consider which fields
--are always required and which are optional.

CREATE DATABASE SoftUni
COLLATE cyrillic_general_ci_as;

USE SoftUni;

CREATE TABLE Towns (
	Id INT IDENTITY PRIMARY KEY,
	[Name] VARCHAR(205) NOT NULL, 
);

CREATE TABLE Addresses (
	Id INT IDENTITY PRIMARY KEY,
	AddressText VARCHAR(205) NOT NULL,
    TownId INT NOT NULL FOREIGN KEY REFERENCES Towns(Id)
);

CREATE TABLE Departments
(
    Id INT IDENTITY PRIMARY KEY,
    [Name] VARCHAR(205) NOT NULL 
);

CREATE TABLE Employees
(
    Id INT IDENTITY PRIMARY KEY,
    FirstName VARCHAR(205) NOT NULL, 
    MiddleName VARCHAR(205), 
	LastName VARCHAR(205) NOT NULL,
	JobTitle VARCHAR(205) NOT NULL,
	DepartmentId INT NOT NULL FOREIGN KEY REFERENCES Departments(Id),
	HireDate DATE NOT NULL,
	Salary DECIMAL(15,2) NOT NULL,
    AddressId INT FOREIGN KEY REFERENCES Addresses(Id)
);

--17
DROP DATABASE SoftUni;

--18
INSERT INTO Towns 
VALUES
	('Sofia'),
	('Plovdiv'),
	('Varna'),
	('Burgas');

INSERT INTO Departments 
VALUES
	('Engineering'),
	('Sales'),
	('Marketing'),
	('Software Development'),
	('Quality Assurance');


INSERT INTO Employees 
	(FirstName, MiddleName, LastName, JobTitle, DepartmentId, HireDate, Salary, AddressId)
VALUES
	('Ivan','Ivanov','Ivanov','.NET Developer',4,'02/01/2013',3500.00,NULL),
	('Petar','Petrov','Petrov','Senior Engineer',1,'03/02/2004',4000.00,NULL),
	('Maria','Petrova','Ivanova','Intern',5,'08/28/2016',525.25,NULL),
	('Georgi','Terziev','Ivanov','CEO',2,'12/09/2007',3000.00,NULL),
	('Peter','Pan','Pan','Intern',3,'08/28/2016',599.88,NULL);

--19
--Use the SoftUni database and first select all records from the Towns, 
--then from Departments and finally from Employees table. Use SQL queries 
--and submit them to Judge at once. Submit your query statements as Prepare DB & Run queries.

SELECT *
FROM Towns;

SELECT *
FROM Departments;

SELECT *
FROM Employees;

--20
--Modify the queries from the previous problem by sorting:
--Towns - alphabetically by name
--Departments - alphabetically by name
--Employees - descending by salary

SELECT *
FROM Towns
ORDER BY NAME;

SELECT *
FROM Departments
ORDER BY NAME;

SELECT *
FROM Employees
ORDER BY Salary DESC;

--21
--Modify the queries from the previous problem to show only some of the columns. For table:
--Towns -> Name
--Departments -> Name
--Employees -> FirstName, LastName, JobTitle, Salary

SELECT [Name]
FROM Towns
ORDER BY NAME;

SELECT [Name]
FROM Departments
ORDER BY NAME;

SELECT FirstName, LastName,
	   JobTitle, Salary
FROM Employees
ORDER BY Salary DESC;

--22
--Use SoftUni database and increase the salary of all employees by 10%. 
--Then show only Salary column for all the records in the Employees table. 

UPDATE Employees
SET Salary*=1.1;

SELECT Salary
FROM Employees;

--23
--Use Hotel database and decrease tax rate by 3% to all payments.
--Then select only TaxRate column from the Payments table. 

UPDATE Payments
SET TaxRate*=0.97;

SELECT TaxRate
FROM Payments;

--24
--Use Hotel database and delete all records from the Occupancies table.
DELETE FROM Occupancies;