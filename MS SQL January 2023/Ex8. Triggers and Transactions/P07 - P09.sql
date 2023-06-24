--7
DECLARE @UserName VARCHAR(50) = 'Stamat'
DECLARE @GameName VARCHAR(50) = 'Safflower'
DECLARE @UserId INT = (SELECT Id FROM Users WHERE Username = @UserName)
DECLARE @GameId INT = (SELECT Id FROM Games WHERE Name = @GameName)
DECLARE @UserGameId int = (SELECT Id FROM UsersGames WHERE UserId = @UserID AND GameId = @GameID)
DECLARE @StamatsBudget MONEY = (SELECT Cash FROM UsersGames WHERE GameId = @GameId AND UserId = @UserId)

BEGIN TRANSACTION
	DECLARE @Price11to12 DECIMAL(18,4) = 
		(SELECT SUM(Price)
		 FROM Items
		 WHERE MinLevel IN (11, 12))

	IF(@StamatsBudget >= @Price11to12)
	BEGIN
		INSERT INTO UserGameItems
		SELECT Id, @UserGameId
		FROM Items
		WHERE Id IN (SELECT Id FROM Items WHERE MinLevel IN (11, 12))

		UPDATE UsersGames
		SET Cash -= @Price11to12
		WHERE GameId = @GameId AND
		UserId = @UserGameId

		SET @StamatsBudget -= @Price11to12
		COMMIT
	END
	ELSE
		ROLLBACK;

BEGIN TRANSACTION
	DECLARE @Price19to21 DECIMAL(18,4) = 
		(SELECT SUM(Price)
		 FROM Items
		 WHERE MinLevel BETWEEN 19 AND 21)

	IF(@StamatsBudget >= @Price19to21)
	BEGIN
		INSERT INTO UserGameItems
		SELECT Id, @UserGameId
		FROM Items
		WHERE Id IN (SELECT Id FROM Items WHERE MinLevel BETWEEN 19 AND 21)

		UPDATE UsersGames
		SET Cash -= @Price19to21
		WHERE GameId = @GameId AND
		UserId = @UserGameId
		COMMIT
	END
	ELSE
		ROLLBACK;

SELECT [Name] AS [Item Name]
FROM Items
WHERE Id IN (
	SELECT ItemId 
	FROM UserGameItems
	WHERE UserGameId = @UserGameID)
ORDER BY [Item Name]
GO

--8
CREATE PROC usp_AssignProject(@emloyeeId INT, @projectID INT)
AS 
BEGIN TRANSACTION
	DECLARE @EmployeeProjectsCount INT = (SELECT COUNT(*) FROM EmployeesProjects WHERE EmployeeID = @emloyeeId);

	IF(@EmployeeProjectsCount >= 3)
	BEGIN
		ROLLBACK
		RAISERROR('The employee has too many projects!', 16, 1) 		
		RETURN	
	END;

	INSERT INTO EmployeesProjects 
	VALUES (@emloyeeId, @projectID);
COMMIT;
GO;

--9
CREATE TABLE Deleted_Employees(
	FirstName VARCHAR(50),
	LastName VARCHAR(50),
	MiddleName VARCHAR(50),
	JobTitle VARCHAR(50),
	DepartmentId INT,
	Salary Money
);
GO;

CREATE TRIGGER tr_OnDeletedEmployees
ON Employees FOR DELETE
AS
BEGIN
	INSERT INTO Deleted_Employees
	(
	 FirstName,
	 LastName,
	 MiddleName,
	 JobTitle,
	 DepartmentId,
	 Salary
	)
	 SELECT 
		FirstName,
		LastName,
		MiddleName,
		JobTitle,
		DepartmentId,
		Salary
	 FROM deleted
END;