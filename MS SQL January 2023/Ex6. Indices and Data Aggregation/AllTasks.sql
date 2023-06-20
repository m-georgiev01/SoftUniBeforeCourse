--1
SELECT COUNT(*) AS Count
FROM WizzardDeposits;

--2
SELECT MAX(MagicWandSize) AS LongestMagicWand
FROM WizzardDeposits;

--3
SELECT DepositGroup, MAX(MagicWandSize) AS LongestMagicWand
FROM WizzardDeposits
GROUP BY DepositGroup;

--4
SELECT TOP(2) DepositGroup
FROM WizzardDeposits
GROUP BY DepositGroup
ORDER BY AVG(MagicWandSize);

--5
SELECT DepositGroup, SUM(DepositAmount) AS TotalSum
FROM WizzardDeposits
GROUP BY DepositGroup;

--6
SELECT DepositGroup, SUM(DepositAmount) AS TotalSum
FROM WizzardDeposits
GROUP BY DepositGroup, MagicWandCreator
HAVING MagicWandCreator = 'Ollivander family';

--7
SELECT DepositGroup, SUM(DepositAmount) AS TotalSum
FROM WizzardDeposits
GROUP BY DepositGroup, MagicWandCreator
HAVING MagicWandCreator = 'Ollivander family'
AND SUM(DepositAmount) < 150000
ORDER BY SUM(DepositAmount) DESC;

--8
SELECT DepositGroup, MagicWandCreator, MIN(DepositCharge) AS MinDepositCharge
FROM WizzardDeposits
GROUP BY DepositGroup, MagicWandCreator
ORDER BY MagicWandCreator, DepositGroup;

--9
SELECT r.AgeGroups, COUNT(*) AS WizardCount
 FROM(
		SELECT
			  CASE
				   WHEN Age BETWEEN 0 AND 10 THEN '[0-10]'
				   WHEN Age BETWEEN 11 AND 20 THEN '[11-20]'
				   WHEN Age BETWEEN 21 AND 30 THEN '[21-30]'
				   WHEN Age BETWEEN 31 AND 40 THEN '[31-40]'
				   WHEN Age BETWEEN 41 AND 50 THEN '[41-50]'
				   WHEN Age BETWEEN 51 AND 60 THEN '[51-60]'
				   ELSE '[61+]'
			   END AS AgeGroups
		  FROM WizzardDeposits) 
AS r
GROUP BY AgeGroups;

--10
SELECT LEFT(FirstName, 1) AS FirstLetter
FROM WizzardDeposits
WHERE DepositGroup = 'Troll Chest'
GROUP BY LEFT(FirstName, 1)
ORDER BY LEFT(FirstName, 1);

--11
SELECT DepositGroup, IsDepositExpired, AVG(DepositInterest) AS AverageInterest
FROM WizzardDeposits
WHERE DepositStartDate > '01/01/1985'
GROUP BY DepositGroup, IsDepositExpired
ORDER BY DepositGroup DESC, IsDepositExpired;

--12
SELECT SUM(Result.SumDifference) 
FROM(
		SELECT Host.DepositAmount - LEAD(Host.DepositAmount) OVER(ORDER BY Id) AS SumDifference
		FROM WizzardDeposits AS Host
) AS Result;

--13
SELECT DepartmentID,
	   SUM(Salary) AS TotalSalary
FROM Employees
GROUP BY DepartmentID;

--14
SELECT DepartmentID,
	   MIN(Salary) AS TotalSalary
FROM Employees
WHERE DepartmentID IN (2, 5, 7)
AND HireDate > '01/01/2000'
GROUP BY DepartmentID;

--15
SELECT * 
INTO NewTable
FROM Employees
WHERE Salary > 30000;

DELETE
FROM NewTable
WHERE ManagerID = 42;

UPDATE NewTable
SET Salary = Salary + 5000
WHERE DepartmentID = 1;

SELECT DepartmentID,
	   AVG(Salary) AS AverageSalary 
FROM NewTable
GROUP BY DepartmentID;

--16
SELECT DepartmentID,
	   MAX(Salary) AS MaxSalary
FROM Employees
GROUP BY DepartmentID
HAVING MAX(Salary) < 30000
OR MAX(Salary) > 70000;

--17
SELECT COUNT(*) AS Count
FROM Employees
WHERE ManagerID IS NULL;

--18
SELECT DepartmentId, 
	   Salary AS ThirdHighestSalary
FROM(
		SELECT DepartmentID,
			   Salary,
		       DENSE_RANK() OVER (PARTITION BY DepartmentId ORDER BY Salary DESC) AS Rank 
		FROM Employees
		GROUP BY DepartmentID, Salary) AS r
WHERE Rank = 3;

--19
SELECT TOP(10) e.FirstName, 
	           e.LastName,
			   e.DepartmentID
FROM(
		SELECT DepartmentID,
			   AVG(Salary) AS AvgSalary
		FROM Employees
		GROUP BY DepartmentID) AS r
JOIN Employees AS e 
ON e.DepartmentID = r.DepartmentID
WHERE e.Salary > r.AvgSalary
ORDER BY DepartmentID;