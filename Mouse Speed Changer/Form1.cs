using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Mouse_Speed_Changer
{
    public partial class Form1 : Form
    {
        const UInt32 SPI_SETMOUSESPEED = 0x0071;

        Rectangle recScreen;

        [DllImport("User32.dll")]
        static extern Boolean SystemParametersInfo(UInt32 uiAction, UInt32 uiParam, UInt32 pvParam, UInt32 fWinIni);

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            recScreen = Screen.FromControl(this).Bounds;
            this.Height = recScreen.Height;
            this.Width = recScreen.Width;
            recScreen.Width -= 2;

            BeginningMouseSpeed = SystemInformation.MouseSpeed;
            SetMouseSpeed(1);

            Running = false;
            Time = 0;
            CurrentMouseSpeed = 0;
            Data = new int[20];
        }

        private void Form1_Click(object sender, EventArgs e)
        {
            if (CurrentMouseSpeed >= Data.Length && !Running)
            {
                int[] arr = new int[Data.Length - 1];
                for (int i = 1; i < Data.Length; i++)
                {
                    arr[i - 1] = (int)Math.Round(((double)(Data[i] - Data[i - 1])) * (((double)(arr.Length)) / ((double)i)));
                }
                int sum = 0;
                foreach (int i in arr)
                {
                    sum += i;
                }
                int avg = sum / (arr.Length);
                if (File.Exists("config"))
                {
                    SetMouseSpeed(BeginningMouseSpeed);
                    StreamReader sr = new StreamReader("config");
                    double sen = 0d;
                    double.TryParse(sr.ReadLine(), out sen);
                    int speed = 0;
                    int.TryParse(sr.ReadLine(), out speed);
                    sr.Close();

                    int newSpeed = (int)Math.Round((sen * (double)speed) / avg);
                    SetMouseSpeed(newSpeed);
                    BeginningMouseSpeed = newSpeed;
                    MessageBox.Show(string.Format("Mouse speed set to {0}", newSpeed.ToString()));
                }
                else
                {
                    SetMouseSpeed(BeginningMouseSpeed);
                    StreamWriter sw = new StreamWriter("config");
                    sw.WriteLine(avg.ToString());
                    sw.WriteLine(BeginningMouseSpeed.ToString());
                    sw.Close();
                    MessageBox.Show("New config saved");
                }
                this.Close();
            }
            else
            {
                if (Running)
                {
                    timer1.Enabled = false;
                    Running = false;
                    Data[CurrentMouseSpeed - 1] = Cursor.Position.X + (recScreen.Width * Time);

                    Graphics g = this.CreateGraphics();
                    g.DrawImage(Mouse_Speed_Changer.Properties.Resources.target, new Point(Cursor.Position.X, CurrentMouseSpeed * 30));
                    g.Dispose();
                }
                else
                {
                    Cursor.Position = new Point(0, Cursor.Position.Y);
                    CurrentMouseSpeed++;
                    SetMouseSpeed(CurrentMouseSpeed);
                    timer1.Enabled = true;
                    Time = 0;
                    Running = true;
                }
            }
        }

        void SetMouseSpeed(int _iMouseSpeed)
        {
            if (_iMouseSpeed > 0 && _iMouseSpeed < 21)
            {
                SystemParametersInfo(SPI_SETMOUSESPEED, 0, (uint)_iMouseSpeed, 0);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (Cursor.Position.X > recScreen.Width)
            {
                Cursor.Position = new Point(0, Cursor.Position.Y);
                Time++;
            }
        }

        bool Running;
        int Time;
        int CurrentMouseSpeed;
        int[] Data;

        int BeginningMouseSpeed;

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SetMouseSpeed(BeginningMouseSpeed);
        }
    }
}
