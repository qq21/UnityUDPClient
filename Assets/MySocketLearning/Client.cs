using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
 



public static class MsgCode
{
    public const string MOVE="1";
}

public class GameMessage
{

}

/// <summary>
/// 客户端接口
/// </summary>
public interface INetworkClient
{
    /// <summary>
    /// 消息处理器
    /// </summary>
    event Action<GameMessage> MessageHandler;

    /// <summary>
    /// 连接服务器
    /// </summary>
    /// <param name="ipaddr"></param>
    /// <param name="port"></param>
    void Connect(string connectStr);

    /// <summary>
    /// 发送数据
    /// </summary>
    void Send(GameMessage msg);

    /// <summary>
    /// 是否连接
    /// </summary>
    bool Connected { get; }

    /// <summary>
    /// 关闭连接
    /// </summary>
    void Close();
}

public class Client : MonoBehaviour
{


    public IpInfo LocalipInfo;

    public IpInfo ServerIpInfo;


    public bool isServer = false;
  
    public IPEndPoint Server = new IPEndPoint(IPAddress.Any, 0);

    private UdpClient udpClient;

    private IPEndPoint _serverEndPoint;

    private Thread t;

    private Vector3[] receiveTran;
    private byte[] receiveData = null;
    public int TempTranID = 1;

    private int port = 7788;
    private string receiveMes = "";
    private string sendMes = "";

    

    // TODO:同步 32 个子物体，按照Id取
    private Dictionary<string, Transform> childPart = new Dictionary<string, Transform>();

    Thread receiveT;
    private void Awake()
    {
        Connect("192.168.1.121",7789);

       

        //if (isServer)
        //{

        //    InvokeRepeating("ReceiveInfoFromClient", 1f, 0.3f);
        //}
        receiveT = new Thread(Recive);
        //TODO: 开启线程
        receiveT.IsBackground = true;
        receiveT.Start();

         
    }

    bool m_start;
    int m_logicFrameDelta;//逻辑帧更新时间
    int m_logicFrameAdd;//累积时间

    private void FixedUpdate()
    {

        //TODO： 只需要 更新操作的物体     如果  没有在操作 则 不需要更新  发送 只需要在操作的时候 进行
        if (SimpleContro.Instance.isMove)
        {
            SendTransformInfo(this.transform);
         
        }
         SetTranInfo(this.transform);
        
      
    }

   

    /// <summary>
    /// 获取本机Ip 建立 连接 服务器 固定
    /// </summary>
    void Connect(string ip,int port)
    {
        //获取本机IP 地址
        LocalipInfo.Ip = IPManager.GetIP(ADDRESSFAM.IPv4);
        this.port = LocalipInfo.Port;
        // IP  端口
        _serverEndPoint = new IPEndPoint(IPAddress.Parse(LocalipInfo.Ip), LocalipInfo.Port);
        //创建  
        udpClient = new UdpClient(_serverEndPoint);


        ServerIpInfo.Ip = ip;
        ServerIpInfo.Port = port;

        Server = new IPEndPoint(IPAddress.Parse(ServerIpInfo.Ip), ServerIpInfo.Port);
         
        
        udpClient.Connect(Server);

        Debug.Log("连接服务器"+ ServerIpInfo.Ip+"成功");
    }



    /// <summary>
    /// 关闭 连接
    /// </summary>
    void Close()
    {
        udpClient.Close();
    }

