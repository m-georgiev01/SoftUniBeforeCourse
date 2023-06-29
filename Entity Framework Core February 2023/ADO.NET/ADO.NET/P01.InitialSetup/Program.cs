using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Text;

string connStringMasterDb = "Server=MAG\\SQLEXPRESS;Database=master;Integrated Security=true;TrustServerCertificate=True;";
string connStringMinionsDb = "Server=MAG\\SQLEXPRESS;Database=MinionsDB;Integrated Security=true;TrustServerCertificate=True;";

SqlConnection masterConn = new SqlConnection(connStringMasterDb);
await masterConn.OpenAsync();
using (masterConn)
{
    await Problem1_1(masterConn);
}

SqlConnection minionsConn = new SqlConnection(connStringMinionsDb);
await minionsConn.OpenAsync();

using (minionsConn)
{
    //await Problem1_2(minionsConn);
    //await Problem1_3(minionsConn);
    //await Problem2(minionsConn);
    //await Problem3(minionsConn);
    //await Problem4(minionsConn);
    //await Problem5(minionsConn);
    //await Problem6(minionsConn);
    //await Problem7(minionsConn);
    //await Problem8(minionsConn);
    //await Problem9(minionsConn);
}

async Task Problem1_1 (SqlConnection connection)
{
    string query = "CREATE DATABASE MinionsDB";
    SqlCommand sqlCommand = new SqlCommand(query, connection);

    await sqlCommand.ExecuteNonQueryAsync();
}

async Task Problem1_2 (SqlConnection connection)
{
    string query = "CREATE TABLE Countries (Id INT PRIMARY KEY IDENTITY,Name VARCHAR(50)) " +
                   "CREATE TABLE Towns(Id INT PRIMARY KEY IDENTITY,Name VARCHAR(50), CountryCode INT FOREIGN KEY REFERENCES Countries(Id)) " +
                   "CREATE TABLE Minions(Id INT PRIMARY KEY IDENTITY,Name VARCHAR(30), Age INT, TownId INT FOREIGN KEY REFERENCES Towns(Id)) " +
                   "CREATE TABLE EvilnessFactors(Id INT PRIMARY KEY IDENTITY, Name VARCHAR(50)) " +
                   "CREATE TABLE Villains (Id INT PRIMARY KEY IDENTITY, Name VARCHAR(50), EvilnessFactorId INT FOREIGN KEY REFERENCES EvilnessFactors(Id)) " +
                   "CREATE TABLE MinionsVillains (MinionId INT FOREIGN KEY REFERENCES Minions(Id),VillainId INT FOREIGN KEY REFERENCES Villains(Id),CONSTRAINT PK_MinionsVillains PRIMARY KEY (MinionId, VillainId))";
    
    SqlCommand sqlCommand = new SqlCommand(query, connection);

    await sqlCommand.ExecuteNonQueryAsync();
}

async Task Problem1_3 (SqlConnection connection)
{
    string query = "INSERT INTO Countries ([Name]) VALUES ('Bulgaria'),('England'),('Cyprus'),('Germany'),('Norway') " +
                   "INSERT INTO Towns ([Name], CountryCode) VALUES ('Plovdiv', 1),('Varna', 1),('Burgas', 1),('Sofia', 1),('London', 2),('Southampton', 2),('Bath', 2),('Liverpool', 2),('Berlin', 3),('Frankfurt', 3),('Oslo', 5) " +
                   "INSERT INTO Minions (Name,Age, TownId) VALUES('Bob', 42, 3),('Kevin', 1, 1),('Bob ', 32, 6),('Simon', 45, 3),('Cathleen', 11, 2),('Carry ', 50, 10),('Becky', 125, 5),('Mars', 21, 1),('Misho', 5, 10),('Zoe', 125, 5),('Json', 21, 1) " +
                   "INSERT INTO EvilnessFactors (Name) VALUES ('Super good'),('Good'),('Bad'), ('Evil'),('Super evil') " +
                   "INSERT INTO Villains (Name, EvilnessFactorId) VALUES ('Gru',2),('Victor',1),('Jilly',3),('Miro',4),('Rosen',5),('Dimityr',1),('Dobromir',2) " +
                   "INSERT INTO MinionsVillains (MinionId, VillainId) VALUES (4,2),(1,1),(5,7),(3,5),(2,7),(11,5),(8,4),(9,7),(7,1),(1,3),(7,3),(5,3),(4,3),(1,2),(2,1)";

    SqlCommand sqlCommand = new SqlCommand(query, connection);

    await sqlCommand.ExecuteNonQueryAsync();
}

