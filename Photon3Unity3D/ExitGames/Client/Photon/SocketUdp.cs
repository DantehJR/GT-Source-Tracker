using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ExitGames.Client.Photon;

public class SocketUdp : IPhotonSocket, IDisposable
{
	private Socket sock;

	private readonly object syncer = new object();

	[Preserve]
	public SocketUdp(PeerBase npeer)
		: base(npeer)
	{
		if (ReportDebugOfLevel(DebugLevel.INFO))
		{
			Listener.DebugReturn(DebugLevel.INFO, "SocketUdp, .Net, Unity.");
		}
		PollReceive = false;
	}

	~SocketUdp()
	{
		Dispose();
	}

	public void Dispose()
	{
		State = PhotonSocketState.Disconnecting;
		if (sock != null)
		{
			try
			{
				if (sock.Connected)
				{
					sock.Close(1);
				}
			}
			catch (Exception ex)
			{
				EnqueueDebugReturn(DebugLevel.INFO, "Exception in Dispose(): " + ex);
			}
		}
		sock = null;
		State = PhotonSocketState.Disconnected;
	}

	public override bool Connect()
	{
		lock (syncer)
		{
			if (!base.Connect())
			{
				return false;
			}
			State = PhotonSocketState.Connecting;
		}
		Thread thread = new Thread(DnsAndConnect);
		thread.IsBackground = true;
		thread.Start();
		return true;
	}

	public override bool Disconnect()
	{
		if (ReportDebugOfLevel(DebugLevel.INFO))
		{
			EnqueueDebugReturn(DebugLevel.INFO, "SocketUdp.Disconnect()");
		}
		lock (syncer)
		{
			State = PhotonSocketState.Disconnecting;
			if (sock != null)
			{
				try
				{
					sock.Close(1);
				}
				catch (Exception ex)
				{
					if (ReportDebugOfLevel(DebugLevel.INFO))
					{
						EnqueueDebugReturn(DebugLevel.INFO, "Exception in Disconnect(): " + ex);
					}
				}
			}
			State = PhotonSocketState.Disconnected;
		}
		return true;
	}

	public override PhotonSocketError Send(byte[] data, int length)
	{
		try
		{
			if (sock == null || !sock.Connected)
			{
				return PhotonSocketError.Skipped;
			}
			sock.Send(data, 0, length, SocketFlags.None);
		}
		catch (SocketException ex)
		{
			if (ex.SocketErrorCode == SocketError.WouldBlock)
			{
				return PhotonSocketError.Busy;
			}
			if (State == PhotonSocketState.Disconnecting || State == PhotonSocketState.Disconnected)
			{
				return PhotonSocketError.Exception;
			}
			SocketErrorCode = (int)ex.SocketErrorCode;
			if (ReportDebugOfLevel(DebugLevel.INFO))
			{
				string text = "";
				if (sock != null)
				{
					text = string.Format(" Local: {0} Remote: {1} ({2}, {3})", sock.LocalEndPoint, sock.RemoteEndPoint, sock.Connected ? "connected" : "not connected", sock.IsBound ? "bound" : "not bound");
				}
				EnqueueDebugReturn(DebugLevel.INFO, string.Format("Cannot send to: {0}. Uptime: {1} ms. {2} {3}\n{4}", ServerAddress, peerBase.timeInt, AddressResolvedAsIpv6 ? " IPv6" : string.Empty, text, ex));
			}
			HandleException(StatusCode.SendError);
			return PhotonSocketError.Exception;
		}
		catch (Exception ex2)
		{
			if (State == PhotonSocketState.Disconnecting || State == PhotonSocketState.Disconnected)
			{
				return PhotonSocketError.Exception;
			}
			if (ReportDebugOfLevel(DebugLevel.INFO))
			{
				string text2 = "";
				if (sock != null)
				{
					text2 = string.Format(" Local: {0} Remote: {1} ({2}, {3})", sock.LocalEndPoint, sock.RemoteEndPoint, sock.Connected ? "connected" : "not connected", sock.IsBound ? "bound" : "not bound");
				}
				EnqueueDebugReturn(DebugLevel.INFO, string.Format("Cannot send to: {0}. Uptime: {1} ms. {2} {3}\n{4}", ServerAddress, peerBase.timeInt, AddressResolvedAsIpv6 ? " IPv6" : string.Empty, text2, ex2));
			}
			if (!sock.Connected)
			{
				EnqueueDebugReturn(DebugLevel.INFO, "Caught Exception in Send(). Ending connection with StatusCode.SendError.");
				HandleException(StatusCode.SendError);
			}
			return PhotonSocketError.Exception;
		}
		return PhotonSocketError.Success;
	}

