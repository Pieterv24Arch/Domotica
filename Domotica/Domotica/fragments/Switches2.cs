
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
			mTimer.Elapsed = new ElapsedEventHandler (Timer_Tick);
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

			return view;
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
					noConnectionAlert(); //Placeholder
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
				}
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

			alert.SetNeutralButton ("Cancel", (senderAlert, EventArgs) => {
				alert.Dispose();
			});
			alert.SetPositiveButton ("Add", (senderAlert, EventArgs) => {
				//If connection is available add entry
				if(GlobalVariables.IpAvailable)
				{
					//Get data from view and add it to list item


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

