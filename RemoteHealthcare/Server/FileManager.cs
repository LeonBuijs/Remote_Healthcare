namespace Server;
/**
 * Klasse voor het uitvoeren en bijhouden van alle FileIO gerelateerde zaken
 * alle files komen in de documents folder te staan
 */
public class FileManager
{
    //TODO: bestand voor beheren geregistreerde clients
    //server stuurt data hiernaar en vergelijkt hashes om te kijken of het geldig is
    
    //TODO: bestand voor beheren geregistreerde artsen
    //server stuurt data hiernaar en vergelijkt hashes om te kijken of het geldig is

    //TODO: bestand per geregistreerde client om sessiewaardes op te slaan
    //per client een mapje maken met alle historische data

    private string rootDirectory;
    public FileManager()
    {
        SetDirectories();
    }

    public bool CheckClientLogin(string NameAndBirthYear)
    {
        return true;//todo
    }

    public bool CheckDoctorLogin(string username, string password)
    {
        return true;//todo
    }

    /**
     * Methode die controleert of alle directories voor de files aangemaakt zijn, zo niet worden ze aangemaakt
     */
    private void SetDirectories()
    {
        rootDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/serverdata";
        
        if (Directory.Exists(rootDirectory))
        {
            Directory.CreateDirectory(rootDirectory);
        }

        if (Directory.Exists(rootDirectory + "/clients"))
        {
            Directory.CreateDirectory(rootDirectory + "/clients");
        }
        
        if (Directory.Exists(rootDirectory + "/doctors"))
        {
            Directory.CreateDirectory(rootDirectory + "/doctors");
        }
        
        if (Directory.Exists(rootDirectory + "/sessions"))
        {
            Directory.CreateDirectory(rootDirectory + "/sessions");
        }
    }
}