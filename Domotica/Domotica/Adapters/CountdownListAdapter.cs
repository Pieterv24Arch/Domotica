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
	public class CountdownListAdapter : BaseAdapter<CountdownItem>
	{
		private List<CountdownItem> mItems;
		private Context mContext;

		public CountdownListAdapter (Context context, List<CountdownItem> items)
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

		public override CountdownItem this[int index] {
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

			return row;
		}
	}
}

