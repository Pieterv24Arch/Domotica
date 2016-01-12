using System;

namespace Domotica
{
	public class MySupportFragment : Android.Support.V4.App.Fragment
	{
		int _title;
		public int title 
		{ 
			get{return _title;}
			protected set{ _title = value; }
		}

		public MySupportFragment ()
		{
			
		}

		public override void OnCreate (Android.OS.Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
		}
	}
}