async Task Problem2 (SqlConnection connection)
{
    string query = "SELECT v.Name, COUNT(mv.VillainId) AS MinionsCount " +
                   "FROM Villains AS v " +
                   "JOIN MinionsVillains AS mv ON v.Id = mv.VillainId " +
                   "GROUP BY v.Id, v.Name " +
                   "HAVING COUNT(mv.VillainId) > 3 " +
                   "ORDER BY COUNT(mv.VillainId)";

    SqlCommand command = new SqlCommand(query, connection);

    SqlDataReader reader = await command.ExecuteReaderAsync();

    while (await reader.ReadAsync())
    {
        string name = (string) reader["Name"];
        int count = (int) reader["MinionsCount"];

        Console.WriteLine($"{name} - {count}");
    }
}

async Task Problem3 (SqlConnection connection)
{
    Console.WriteLine("Enter id: ");
    int id = int.Parse(Console.ReadLine());

    string query = "SELECT Name FROM Villains WHERE Id = @Id";

    SqlCommand command = new SqlCommand(query, connection);
    command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

    string? villianName = (string?) await command.ExecuteScalarAsync();

    if (string.IsNullOrEmpty(villianName))
    {
        Console.WriteLine($"No villain with ID {id} exists in the database.");
        return;
    }

    Console.WriteLine($"Villian: {villianName}");

    string secondQuery = "SELECT ROW_NUMBER() OVER (ORDER BY m.Name) AS RowNum," +
                                                                        "m.Name, " +
                                                                        "m.Age " +
                                                   "FROM MinionsVillains AS mv " +
                                                   "JOIN Minions As m ON mv.MinionId = m.Id " +
                                                   "WHERE mv.VillainId = @Id " +
                                                   "ORDER BY m.Name";

    SqlCommand cmd = new SqlCommand(secondQuery, connection);
    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;

    SqlDataReader reader = await cmd.ExecuteReaderAsync();

    if(!reader.HasRows)
    {
        Console.WriteLine("(no minions)");
        return;
    }

    while (await reader.ReadAsync())
    {
        var rowNum = reader["RowNum"];
        var name = reader["Name"];
        var age = reader["Age"];

        Console.WriteLine($"{rowNum}. {name} {age}");
    }
}

