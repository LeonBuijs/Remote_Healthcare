using System.Windows.Forms;
using Client.Forms;

namespace Client;

class Program
{
    static void Main(string[] args)
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new DeviceConnection());
    }
}