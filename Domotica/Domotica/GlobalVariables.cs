using System;

//This class serves the purpose of containing variables needed everywhere in the project.
namespace Domotica
{
	public static class GlobalVariables
	{
		static string _IPAddress;
		static int _PortAddress;
		static bool _IpAvailable;

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
	}
}

