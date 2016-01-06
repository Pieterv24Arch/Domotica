//This class extends the fragment class


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Domotica
{
	public class Connection1 : Android.Support.V4.App.Fragment
	{
		//Connection Variables
		private Button mConnectionButton;
		private EditText mIpField;
		private EditText mPortField;
		private TextView mConnection_Text;
		private ConnectionProtocol connect = new ConnectionProtocol();

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// Use this to return your custom view for this Fragment
			// return inflater.Inflate(Resource.Layout.YourFragment, container, false);

			//Inflate the layout of the Connetion fragment
			View view = inflater.Inflate (Resource.Layout.Connection, container, false);
			//Connection Layout items
			mConnectionButton = view.FindViewById<Button>(Resource.Id.ConnectionButton);
			mIpField = view.FindViewById<EditText>(Resource.Id.editTextIP);
			mPortField = view.FindViewById<EditText>(Resource.Id.editTextPort);
			mConnection_Text = view.FindViewById<TextView>(Resource.Id.Connection_Text);

			//Connection Event Handlers
			mConnectionButton.Click += delegate {
				int tempIntContainer;
				//set Global IpAddress variable equal to the text in the ipField
				GlobalVariables.IPAddress = mIpField.Text;
				//Check if the portnumber is in fact a number
				int.TryParse(mPortField.Text, out tempIntContainer);
				GlobalVariables.PortAddress = tempIntContainer;
				//show text refreshing... in the fragment
				mConnection_Text.Text = "Refreshing...";
				//Que connectiontest and updating text accoardingly
				ThreadPool.QueueUserWorkItem(args => {
					connect.TestConnection(mConnection_Text);
					Activity.RunOnUiThread(() => {
						mConnection_Text.Text = GlobalVariables.IpAvailable ? "Connection Succesfull" : "Connection Failed";
					});
				});
			};
			return view;
		}
	}
}

