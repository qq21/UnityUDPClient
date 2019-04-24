using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;

/// <summary>
/// 客户端
/// </summary>
public class Client : MonoBehaviour
{
    public IpInfo LocalipInfo;
    public bool isServer = false; 
    public IPEndPoint Server = new IPEndPoint(IPAddress.Any, 0);
    public static Client Instance
    {
        get
        {
            if (_instance==null)
            {
                _instance = new GameObject("Client").AddComponent<Client>();
            }
            return
                _instance;
        }
    }
    private static Client _instance; 
    
    private UdpClient udpClient;

    private IPEndPoint _clientEndPoint;

    private NetData _netData;
 
    private int port = 7788;

    bool m_start;
    int m_logicFrameDelta;//逻辑帧更新时间
    int m_logicFrameAdd=20;//累积时间

    private string receiveNetId;
    private bool needUpdate = false;
    /// <summary>
    /// 接受 线程
    /// </summary>
    Thread receiveT;
   
    #region Private Field

    #region Mono'life method

    private void Awake()
    {
        //    netTransDic.Add(transform.GetInstanceID().ToString(), transform);

        DontDestroyOnLoad(this);

        YEvent.EventCenter.AddListener<Transform, string>(YEvent.EventType.OnSendTransInfo, SendTransformInfo);

        //持有 数据 引用
        _netData = NetData.Instance;
        

    }

    /// <summary>
    /// 点击连接的时候 调用
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="port"></param>
    /// <returns></returns>
    public bool OnConnectClick(string ip, int port = 7789)
    {

        udpClient = BuildLocalClient(); //建立本地
        bool state = Connect(ip, port,udpClient);

        //连接成功
        if (state)
        {
            // 开启接受线程
            receiveT = new Thread(Recive);
            //允许可以在后台跑
            receiveT.IsBackground = true;
            receiveT.Start();
            //发送加入消息
            SendJoinMsg();
        }
        else
        {
            Debug.LogWarningFormat("连接失败:{0}{1}{2}", ip,":", port);
        }
        return state;

    }

    private void OnEnable()
    {
        _instance = this;
    }

    private void FixedUpdate()
    {

        //   数据应该存放在 一个数据类里     只需要 更新操作的物体     如果  没有在操作 则 不需要更新  发送 只需要在操作的时候 进行
        //每 20帧 更新，
        if (needUpdate)
        {
            m_logicFrameDelta++;
            if (_netData.netTransDic.ContainsKey(receiveNetId))
            {
                SetTranInfo(_netData.netTransDic[receiveNetId]);
            }

            if (m_logicFrameDelta>m_logicFrameAdd )
            {
                needUpdate = false;
                m_logicFrameDelta = 0;
            }
            
        }

    }

    private void OnGUI()
    {
        GUILayout.TextField("id:" + receiveNetId);

        if (_netData.receiveData != null)
        {
            GUILayout.TextField("pos:" + _netData.receiveTran[0] + "rot:" + _netData.receiveTran[1] + "scale:" +_netData.receiveTran[2]);
            GUILayout.TextField(_netData.receiveMes);
        }
    }

    /// <summary>
    ///退出的时候   关闭客服端连接，关闭线程
    /// </summary>
    private void OnDestroy()
    {
        Close();
        receiveT.Abort();
    }
    #endregion



    #region Receive Code
    void ReceiveTransCode(string[] data,string id,char splitSign= '_')
    {
        Vector3[] trans = new Vector3[data.Length-1];
        Debug.Log(data.Length);
        for (int i = 1; i <= data.Length - 1; i++)
        {
            //1 2 3
            string[] pos = data[i].Split(splitSign);

            float x = 0;
            float y = 0;
            float z = 0;

            if (float.TryParse(pos[0], out x) && float.TryParse(pos[1], out y) && float.TryParse(pos[2], out z))
            {
                trans[i - 1] = new Vector3(x, y, z);
            }
            else
            {
                Debug.LogError("Erro Parse" + "x：" + pos[0] + "y：" + pos[1] + "z：" + pos[2]);
            }
        }


        //更新 位置  信息/../'././.  Update  .....
        _netData.receiveTran = trans;
        //更新
        receiveNetId = id;
        needUpdate = true;
    }
    void ReceiveJoinCode(string data)
    {
        _netData.receiveMes = data + "加入了房间";
        Debug.Log(data);
    }
    void SendJoinMsg()
    {
        string mes = "2@" + MsgCode.JOIN + ":" + LocalipInfo.Ip;
        Send(mes);
    }
    void ReceiveQuitCode(string data)
    {
        _netData.receiveMes = data + "退出了房间";
        Debug.Log(data);
    }
    #endregion


    UdpClient BuildLocalClient()
    { 
        //获取本机IP 地址
        LocalipInfo.Ip = IPManager.GetIP(ADDRESSFAM.IPv4);

        this.port = LocalipInfo.Port;
        // IP  端口
        _clientEndPoint = new IPEndPoint(IPAddress.Parse(LocalipInfo.Ip), LocalipInfo.Port);
        //创建  本地通信客服端
       return new UdpClient(_clientEndPoint);
    }

