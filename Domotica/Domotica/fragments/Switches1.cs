
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System.Threading;

namespace Domotica
{
	public class Switches1 : MySupportFragment
	{
		public Switches1()
		{
			this.title = Resource.String.Switches1;
		}

		//Switch Variables
		private Switch Adapter1;
		private Switch Adapter2;
		private Switch Adapter3;
		private Switch Adapter4;
		private Switch Adapter5;
		private Button buttonRefresh;
		private List<Switch> _Adapters;
		private bool backgroundChange = false;

		//create connect object using the connectionprotocol class
		private ConnectionProtocol connect = new ConnectionProtocol();

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Create your fragment here
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// Use this to return your custom view for this Fragment
			// return inflater.Inflate(Resource.Layout.YourFragment, container, false);

			//inflate layout of switches1
			View view = inflater.Inflate (Resource.Layout.Switches1, container, false);

			//Assign variables to the switches and button on the layout
			Adapter1 = view.FindViewById<Switch>(Resource.Id.Ch1);
			Adapter2 = view.FindViewById<Switch>(Resource.Id.Ch2);
			Adapter3 = view.FindViewById<Switch>(Resource.Id.Ch3);
			Adapter4 = view.FindViewById<Switch>(Resource.Id.Ch4);
			Adapter5 = view.FindViewById<Switch>(Resource.Id.ChAll);
			buttonRefresh = view.FindViewById<Button> (Resource.Id.buttonRefresh);
			//Add all switches to a list
			_Adapters = new List<Switch>() { Adapter1, Adapter2, Adapter3, Adapter4, Adapter5 };

			//Switches Event Handler
			//Actions to perform if a switch is toggled
			Adapter1.CheckedChange += delegate(object sender, CompoundButton.CheckedChangeEventArgs e) {
				Changed(Adapter1, e.IsChecked);
			};
			Adapter2.CheckedChange += delegate(object sender, CompoundButton.CheckedChangeEventArgs e) {
				Changed(Adapter2, e.IsChecked);
			};
			Adapter3.CheckedChange += delegate(object sender, CompoundButton.CheckedChangeEventArgs e) {
				Changed(Adapter3, e.IsChecked);
			};
			Adapter4.CheckedChange += delegate(object sender, CompoundButton.CheckedChangeEventArgs e) {
				Changed(Adapter4, e.IsChecked);
			};
			Adapter5.CheckedChange += delegate(object sender, CompoundButton.CheckedChangeEventArgs e) {
				Changed(Adapter5, e.IsChecked);
			};
			buttonRefresh.Click += delegate {
				if(GlobalVariables.IpAvailable)
					ThreadPool.QueueUserWorkItem(o => checkSwitches());
				else
					noConnectionAlert();
			};
			return view;
		}

		public void Changed (Switch lAdapter, bool e)
		{
			bool rightMode = (GlobalVariables.Mode == "Switch Mode");
			if (!backgroundChange && rightMode)//check if statechange is comming from user
				ThreadPool.QueueUserWorkItem (o => switchControl (lAdapter, e));
			if(!GlobalVariables.IpAvailable)//set switch to false if no connection is available
				lAdapter.Checked = false;
				if (!rightMode)
			{
				lAdapter.Checked = false;
				wrongModeAlert ();
			}
			
		}



		//Send commands to toggle a switch to the arduino
		public void switchControl(Switch lAdapter, bool state)
		{
			if (GlobalVariables.IpAvailable)
			{
				//what switch should be toggled
				switch (lAdapter.Text)
				{
					case "Switch 1":
						//what command should be send to the arduino
						connect.tell (state ? "Ch1ON" : "Ch1OFF");
						break;
					case "Switch 2":
						connect.tell (state ? "Ch2ON" : "Ch2OFF");
						break;
					case "Switch 3":
						connect.tell (state ? "Ch3ON" : "Ch3OFF");
						break;
					case "Switch 4":
						connect.tell (state ? "Ch4ON" : "Ch4OFF");
						break;
					case "All Switches":
						connect.tell (state ? "ChAllON" : "ChAllOFF");
						break;
				}
				//sync the state of the switches in the app with the state of the switches in the arduino
				checkSwitches ();
			} else
			{
				//if there is no connection give an alert
				noConnectionAlert ();
			}
		}

		//method that syncs the switches in the app with those in the arduino
		public void checkSwitches()
		{
			//Check if there is a connection available
			if (GlobalVariables.IpAvailable)
			{
				//instruct switches that this is a change made by the system(to prevent app from sending a command to the arduino agian as duplicate)
				backgroundChange = true;
				//ask arduino for the states of the switches and put them in an array, splitting them at the ','
				string[] states = connect.ask ("States").Split (',');
				//make list of bools
				List<bool> boolStates = new List<bool> ();
				//convert the strings in the states array to booleans and add them to the list of bools
				foreach (string s in states)
				{
					if (s == "true")
						boolStates.Add (true);
					else if (s == "false")
						boolStates.Add (false);
				}
				//change states of the switches acoarding to the values in the bool array
				if (boolStates.Count == 4)
				{
					Activity.RunOnUiThread (() => {
						for (int i = 0; i < 4; i++)
						{
							if (_Adapters [i].Checked != boolStates [i])
							{
								_Adapters [i].Checked = boolStates [i];
							}
						}
						//check if all individual control switches are either true or false.
						//if so then change state of the switch for all switches
						if (boolStates.Contains (!boolStates [0]))
							_Adapters [4].Checked = false;
						else
							_Adapters [4].Checked = boolStates [0];
						//allow user to make changes agian
						backgroundChange = false;
					});
				}
			} else
			{
				//if no connection is found then show alert
				noConnectionAlert ();
			}
		}

		//no connection alert
		public void noConnectionAlert()
		{
			AlertDialog.Builder alert = new AlertDialog.Builder (this.Activity);
			alert.SetTitle ("No Connection");
			alert.SetMessage ("The app could not connect to the arduino.\nPlease check if a valid IP is entered");
			alert.SetNeutralButton ("OK", (senderAlert, EventArgs) => {
				alert.Dispose ();
			});
			Activity.RunOnUiThread (() => {
				alert.Show ();
			});
		}

		public void wrongModeAlert()
		{
			AlertDialog.Builder alert = new AlertDialog.Builder (this.Activity);
			alert.SetTitle ("Wrong Mode Selected");
			alert.SetMessage ("You are in the wrong mode to control the switches with this page.\n" +
				"Please change modes to use this feature.");
			alert.SetNeutralButton ("OK", (senderAlert, EventArgs) => {
				alert.Dispose ();
			});
			Activity.RunOnUiThread (() => {
				alert.Show();
			});
		}
	}
}

