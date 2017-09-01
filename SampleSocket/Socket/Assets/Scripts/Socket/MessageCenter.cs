/// <summary>
/// 网络消息处理中心
/// 
/// create at 2014.8.26 by sun
/// </summary>


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//public struct GameEvtData
//{
//    public GameEvtType eventType;
//    public object data;
//}

public struct NetEvtData
{
    public int msgID;
    public byte[] data;
}

//public delegate void GameEvtHandle(object data);
public delegate void NetEvtHandle(byte[] data);

public class MessageCenter : SingletonMonoBehaviour<MessageCenter>
{
    private Dictionary<int, NetEvtHandle> netMessageEventList = new Dictionary<int, NetEvtHandle>();
	public Queue<MsgData> netMessageDataQueue = new Queue<MsgData>();
	public Queue<MsgData> sendMsgQueue = new Queue<MsgData>();

//    private Dictionary<GameEvtType, GameEvtHandle> gameLogicEventList = new Dictionary<GameEvtType, GameEvtHandle>();
//    public Queue<GameEvtData> gameLogicDataQueue = new Queue<GameEvtData>();
//
    //添加网络事件观察者
    public void addObsever(int protocalType, NetEvtHandle callback)
    {
        if (netMessageEventList.ContainsKey(protocalType))
        {
            netMessageEventList[protocalType] += callback;
        }else{
            netMessageEventList.Add(protocalType, callback);
        }
    }
    //删除网络事件观察者
    public void removeObserver(int protocalType, NetEvtHandle callback)
    {
        if (netMessageEventList.ContainsKey(protocalType))
        {
            netMessageEventList[protocalType] -= callback;
            if (netMessageEventList[protocalType] == null)
            {
                netMessageEventList.Remove(protocalType);
            }
        }
    }
    
//    //推送消息
//	public void PostEvent(GameEvtType eventType, object data = null)
//    {
//        if (gameLogicEventList.ContainsKey(eventType))
//        {
//            gameLogicEventList[eventType](data);
//        }
//    }
    

    void Update()
    {
        while (netMessageDataQueue.Count > 0)
        {
            lock (netMessageDataQueue)
            {
                MsgData msgData = netMessageDataQueue.Dequeue();
                if (netMessageEventList.ContainsKey(msgData.msgID))
                {
                    netMessageEventList[msgData.msgID](msgData.data);
                }
            }
        }

//		while (sendMsgQueue.Count > 0) {
//			lock (sendMsgQueue) {
//				MsgData md = sendMsgQueue.Dequeue ();
//				SocketManager.Instance.RealSend (md.ToBytes());
//			}
//		}
    }
}