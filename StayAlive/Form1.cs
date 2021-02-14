using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Configuration;

namespace StayAlive
{
    public partial class Form1 : Form
    {
        private string webApp;
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;
        private const int MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        private const int MOUSEEVENTF_MIDDLEUP = 0x0040;

        [FlagsAttribute]
        public enum EXECUTION_STATE : uint
        {
            ES_AWAYMODE_REQUIRED = 0x00000040,
            ES_CONTINUOUS = 0x80000000,
            ES_DISPLAY_REQUIRED = 0x00000002,
            ES_SYSTEM_REQUIRED = 0x00000001
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);

        private Stopwatch sw = new Stopwatch();
        public Form1()
        {
            InitializeComponent();
           
            SetThreadExecutionState(EXECUTION_STATE.ES_DISPLAY_REQUIRED | EXECUTION_STATE.ES_SYSTEM_REQUIRED | EXECUTION_STATE.ES_CONTINUOUS);

            webApp = ConfigurationManager.AppSettings["browser "];
            timer1.Tick += new EventHandler(timer1_Tick); // Everytime timer ticks, timer_Tick will be called
            timer1.Interval = (1000) * (1);              // Timer will tick every second
            timer1.Enabled = true;                       // Enable the timer
            timer1.Start();                              // Start the timer
            sw.Start();                                  // Start the stopwatch

            label1.AutoSize = true;
            label1.Text = String.Empty;

            this.Controls.Add(label1);
        }
        private int cnt = 0;
        private string timeFormat = "hh:mm:ss tt" ;
        private bool displayEts = false;
        
        private void timer1_Tick(object sender, EventArgs e)
        {
            int  days = sw.Elapsed.Days, hrs = sw.Elapsed.Hours, mins = sw.Elapsed.Minutes, secs = sw.Elapsed.Seconds;
            label1.Text = DateTime.Now.ToString(timeFormat);
            if (days < 10)
                label2.Text = "0" + days + ":";
            else
                label2.Text = days + ":" ;
            if (hrs < 10)
                label2.Text += "0" + hrs + ":";
            else
                label2.Text += hrs + ":";
            if (mins < 10)
                label2.Text += "0" + mins + ":";
            else
                label2.Text += mins + ":";
            if (secs < 10)
                label2.Text += "0" + secs;
            else
                label2.Text += secs;

            // now click the middle mouse button - should not bother anything since usually 
            // the middle mouse button does not exist
            if (cnt >= Convert.ToInt32(ConfigurationManager.AppSettings["mouseClickInterval"])) 
            {
                // click the mouse butten every "moustClickInterval's"
                //InputSimulator.SimulateKeyPress(VirtualKeyCode.MBUTTON);
                //Call the imported function with the cursor's current position
                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint) Cursor.Position.X, (uint) Cursor.Position.Y, 0, 0);

                cnt = 0;         
            } 
            else
            {
                cnt++;
            }

            if (ConfigurationManager.AppSettings["quitTime"] != null)
            {
                switch (DateTime.Now.DayOfWeek)
                {
                    case DayOfWeek.Monday:
                    case DayOfWeek.Tuesday:
                    case DayOfWeek.Wednesday:
                    case DayOfWeek.Thursday:
                    case DayOfWeek.Friday:
                        {
                            if (!displayEts && DateTime.Now.Hour == Convert.ToInt32(ConfigurationManager.AppSettings["quitTime"])
                                && !timeEntered.Checked)
                            {
                                displayEts = true;
                                enableAutoEtsBtn.Enabled = true;
                                Process.Start(webApp, ConfigurationManager.AppSettings["etsWebpage"]);
                                //System.Media.SoundPlayer player = new System.Media.SoundPlayer(@"C:\Users\zf297a\Music\sounds\to-arms-bugle-call.wav");
                                String sound = ConfigurationManager.AppSettings["etsAlert"];
                                Wav.Play(sound);
                            }
                            break;
                        }                
                }
            

                //at midnight reset the alarm
                if (displayEts && DateTime.Now.Hour == 0)
                {
                    displayEts = false;
                    timeEntered.Checked = false;
                    enableAutoEtsBtn.Enabled = false;
                }

            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            webApp = ConfigurationManager.AppSettings["browser"];

            if (ConfigurationManager.AppSettings["timeFormat"].Equals("std"))
                timeFormat = "hh:mm:ss tt";
            else if (ConfigurationManager.AppSettings["timeFormat"].Equals("mil"))
                timeFormat = "HH:mm:ss";
            else
                timeFormat = ConfigurationManager.AppSettings["timeFormat"];

            string webpage = ConfigurationManager.AppSettings["webpage"];

            if ( ! webpage.Equals("none"))
                Process.Start(webApp, webpage);
            this.DesktopLocation = new Point(0,0);

            this.linkLabel1.Text = ConfigurationManager.AppSettings["linkLabel"] ;
            int ll = this.linkLabel1.Text.Length ;
            this.linkLabel1.Links.Add(0,ll,ConfigurationManager.AppSettings["hyperLink"]) ;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            displayEts = false;
            enableAutoEtsBtn.Enabled = false;

        }

        
    }
}
