--1
SELECT TOP(5) e.EmployeeID, e.JobTitle,
		      e.AddressID, a.AddressText
FROM Employees AS e
JOIN Addresses AS a ON e.AddressID = a.AddressID
ORDER BY e.AddressID;

--2
SELECT TOP(50) e.FirstName, e.LastName,
		       t.[Name], a.AddressText
FROM Employees AS e
JOIN Addresses AS a ON e.AddressID = a.AddressID
JOIN Towns AS t ON a.TownID = t.TownID
ORDER BY e.FirstName,
		 e.LastName;

--3
SELECT e.EmployeeID, e.FirstName,
	   e.LastName, d.[Name] AS DepartmentName
FROM Employees AS e
JOIN Departments AS d ON e.DepartmentID = d.DepartmentID
WHERE d.[Name] = 'Sales'
ORDER BY e.EmployeeID;

--4
SELECT TOP(5) e.EmployeeID, e.FirstName,
			  e.Salary, d.[Name] AS DepartmentName
FROM Employees AS e
JOIN Departments AS d ON e.DepartmentID = d.DepartmentID
WHERE e.Salary > 15000
ORDER BY e.DepartmentID;

--5
SELECT DISTINCT TOP(3) e.EmployeeID, e.FirstName
FROM Employees AS e
LEFT JOIN EmployeesProjects AS ep ON e.EmployeeID = ep.EmployeeID
WHERE ep.ProjectID IS NULL
ORDER BY e.EmployeeID;

--6
SELECT e.FirstName, e.LastName,
	   e.HireDate, d.[Name] AS DeptName
FROM Employees AS e
JOIN Departments AS d ON e.DepartmentID = d.DepartmentID
WHERE e.HireDate > '1999-01-01' AND
	  d.[Name] IN ('Sales', 'Finance')
ORDER BY e.HireDate;

--7
SELECT TOP(5) e.EmployeeID, e.FirstName, p.[Name]
FROM Employees AS e
JOIN EmployeesProjects AS ep ON ep.EmployeeID = e.EmployeeID
JOIN Projects AS p ON ep.ProjectID = p.ProjectID
WHERE p.StartDate > '2002-08-13' AND
	  p.EndDate IS NULL
ORDER BY e.EmployeeID;

--8
SELECT e.EmployeeID, e.FirstName,
CASE
    WHEN DATEPART(YEAR, p.StartDate) >= 2005 THEN NULL
    ELSE p.[Name]
END AS ProjectName
FROM Employees AS e
JOIN EmployeesProjects AS ep ON ep.EmployeeID = e.EmployeeID
JOIN Projects AS p ON ep.ProjectID = p.ProjectID
WHERE e.EmployeeID = 24;

--9
SELECT e.EmployeeID, e.FirstName,
	   m.EmployeeID AS ManagerID,
	   m.FirstName AS ManagerName
FROM Employees AS e
JOIN Employees AS m ON e.ManagerID = m.EmployeeID
WHERE e.ManagerID IN (3, 7)
ORDER BY e.EmployeeID;

--10
SELECT TOP(50) e.EmployeeID,
			   e.FirstName + ' ' + e.LastName AS EmployeeName,
			   m.FirstName + ' ' + m.LastName AS ManagerName,
			   d.[Name] AS DepartmentName
FROM Employees AS e
JOIN Employees AS m ON e.ManagerID = m.EmployeeID
JOIN Departments AS d ON e.DepartmentID = d.DepartmentID
ORDER BY e.EmployeeID;

--11
SELECT MIN(t.AvgSalary)
FROM
	(
	SELECT AVG(e.Salary) AS AvgSalary
	FROM Employees AS e
	JOIN Departments AS d ON e.DepartmentID= d.DepartmentID
	GROUP BY e.DepartmentID
	) AS t;

--12
SELECT c.CountryCode, m.MountainRange,
	   p.PeakName, p.Elevation
FROM Countries AS c
JOIN MountainsCountries AS mc ON c.CountryCode = mc.CountryCode
JOIN Mountains AS m ON mc.MountainId = m.Id
JOIN Peaks AS p ON m.Id = p.MountainId
WHERE c.CountryCode = 'BG' AND
	  p.Elevation > 2835
ORDER BY p.Elevation DESC;

--13
SELECT c.CountryCode, COUNT(m.MountainRange) AS MountainRanges
FROM Countries AS c
JOIN MountainsCountries AS mc ON c.CountryCode = mc.CountryCode
JOIN Mountains AS m ON mc.MountainId = m.Id
GROUP BY c.CountryCode
HAVING c.CountryCode IN ('BG', 'US', 'RU');

--14
SELECT TOP(5) c.CountryName, r.RiverName
FROM Countries AS c
LEFT JOIN CountriesRivers AS ca ON c.CountryCode = ca.CountryCode
LEFT JOIN Rivers AS r ON ca.RiverId = r.Id
WHERE c.ContinentCode = 'AF'
ORDER BY c.CountryName;

--15
SELECT ContinentCode, CurrencyCode, CurrencyUsage 
FROM(
	SELECT ContinentCode, CurrencyCode,
	COUNT(CurrencyCode) AS CurrencyUsage,
	DENSE_RANK() OVER(PARTITION BY ContinentCode ORDER BY COUNT(CurrencyCode) DESC) AS Ranking  
	FROM Countries
	GROUP BY ContinentCode, CurrencyCode) AS k
WHERE Ranking = 1 AND CurrencyUsage > 1;

--16
SELECT COUNT(*) AS [Count]
FROM Countries AS c
LEFT JOIN MountainsCountries AS mc on c.CountryCode = mc.CountryCode
WHERE mc.MountainId IS NULL;

--17
SELECT TOP(5) c.CountryName,
		      MAX(p.Elevation) AS HighestPeakElevation,
		      MAX(r.Length) AS LongestRiverLength
FROM Countries AS c
FULL JOIN MountainsCountries AS mc ON c.CountryCode = mc.CountryCode
FULL JOIN Mountains AS m on mc.MountainId = m.Id
FULL JOIN Peaks AS p on m.Id = p.MountainId
FULL JOIN CountriesRivers AS cr ON c.CountryCode = cr.CountryCode
FULL JOIN Rivers AS r ON cr.RiverId = r.Id
GROUP BY c.CountryName
ORDER BY HighestPeakElevation DESC,
		 LongestRiverLength DESC,
		 c.CountryName;

--18
SELECT TOP(5)	[Country],
				ISNULL([Highest Peak Name], '(no highest peak)'),
				ISNULL([Highest Peak Elevation], 0),
				ISNULL(k.MountainRange, '(no mountain)')
FROM
(
	SELECT	c.CountryName AS [Country],
			p.PeakName AS [Highest Peak Name],
			MAX(p.Elevation) AS [Highest Peak Elevation],
			m.MountainRange,
			DENSE_RANK() OVER (PARTITION BY c.CountryName ORDER BY MAX(p.Elevation) DESC) AS Ranking
			FROM
			Countries AS c
				LEFT JOIN MountainsCountries AS mc ON c.CountryCode = mc.CountryCode
				LEFT JOIN Mountains AS m ON m.Id = mc.MountainId
				LEFT JOIN Peaks AS p ON m.Id = p.MountainId	
				GROUP BY c.CountryName, p.PeakName, m.MountainRange) AS k
WHERE Ranking = 1
ORDER BY [Country], [Highest Peak Name];