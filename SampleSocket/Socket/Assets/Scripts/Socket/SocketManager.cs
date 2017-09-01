using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System;
using System.Text;
using System.IO;
using Google.Protobuf;



public class SocketManager
{
	private static SocketManager instance;

	private string currIP;
	private int currPort;

	private bool isConnected = false;

	public bool IsConnceted { get { return isConnected; } }

	private Socket clientSocket = null;
	private Thread receiveThread = null;
	private Thread sendThread = null;

	byte[] revBuff = new byte[2048];

	private SocketBuff socketBuff = new SocketBuff();

	public static SocketManager Instance {
		get {
			if (instance == null) {
				instance = new SocketManager ();
			}
			return instance;
		}
	}


	private void ReConnect ()
	{
	}

	/// <summary>
	/// 连接
	/// </summary>
	private void onConnet ()
	{
		try {
			clientSocket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//创建套接字
			IPAddress ipAddress = IPAddress.Parse (currIP);//解析IP地址
			IPEndPoint ipEndpoint = new IPEndPoint (ipAddress, currPort);
			IAsyncResult result = clientSocket.BeginConnect (ipEndpoint, new AsyncCallback (onConnectSucess), clientSocket);//异步连接
			bool success = result.AsyncWaitHandle.WaitOne (5000, true);
			if (!success) { //超时
				onConnectOuttime ();
			}
		} catch (System.Exception e) {
			onConnectFail ();
		}
	}

	private void onConnectSucess (IAsyncResult iar)
	{
		try {
			Socket client = (Socket)iar.AsyncState;
			client.EndConnect (iar);
			isConnected = true;

			sendThread = new Thread (new ThreadStart (onSendSocket));
			sendThread.IsBackground = true;
			sendThread.Start ();

			receiveThread = new Thread (new ThreadStart (onReceiveSocket));
			receiveThread.IsBackground = true;
			receiveThread.Start ();
			Debug.Log ("连接成功");
		} catch (Exception exp) {
			Debug.Log("connect result ,create thread error:"+exp);

			Close ();
		}
	}

	private void onConnectOuttime ()
	{
		close ();
	}

	private void onConnectFail ()
	{
		close ();
	}

	private void onSendSocket ()
	{
		while (true) {
			if (!clientSocket.Connected) {
				isConnected = false;
				ReConnect ();
				break;
			}
			lock (MessageCenter.Instance.sendMsgQueue) {
				while (MessageCenter.Instance.sendMsgQueue.Count > 0) {
					MsgData md = MessageCenter.Instance.sendMsgQueue.Dequeue ();

					RealSend (md.ToBytes());
				}
			}
		}
	}

	/// <summary>
	/// 接受网络数据
	/// </summary>
	private void onReceiveSocket ()
	{
		while (true) {
			if (!clientSocket.Connected) {
				isConnected = false;
				ReConnect ();
				break;
			}
			try {
				int receiveLen = clientSocket.Receive (revBuff);
				if (receiveLen <= 0) {
					Debug.Log ("接收到的数据长度位0");
					continue;
				}

				socketBuff.WriteBuff(this.revBuff,receiveLen);

				MsgData md;
				while(socketBuff.ReadMsg(out md)){
					// 锁死消息中心消息队列，并添加数据
                    lock (MessageCenter.Instance.netMessageDataQueue)
					{
						MessageCenter.Instance.netMessageDataQueue.Enqueue(md);
                    }
				}
				
			} catch (System.Exception e) {
				Debug.Log ("receve exception:" + e);
				clientSocket.Disconnect (true);
				clientSocket.Shutdown (SocketShutdown.Both);
				clientSocket.Close ();
				break;
			}
		}
	}


	/// <summary>
	/// 连接服务器
	/// </summary>
	/// <param name="currIP"></param>
	/// <param name="currPort"></param>
	public void Connect (string currIP, int currPort)
	{
		if (!IsConnceted) {
			this.currIP = currIP;
			this.currPort = currPort;
			onConnet ();
		}
	}

	public void SendMsg (short msgID, IMessage data)
	{
		MsgData sd = new MsgData (msgID, data);
		lock (MessageCenter.Instance.sendMsgQueue) {
			MessageCenter.Instance.sendMsgQueue.Enqueue (sd);
		}
	}

	public void RealSend (byte[] data)
	{
		if (clientSocket == null || !clientSocket.Connected) {
			ReConnect ();
			return;
		}
		clientSocket.BeginSend (data, 0, data.Length, SocketFlags.None, new AsyncCallback (onSendMsg), clientSocket);
	}

	/// <summary>
	/// 发送消息结果回掉，可判断当前网络状态
	/// </summary>
	/// <param name="asyncSend"></param>
	private void onSendMsg (IAsyncResult asyncSend)
	{
		try {
			Socket client = (Socket)asyncSend.AsyncState;
			client.EndSend (asyncSend);
			Debug.Log("send sucess");
		} catch (Exception e) {
			Debug.Log ("send msg exception:" + e.StackTrace);
		}
	}

	public void Close ()
	{
		close ();
	}


	/// <summary>
	/// 断开
	/// </summary>
	private void close ()
	{
		Debug.Log ("===================colse");
		if (!isConnected) {
			return;
		}

		isConnected = false;

		if (sendThread != null) {
			sendThread.Abort ();
			sendThread.Join ();
			sendThread = null;
		}

		if (receiveThread != null) {
			receiveThread.Abort ();
			receiveThread.Join ();
			receiveThread = null;
		}

		if (clientSocket != null && clientSocket.Connected) {
			clientSocket.Close ();
			clientSocket = null;
		}
	}

}