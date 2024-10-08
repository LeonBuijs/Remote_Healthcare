using System.Windows;

namespace Arts;

public partial class ClientWindow : Window, IDataUpdateCallback
{
    private string clientId;

    public ClientWindow(string clientId)
    {
        Program.AddCallbackMember(this);
        this.clientId = clientId;
        InitializeComponent();
    }

    public void UpdateData(string clientId, string data)
    {
        throw new NotImplementedException();

        if (this.clientId.Equals(clientId))
        {
            //todo handle data
        }
    }
}