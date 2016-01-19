using System;
using Android.Text.Format;

namespace Domotica
{
	public class TimerItem
	{
		public DateTime mTime;
		public String mSwitch;
		public int mSwitchi;
		public bool mSwitchState;
		public int mSwitchStatei;

		public TimerItem (DateTime setTime, string setSwitch, int setSwitchi, bool setSwitchState, int setSwitchStatei)
		{
			mTime = setTime;
			mSwitch = setSwitch;
			mSwitchi = setSwitchi;
			mSwitchState = setSwitchState;
			mSwitchStatei = setSwitchStatei;
		}
	}
}

