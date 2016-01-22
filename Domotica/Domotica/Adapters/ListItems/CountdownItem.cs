using System;
using Android.Text.Format;

namespace Domotica
{
	public class CountdownItem
	{
		public DateTime mTime;
		public String mSwitch;
		public int mSwitchi;
		public bool mSwitchState;
		public int mSwitchStatei;

		public CountdownItem (DateTime setTime, string setSwitch, int setSwitchi, bool setSwitchState, int setSwitchStatei)
		{
			mTime = setTime;
			mSwitch = setSwitch;
			mSwitchi = setSwitchi;
			mSwitchState = setSwitchState;
			mSwitchStatei = setSwitchStatei;
		}
	}
}

