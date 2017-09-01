using UnityEngine;
using System.Collections;
using System.Text;
using System;
using Google.Protobuf;
using UnityEngine.UI;


public class Client : MonoBehaviour {

    public Transform BtnRoot;

    void Start()
    {



        RegeditControl();
    }
    

    void OnEnable()
    {
//        MessageCenter.Instance.AddEventListener(GameEvtType.NoticeInfo, CallBack_PoseEvent);
        MessageCenter.Instance.addObsever(MsgID.sc_protobuf_login, CallBack_ProtoBuff_LoginServer);
        MessageCenter.Instance.addObsever(MsgID.sc_binary_login, CallBack_Binary_LoginServer);
    }

    void OnDisable()
    {
//        MessageCenter.Instance.RemoveEventListener(GameEvtType.NoticeInfo, CallBack_PoseEvent);
        MessageCenter.Instance.removeObserver(MsgID.sc_protobuf_login, CallBack_ProtoBuff_LoginServer);
        MessageCenter.Instance.removeObserver(MsgID.sc_binary_login, CallBack_Binary_LoginServer);
    }

    void OnApplicationQuit()
    {
        SocketManager.Instance.Close();
    }

    private void RegeditControl()
    {
        BtnRoot.Find("Btn_Connect").GetComponent<Button>().onClick.AddListener(OnButton_Connect);
        BtnRoot.Find("Btn_DisConnect").GetComponent<Button>().onClick.AddListener(OnButton_DisConnect);
        BtnRoot.Find("Btn_PostEvent_NoticeInfo").GetComponent<Button>().onClick.AddListener(OnButton_PostEvent);
        BtnRoot.Find("Btn_SendMsg_Protobuf").GetComponent<Button>().onClick.AddListener(OnButton_ProtoBuff_SendMsg);
        BtnRoot.Find("Btn_SendMsg_Binary").GetComponent<Button>().onClick.AddListener(OnButton_Binary_SendMsg);
    }


    private void OnButton_Connect()
    {
        SocketManager.Instance.Connect(GameConst.IP, GameConst.Port);
    }

    private void OnButton_DisConnect()
    {
        SocketManager.Instance.Close();
    }

    private void OnButton_PostEvent()
    {
//        string _content = "GameLogicEvent";
//        MessageCenter.Instance.PostEvent(GameEvtType.NoticeInfo, _content);
    }

    private void OnButton_ProtoBuff_SendMsg()
    {
        CS_LOGINSERVER loginServer = new CS_LOGINSERVER();
		loginServer.Account = "name";
		loginServer.Password = "123456";
        SocketManager.Instance.SendMsg(MsgID.sc_protobuf_login, loginServer);
    }
    
    private void OnButton_Binary_SendMsg()
    {
//        ByteStreamBuff _tmpbuff = new ByteStreamBuff();
//        _tmpbuff.Write_Int(1314);
//        _tmpbuff.Write_Float(99.99f);
//        _tmpbuff.Write_UniCodeString("Claine");
//        _tmpbuff.Write_UniCodeString("123456");
//        SocketManager.Instance.SendMsg(eProtocalCommand.sc_binary_login, _tmpbuff);
    }



    private void CallBack_PoseEvent(object _eventParam)
    {
        string _content = (string)_eventParam;
        Debug.Log(_content);
    }

    private void CallBack_ProtoBuff_LoginServer(byte[] _msgData)
    {
		CS_LOGINSERVER _tmpLoginServer = CS_LOGINSERVER.Parser.ParseFrom (_msgData);//SocketManager.ProtoBufDeserialize<CS_LOGINSERVER>(_msgData);
		Debug.Log("account:"+_tmpLoginServer.Account);
		Debug.Log("password:"+_tmpLoginServer.Password);
    }

    private void CallBack_Binary_LoginServer(byte[] _msgData)
    {
//        ByteStreamBuff _tmpbuff = new ByteStreamBuff(_msgData);
//        Debug.Log(_tmpbuff.Read_Int());
//        Debug.Log(_tmpbuff.Read_Float());
//        Debug.Log(_tmpbuff.Read_UniCodeString());
//        Debug.Log(_tmpbuff.Read_UniCodeString());
//        _tmpbuff.Close();
//        _tmpbuff = null;
    }
}
