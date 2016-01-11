using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
//Using a SupportToolbar alias since another toolbar type already exists
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
//Added external support libraries v4 & v7 for toolbar/navigation drawer
using Android.Support.V7.App;
using Android.Support.V4.Widget;
//Using a SupportFragment alias since another fragment type already exists
using SupportFragment = Android.Support.V4.App.Fragment;
//Using alias since actionbardrawertoggle type already exists
using SupportActionBarDrawerToggle = Android.Support.V7.App.ActionBarDrawerToggle;

namespace Domotica
{
	//ConfigurationChanges tells the app to not destroy it's view when orientation is changed. so preventing the app from reloading completeley every time the orientation is changed
	[Activity (Label = "Domotica", MainLauncher = true, ConfigurationChanges = ( Android.Content.PM.ConfigChanges.Orientation |Android.Content.PM.ConfigChanges.ScreenSize ) ,Icon = "@mipmap/icon", Theme="@style/MyTheme")]
	public class MainActivity : AppCompatActivity
	{
		//UI Variables
		//Variable linking to the supporttoolbar from android.support.v7.widget.toolbar
		private SupportToolbar mToolbar;
		//Class for controling the toggle of the navigation drawer
		private ActionBarDrawerToggle mDrawerToggle;
		private DrawerLayout mDrawerLayout;
		//List view that contains the items of the navigation drawer
		private ListView mDrawer;
		//Adaper for using an array of strings to populate the listview
		private ArrayAdapter mAdapter;
		//List for the strings that are to populate the listview
		private List<string> mDrawerData;
		//Keeps track of what fragment is currently shown
		private SupportFragment mCurrentFragment;
		//Fragments.
		private Home mHome;
		private Switches1 mSwitches1;
		private Switches2 mSwitches2;
		private Sensors1 mSensors1;
		private Sensors2 mSensors2;
		private Connection1 mConnection1;
		private Mode1 mMode1;
		private FAQ mFAQ;

		//Stack to keep track of what fragments have been shown. 
		//Is used for the back button
		private Stack<SupportFragment> mStackFragment;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			//Ui Items
			mToolbar = FindViewById<SupportToolbar> (Resource.Id.toolbar);
			mDrawerLayout = FindViewById<DrawerLayout> (Resource.Id.drawer_layout);
			mDrawer = FindViewById<ListView> (Resource.Id.left_drawer);


			//Create fragments
			mHome = new Home();
			mSwitches1 = new Switches1 ();
			mSwitches2 = new Switches2 ();
			mSensors1 = new Sensors1 ();
			mSensors2 = new Sensors2 ();
			mConnection1 = new Connection1 ();
			mMode1 = new Mode1 ();
			mFAQ = new FAQ ();

			//create stack
			mStackFragment = new Stack<SupportFragment> ();

			//Create Toolbar
			SetSupportActionBar (mToolbar);

			SupportActionBar.SetHomeButtonEnabled(true);
			SupportActionBar.SetDisplayHomeAsUpEnabled(true);﻿

			//Create All Fragments and hide them
			//Hiding is to later be able to call the fragment to the front
			//They are basicly all hiding in the background till you call them
			//These stack. so the fragment on the bottom of the stack is added first
			var trans = SupportFragmentManager.BeginTransaction ();

			trans.Add (Resource.Id.fragmentContainter, mFAQ, "FAQ");
			trans.Hide (mFAQ);

			trans.Add (Resource.Id.fragmentContainter, mMode1, "Mode1");
			trans.Hide (mMode1);

			trans.Add (Resource.Id.fragmentContainter, mConnection1, "Connection1");
			trans.Hide (mConnection1);

			trans.Add (Resource.Id.fragmentContainter, mSensors2, "Sensors2");
			trans.Hide (mSensors2);

			trans.Add (Resource.Id.fragmentContainter, mSensors1, "Sensors1");
			trans.Hide (mSensors1);

			trans.Add (Resource.Id.fragmentContainter, mSwitches2, "Switches2");
			trans.Hide(mSwitches2);

			trans.Add (Resource.Id.fragmentContainter, mSwitches1, "Switches1");
			trans.Hide (mSwitches1);

			trans.Add (Resource.Id.fragmentContainter, mHome, "Home");

			trans.Commit ();

