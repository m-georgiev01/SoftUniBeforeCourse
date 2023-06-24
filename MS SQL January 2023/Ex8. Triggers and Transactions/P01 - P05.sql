--1
CREATE TABLE Logs(
	LogId INT IDENTITY PRIMARY KEY,
	AccountId INT FOREIGN KEY REFERENCES Accounts(Id),
	OldSum MONEY,
	NewSum MONEY
);

GO;

CREATE TRIGGER tr_AddToLogsOnAccountBalanceUpdate 
ON Accounts
FOR UPDATE
AS
IF (UPDATE(Balance))
BEGIN
	INSERT INTO Logs (AccountId, OldSum, NewSum)
	SELECT i.Id, d.Balance, i.Balance
	FROM inserted AS i
	JOIN deleted AS d
	ON i.Id = d.Id;
END;
GO;

--2
CREATE TABLE NotificationEmails(
	Id INT IDENTITY PRIMARY KEY,
	Recipient INT NOT NULL,
	Subject VARCHAR(MAX) NOT NULL,
	Body VARCHAR(MAX) NOT NULL,
);

GO;

CREATE TRIGGER tr_CreateEmailOnNewLog 
ON Logs
FOR INSERT
AS
BEGIN
	INSERT INTO NotificationEmails(Recipient, Subject, Body)
	SELECT AccountId,
		   CONCAT('Balance change for account: ', AccountId),
		   CONCAT('On ', GETDATE(), ' your balance was changed from ', OldSum, ' to ', NewSum)
	FROM inserted
END;
GO;

--3
CREATE PROC usp_DepositMoney (@AccountId INT, @MoneyAmount MONEY)
AS
BEGIN
	BEGIN TRANSACTION
		IF(@MoneyAmount < 0 OR @MoneyAmount IS NULL)
		BEGIN
			ROLLBACK;
			THROW 50001, 'Invalid amount of money', 1;
		END;

		IF (NOT EXISTS
		(
			SELECT Id
			FROM Accounts
			WHERE Id = @AccountId
		) OR @AccountId IS NULL)
		BEGIN
			ROLLBACK;
			THROW 50002, 'Invalid account', 1;
		END

		UPDATE Accounts
		SET Balance += @MoneyAmount
		WHERE Id = @AccountId
	COMMIT
END;
GO;

--4
CREATE PROC usp_WithdrawMoney (@AccountId INT, @MoneyAmount MONEY) 
AS
BEGIN
	BEGIN TRANSACTION
		IF (@MoneyAmount < 0 OR @MoneyAmount IS NULL)
		BEGIN
			ROLLBACK;
			THROW 50001, 'Invalid amount of money', 1;
		END;

		IF (NOT EXISTS
		(
			SELECT Id
			FROM Accounts
			WHERE Id = @AccountId
		) OR @AccountId IS NULL)
		BEGIN
			ROLLBACK;
			THROW 50002, 'Invalid account', 1;
		END;

		UPDATE Accounts
		SET Balance -= @MoneyAmount
		WHERE Id = @AccountId;
	COMMIT;
END;
GO;

--5
CREATE PROC usp_TransferMoney(@SenderId INT, @ReceiverId INT, @Amount MONEY)
AS
BEGIN
	BEGIN TRANSACTION
		IF (@Amount < 0 OR @Amount IS NULL)
		BEGIN
			ROLLBACK;
			THROW 50001, 'Invalid amount of money', 1;
		END;

		IF (NOT EXISTS
		(
			SELECT Id
			FROM Accounts
			WHERE Id = @SenderId
		) OR @SenderId IS NULL)
		BEGIN
			ROLLBACK;
			THROW 50002, 'Invalid sender', 1;
		END;

		IF (NOT EXISTS
		(
			SELECT Id
			FROM Accounts
			WHERE Id = @ReceiverId
		) OR @ReceiverId IS NULL)
		BEGIN
			ROLLBACK;
			THROW 50002, 'Invalid receiver', 1;
		END;

		EXEC dbo.usp_WithdrawMoney @SenderId, @Amount;
		EXEC dbo.usp_DepositMoney @ReceiverId, @Amount;
	COMMIT;
END;