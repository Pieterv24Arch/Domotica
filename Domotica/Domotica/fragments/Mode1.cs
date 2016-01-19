
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
	public class Mode1 : MySupportFragment
	{

		Spinner mModeSpinner;

		public Mode1()
		{
			this.title = Resource.String.Mode;
		}

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			HasOptionsMenu = true;
			GlobalVariables.Mode = "Switch Mode";
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// Use this to return your custom view for this Fragment
			// return inflater.Inflate(Resource.Layout.YourFragment, container, false);

			View view = inflater.Inflate (Resource.Layout.Mode, container, false);

			//assign id from layout to mModeSpinner
			mModeSpinner = view.FindViewById<Spinner> (Resource.Id.ModeSpinner);

			//event handler for if different item is selected;
			mModeSpinner.ItemSelected += MModeSpinner_ItemSelected;

			return view;
		}

		void MModeSpinner_ItemSelected (object sender, AdapterView.ItemSelectedEventArgs e)
		{
			GlobalVariables.Mode = mModeSpinner.SelectedItem.ToString();
		}

		public override void OnCreateOptionsMenu (IMenu menu, MenuInflater inflater)
		{
			base.OnCreateOptionsMenu (menu, inflater);
			inflater.Inflate (Resource.Menu.help_menu, menu);
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			if (item.ItemId == Resource.Id.Help_Button) {
				AlertDialog.Builder alert = new AlertDialog.Builder (this.Activity);
				LayoutInflater inflater = this.GetLayoutInflater (null);
				View dialogView = inflater.Inflate (Resource.Layout.HelpHolder, null);
				alert.SetTitle ("Help");
				alert.SetView (dialogView);

				TextView helpText = dialogView.FindViewById<TextView> (Resource.Id.Help_Text);

				helpText.Text = GetString (Resource.String.ModesHelp);

				alert.SetNeutralButton ("Close", (senderAlert, EventArgs) => {
					alert.Dispose ();
				});

				Activity.RunOnUiThread (() => {
					alert.Show ();
				});
			}
			return base.OnOptionsItemSelected (item);
		}
	}
		
}