    /// <summary>
    /// 是否连接 
    /// </summary>
    public bool Connected
    {
        get { return udpClient.Client.Connected;}
    }


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

      
    }

   

    /// <summary>
    ///  TODO: 根据收到信息的ID 同步不同的部位
    /// </summary>
    /// <param name="Target"></param>
    void SetTranInfo(Transform Target,float speed=3)
    {
        if (receiveData!=null)
        {
            Target.localPosition = Vector3.Lerp(Target.localPosition, receiveTran[0],Time.deltaTime*speed) ;
            Target.localRotation.eulerAngles.Set(receiveTran[1].x, receiveTran[1].y, receiveTran[1].z) ;
            Target.localScale = receiveTran[2];
        }
    }

    /// <summary>
    /// 从服务器 接受消息  开启线程
    /// </summary>
    public void Recive()
    {
        while (true)
        {
            receiveData = udpClient.Receive(ref Server);

            // 获取数据
            receiveMes = GetDataString(receiveData)[1];

            receiveTran = HanleData(receiveMes);
            Debug.LogWarning(receiveMes);
            Debug.Log("Pos" + receiveTran[0] + "Rot:" + receiveTran[1] + "Scale" + receiveTran[2]);
        }
    }

    /// <summary>
    /// 返回 字符串数组 0:[id] 1:[数据] 
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    string[] GetDataString(byte[] data)
    {

        return Encoding.UTF8.GetString(data,0,data.Length).Split('@');
    }

    void SendTransformInfo(Transform target)
    {
        if (Connected)
        {
            //TODO: 在Target身上 关联Id TempTranID   目前通过GetInstanceID() 来进行 关联

            //TODO: 来设置  在收到消息的时候 ,需要一个字典管理
            TempTranID = target.GetInstanceID();

            string tranData = TempTranID + "@"+GetTransformInfo(target);

            byte[] data = Encoding.UTF8.GetBytes(tranData);

            udpClient.Send(data, data.Length);
            
            ////取得数据
            //sendMes = GetDataString(data); 
            //Debug.LogWarning(sendMes);
            //Vector3[] trans = HanleData(sendMes);
            //Debug.Log("Pos" + trans[0] + "Rot:" + trans[1] + "Scale" + trans[2]);
        }
    }

    /// <summary>
    /// 协议头 1_  把 Transform 信息 转换成 string
    /// </summary>
    /// <returns></returns>
    public string GetTransformInfo(Transform tartget)
    {
        Vector3 data = tartget.localPosition;
        Vector3 RotaData = tartget.localRotation.eulerAngles;
        Vector3 ScaleInfo = tartget.localScale;

        string pos = MsgCode.MOVE+":"+data.x + "_" + data.y + "_" + data.z+":"+RotaData.x+"_"+RotaData.y+"_"+RotaData.z+":"+ScaleInfo.x+"_"+ScaleInfo.y+"_"+ScaleInfo.z;

        return pos;
    }


    /// <summary>
    ///  Handle Data  解析 transform 数据
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public Vector3[] HanleData (string data)
    {

        string[] datas = data.Split(':');

        Vector3[] trans = new Vector3[3];

        switch (datas[0])
        {
            case  MsgCode.MOVE:
                

                for (int i = 1; i <=3; i++)
                {
                    string[] pos = datas[i].Split('_');
                    float x = 0;
                    float y = 0;
                    float z = 0;

                    if (float.TryParse(pos[0], out x) && float.TryParse(pos[1], out y) && float.TryParse(pos[2], out z))
                    {
                        trans[i-1] = new Vector3(x, y, z);
                    }
                    else
                    {
                        Debug.LogError("Erro Parse" + "x：" + pos[0] + "y：" + pos[1] + "z：" + pos[2]);
                    }
                }
                break;
        }

        return trans;
    }

    private void OnDestroy()
    {
        Close();
        receiveT.Abort();
        
    }

    ///// <summary>
    ///// 广播  数据
    ///// </summary>
    //void BoradCastClient(string data)
    //{
    //    foreach (var client in users)
    //    {
    //        if (!client.Value.isConnected)
    //        {
    //            users.Remove(client.Key);
    //        }
    //        else
    //        {
    //            byte[] ByteData = Encoding.UTF8.GetBytes(data);

    //            // 向  客服端 同步 数据
    //            udpClient.Send(ByteData, ByteData.Length, client.Value.IPEndPoint);
    //        }
    //    }
    //}


}

