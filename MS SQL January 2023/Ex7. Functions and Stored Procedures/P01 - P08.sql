--1
CREATE PROC usp_GetEmployeesSalaryAbove35000
AS
BEGIN
	SELECT FirstName, LastName
	FROM Employees
	WHERE Salary > 35000
END;
GO;

--2
CREATE PROC usp_GetEmployeesSalaryAboveNumber (@MinSalary DECIMAL(18, 4))
AS
BEGIN
	SELECT FirstName, LastName
	FROM Employees
	WHERE Salary >= @MinSalary
END;
GO;

--3
CREATE PROC usp_GetTownsStartingWith (@StartString NVARCHAR(255))
AS
BEGIN
	SELECT Name
	FROM Towns
	WHERE Name LIKE CONCAT(@StartString, '%')
END;
GO;

--4
CREATE PROC usp_GetEmployeesFromTown (@TownName NVARCHAR(255))
AS
BEGIN
	SELECT e.FirstName, e.LastName
	FROM Employees AS e
	JOIN Addresses AS a
	ON e.AddressID = a.AddressID
	JOIN Towns AS t
	ON a.TownID = t.TownID
	WHERE t.Name = @TownName
END;
GO;

--5
CREATE FUNCTION ufn_GetSalaryLevel (@Salary DECIMAL(18,4)) 
RETURNS VARCHAR(10) AS
BEGIN

	IF (@Salary < 30000)
		RETURN 'Low'
	ELSE IF (@Salary BETWEEN 30000 AND 50000)
		RETURN 'Average'

	RETURN 'High'
END;

GO;

--6
CREATE PROC usp_EmployeesBySalaryLevel (@SalaryLevel VARCHAR(10))
AS
BEGIN
	SELECT FirstName, LastName
	FROM Employees
	WHERE dbo.ufn_GetSalaryLevel(Salary) = @SalaryLevel
END;

GO;

--7
CREATE FUNCTION ufn_IsWordComprised(@SetOfLetters NVARCHAR(255), @Word NVARCHAR(255))
RETURNS BIT AS
BEGIN
	DECLARE @i INT = 0;
	DECLARE @MatchingCharacters INT = 0;

	WHILE(@i <= LEN(@Word))
	BEGIN
		DECLARE @Letter VARCHAR(1) = SUBSTRING(@Word, @i, 1);
		DECLARE @IsIncluded BIT = CHARINDEX(@Letter, @SetOfLetters);

		IF(@IsIncluded <> 0)
		BEGIN
			 SET @MatchingCharacters = @MatchingCharacters + 1;
		END

		SET @i = @i + 1;
	END

	IF(@MatchingCharacters = LEN(@Word))
	BEGIN
		RETURN 1;
	END

	RETURN 0;
END

GO;

--8
CREATE PROCEDURE usp_DeleteEmployeesFromDepartment(@DepartmentId INT)
AS
BEGIN
	ALTER TABLE Departments
	ALTER COLUMN ManagerId INT NULL

	DELETE
	FROM EmployeesProjects
	WHERE EmployeeID IN (
		SELECT EmployeeID
		FROM Employees
		WHERE DepartmentID = @DepartmentId)

	UPDATE Employees
	SET ManagerID = NULL
	WHERE ManagerID IN (
		SELECT EmployeeID
		FROM Employees
		WHERE DepartmentID = @DepartmentId)

	UPDATE Departments
	SET ManagerID = NULL
	WHERE DepartmentID = @DepartmentId

	DELETE
	FROM Employees
	WHERE DepartmentID = @DepartmentId;

	DELETE
	FROM Departments
	WHERE DepartmentID = @DepartmentId

	SELECT COUNT(*)
	FROM Employees
	WHERE DepartmentID = @DepartmentId
END