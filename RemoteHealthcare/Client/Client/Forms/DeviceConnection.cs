using System;
using System.Windows.Forms;
using ClientGUI;

namespace Client;

public partial class DeviceConnection : Form
{
    private MessageHandler messageHandler;
    private BLEHandler bleHandler;
    public DeviceConnection()
    {
        InitializeComponent();
    }

    private void connectButton_Click(object sender, EventArgs e)
    {
        var bikeNumber = CheckBikeNumberBox();

        if (bikeNumber == "")
        {
            return;
        }
        
        
    }

    private string CheckBikeNumberBox()
    {
        var bikeNumber = bikeNumberTextBox.Text;
        
        if (string.IsNullOrWhiteSpace(bikeNumber))
        {
            return "";
        }

        return bikeNumber;
    }
}