    /// <summary>
    /// 连接服务器
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="port"></param>
    bool Connect(string ip, int port, UdpClient udpClient )
    {
      
        Server = new IPEndPoint(IPAddress.Parse(ip),port); 
        udpClient.Connect(Server);
    
        if ( udpClient.Client.Connected)
        {
            Debug.Log("连接服务器" +ip +"成功");
        }
        else
        {
            Debug.LogWarning("连接失败");
        }

        return udpClient.Client.Connected;

    }

    /// <summary>
    /// 关闭客服端连接  发送退出 消息
    /// </summary>
    void Close()
    {
        string mes ="1@"+ MsgCode.QUIT+":"+LocalipInfo.Ip;
        Send(mes);
        udpClient.Close();

    }

    /// <summary>
    /// 返回 字符串数组 0:[id] 1:[数据] 
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    string[] GetDataString(byte[] data)
    {
        return Encoding.UTF8.GetString(data, 0, data.Length).Split('@');
    }

    //格式： ID @ MsgCod: data1_data2_data3: data4_data5_data6:
    public void SendTransformInfo(Transform target,string id)
    {
        if (Connected)
        {    
             //通过 在 NetData里获取 数据 id 
            string tranData =  GetTransformInfo(target,id);

            if (!_netData.netTransDic.ContainsKey(id))
                _netData.netTransDic.Add(id, target);

            Send(tranData);

        }
    }    

    /// <summary>
    /// 获取本机Ip 建立 连接 服务器 固定
    /// </summary>
    /// <summary>
    ///  TODO: 根据收到信息的ID 同步不同的部位,更新相机
    /// </summary>
    /// <param name="Target"></param>
    void SetTranInfo(Transform Target, float speed = 5)
    {
        if (_netData.receiveData != null)
        {
            Target.localPosition = Vector3.Lerp(Target.localPosition, _netData.receiveTran[0], Time.deltaTime * speed);
            Target.localRotation.eulerAngles.Set(_netData.receiveTran[1].x, _netData.receiveTran[1].y, _netData.receiveTran[1].z);
            Target.localScale = _netData.receiveTran[2];
        }
    }

    /// <summary>
    /// 接受数据   放在线程中
    /// </summary>
    void Recive()
    {
        while (true)
        {
            _netData.receiveData = udpClient.Receive(ref Server);   
            HandleData(GetDataString(_netData.receiveData));         
        }
    }
    #endregion

    /// <summary>
    /// 收 从服务器 接受 消息 收发 数据 用 二进制
    /// </summary>
    /// <returns></returns>
    /// <summary>
    /// 发送 数据
    /// </summary>
    /// <param name="msg"></param>
    public void Send( string msg)
    {
        if (msg==null)
        {
            return;
        }
        byte[] data = Encoding.UTF8.GetBytes(msg); 
        udpClient.Send(data, data.Length);
    
    }

    /// <summary>
    /// 是否连接 
    /// </summary>
    public bool Connected
    {
        get { return udpClient.Client.Connected; }
    }

    /// <summary>
    /// 从服务器 接受消息  开启线程
    /// </summary>
 
    /// <summary>
    ///  Handle Data  解析 transform 数据
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public void HandleData(string[] data)
    {
        Debug.Log("id为" + data[0]);

        //[0] id  [1] data   id+@+MsgCode+:+x1_y1_z1+:x2_y2_z2+:x3_y3_z3
        string[] datas = data[1].Split(':');

        if (datas.Length<2)
        {
            return;
        }
        switch (datas[0])
        {
            case MsgCode.MOVE:
            {
                ReceiveTransCode(datas,data[0]);
                break;
            }
            case MsgCode.QUIT:
            {
                ReceiveQuitCode(datas[1]);
                //(data[1] + "Quit");
                break;
            }
            case MsgCode.JOIN:
            {
                ReceiveJoinCode(data[1]);
                break;
            }
        } 
    }

    /// <summary>
    ///  物体id +@+协议头 1_  把 Transform 信息 转换成 string
    /// </summary>
    /// <returns>返回规则是 id+@+MsgCode+:+x1_y1_z1+:x2_y2_z2+:x3_y3_z3   依此对应的是position eulerAngles scale 以上 均是Local</returns>
    public string GetTransformInfo(Transform target,string id)
    {
        Vector3 data = target.localPosition;
        Vector3 RotaData = target.localRotation.eulerAngles;
        Vector3 ScaleInfo = target.localScale;
        string pos = id+ "@" + MsgCode.MOVE+":"+data.x + "_" + data.y + "_" + data.z+":"+RotaData.x+"_"+RotaData.y+"_"+RotaData.z+":"+ScaleInfo.x+"_"+ScaleInfo.y+"_"+ScaleInfo.z;
        return pos;
    }
 
 


}

