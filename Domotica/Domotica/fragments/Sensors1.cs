
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
using System.Timers;

namespace Domotica
{
	public class Sensors1 : Android.Support.V4.App.Fragment
	{
		//TextViews
		TextView Sensor1;
		TextView Sensor2;

		//Checkboxes
		CheckBox Sensor1Check;
		CheckBox Sensor2Check;

		//Interactive elements
		Button refreshButton;
		Switch refreshToggleSwitch;

		//Timer
		Timer mTimer;

		ConnectionProtocol connect = new ConnectionProtocol();

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			mTimer = new Timer ();
			mTimer.Interval = 1000;
			mTimer.Elapsed += new ElapsedEventHandler (getValues);
			//mTimer.Enabled = true;
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// Use this to return your custom view for this Fragment
			// return inflater.Inflate(Resource.Layout.YourFragment, container, false);

			//Inflate the Sensors1 layout
			View view = inflater.Inflate (Resource.Layout.Sensors1, container, false);



			//Sersor Textviews and buttons found in the layout
			Sensor1 = view.FindViewById<TextView> (Resource.Id.sensor1Text);
			Sensor2 = view.FindViewById<TextView> (Resource.Id.sensor2Text);
			Sensor1Check = view.FindViewById<CheckBox> (Resource.Id.Sensor1_Checkbox);
			Sensor2Check = view.FindViewById<CheckBox> (Resource.Id.Sensor2_Checkbox);
			refreshButton = view.FindViewById<Button> (Resource.Id.Refresh_Sensors);
			refreshToggleSwitch = view.FindViewById<Switch> (Resource.Id.Toggle_SensorRefresh);


			//get values if button is pressed
			refreshButton.Click += delegate {
				getValues();
			};

			//Start/Stop timer if switch is toggled
			refreshToggleSwitch.CheckedChange += delegate(object sender, CompoundButton.CheckedChangeEventArgs e) {
				//if ip is available enable or disable timer
				if(GlobalVariables.IpAvailable)
					mTimer.Enabled = e.IsChecked;
				//else disable timer(if running), set toggle to false and show alert that no connection is available
				else
				{
					mTimer.Enabled = false;
					refreshToggleSwitch.Checked = false;
					noConnectionAlert();
				}
			};

			return view;
		}

		//Method for getting sensorValues for buttonPress
		public void getValues()
		{
			if (GlobalVariables.IpAvailable)
			{
				//get string with data and split it at the ,
				string[] tempString = connect.ask ("getVal").Split (',');
				//check if received data matches preset
				if (tempString.Length == 2)
				{
					//set textviews to recieved sensorvalues
					Activity.RunOnUiThread (() => {
						if(Sensor1Check.Checked)
							Sensor1.Text = tempString [0];
						else 
							Sensor1.Text = " ";
						if(Sensor2Check.Checked)
							Sensor2.Text = tempString [1];
						else
							Sensor2.Text = " ";
					});
				}
			} else
			{
				//if no connection is detected give a noconnection allert
				noConnectionAlert ();
			}
		}


		//does the same as the method above but is called by the timer
		public void getValues(object sender, ElapsedEventArgs e)
		{
			if (GlobalVariables.IpAvailable)
			{
				string[] tempString = connect.ask ("getVal").Split (',');
				if (tempString.Length == 2)
				{
					Activity.RunOnUiThread (() => {
						if(Sensor1Check.Checked)
							Sensor1.Text = tempString [0];
						else 
							Sensor1.Text = " ";
						if(Sensor2Check.Checked)
							Sensor2.Text = tempString [1];
						else
							Sensor2.Text = " ";
					});
				}
			}
		}

		//Show alert for no connection detected
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
	}
}

