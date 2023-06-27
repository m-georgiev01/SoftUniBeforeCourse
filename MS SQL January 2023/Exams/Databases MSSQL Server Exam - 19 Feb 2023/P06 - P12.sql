--6
SELECT b.Id,
	   b.[Name],
	   b.YearPublished,
	   c.[Name]
FROM Boardgames AS b
JOIN Categories AS c
ON b.CategoryId = c.Id
WHERE c.[Name] = 'Strategy games'
OR c.[Name] = 'Wargames'
ORDER BY b.YearPublished DESC

--7
SELECT c.Id,
	   CONCAT_WS(' ', c.FirstName, c.LastName) AS CreatorName,
	   c.Email
FROM Creators AS c
LEFT JOIN CreatorsBoardgames AS cb
ON c.Id = cb.CreatorId
WHERE cb.CreatorId IS NULL
ORDER BY CreatorName

--8
SELECT TOP(5) b.[Name],
			  b.Rating,
			  c.[Name]
FROM Boardgames AS b
JOIN Categories AS c
ON b.CategoryId = c.Id
JOIN PlayersRanges AS pr
ON b.PlayersRangeId = pr.Id
WHERE b.Rating > 7
AND b.[Name] LIKE '%a%'
OR b.Rating > 7.5
AND pr.PlayersMin >= 2
AND pr.PlayersMax <= 5
ORDER BY b.[Name], b.Rating DESC

--9
SELECT CONCAT_WS(' ', c.FirstName, c.LastName) AS FullName,
	   c.Email,
	   MAX(b.Rating) AS Rating
FROM Creators AS c
JOIN CreatorsBoardgames AS cb
ON c.Id = cb.CreatorId
JOIN Boardgames AS b
ON cb.BoardgameId = b.Id
WHERE c.Email LIKE '%.com'
GROUP BY c.FirstName, c.LastName, c.Email
ORDER BY FullName

--10
SELECT c.LastName,
	   CEILING(AVG(b.Rating)) AS AverageRating,
	   p.[Name]
FROM Creators AS c
JOIN CreatorsBoardgames AS cb
ON c.Id = cb.CreatorId
JOIN Boardgames AS b
ON cb.BoardgameId = b.Id
JOIN Publishers AS p
ON b.PublisherId = p.Id
WHERE p.[Name] = 'Stonemaier Games'
GROUP BY c.LastName, p.[Name]
ORDER BY AVG(b.Rating) DESC

GO
--11
CREATE FUNCTION udf_CreatorWithBoardgames(@name VARCHAR(255))
RETURNS INT 
AS
BEGIN
    DECLARE @count int;
    
	SELECT @count = COUNT(*)
	FROM Creators AS c
	JOIN CreatorsBoardgames AS cb
	ON c.Id = cb.CreatorId
	WHERE c.FirstName = @name
	GROUP BY c.FirstName;

	RETURN @count;
END;
GO

SELECT dbo.udf_CreatorWithBoardgames('Bruno')
GO

--12
CREATE PROCEDURE usp_SearchByCategory
	@category VARCHAR(255)
AS
	SELECT b.[Name],
		   b.YearPublished,
		   b.Rating,
		   c.[Name],
		   p.[Name],
		   CONCAT(ps.PlayersMin, ' people') AS MinPlayers,
		   CONCAT(ps.PlayersMax, ' people') AS MaxPlayers
	FROM Boardgames AS b
	JOIN Categories AS c
	ON b.CategoryId = c.Id
	JOIN Publishers AS p
	ON b.PublisherId = p.Id
	JOIN PlayersRanges AS ps
	ON b.PlayersRangeId = ps.Id
	WHERE c.[Name] = @category
	ORDER BY p.[Name], b.YearPublished DESC