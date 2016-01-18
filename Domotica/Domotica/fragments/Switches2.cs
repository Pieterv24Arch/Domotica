
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
	public class Switches2 : MySupportFragment
	{
		public Switches2()
		{
			this.title = Resource.String.Switches2;
		}

		//Variables
		ListView mTimerList;
		Switch mTimerToggle;
		Timer mTimer;

		List<TimerItem> mTimerData;

		TimerListAdapter mlistAdapter;

		//Connect to arduino
		ConnectionProtocol connect = new ConnectionProtocol();

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			HasOptionsMenu = true;

			mTimer = new Timer ();
			mTimer.Interval = 1000;
			mTimer.Elapsed += new ElapsedEventHandler (Timer_Tick);
			// Create your fragment here
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// Use this to return your custom view for this Fragment
			// return inflater.Inflate(Resource.Layout.YourFragment, container, false);

			View view = inflater.Inflate (Resource.Layout.Switches2, container, false);

			mTimerList = view.FindViewById<ListView> (Resource.Id.TimerList);
			mTimerToggle = view.FindViewById<Switch> (Resource.Id.TimerCheckSwitch);

			//change textColor of switch
			mTimerToggle.SetTextColor(Android.Graphics.Color.White);

			mTimerData = new List<TimerItem> ();

			mlistAdapter = new TimerListAdapter (this.Activity, mTimerData);
			mTimerList.Adapter = mlistAdapter;

			mTimerList.ItemClick += MTimerList_ItemClick;
			mTimerList.ItemLongClick += MTimerList_ItemLongClick;
			mTimerToggle.CheckedChange += MTimerToggle_CheckedChange;

			return view;
		}

		void MTimerToggle_CheckedChange (object sender, CompoundButton.CheckedChangeEventArgs e)
		{
			if (GlobalVariables.IpAvailable && GlobalVariables.Mode == "Timer Mode" && mTimerData.Count > 0)
			{
				if (e.IsChecked)
					mTimer.Enabled = true;
				else
					mTimer.Enabled = false;
			} else
			{
				mTimerToggle.Checked = false;
				mTimer.Enabled = false;
				if (!GlobalVariables.IpAvailable)
					noConnectionAlert ();
				if (GlobalVariables.Mode != "Timer Mode")
					wrongModeAlert ();
				else
					noEntryalert ();
			} 
		}

		void MTimerList_ItemLongClick (object sender, AdapterView.ItemLongClickEventArgs e)
		{
			AlertDialog.Builder alert = new AlertDialog.Builder (this.Activity);
			alert.SetTitle ("Entry Customization");
			alert.SetMessage ("Do you want to delete or edit this entry?");
			alert.SetNeutralButton ("Cancel", (senderAlert, EventArgs) => {
				alert.Dispose ();
			});
			alert.SetPositiveButton ("Edit", (senderAlert, EventArgs) => {
				if(GlobalVariables.IpAvailable)
					//Edit Entry
					entryEdit(e.Position);
				else
					noConnectionAlert();
			});
			alert.SetNegativeButton ("Delete", (senderAlert, EventArgs) => {
				mTimerData.RemoveAt(e.Position);
				mlistAdapter.NotifyDataSetChanged();
			});
			Activity.RunOnUiThread (() => {
				alert.Show ();
			});
		}

		void MTimerList_ItemClick (object sender, AdapterView.ItemClickEventArgs e)
		{
			Toast.MakeText (this.Activity, Resource.String.listShortClick, ToastLength.Long).Show ();
		}

		public override void OnCreateOptionsMenu (IMenu menu, MenuInflater inflater)
		{
			base.OnCreateOptionsMenu (menu, inflater);
			inflater.Inflate (Resource.Menu.add_menu, menu);
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			if (item.ItemId == Resource.Id.Add_Button)
			{
				if (GlobalVariables.IpAvailable)
				{
					//Add Timer
					entryAdd();
				}
				else
					noConnectionAlert();
			}
			if (item.ItemId == Resource.Id.Help_Button) {
				AlertDialog.Builder alert = new AlertDialog.Builder (this.Activity);
				LayoutInflater inflater = this.GetLayoutInflater (null);
				View dialogView = inflater.Inflate (Resource.Layout.HelpHolder, null);
				alert.SetTitle ("Help");
				alert.SetView (dialogView);

				TextView helpText = dialogView.FindViewById<TextView> (Resource.Id.Help_Text);

				helpText.Text = GetString (Resource.String.SwitchHelp);

				alert.SetNeutralButton ("Close", (senderAlert, EventArgs) => {
					alert.Dispose ();
				});

				Activity.RunOnUiThread (() => {
					alert.Show ();
				});
			}
			return base.OnOptionsItemSelected (item);
		}

		public void entryAdd()
		{
			AlertDialog.Builder alert = new AlertDialog.Builder (this.Activity);
			LayoutInflater inflater = this.GetLayoutInflater (null);
			View dialogView = inflater.Inflate (Resource.Layout.TimerSensorLayout, null);
			alert.SetTitle ("Add Entry");
			alert.SetView (dialogView);

			EditText hourField = dialogView.FindViewById<EditText> (Resource.Id.TimeHourField);
			EditText minuteField = dialogView.FindViewById<EditText> (Resource.Id.TimeMinuteField);
			Spinner switchIdentity = dialogView.FindViewById<Spinner> (Resource.Id.TimerSwitchNameSpinner);
			Spinner switchState = dialogView.FindViewById<Spinner> (Resource.Id.TimerStateSpinner);

			alert.SetNeutralButton ("Cancel", (senderAlert, EventArgs) => {
				alert.Dispose();
			});
			alert.SetPositiveButton ("Add", (senderAlert, EventArgs) => {
				//If connection is available add entry
				if(GlobalVariables.IpAvailable)
				{
					
					int hours;
					int minutes;
					int.TryParse(hourField.Text, out hours);
					int.TryParse(minuteField.Text, out minutes);
					bool state = ((switchState.SelectedItem.ToString() == "On") ? true : false);
					//Get data from view and add it to list item
					mTimerData.Add(new TimerItem(
						new DateTime(1,1,1,hours, minutes, 0), 
						switchIdentity.SelectedItem.ToString(), 
						switchIdentity.SelectedItemPosition, 
						state,
						switchState.SelectedItemPosition));


					//notify listview that values have changed
					mlistAdapter.NotifyDataSetChanged();
				}
			});
			Activity.RunOnUiThread (() => {
				alert.Show();
			});
		}

		public void entryEdit(int index)
		{
			AlertDialog.Builder alert = new AlertDialog.Builder (this.Activity);
			LayoutInflater inflater = this.GetLayoutInflater (null);
			View dialogView = inflater.Inflate (Resource.Layout.TimerSensorLayout, null);
			alert.SetTitle ("Add Entry");
			alert.SetView (dialogView);

			EditText hourField = dialogView.FindViewById<EditText> (Resource.Id.TimeHourField);
			EditText minuteField = dialogView.FindViewById<EditText> (Resource.Id.TimeMinuteField);
			Spinner switchIdentity = dialogView.FindViewById<Spinner> (Resource.Id.TimerSwitchNameSpinner);
			Spinner switchState = dialogView.FindViewById<Spinner> (Resource.Id.TimerStateSpinner);

			hourField.Text = mTimerData [index].mTime.Hour.ToString();
			minuteField.Text = mTimerData [index].mTime.Minute.ToString();
			switchIdentity.SetSelection(mTimerData [index].mSwitchi);
			switchState.SetSelection(mTimerData [index].mSwitchStatei);

			alert.SetNeutralButton ("Cancel", (senderAlert, EventArgs) => {
				alert.Dispose();
			});
			alert.SetPositiveButton ("Edit", (senderAlert, EventArgs) => {
				//If connection is available add entry
				if(GlobalVariables.IpAvailable)
				{

					int hours;
					int minutes;
					int.TryParse(hourField.Text, out hours);
					int.TryParse(minuteField.Text, out minutes);
					bool state = ((switchState.SelectedItem.ToString() == "On") ? true : false);
					//Get data from view and add it to list item
					mTimerData[index] = (new TimerItem(
						new DateTime(1,1,1,hours, minutes, 0), 
						switchIdentity.SelectedItem.ToString(), 
						switchIdentity.SelectedItemPosition, 
						state,
						switchState.SelectedItemPosition));


					//notify listview that values have changed
					mlistAdapter.NotifyDataSetChanged();
				}
			});
			Activity.RunOnUiThread (() => {
				alert.Show();
			});
		}

		public void Timer_Tick(object sender, ElapsedEventArgs e)
		{
			DateTime currentTime = DateTime.Now;
			foreach (TimerItem t in mTimerData) {
				if (t.mTime.Hour == currentTime.Hour && t.mTime.Minute == currentTime.Minute) {
					switchControl (t.mSwitch, t.mSwitchState);
				}
			}
		}

		public void switchControl(string switchName, bool state)
		{
			if (GlobalVariables.IpAvailable)
			{
				string[] states = connect.ask ("States").Split (',');
				List<bool> boolStates = new List<bool> ();
				//convert the strings in the states array to booleans and add them to the list of bools
				foreach (string s in states)
				{
					if (s == "true")
						boolStates.Add (true);
					else if (s == "false")
						boolStates.Add (false);
				}
				//what switch should be toggled
				switch (switchName)
				{
				case "Switch 1":
					if (boolStates [0] != state)
					{
						//what command should be send to the arduino
						connect.tell (state ? "Ch1ON" : "Ch1OFF");
					}
					break;
				case "Switch 2":
					if (boolStates [1] != state)
					{
						connect.tell (state ? "Ch2ON" : "Ch2OFF");
					}
					break;
				case "Switch 3":
					if (boolStates [2] != state)
					{
						connect.tell (state ? "Ch3ON" : "Ch3OFF");
					}
					break;
				case "Switch 4":
					if (boolStates [3] != state)
					{
						connect.tell (state ? "Ch4ON" : "Ch4OFF");
					}
					break;
				case "All Switches":
					if (boolStates.Contains(!state))
					{
						connect.tell (state ? "ChAllON" : "ChAllOFF");
					}
					break;
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

		public void wrongModeAlert()
		{
			AlertDialog.Builder alert = new AlertDialog.Builder (this.Activity);
			alert.SetTitle ("Wrong Mode Selected");
			alert.SetMessage ("You are in the wrong mode to use thresholds.\n" +
				"Please change the mode to \"Timer Mode\" to use this feature.");
			alert.SetNeutralButton ("OK", (senderAlert, EventArgs) => {
				alert.Dispose ();
			});
			Activity.RunOnUiThread (() => {
				alert.Show();
			});
		}

		public void noEntryalert()
		{
			AlertDialog.Builder alert = new AlertDialog.Builder (this.Activity);
			alert.SetTitle ("No entries");
			alert.SetMessage ("There are no entries");
			alert.SetNeutralButton ("OK", (senderAlert, EventArgs) => {
				alert.Dispose ();
			});
			Activity.RunOnUiThread (() => {
				alert.Show();
			});
		}
	}
}

