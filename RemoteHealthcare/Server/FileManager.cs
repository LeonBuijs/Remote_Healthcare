namespace Server;

/**
 * Klasse voor het uitvoeren en bijhouden van alle FileIO gerelateerde zaken
 * alle files komen in de documents folder te staan
 */

//TODO: bepalen hoe de data opgeslagen wordt: hashen encrypten etc.
public class FileManager
{
    public string rootDirectory { get; set; }
    public string clientDirectory { get; set; }
    public string doctorDirectory { get; set; }
    public string sessionDirectory { get; set; }

    public FileManager()
    {
        SetDirectories();
    }

    /**
     * Methode die controleert of de login van de client geldig is en bestaat in de server
     * layout nameBirthAndYear = {firstName} {lastName} {birthDate}
     * e.g. Jan Jannsen 01012000
     */
    public bool CheckClientLogin(string nameAndBirthDate)
    {
        var allClients = ReadAllLines(clientDirectory + "/clientData.txt");

        foreach (var client in allClients)
        {
            if (client == nameAndBirthDate)
            {
                return true;
            }
        }

        return false;
    }

    /**
     * Methode die controleert of de username en login van de doctor geldig is en bestaat in de server
     * Username en wachtwoord worden gescheiden door een spatie
     */
    public bool CheckDoctorLogin(string username, string password)
    {
        var doctorLogin = username + " " + password;

        var allDoctors = ReadAllLines(doctorDirectory + "/doctorData.txt");

        foreach (var doctor in allDoctors)
        {
            if (doctor == doctorLogin)
            {
                return true;
            }
        }

        return false;
    }

    /**
     * Methode om een nieuwe client aan de server toe te voegen
     */
    public void AddNewClient(string index)
    {
        //todo login hashen en/of encrypten
        var path = sessionDirectory + "/" + index;


        if (!Directory.Exists(path))
        {
            WriteToFile(clientDirectory + "/clientData.txt", $"{index}");
            Directory.CreateDirectory(path);
        }
    }

    /**
     * Methode om sessies van client op te halen
     */
    public List<string>? GetAllClientSessions(string client)
    {
        var allSessionData = new List<string>();

        string[] allSessions;

        try
        {
            allSessions = Directory.GetFiles(sessionDirectory + "/" + client);
        }
        catch (Exception e)
        {
            return null;
        }

        foreach (var session in allSessions)
        {
            allSessionData.Add(GetDataFromSession(session));
        }

        return allSessionData;
    }

    /**
     * Methode voor het lezen van de inhoud van het bestand
     */
    private static string GetDataFromSession(string session)
    {
        var fileContents = File.ReadAllLines(session);

        if (fileContents.Length > 1)
        {
            return fileContents[0];
        }

        return "";
    }

    /**
     * Methode om een nieuwe doctor aan de server toe te voegen
     */
    public void AddNewDoctor(string username, string password)
    {
        //todo login hashen en/of encrypten
        WriteToFile(doctorDirectory + "/doctorData.txt", $"{username} {password}");
    }

    /**
     * Methode die controleert of alle directories voor de files aangemaakt zijn, zo niet worden ze aangemaakt
     */
    private void SetDirectories()
    {
        rootDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/serverdata";

        Console.WriteLine("Root directory: " + rootDirectory);

        clientDirectory = rootDirectory + "/clients";
        doctorDirectory = rootDirectory + "/doctors";
        sessionDirectory = rootDirectory + "/sessions";

        if (!Directory.Exists(rootDirectory))
        {
            Directory.CreateDirectory(rootDirectory);
        }

        if (!Directory.Exists(clientDirectory))
        {
            Directory.CreateDirectory(clientDirectory);
        }

        if (!Directory.Exists(doctorDirectory))
        {
            Directory.CreateDirectory(doctorDirectory);
        }

        if (!Directory.Exists(sessionDirectory))
        {
            Directory.CreateDirectory(sessionDirectory);
        }
    }

    /**
     * Methodes voor het schrijven of uitlezen van data naar de betreffende bestanden en directories
     */
    public void WriteToFile(string filePath, string content)
    {
        var writer = File.AppendText(filePath);
        writer.WriteLine(content);
        writer.Close();
    }

    public string[] ReadAllLines(string filePath)
    {
        if (File.Exists(filePath))
        {
            return File.ReadAllLines(filePath);
        }

        return ["File does not exist"];
    }

    // TODO: mogelijk versimpelen van vergelijken van tijden
    /**
     * Async methode om gewenste waardes van een sessie te berekenen
     * Na het berekenen van de gewenste waarden worden alle andere waarden vervangen in het bestand door de gewenste waarden
     *
     * voorbeeld: 2024-24-10 13-49-31 0:10:21 7 70 92 153
     */
    public async Task CalculateDataFromSession(ClientConnection connection, string clientName, string sessionTime)
    {
        Console.WriteLine("Calculating data from session: " + clientName);
        
        await Task.Run(() =>
        {
            Console.WriteLine("Calculating data from session in task: " + clientName);

            var date = connection.SessionTime;

            var duration = GetDuration(date);

            // Lijsten waar alle data uit bestand in komt te staan
            var allSpeeds = new List<int>();
            var allHeartRates = new List<int>();

            var filePath = $"{sessionDirectory}/{clientName}/{sessionTime}";

            Console.WriteLine($"Calculating {filePath}");

            var fileContents = ReadAllLines(filePath);

            //Checks om te kijken of er geen foutieve waarden zijn
            if (fileContents.Length == 0)
            {
                return;
            }

            if (fileContents[0].Equals("File does not exist"))
            {
                return;
            }

            // Verschillende data ophalen en uit string opsplitsen
            foreach (var data in fileContents)
            {
                var split = data.Split(" ");

                allSpeeds.Add(int.Parse(split[0]));
                allHeartRates.Add(int.Parse(split[5]));
            }

            var averageSpeed = (int) allSpeeds.Average();
            var maxSpeed = allSpeeds.Max();

            var averageHeartRate = (int) allHeartRates.Average();
            var maxHeartRate = allHeartRates.Max();

            var calculatedData = $"{date} {duration} {averageSpeed} {maxSpeed} {averageHeartRate} {maxHeartRate}";

            Console.WriteLine($"Calculated data: {calculatedData}");

            File.WriteAllText(filePath, calculatedData);
        });
    }

    /**
     * Helper methode om de duratie van de sessie te verkrijgen
     */
    private static string GetDuration(string date)
    {
        // Verschil in tijd berekenen met DateTime objecten
        var start = date.Split(" ");

        var startDate = start[0].Split("-");
        var startTime = start[1].Split("-");

        var startYear = startDate[0];
        var startDay = startDate[1];
        var startMonth = startDate[2];

        var startHour = startTime[0];
        var startMinute = startTime[1];
        var startSecond = startTime[2];

        Console.WriteLine($"new date: {startYear} {startMonth} {startDay} {startHour} {startMinute} {startSecond}");

        DateTime startDateTime = new DateTime(Convert.ToInt32(startYear), Convert.ToInt32(startMonth),
            Convert.ToInt32(startDay), Convert.ToInt32(startHour), Convert.ToInt32(startMinute),
            Convert.ToInt32(startSecond));

        TimeSpan difference = DateTime.Now - startDateTime;
        var duration = $"{difference.Hours}:{difference.Minutes}:{difference.Seconds}";
        return duration;
    }
}