﻿
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
using defaultFragmentTransaction = Android.App.FragmentTransaction;
using defaultFragmentManager = Android.App.FragmentManager;
using System.Timers;

namespace Domotica
{
	public class Sensors2 : MySupportFragment
	{
		public Sensors2()
		{
			this.title = Resource.String.Sensors2;
		}

		//TextViews
		TextView mSensor1;
		TextView mSensor2;

		//Checkboxes
		CheckBox mSensor1Check;
		CheckBox mSensor2Check;

		//Interactive elements
		Button mRefreshButton;
		Switch mRefreshToggleSwitch;

		ListView mListview;
		List<SensorItem> mDataList;

		SensorListAdapter mListAdapter;

		Timer mTimer;

		ConnectionProtocol connect = new ConnectionProtocol();

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			mTimer = new Timer ();
			mTimer.Interval = 1000;
			mTimer.Elapsed += new ElapsedEventHandler (Timer_Tick);

			HasOptionsMenu = true;
			// Create your fragment here
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// Use this to return your custom view for this Fragment
			// return inflater.Inflate(Resource.Layout.YourFragment, container, false);

			View view = inflater.Inflate (Resource.Layout.Sensors2, container, false);

			mListview = view.FindViewById<ListView> (Resource.Id.SensorThresholdList);
			mSensor1 = view.FindViewById<TextView> (Resource.Id.sensor1Text);
			mSensor2 = view.FindViewById<TextView> (Resource.Id.sensor2Text);
			mSensor1Check = view.FindViewById<CheckBox> (Resource.Id.Sensor1_Checkbox);
			mSensor2Check = view.FindViewById<CheckBox> (Resource.Id.Sensor2_Checkbox);
			mRefreshButton = view.FindViewById<Button> (Resource.Id.Refresh_Sensors);
			mRefreshToggleSwitch = view.FindViewById<Switch> (Resource.Id.Toggle_SensorRefresh);

			mDataList = new List<SensorItem>();
			//mDataList.Add (new SensorItem ("Lightsensor", "=>", 455, "Switch 1")); //For Debuggin purposes

			mListAdapter = new SensorListAdapter (this.Activity, mDataList);
			mListview.Adapter = mListAdapter;

			mListview.ItemClick += MListview_ItemClick;
			mListview.ItemLongClick += MListview_ItemLongClick;

			mRefreshButton.Click += MRefreshButton_Click;
			mRefreshToggleSwitch.CheckedChange += MRefreshToggleSwitch_CheckedChange;


			return view;
		}

		void MRefreshToggleSwitch_CheckedChange (object sender, CompoundButton.CheckedChangeEventArgs e)
		{
			if (GlobalVariables.IpAvailable) 
			{
				mTimer.Enabled = true;
			} 
			else
			{
				mTimer.Enabled = false;
				noConnectionAlert ();
			}
		}

		void MRefreshButton_Click (object sender, EventArgs e)
		{
			Timer_Tick (null, null);
		}

		void MListview_ItemLongClick (object sender, AdapterView.ItemLongClickEventArgs e)
		{
			AlertDialog.Builder alert = new AlertDialog.Builder (this.Activity);
			alert.SetTitle ("Entry Customization");
			alert.SetMessage ("Do you want to delete or edit this entry?");
			alert.SetNeutralButton ("Cancel", (senderAlert, EventArgs) => {
				alert.Dispose ();
			});
			alert.SetPositiveButton ("Edit", (senderAlert, EventArgs) => {
				if(GlobalVariables.IpAvailable)
					entryEdit(e.Position);
				else
					noConnectionAlert();
			});
			alert.SetNegativeButton ("Delete", (senderAlert, EventArgs) => {
				mDataList.RemoveAt(e.Position);
				mListAdapter.NotifyDataSetChanged();
			});
			Activity.RunOnUiThread (() => {
				alert.Show ();
			});
		}

		void MListview_ItemClick (object sender, AdapterView.ItemClickEventArgs e)
		{
			Toast.MakeText (this.Activity, Resource.String.sensorShortClick, ToastLength.Long).Show ();
		}

		public override void OnCreateOptionsMenu (IMenu menu, MenuInflater inflater)
		{
			base.OnCreateOptionsMenu (menu, inflater);
			inflater.Inflate (Resource.Menu.action_menu, menu);
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			if (item.ItemId == Resource.Id.Add_Button)
			{
				if (GlobalVariables.IpAvailable)
					entryAdd ();
				else
					noConnectionAlert();
			}

			return base.OnOptionsItemSelected (item);
		}