async Task Problem4 (SqlConnection connection)
{
    string[] minionTokens = Console.ReadLine().Split(" ", StringSplitOptions.RemoveEmptyEntries);
    string minionName = minionTokens[1];
    int minionAge = int.Parse(minionTokens[2]);
    string townName = minionTokens[3];

    string villianName = Console.ReadLine().Split(" ", StringSplitOptions.RemoveEmptyEntries)[1];

    string query = "SELECT Id FROM Towns WHERE Name = @townName";
    SqlCommand commandCheckTown = new SqlCommand(query, connection);
    commandCheckTown.Parameters.Add("@townName", SqlDbType.VarChar, 50).Value = townName;

    int? townId = (int?) await commandCheckTown.ExecuteScalarAsync();

    if(townId is null) 
    {
        query = "INSERT INTO Towns (Name) VALUES (@townName)";
        SqlCommand commandInsertTown = new SqlCommand(query, connection);
        commandInsertTown.Parameters.Add("@townName", SqlDbType.VarChar, 50).Value = townName;

        await commandInsertTown.ExecuteNonQueryAsync();

        townId = (int?) await commandCheckTown.ExecuteScalarAsync();

        Console.WriteLine($"Town {townName} was added to the database.");
    }

    query = "SELECT Id FROM Villains WHERE Name = @Name";
    SqlCommand commandCheckVillian = new SqlCommand(query, connection);
    commandCheckVillian.Parameters.Add("@Name", SqlDbType.VarChar, 50).Value = villianName;
    int? villainId = (int?) await commandCheckVillian.ExecuteScalarAsync();

    if (villainId is null)
    {
        query = "INSERT INTO Villains (Name, EvilnessFactorId)  VALUES (@villainName, 4)";
        SqlCommand commandInsertVillian = new SqlCommand(query, connection);
        commandInsertVillian.Parameters.Add("@villainName", SqlDbType.VarChar, 50).Value = villianName;
        await commandInsertVillian.ExecuteNonQueryAsync();

        villainId = (int?)await commandCheckVillian.ExecuteScalarAsync();

        Console.WriteLine($"Villain {villianName} was added to the database.");
    }

    query = @"INSERT INTO Minions (Name, Age, TownId) VALUES (@nam, @age, @townId)";
    SqlCommand commandInsertMinion = new SqlCommand(query, connection);
    commandInsertMinion.Parameters.Add("@nam", SqlDbType.VarChar, 30).Value = minionName;
    commandInsertMinion.Parameters.Add("@age", SqlDbType.Int).Value = minionAge;
    commandInsertMinion.Parameters.Add("@townId", SqlDbType.Int).Value = townId;

    await commandInsertMinion.ExecuteNonQueryAsync();

    query = "SELECT Id FROM Minions WHERE Name = @Name AND Age = @Age AND TownId = @TownId";
    SqlCommand commandGetMinionId = new SqlCommand(query, connection);
    commandGetMinionId.Parameters.Add("@Name", SqlDbType.VarChar, 30).Value = minionName;
    commandGetMinionId.Parameters.Add("@Age", SqlDbType.Int).Value = minionAge;
    commandGetMinionId.Parameters.Add("@TownId", SqlDbType.Int).Value = townId;

    int? minionId = (int?)await commandGetMinionId.ExecuteScalarAsync();

    query = "INSERT INTO MinionsVillains (MinionId, VillainId) VALUES (@villainId, @minionId)";
    SqlCommand commandInsertMinionsVillians = new SqlCommand(query, connection);

    commandInsertMinionsVillians.Parameters.Add("@villainId", SqlDbType.Int).Value = villainId;
    commandInsertMinionsVillians.Parameters.Add("@minionId", SqlDbType.Int).Value = minionId;

    await commandInsertMinionsVillians.ExecuteNonQueryAsync();


    Console.WriteLine($"Successfully added {minionName} to be minion of {villianName}.");
}

async Task Problem5(SqlConnection connection)
{
    Console.WriteLine("Enter country's name:");
    string countryName = Console.ReadLine();

    string query = "UPDATE Towns " +
                   "SET Name = UPPER(Name) " +
                   "WHERE CountryCode = (" +
                                         "SELECT c.Id " +
                                         "FROM Countries AS c " +
                                         "WHERE c.Name = @countryName)";

    SqlCommand cmdUpdateTownsNames = new SqlCommand(query, connection);
    cmdUpdateTownsNames.Parameters.Add("@countryName", SqlDbType.VarChar, 50).Value = countryName;
    
    int affectedRows = await cmdUpdateTownsNames.ExecuteNonQueryAsync();

    if (affectedRows == 0)
    {
        Console.WriteLine("No town names were affected.");
        return;
    }

    Console.WriteLine($"{affectedRows} town names were affected.");

    query = "SELECT t.Name " +
                   "FROM Towns as t " +
                   "JOIN Countries AS c ON c.Id = t.CountryCode " +
                   "WHERE c.Name = @countryName";

    SqlCommand cmdGetCountriesTowns = new SqlCommand(query, connection);
    cmdGetCountriesTowns.Parameters.Add("@countryName", SqlDbType.VarChar, 50).Value = countryName;

    List<string> townNames = new();

    using SqlDataReader reader = await cmdGetCountriesTowns.ExecuteReaderAsync();

    while(await reader.ReadAsync())
    {
        string townName = (string) reader["Name"];
        townNames.Add(townName);
    }

    Console.WriteLine($"[{string.Join(", ", townNames)}]");
}

