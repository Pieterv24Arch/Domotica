using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Widget;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.App;

namespace Domotica
{
	public class TimerListAdapter : BaseAdapter<TimerItem>
	{
		List<TimerItem> mItems;
		Context mContext;

		public TimerListAdapter (Context context, List<TimerItem> items)
		{
			mItems = items;
			mContext = context;
		}

		public override int Count {
			get {
				return mItems.Count;
			}
		}

		public override long GetItemId (int position)
		{
			return position;
		}

		public override TimerItem this[int index] {
			get {
				return mItems [index];
			}
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			View row = convertView;
			if (row == null)
			{
				row = LayoutInflater.From (mContext).Inflate (Resource.Layout.TimerItemView, null, false);
			}

			TextView TimeField = row.FindViewById<TextView> (Resource.Id.Time);
			TextView SwitchField = row.FindViewById<TextView> (Resource.Id.timeSwitch);
			TextView SwitchStateField = row.FindViewById<TextView> (Resource.Id.timeSwitchState);

			string tempTimeString = String.Format("{0}:{1}", mItems [position].mTime.Hour, mItems [position].mTime.Minute);
			TimeField.Text = tempTimeString;
			SwitchField.Text = mItems [position].mSwitch;
			SwitchStateField.Text = (mItems [position].mSwitchState ? "True" : "False");

			return row;
		}
	}
}

