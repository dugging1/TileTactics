using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TileTactics.Network {
	/// <summary>
	/// Used for passing relevent data across async methods
	/// </summary>
	public class StateObject {
		//Client socket
		public Socket workSocket;
		public const int bufferSize = 2048;
		public byte[] buffer = new byte[bufferSize];
		public int messageLength = -1;
		public int recieved;
		public List<byte> content = new List<byte>();
	}
}
