using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using WpfStopwatch.Entities;
using WpfStopwatch.Repository;

namespace WpfStopwatch
{
    /// <summary>
    ///
    /// </summary>
    public partial class MainWindow : Window
    {
        public LapRepository LapsRepository { get; }
        private DispatcherTimer dt = new DispatcherTimer();
        private Stopwatch sw = new Stopwatch();
        private bool isSF;
        private string strClock = "";

        public TimeSpan tsNow, ts = TimeSpan.FromHours(0);

        public MainWindow()
        {
            InitializeComponent();
            LapsRepository = new LapRepository();

            #region Notify

            MyNotifyIcon = new System.Windows.Forms.NotifyIcon();

            var dir = System.IO.Directory.GetParent(System.IO.Directory.GetParent(Environment.CurrentDirectory)
                          .ToString()) + "\\Resource\\stopwatch.ico";
            MyNotifyIcon.Icon = new System.Drawing.Icon(dir);
            MyNotifyIcon.MouseDoubleClick += MyNotifyIcon_MouseDoubleClick;

            #endregion Notify

            dt.Tick += dt_Tick;
            dt.Interval = new TimeSpan(0, 0, 0, 0, 1);
            dt.Start();
        }

        #region Notify_

        private System.Windows.Forms.NotifyIcon MyNotifyIcon;

        private void MyNotifyIcon_MouseDoubleClick(object sender,
            System.Windows.Forms.MouseEventArgs e)
        {
            this.WindowState = WindowState.Normal;
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.ShowInTaskbar = false;
                MyNotifyIcon.BalloonTipTitle = @"Minimize Successful";
                MyNotifyIcon.BalloonTipText = @"Minimized the app ";
                MyNotifyIcon.ShowBalloonTip(400);
                MyNotifyIcon.Visible = true;
            }
            else if (this.WindowState == WindowState.Normal)
            {
                MyNotifyIcon.Visible = false;
                this.ShowInTaskbar = true;
            }
        }

        #endregion Notify_

        private void dt_Tick(object sender, EventArgs e)
        {
            if (sw.IsRunning)
            {
                ts = sw.Elapsed;
                seconds.Angle = Math.Abs(seconds.Angle - 360) < 0 ? 0 : ts.Seconds * 6;
                minutes.Angle = Math.Abs(minutes.Angle - 360) < 0 ? 0 : ts.Minutes * 6;

                strClock = $"{ts.Minutes:00}:{ts.Seconds:00}:{ts.Milliseconds / 10:00}";
                tbClock.Text = strClock;
            }
        }

        private void btReset_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sw.IsRunning || !isSF)
            {
                sw.Reset();
                seconds.Angle = minutes.Angle = 0;
                isSF = false;
                ((LapRepository)Resources["repository"]).Laps.Clear();
                ((LapRepository)Resources["repository"]).Laps.Add(
                    new Lap
                    {
                        LapNumber = 0,
                        TSpan = TimeSpan.FromHours(0),
                        TLap = TimeSpan.FromHours(0)
                    });
            }
        }

        private void btClose_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void btMin_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btStartStop_MouseDown(object sender, MouseButtonEventArgs e)
        {
            isSF = !isSF;

            if (isSF)
                sw.Start();
            else
                sw.Stop();
        }

        private void btLaps_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (isSF)
            {
                tsNow = ((LapRepository)Resources["repository"]).Laps.Last().TSpan;
                var lap = new Lap
                {
                    LapNumber = ((LapRepository)Resources["repository"]).Laps.Count,
                    TSpan = ts,
                    TLap = ts - tsNow
                };

                ((LapRepository)Resources["repository"]).Laps.Add(lap);

                dgLaps.ScrollIntoView(lap, dgLaps.Columns[0]);

                var laps = ((LapRepository)Resources["repository"])
                    .Laps.Where(t => t.TSpan != TimeSpan.FromHours(0));
                var bestSingle = laps
                    .Where(t => t.TLap == laps.Min(x => x.TLap))
                    .Select(l => new { nlap = l.LapNumber, tLap = l.TLap })
                    .Single();

                bestLap.Text = "Best laps: #" +
                               bestSingle.nlap.ToString("N0") + " = " +
                               bestSingle.tLap.ToString("mm':'ss':'ff");
            }
        }
    }
}