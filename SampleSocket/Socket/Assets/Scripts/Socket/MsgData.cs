using UnityEngine;
using System.Collections;
using System.IO;  
using System;  
using System.Text;  
using Google.Protobuf;
using System.Net;
public class MsgData
{
	public byte[] data;
	public short msgID;
	public short msgLen;

	public MsgData(short msgID,IMessage protoData){
		using (MemoryStream memoryStream = new MemoryStream ()) {
			//Serializer.Serialize (memoryStream, ptotoData);
			//CS_LOGINSERVER.Parser.ParseFrom(protoData);
			protoData.WriteTo(memoryStream);
			data = memoryStream.ToArray ();
			this.msgLen = (short)(data.Length + 2);
			this.msgID = msgID;
		}
	}

	public MsgData(short msgID,byte[] bytes){
		this.msgID = msgID;
		this.data = bytes;
		this.msgLen = (short)(bytes.Length + 2);
	}

	public byte[] ToBytes(){
		using (MemoryStream memoryStream = new MemoryStream ()) {
			BinaryWriter binaryWriter = new BinaryWriter (memoryStream);  
			memoryStream.Seek (0, SeekOrigin.Begin);  
			binaryWriter.Write (msgLen);
			memoryStream.Seek (2, SeekOrigin.Begin);
			binaryWriter.Write (msgID);
			memoryStream.Seek (4, SeekOrigin.Begin);
			binaryWriter.Write (data);

//			binaryWriter.Seek (msgLen, SeekOrigin.Begin);
//			binaryWriter.Write ('\0');
			return memoryStream.ToArray();
		}
	}

	public void appendData(byte[] leftData, int len){
		Array.Copy (leftData, data, len);
	}

	private short convertToLittle(short num){
		byte[] bytes = BitConverter.GetBytes (num);
		Array.Reverse (bytes);
		Debug.Log ("----------little:"+IPAddress.NetworkToHostOrder(num)+"-" + BitConverter.ToInt16 (bytes, 0)+"="+num); //014  410
		foreach(byte k in bytes){
			Debug.Log ("key:"+ k);
		}
		return BitConverter.ToInt16 (bytes,0);
	}

}

