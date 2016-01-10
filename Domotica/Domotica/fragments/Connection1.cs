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
		private EditText mIpField1;
		private EditText mIpField2;
		private EditText mIpField3;
		private EditText mIpField4;
		private EditText mPortField;
		private TextView mConnection_Text;

		List<EditText> ipFields;

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
			//Elements of the ip text fields. ip is made out of the 4 edit text fields
			mIpField1 = view.FindViewById<EditText>(Resource.Id.editTextIP1);
			mIpField2 = view.FindViewById<EditText>(Resource.Id.editTextIP2);
			mIpField3 = view.FindViewById<EditText>(Resource.Id.editTextIP3);
			mIpField4 = view.FindViewById<EditText>(Resource.Id.editTextIP4);
			//list of ipfield for easy checking
			ipFields = new List<EditText>(){mIpField1, mIpField2, mIpField3, mIpField4};

			mPortField = view.FindViewById<EditText>(Resource.Id.editTextPort);
			mConnection_Text = view.FindViewById<TextView>(Resource.Id.Connection_Text);

			//Connection Event Handlers
			mConnectionButton.Click += delegate {
				int tempIntContainer;
				//Check if all field of the ip are populated
				for(int i = 0; i < ipFields.Count; i++)
				{
					if(ipFields[i].Text == "")
					{
						ipFields[i].Text = "0";
					}
				}
				//set Global IpAddress variable equal to the text in the ipField
				GlobalVariables.IPAddress = string.Format("{0}.{1}.{2}.{3}",mIpField1.Text, mIpField2.Text, mIpField3.Text, mIpField4.Text);
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

