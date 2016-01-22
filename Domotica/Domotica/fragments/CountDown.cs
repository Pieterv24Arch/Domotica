
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

namespace Domotica
{
	public class CountDown : MySupportFragment
	{
		List<CountdownItem> mDataList; 

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
			View view = inflater.Inflate(Resource.Layout.FAQ, container, false);

			return view;
		}

		public override void OnCreateOptionsMenu (IMenu menu, MenuInflater inflater)
		{
			inflater.Inflate (Resource.Menu.add_menu, menu);
			base.OnCreateOptionsMenu (menu, inflater);
		}

		/*public override bool OnOptionsItemSelected (IMenuItem item)
		{
			if (item.ItemId == Resource.Id.Add_Button)
			{
				if (GlobalVariables.IpAvailable)
				{
					if (mDataList.Count < 2)
						//entryAdd ();
					else
						
						//entryLimitAlert ();		
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

				helpText.Text = GetString (Resource.String.SensorsHelp);

				alert.SetNeutralButton ("Close", (senderAlert, EventArgs) => {
					alert.Dispose ();
				});

				Activity.RunOnUiThread (() => {
					alert.Show ();
				});
			}
			return base.OnOptionsItemSelected (item);
		}*/

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
				"Please change the mode to \"Threshold Mode\" to use this feature.");
			alert.SetNeutralButton ("OK", (senderAlert, EventArgs) => {
				alert.Dispose ();
			});
			Activity.RunOnUiThread (() => {
				alert.Show();
			});
		}
	}
}