async Task Problem6(SqlConnection connection)
{
    Console.WriteLine("Enter Vilian's Id:");
    int villainId = int.Parse(Console.ReadLine());

    string query = "SELECT Name FROM Villains WHERE Id = @villainId";
    SqlCommand cmdGetVillianName = new SqlCommand(query, connection);
    cmdGetVillianName.Parameters.Add("@villainId", SqlDbType.Int).Value = villainId;

    string? villainName = (string?) await cmdGetVillianName.ExecuteScalarAsync();

    if (string.IsNullOrEmpty(villainName))
    {
        Console.WriteLine("No such villain was found.");
        return;
    }

    query = "DELETE FROM MinionsVillains " +
            "WHERE VillainId = @villainId";
    SqlCommand cmdDeleteFromMinionsVillians = new SqlCommand(query, connection);
    cmdDeleteFromMinionsVillians.Parameters.AddWithValue("@villainId", villainId);

    int deletedMinions = await cmdDeleteFromMinionsVillians.ExecuteNonQueryAsync();

    Console.WriteLine($"{villainName} was deleted.");
    Console.WriteLine($"{deletedMinions} minions were released.");

    query = "DELETE FROM Villains " +
            "WHERE Id = @villainId";
    SqlCommand cmdDeleteFromVillains = new SqlCommand(query, connection);
    cmdDeleteFromVillains.Parameters.AddWithValue("@villainId", villainId);

    await cmdDeleteFromVillains.ExecuteNonQueryAsync();
}

async Task Problem7(SqlConnection connection)
{
    string query = "SELECT Name FROM Minions";
    SqlCommand cmdGetAllMinionsNames = new SqlCommand(query, connection);

    using SqlDataReader reader = await cmdGetAllMinionsNames.ExecuteReaderAsync();

    List<string> minionsNames = new();

    while (await reader.ReadAsync())
    {
        string minionName = (string) reader["Name"];
        minionsNames.Add(minionName);
    }

    int cycles = minionsNames.Count / 2;
    StringBuilder sb = new();
    for (int i = 0; i < cycles; i++)
    {
        sb.AppendLine(minionsNames[i]);
        sb.AppendLine(minionsNames[minionsNames.Count - 1 - i]);
    }

    //add middle element if the count is odd
    if (minionsNames.Count % 2 != 0)
    {
        sb.AppendLine(minionsNames[minionsNames.Count / 2]);
    }

    Console.WriteLine(sb.ToString().TrimEnd());
}

async Task Problem8(SqlConnection connection)
{
    int[] ids = Console.ReadLine().Split(" ").Select(int.Parse).ToArray();

    foreach (var id in ids)
    {
        string updateQuery = "UPDATE Minions " +
                             "SET Name = UPPER(LEFT(Name, 1)) + SUBSTRING(Name, 2, LEN(Name)), Age += 1" +
                             "WHERE Id = @Id";

        SqlCommand cmdUpdate = new SqlCommand(updateQuery, connection);
        cmdUpdate.Parameters.Add("@Id", SqlDbType.Int).Value = id;
        
        await cmdUpdate.ExecuteNonQueryAsync();
    }

    string query = "SELECT Name, Age FROM Minions";
    SqlCommand cmdGetNameAndAgeForAllMinions = new SqlCommand(query, connection);

    using SqlDataReader reader = await cmdGetNameAndAgeForAllMinions.ExecuteReaderAsync();

    List<string> names = new();

    while (await reader.ReadAsync())
    {
        string name = (string) reader["Name"];
        var age = reader.GetInt32(1);

        Console.WriteLine(name + " " + age);
    }
}

async Task Problem9(SqlConnection connection)
{
    Console.WriteLine("Enter minion's id:");
    int minionId = int.Parse(Console.ReadLine());

    string query = "CREATE PROC usp_GetOlder @id INT" +
                   "AS" +
                   "UPDATE Minions " +
                   "SET Age += 1 " +
                   "WHERE Id = @id";
    SqlCommand cmdCreateUSP = new SqlCommand(query, connection);
    await cmdCreateUSP.ExecuteNonQueryAsync();

    query = "EXEC dbo.usp_GetOlder @Id";
    SqlCommand cmdExecUSP = new SqlCommand(query, connection);
    cmdExecUSP.Parameters.Add("@Id", SqlDbType.Int).Value = minionId;
    await cmdExecUSP.ExecuteNonQueryAsync();


    query = "SELECT Name, Age FROM Minions WHERE Id = @Id";
    SqlCommand cmdGetInfoForMinionWithId = new SqlCommand(query, connection);
    using SqlDataReader reader = await cmdGetInfoForMinionWithId.ExecuteReaderAsync();

    var names = new List<string>();

    while (await reader.ReadAsync())
    {
        string name = reader.GetString(0);
        var age = reader.GetInt32(1);

        Console.WriteLine(name + " - " + age + " years old");
    }
}