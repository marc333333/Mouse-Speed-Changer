using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;


namespace Set_Mouse_Speed
{
    class Program
    {
        const UInt32 SPI_SETMOUSESPEED = 0x0071;

        [DllImport("User32.dll")]
        static extern Boolean SystemParametersInfo(UInt32 uiAction, UInt32 uiParam, UInt32 pvParam, UInt32 fWinIni);

        static void Main(string[] args)
        {
            int i = 0;
            if (int.TryParse(args[0], out i))
            {
                if (i > 0 && i <= 20)
                {
                    SetMouseSpeed(i); 
                    Console.WriteLine("Mouse speed set to {0}", i);
                }
                else
                {
                    Console.WriteLine("Error");
                }
            }
            else
            {
                Console.WriteLine("Error");
            }
            Console.ReadKey();
        }

        static void SetMouseSpeed(int _iMouseSpeed)
        {
            if (_iMouseSpeed > 0 && _iMouseSpeed < 21)
            {
                SystemParametersInfo(SPI_SETMOUSESPEED, 0, (uint)_iMouseSpeed, 0);
            }
        }
    }
}
