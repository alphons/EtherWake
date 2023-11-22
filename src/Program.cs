using System.Net;
using System.Net.Sockets;

/*
 * Author: Alphons van der Heijden
 * Date: nov 11, 2023
 * Broadcasting WOL (Wake-On-Lan) packet 0x0842 https://en.wikipedia.org/wiki/EtherType
 * Payload is mac address
 * 
 * .net core 8
 */

var Mac = "DE:AD:BE:EF:12:34";

if(args.Length == 0)
{
	Console.WriteLine($"Usage: EtherWake {Mac}");
	return;
}

if (args.Length>0)
	Mac = args[0];

var client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

await client.ConnectAsync(IPAddress.Broadcast, 0); // random port

client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);

byte[] broadcast = [ 0xff, 0xff, 0xff, 0xff, 0xff, 0xff ];

var macaddress = Mac.Split(':').Select(x => Convert.ToByte(x, 16)).ToArray();

byte[] ethertype = [ 0x08, 0x42 ];

var arrays = new List<byte[]> { broadcast, macaddress, ethertype, broadcast  };

for(int i=0;i<16;i++)
	arrays.Add(macaddress);

var buffer = arrays.SelectMany(s => s).ToArray();

int len = await client.SendAsync(buffer);

var success = len == buffer.Length;

Console.WriteLine($"sended WOL {success}");

