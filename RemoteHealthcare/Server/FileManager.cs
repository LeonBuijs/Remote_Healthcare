namespace Server;

/**
 * Klasse voor het uitvoeren en bijhouden van alle FileIO gerelateerde zaken
 * alle files komen in de documents folder te staan
 */

//TODO: bepalen hoe de data opgeslagen wordt: hashen encrypten etc.
public class FileManager
{
    //TODO: bestand voor beheren geregistreerde clients
    //server stuurt data hiernaar en vergelijkt hashes om te kijken of het geldig is

    //TODO: bestand voor beheren geregistreerde artsen
    //server stuurt data hiernaar en vergelijkt hashes om te kijken of het geldig is

    //TODO: bestand per geregistreerde client om sessie waardes op te slaan
    //per client een mapje maken met alle historische data

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
    //todo
    public List<string>? getAllClientSessions(string client)
    {
        List<string> allSessionData = new List<string>();

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
            allSessionData.Add(getDataFromSession(session));
        }

        return allSessionData;
    }

    /**
     * Methode voor het omrekenen van data uit het bestand en zet het in een string waarde
     */
    private string getDataFromSession(string session)
    {
        var fileContents = File.ReadAllLines(session);

        var split = session.Split("/");
        var sessionDate = split[split.Length - 1].Replace(".txt", "");
        //TODO file uitlezen en data berekenen
        return $"{sessionDate} ";
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
}