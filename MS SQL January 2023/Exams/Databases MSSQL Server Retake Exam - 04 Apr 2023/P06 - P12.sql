--6
SELECT p.Id, p.[Name],
	   p.Price, c.[Name]
FROM Products AS p
JOIN Categories AS c
ON p.CategoryId = c.Id
WHERE c.[Name] = 'ADR' OR
	  c.[Name] = 'Others'
ORDER BY Price DESC;

--7
SELECT c.Id,
	   c.[Name] AS Client,
	   CONCAT_WS(', ', CONCAT(StreetName, ' ',  a.StreetNumber), a.City, a.PostCode, co.[Name]) AS [Address]
FROM Clients AS c
LEFT JOIN ProductsClients AS pc
ON c.Id = pc.ClientId
JOIN Addresses AS a
ON c.AddressId = a.Id
JOIN Countries AS co
ON a.CountryId = co.Id
WHERE pc.ClientId IS NULL
ORDER BY c.[Name];

--8
SELECT TOP(7) i.Number,
			  i.Amount,
			  c.[Name]
FROM Invoices AS i
JOIN Clients AS c
ON i.ClientId = c.Id
WHERE i.IssueDate <= '2023-01-01'
AND i.Currency = 'EUR' 
OR i.Amount >= 500
AND c.NumberVAT LIKE 'DE%'
ORDER BY i.Number, i.Amount DESC;  

--9
SELECT c.[Name],
	   MAX(p.Price) AS Price,
	   c.NumberVAT
FROM Clients AS c
JOIN ProductsClients AS pc
ON c.Id = pc.ClientId
JOIN Products AS p
ON pc.ProductId = p.Id
WHERE c.[Name] NOT LIKE '%KG'
GROUP BY c.[Name], c.NumberVAT
ORDER BY MAX(p.Price) DESC

--10
SELECT t.[Name],
	   FLOOR(AVG(t.Price))
FROM (
	SELECT c.[Name], p.Price
	FROM Clients AS c
	JOIN ProductsClients AS pc
	ON c.Id = pc.ClientId
	JOIN Products AS p
	ON pc.ProductId = p.Id
	JOIN Vendors AS v
	ON p.VendorId = v.Id
	WHERE v.NumberVAT LIKE '%FR%'
) AS t
GROUP BY t.[Name]
ORDER BY AVG(t.Price)
GO
--11
CREATE FUNCTION udf_ProductWithClients(@name VARCHAR(255))
RETURNS INT 
AS
BEGIN
    DECLARE @count int;
    
	SELECT @count = COUNT(*)
	FROM Products AS p
	JOIN ProductsClients AS pc
	ON p.Id = pc.ProductId
	WHERE p.[Name] = @name
	GROUP BY p.[Name];

	RETURN @count;
END;
GO

SELECT dbo.udf_ProductWithClients('DAF FILTER HU12103X')
GO


--12
CREATE PROCEDURE usp_SearchByCountry 
	@country VARCHAR(255)   
AS
	SELECT v.[Name] AS Vendor,
		   v.NumberVAT AS VAT,
		   CONCAT_WS(' ', a.StreetName, a.StreetNumber) AS [Street Info],
		   CONCAT_WS(' ', a.City,  a.PostCode) AS [City Info]
	FROM Vendors AS v
	JOIN Addresses AS a
	ON v.AddressId = a.Id
	JOIN Countries AS c
	ON a.CountryId = c.Id
	WHERE c.[Name] = @country
	ORDER BY v.[Name], a.City
GO 

EXEC usp_SearchByCountry 'France'