	public override PhotonSocketError Receive(out byte[] data)
	{
		data = null;
		return PhotonSocketError.NoData;
	}

	internal void DnsAndConnect()
	{
		IPAddress[] ipAddresses = GetIpAddresses(ServerAddress);
		if (ipAddresses == null)
		{
			return;
		}
		string text = string.Empty;
		IPAddress[] array = ipAddresses;
		foreach (IPAddress iPAddress in array)
		{
			try
			{
				sock = new Socket(iPAddress.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
				sock.Blocking = false;
				sock.Connect(iPAddress, ServerPort);
				if (sock != null && sock.Connected)
				{
					break;
				}
			}
			catch (SocketException ex)
			{
				if (ReportDebugOfLevel(DebugLevel.WARNING))
				{
					text = text + ex?.ToString() + " " + ex.ErrorCode + "; ";
					EnqueueDebugReturn(DebugLevel.WARNING, "SocketException caught: " + ex?.ToString() + " ErrorCode: " + ex.ErrorCode);
				}
			}
			catch (Exception ex2)
			{
				if (ReportDebugOfLevel(DebugLevel.WARNING))
				{
					text = text + ex2?.ToString() + "; ";
					EnqueueDebugReturn(DebugLevel.WARNING, "Exception caught: " + ex2);
				}
			}
		}
		if (sock == null || !sock.Connected)
		{
			if (ReportDebugOfLevel(DebugLevel.ERROR))
			{
				EnqueueDebugReturn(DebugLevel.ERROR, "Failed to connect to server after testing each known IP. Error(s): " + text);
			}
			HandleException(StatusCode.ExceptionOnConnect);
		}
		else
		{
			AddressResolvedAsIpv6 = sock.AddressFamily == AddressFamily.InterNetworkV6;
			IPhotonSocket.ServerIpAddress = sock.RemoteEndPoint.ToString();
			State = PhotonSocketState.Connected;
			peerBase.OnConnect();
			Thread thread = new Thread(ReceiveLoop);
			thread.IsBackground = true;
			thread.Start();
		}
	}

	public void ReceiveLoop()
	{
		byte[] array = new byte[MTU];
		while (State == PhotonSocketState.Connected)
		{
			try
			{
				if (sock.Poll(5000, SelectMode.SelectRead))
				{
					int length = sock.Receive(array);
					HandleReceivedDatagram(array, length, willBeReused: true);
				}
			}
			catch (SocketException ex)
			{
				if (State != PhotonSocketState.Disconnecting && State != PhotonSocketState.Disconnected)
				{
					if (ReportDebugOfLevel(DebugLevel.ERROR))
					{
						EnqueueDebugReturn(DebugLevel.ERROR, "Receive issue. State: " + State.ToString() + ". Server: '" + ServerAddress + "' ErrorCode: " + ex.ErrorCode + " SocketErrorCode: " + ex.SocketErrorCode.ToString() + " Message: " + ex.Message + " " + ex);
					}
					SocketErrorCode = (int)ex.SocketErrorCode;
					HandleException(StatusCode.ExceptionOnReceive);
				}
			}
			catch (Exception ex2)
			{
				if (State != PhotonSocketState.Disconnecting && State != PhotonSocketState.Disconnected)
				{
					if (ReportDebugOfLevel(DebugLevel.ERROR))
					{
						EnqueueDebugReturn(DebugLevel.ERROR, "Receive issue. State: " + State.ToString() + ". Server: '" + ServerAddress + "' Message: " + ex2.Message + " Exception: " + ex2);
					}
					HandleException(StatusCode.ExceptionOnReceive);
				}
			}
		}
		lock (syncer)
		{
			if (State != PhotonSocketState.Disconnecting && State != PhotonSocketState.Disconnected)
			{
				Disconnect();
			}
		}
	}
}
