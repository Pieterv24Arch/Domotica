using System;

//This class serves the purpose of containing variables needed everywhere in the project.
namespace Domotica
{
	public static class GlobalVariables
	{
		static string _IPAddress;
		static int _PortAddress;
		static bool _IpAvailable;
		static int _Sensor1Value;
		static int _Sensor2Value;
		static bool _Sensor1Get;
		static bool _Sensor2Get;

		public static string IPAddress
		{
			get {
				return _IPAddress;
			}
			set {
				_IPAddress = value;
			}
		}

		public static int PortAddress
		{
			get {
				return _PortAddress;
			}
			set {
				_PortAddress = value;
			}
		}

		public static bool IpAvailable
		{
			get {
				return _IpAvailable;
			}
			set {
				_IpAvailable = value;
			}
		}

		public static int Sensor1Value
		{
			get {
				return _Sensor1Value;
			}
			set {
				_Sensor1Value = value;
			}
		}

		public static int Sensor2Value
		{
			get {
				return _Sensor2Value;
			}
			set {
				_Sensor2Value = value;
			}
		}

		public static bool Sensor1Get
		{
			get {
				return _Sensor1Get;
			}
			set {
				_Sensor1Get = value;
			}
		}

		public static bool Sensor2Get
		{
			get {
				return _Sensor2Get;
			}
			set {
				_Sensor2Get = value;
			}
		}
	}
}

