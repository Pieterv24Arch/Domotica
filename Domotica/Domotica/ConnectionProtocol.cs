
//This class is for the the handling of connectin with the arduino

using System;
//An alias is used to prevent conflict between the socetclass of system.net and java.net
using SystemSocket = System.Net.Sockets.Socket;
using System.Net.Sockets;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using Android.Widget;
using Android.App;

namespace Domotica
{
	public class ConnectionProtocol
	{
		public ConnectionProtocol ()
		{
		}
		//Test if valid connection to arduino is available by pinging it
		//Result is written to global variable
		public void TestConnection(TextView text)
		{
			Ping p = new Ping ();
			PingReply reply = p.Send (GlobalVariables.IPAddress);
			GlobalVariables.IpAvailable = (reply.Status == IPStatus.Success);
		}

		//Start socket connection
		public SystemSocket open()
		{
			SystemSocket socket = new SystemSocket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			IPAddress ip = IPAddress.Parse (GlobalVariables.IPAddress);
			IPEndPoint endpoint = new IPEndPoint (ip, GlobalVariables.PortAddress);
			socket.Connect (endpoint);
			return socket;
		}

		//Encode and send message over socet port
		public void write(SystemSocket socket, string text)
		{
			socket.Send(Encoding.ASCII.GetBytes(text));
		}

		//Read incomming socket messages
		public string read(SystemSocket socket)
		{
			byte[] bytes = new byte[4096];
			int bytesRec = socket.Receive(bytes);
			string text = Encoding.ASCII.GetString(bytes, 0, bytesRec);
			return text;
		}

		//Close Socket connection
		public void close(SystemSocket socket)
		{
			socket.Close();
		}

		//tell arduino what to do without expecting a response
		//The try/catch is used to prevent an error resulting in the app crashing if no connection is available
		public void tell(string message)
		{
			try {
				SystemSocket s = open ();
				write (s, message);
				close (s);
			}
			catch {
				//if error is caught set ipavailable to false since a lot of actions depend on this being true to execute their actions
				GlobalVariables.IpAvailable = false;
			}
		}

		//tell arduino what to return
		public string ask(string message)
		{
			try {
				SystemSocket s = open ();
				write (s, message);
				string awnser = read (s);
				close (s);
				return awnser;
			}
			catch {
				GlobalVariables.IpAvailable = false;
				return "null";
			}
		}
	}
}

