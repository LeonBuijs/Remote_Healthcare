namespace ClientGUI;

public class Storage
{
    private List<String> _data;

    public Storage()
    {
        _data = new List<string>();
    }

    public void AddData(string data)
    {
        _data.Add(data);
    }

    public void removeData(string data)
    {
        _data.Remove(data);
    }

    public String GetDataWithIndex(int index)
    {
        if (index < 0 || index >= _data.Count)
        {
            Console.WriteLine("Index buiten bereik !");
            return null;
        }
        return _data[index];
    }

    public String GetAllData()
    {
        String data = "";
        for (int i = 0; i < _data.Count; i++)
        {
            data = _data[1];
        }
        return data;
    }
}