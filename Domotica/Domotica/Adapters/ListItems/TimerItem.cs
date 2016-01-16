using System;
using Android.Text.Format;

namespace Domotica
{
	public class TimerItem
	{
		public DateTime mTime;
		public String mSwitch;
		public bool mSwitchState;

		public TimerItem (DateTime setTime, string setSwitch, bool setSwitchState)
		{
			mTime = setTime;
			mSwitch = setSwitch;
			mSwitchState = setSwitchState;
		}
	}
}

