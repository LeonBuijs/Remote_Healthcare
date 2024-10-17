using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client;

class Program
{
    // private static BLEHandler ble;
    static void Main(string[] args)
    {
        // StartSimulation(); todo uitcommenten voor simulator
        
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new Form());

        Console.Read();
    }
}