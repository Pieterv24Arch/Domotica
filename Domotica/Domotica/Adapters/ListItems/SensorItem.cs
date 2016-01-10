using System;

namespace Domotica
{
	public class SensorItem
	{
		public string mSensorName;
		public int mSensorNamei;
		public string mRelation;
		public int mRealationi;
		public int mValue;
		public string mSwitchIdentity;
		public int mSwitchIdentityi;

		public SensorItem (string sensorName, int sensorNamei, string relation, int relationi, int value, string switchIdentity, int switchIdentityi)
		{
			mSensorName = sensorName;
			mSensorNamei = sensorNamei;
			mRelation = relation;
			mRealationi = relationi;
			mValue = value;
			mSwitchIdentity = switchIdentity;
			mSwitchIdentityi = switchIdentityi;
		}
	}
}