			//Initial shown fragment is the Home fragment
			mCurrentFragment = mHome;

			//Set Strings to populate the listview of the navigation drawer
			mDrawerData = new List<string> () {"Home", "Switches", "Sensors", "Sensor Threshold", "Timers","Connection", "Modes", "FAQ"};
			//Set adaper to show these in the listview. The resource.layout.mytextview is the place that the adapter uses to create each list item
			mAdapter = new ArrayAdapter<string> (this, Resource.Layout.mytextview, mDrawerData);
			//set adapter drawer to the one just created
			mDrawer.Adapter = mAdapter;
			//Enable DrawerToggle
			mDrawerToggle = new ActionBarDrawerToggle (
				this,
				mDrawerLayout,
				Resource.String.openDrawer,
				Resource.String.closeDrawer
			);

			//Set listener for the event of the drawertoggle being clicked
			mDrawerLayout.SetDrawerListener (mDrawerToggle);
			SupportActionBar.SetHomeButtonEnabled (true);
			SupportActionBar.SetDisplayShowTitleEnabled (true);
			//sync state of drawer with the drawertoggle
			mDrawerToggle.SyncState ();

			//if this is the frist time the view is created set title on the toolbar to Home since this this the fragment you're greeted with
			if (savedInstanceState != null)
			{
			}
			else
			{
				SupportActionBar.SetTitle (Resource.String.Home);	
			}

			//Event handler
			//When event mDrawer.ItemClick is executed. run method MDrawer_ItemClick
			mDrawer.ItemClick += MDrawer_ItemClick;

		}

		//Get input from drawertoggle
		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			mDrawerToggle.OnOptionsItemSelected (item);
			return base.OnOptionsItemSelected (item);
		}

		//Synch drawertoggle with drawerstate before creation
		protected override void OnPostCreate (Bundle savedInstanceState)
		{
			base.OnPostCreate (savedInstanceState);
			mDrawerToggle.SyncState ();
		}

		//if back button is pressed get last seen fragment from the stack
		public override void OnBackPressed ()
		{
			if (SupportFragmentManager.BackStackEntryCount > 0) 
			{
				SupportFragmentManager.PopBackStack ();
				mCurrentFragment = mStackFragment.Pop ();
			} 
			else 
			{
				base.OnBackPressed ();
			}
		}

		//Method to be executed when an item in the drawer is clicked
		void MDrawer_ItemClick (object sender, AdapterView.ItemClickEventArgs e)
		{
			//get position of the item that is clicked and change fragment and title accoardingly
			switch(e.Position)
			{
				case 0:
					changeFragment(mHome);
					SupportActionBar.SetTitle (Resource.String.Home);
					break;
				case 1:
					changeFragment(mSwitches1);
					SupportActionBar.SetTitle (Resource.String.Switches1);	
					break;
				case 2:
					changeFragment(mSensors1);
					SupportActionBar.SetTitle (Resource.String.Sensors1);	
					break;
				case 3:
					changeFragment(mSensors2);
					SupportActionBar.SetTitle (Resource.String.Sensors2);	
					break;
				case 4:
					changeFragment(mSwitches2);
					SupportActionBar.SetTitle (Resource.String.Switches2);
					break;
				case 5:
					changeFragment(mConnection1);
					SupportActionBar.SetTitle (Resource.String.Connection);
					break;
				case 6:
					changeFragment(mMode1);
					SupportActionBar.SetTitle (Resource.String.Mode);
					break;
				case 7:
					changeFragment (mFAQ);
					SupportActionBar.SetTitle (Resource.String.FAQ);
					break;
			}	
		}

		//Change Shown fragment and hide current
		private void changeFragment(SupportFragment fragment1)
		{
			if (fragment1 != mCurrentFragment) 
			{
				var trans = SupportFragmentManager.BeginTransaction ();
				trans.SetCustomAnimations (Resource.Animation.first_slide_in, Resource.Animation.first_slide_out, Resource.Animation.second_slide_in, Resource.Animation.second_slide_out);
				trans.Hide (mCurrentFragment);
				trans.Show (fragment1);
				trans.AddToBackStack (null);
				trans.Commit ();

				mStackFragment.Push (mCurrentFragment);
				mCurrentFragment = fragment1;
			}
			mDrawerLayout.CloseDrawer (mDrawer);
		}
	}
}


