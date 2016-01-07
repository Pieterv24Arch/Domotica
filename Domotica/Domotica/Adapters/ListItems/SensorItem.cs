using System;

namespace Domotica
{
	public class SensorItem
	{
		public string mSensorName;
		public string mRelation;
		public int mValue;
		public string mSwitchIdentity;

		public SensorItem (string sensorName, string relation, int value, string switchIdentity)
		{
			mSensorName = sensorName;
			mRelation = relation;
			mValue = value;
			mSwitchIdentity = switchIdentity;
		}
	}
}