		public void entryAdd()
		{
			AlertDialog.Builder alert = new AlertDialog.Builder (this.Activity);
			LayoutInflater inflater = this.GetLayoutInflater (null);
			View dialogView = inflater.Inflate (Resource.Layout.AddSensorLayout, null);
			alert.SetTitle ("Add Entry");
			alert.SetView (dialogView);

			Spinner mSensorName = dialogView.FindViewById<Spinner> (Resource.Id.SensornameSpinner);
			Spinner mRelation = dialogView.FindViewById<Spinner> (Resource.Id.SensorRelation);
			EditText mValue = dialogView.FindViewById<EditText> (Resource.Id.SensorValue);
			Spinner mSwitch = dialogView.FindViewById<Spinner> (Resource.Id.SwitchNameSpinner);

			alert.SetNeutralButton ("Cancel", (senderAlert, EventArgs) => {
				alert.Dispose();
			});
			alert.SetPositiveButton ("Add", (senderAlert, EventArgs) => {
				//If connection is available add entry
				if(GlobalVariables.IpAvailable)
				{
					int tempValue;
					if(mValue.Text == "") tempValue = 0;
					else tempValue = Convert.ToInt16(mValue.Text);
					mDataList.Add(new SensorItem(
						mSensorName.SelectedItem.ToString(),
						mSensorName.SelectedItemPosition,
						mRelation.SelectedItem.ToString(),
						mRelation.SelectedItemPosition,
						tempValue,
						mSwitch.SelectedItem.ToString(),
						mSwitch.SelectedItemPosition));
					//notify listview that values have changed
					mListAdapter.NotifyDataSetChanged();
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
			View dialogView = inflater.Inflate (Resource.Layout.AddSensorLayout, null);
			alert.SetTitle ("Edit Entry");
			alert.SetView (dialogView);

			Spinner mSensorName = dialogView.FindViewById<Spinner> (Resource.Id.SensornameSpinner);
			Spinner mRelation = dialogView.FindViewById<Spinner> (Resource.Id.SensorRelation);
			EditText mValue = dialogView.FindViewById<EditText> (Resource.Id.SensorValue);
			Spinner mSwitch = dialogView.FindViewById<Spinner> (Resource.Id.SwitchNameSpinner);

			mSensorName.SetSelection (mDataList[index].mSensorNamei);
			mRelation.SetSelection (mDataList[index].mRealationi);
			mValue.Text = mDataList [index].mValue.ToString ();
			mSwitch.SetSelection (mDataList[index].mSwitchIdentityi);

			alert.SetNeutralButton ("Cancel", (senderAlert, EventArgs) => {
				alert.Dispose();
			});
			alert.SetPositiveButton ("Confirm", (senderAlert, EventArgs) => {
				if(GlobalVariables.IpAvailable)
				{
					int tempValue;
					if(mValue.Text == "") tempValue = 0;
					else tempValue = Convert.ToInt16(mValue.Text);
					mDataList[index] = new SensorItem(
						mSensorName.SelectedItem.ToString(),
						mSensorName.SelectedItemPosition,
						mRelation.SelectedItem.ToString(),
						mRelation.SelectedItemPosition,
						tempValue,
						mSwitch.SelectedItem.ToString(),
						mSwitch.SelectedItemPosition);
					//notify listview that data has changed
					mListAdapter.NotifyDataSetChanged();
				}
			});
			Activity.RunOnUiThread (() => {
				alert.Show();
			});
		}

		public void checkEntries(string[] tempString)
		{
			if (tempString.Length == 2)
			{
				foreach (SensorItem s in mDataList)
				{
					switch (s.mSensorName)
					{
						case "Temperature Sensor":
							//bool state = checkValues (tempString [0], s.mRelation, s.mValue);
							switchControl (s.mSwitchIdentity, checkValues (tempString [0], s.mRelation, s.mValue));
							break;
						case "Light Sensor":
							switchControl (s.mSwitchIdentity, checkValues (tempString [1], s.mRelation, s.mValue));
							break;
					}
				}
			}
		}

		public bool checkValues (string sensorValue, string relation, int thresholdValue)
		{
			int intValue = Convert.ToInt16 (sensorValue);
			switch (relation)
			{
				case "Lower Than":
					if (intValue < thresholdValue)
						return true;
					else
						return false;
				case "Greater Than":
					if (intValue > thresholdValue)
						return true;
					else
						return false;
				default:
					return false;
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

		public void Timer_Tick(object sender, ElapsedEventArgs e)
		{
			
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

