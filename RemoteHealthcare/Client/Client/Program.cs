using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client;

class Program
{
    // private static BLEHandler ble;
    static void Main(string[] args)
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new ServerLogin());

        Console.Read();
    }
}