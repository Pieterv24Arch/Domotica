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
	public class SensorListAdapter : BaseAdapter<SensorItem>
	{
		private List<SensorItem> mItems;
		private Context mContext;

		public SensorListAdapter (Context context, List<SensorItem> items)
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

		public override SensorItem this[int index] {
			get {
				return mItems [index];
			}
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			View row = convertView;
			if (row == null)
			{
				row = LayoutInflater.From (mContext).Inflate (Resource.Layout.SensorItemView, null, false);
			}

			TextView mSensorName = row.FindViewById<TextView> (Resource.Id.SensorName);
			TextView mSensorThresholdRelation = row.FindViewById<TextView> (Resource.Id.SensorThresholdRelation);
			TextView mSensorThreshold = row.FindViewById<TextView> (Resource.Id.SensorThreshold);
			TextView mSensorSwitchControl = row.FindViewById<TextView> (Resource.Id.SensorSwitchControl);

			mSensorName.Text = mItems [position].mSensorName;
			mSensorThresholdRelation.Text = mItems [position].mRelation;
			switch (mItems [position].mRelation)
			{
				case "Lower Than":
					mSensorThresholdRelation.Text = "<";
					break;
				case "Greater Than":
					mSensorThresholdRelation.Text = ">";
					break;
			}
			mSensorThreshold.Text = Convert.ToString(mItems [position].mValue);
			mSensorSwitchControl.Text = mItems [position].mSwitchIdentity;

			return row;
		}
	}
}

