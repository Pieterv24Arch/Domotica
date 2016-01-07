using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace DomoticaApp
{
    [Activity(Label = "DomoticaApp", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        const int port = 32545;
        private bool backgroundChange = false;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Switch Adatper1 = FindViewById<Switch>(Resource.Id.Ch1);
            Switch Adatper2 = FindViewById<Switch>(Resource.Id.Ch2);
            Switch Adatper3 = FindViewById<Switch>(Resource.Id.Ch3);
            Switch Adatper4 = FindViewById<Switch>(Resource.Id.Ch4);
            Switch Adatper5 = FindViewById<Switch>(Resource.Id.ChAll);
            EditText IpField = FindViewById<EditText>(Resource.Id.editTextIP);

            List<Switch> Adapters = new List<Switch>() { Adatper1, Adatper2, Adatper3, Adatper4, Adatper5 };
            if (!backgroundChange)
            {
                Adatper1.CheckedChange += delegate(object sender, CompoundButton.CheckedChangeEventArgs e)
                {
                    if (!backgroundChange)
                    {
                        ThreadPool.QueueUserWorkItem(o => switchControl(1, e.IsChecked, Adapters, IpField.Text));
                    }
                };
                Adatper2.CheckedChange += delegate(object sender, CompoundButton.CheckedChangeEventArgs e)
                {
                    if (!backgroundChange)
                    {
                        ThreadPool.QueueUserWorkItem(o => switchControl(2, e.IsChecked, Adapters, IpField.Text));
                    }
                };
                Adatper3.CheckedChange += delegate(object sender, CompoundButton.CheckedChangeEventArgs e)
                {
                    if (!backgroundChange)
                    {
                        ThreadPool.QueueUserWorkItem(o => switchControl(3, e.IsChecked, Adapters, IpField.Text));
                    }
                };
                Adatper4.CheckedChange += delegate(object sender, CompoundButton.CheckedChangeEventArgs e)
                {
                    if (!backgroundChange)
                    {
                        ThreadPool.QueueUserWorkItem(o => switchControl(4, e.IsChecked, Adapters, IpField.Text));
                    }
                };
                Adatper5.CheckedChange += delegate(object sender, CompoundButton.CheckedChangeEventArgs e)
                {
                    if (!backgroundChange)
                    {
                        ThreadPool.QueueUserWorkItem(o => switchControl(5, e.IsChecked, Adapters, IpField.Text));
                    }
                };
            }
        }

        public Socket open(string ipaddress, int portnr)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ip = IPAddress.Parse(ipaddress);
            IPEndPoint endpoint = new IPEndPoint(ip, portnr);
            socket.Connect(endpoint);
            return socket;
        }

        public void write(Socket socket, string text)
        {
            socket.Send(Encoding.ASCII.GetBytes(text));
        }

        public string read(Socket socket)
        {
            byte[] bytes = new byte[4096];
            int bytesRec = socket.Receive(bytes);
            string text = Encoding.ASCII.GetString(bytes, 0, bytesRec);
            return text;
        }

        public void close(Socket socket)
        {
            socket.Close();
        }

        // datagram like conversation with server
        public void tell(string ipaddress, int portnr, string message)
        {
            Socket s = open(ipaddress, portnr);
            write(s, message);
            close(s);
        }

        public string ask(string ipaddress, int portnr, string message)
        {
            Socket s = open(ipaddress, portnr);
            write(s, message);
            string awnser = read(s);
            close(s);
            return awnser;
        }

        public void switchControl(int switchNr, bool state, List<Switch> Switches, string ipAdress)
        {
            switch (switchNr)
            {
                case 1:
                    tell(ipAdress, port, state ? "Ch1ON" : "Ch1OFF");
                    break;
                case 2:
                    tell(ipAdress, port, state ? "Ch2ON" : "Ch2OFF");
                    break;
                case 3:
                    tell(ipAdress, port, state ? "Ch3ON" : "Ch3OFF");
                    break;
                case 4:
                    tell(ipAdress, port, state ? "Ch4ON" : "Ch4OFF");
                    break;
                case 5:
                    tell(ipAdress, port, state ? "ChAllON" : "ChAllOFF");
                    break;
                default:
                    break;
            }
            checkSwitches(ipAdress, Switches);
        }

        public void checkSwitches(string IpAdress, List<Switch> Switches)
        {
            backgroundChange = true;
            string[] states = ask(IpAdress, port, "States").Split(',');
            List<bool> boolStates = new List<bool>();
            foreach (string s in states)
            {
                if (s == "true") boolStates.Add(true);
                else boolStates.Add(false);
            }
            RunOnUiThread(() =>
            {
                for (int i = 0; i < 4; i++)
                {
                    if (Switches[i].Checked != boolStates[i])
                    {
                        Switches[i].Checked = boolStates[i];
                    }
                }
                if (boolStates.Contains(!boolStates[0])) Switches[4].Checked = false;
                else Switches[4].Checked = boolStates[0];
                backgroundChange = false;
            });
            //backgroundChange = false;
        }
    }
}

