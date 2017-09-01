using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System;
using System.Text;
using System.IO;
using Google.Protobuf;

public class SocketBuff
{
	private byte[] buff;
	private int buffLen;
	private int pos = 0;
	private short msgLen=0;
	private short msgID=0;
	private MsgData msgData;

	public SocketBuff(int buffLen = 2048)
	{
		if (buffLen <= 0)
		{
			this.buffLen = 2048;
		}else{
			this.buffLen = buffLen;
		}
		this.buff = new byte[this.buffLen];
	}

	public void WriteBuff(byte[] data,int len){

		Debug.Log ("writebuff len:" + len +" pos:"+pos);
		if (len > buffLen) {
			byte[] tmp = new byte[pos+len];
			Array.Copy (this.buff, 0, tmp, 0, pos);
			Array.Copy (data, 0, tmp, pos, len);
			this.buff = tmp;
		} else {
			Array.Copy (data, 0, this.buff, pos, len);
		}
		this.pos += len;
	}

	// 一次度一条消息，如果不够就返回true，缓存消息长度做判断
	public bool ReadMsg(out MsgData md){			
		md = null;
		if (this.pos == 0) {
			return false;
		}
		Debug.Log("begin read msg"+ msgLen);

		if (this.msgLen <= 0) {
			this.msgLen = BitConverter.ToInt16 (this.buff, 0);
			this.msgID = BitConverter.ToInt16 (this.buff, 2);
		}
		Debug.Log ("this.msgLen:" + this.msgLen + " " + this.msgID);
		if (msgLen > pos) {
			return false;
		}
		byte[] tmp = new byte[msgLen - 2];
		Array.Copy (this.buff, 4, tmp, 0, msgLen - 2);
		msgData = new MsgData (msgID, tmp);
		md = msgData;
		tmp = new byte[this.buffLen];
		int left = this.pos - (this.msgLen + 2);

		if (left > 0) {
			Array.Copy (this.buff, this.msgLen, tmp, 0, left);
		}
		this.buff = tmp;
		this.pos = this.pos - this.msgLen - 2; //消息长度+消息长度自己2个字节
		this.msgLen = 0;
		this.msgID = 0;

		return true;
	}

}

