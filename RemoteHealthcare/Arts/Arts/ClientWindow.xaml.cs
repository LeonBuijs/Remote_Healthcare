using System.Windows;

namespace Arts;

public partial class ClientWindow : Window, IDataUpdateCallback
{
    private string clientId;
    private NetworkProcessor networkProcessor;

    public ClientWindow(string clientId, NetworkProcessor networkProcessor)
    {
        InitializeComponent();
        this.networkProcessor = networkProcessor;
        this.networkProcessor.AddCallbackMember(this);
        this.clientId = clientId;
    }

    /**
     * <summary>
     * Methode die controleert of deze window de client is waarvan een update is gestuurd.
     * Bij true zal het GUI worden aangepast met de nieuwe waardes.
     * </summary>
     * <param name="clientId">The id of the client which the data originates from</param>>
     * <param name="data">The data itself, containing the speed, distance etc.</param>
     */
    public void UpdateData(string clientId, string data)
    {
        throw new NotImplementedException();

        if (this.clientId.Equals(clientId))
        {
            //todo handle data
            
        }
    }
}