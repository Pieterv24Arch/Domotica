
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

namespace Domotica
{
	public class Sensors2 : Android.Support.V4.App.Fragment
	{
		ListView mListview;
		List<SensorItem> mDataList;

		SensorListAdapter mListAdapter;

		ConnectionProtocol connect = new ConnectionProtocol();

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			HasOptionsMenu = true;
			// Create your fragment here
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// Use this to return your custom view for this Fragment
			// return inflater.Inflate(Resource.Layout.YourFragment, container, false);

			View view = inflater.Inflate (Resource.Layout.Sensors2, container, false);

			mListview = view.FindViewById<ListView> (Resource.Id.SensorThresholdList);

			mDataList = new List<SensorItem>();
			//mDataList.Add (new SensorItem ("Lightsensor", "=>", 455, "Switch 1")); //For Debuggin purposes

			mListAdapter = new SensorListAdapter (this.Activity, mDataList);
			mListview.Adapter = mListAdapter;

			mListview.ItemClick += MListview_ItemClick;
			mListview.ItemLongClick += MListview_ItemLongClick;


			return view;
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
				entryEdit(e.Position);
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

		public void switchControl(int switchNr, bool state)
		{
			if (GlobalVariables.IpAvailable)
			{
				//what switch should be toggled
				switch (switchNr)
				{
					case 1:
						//what command should be send to the arduino
						connect.tell (state ? "Ch1ON" : "Ch1OFF");
						break;
					case 2:
						connect.tell (state ? "Ch2ON" : "Ch2OFF");
						break;
					case 3:
						connect.tell (state ? "Ch3ON" : "Ch3OFF");
						break;
					case 4:
						connect.tell (state ? "Ch4ON" : "Ch4OFF");
						break;
					case 5:
						connect.tell (state ? "ChAllON" : "ChAllOFF");
						break;
				}
			}
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
				entryAdd ();
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
				int tempValue;
				if(mValue.Text == "") tempValue = 0;
				else tempValue = Convert.ToInt16(mValue.Text);
				mDataList.Add(new SensorItem(
					mSensorName.SelectedItem.ToString(),
					mRelation.SelectedItem.ToString(),
					tempValue,
					mSwitch.SelectedItem.ToString()));
				mListAdapter.NotifyDataSetChanged();
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
				int tempValue;
				if(mValue.Text == "") tempValue = 0;
				else tempValue = Convert.ToInt16(mValue.Text);
				mDataList[index] = new SensorItem(
					mSensorName.SelectedItem.ToString(),
					mRelation.SelectedItem.ToString(),
					tempValue,
					mSwitch.SelectedItem.ToString());
				mListAdapter.NotifyDataSetChanged();
			});
			Activity.RunOnUiThread (() => {
				alert.Show();
			});
		}

	}
		
}

