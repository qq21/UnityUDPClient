using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class UDPClient
{


    public bool isConnected {
        get
        {
            return Client.Client.Connected;
        }
    }
    public UdpClient Client;

    private IPEndPoint _selfIpEndPont;
    public IPEndPoint IPEndPoint
    {
        get { return _selfIpEndPont; }
    }
    private IPEndPoint ReceiveIpPoint;//监听

    /// <summary>
    /// 向 iPEndPoint 发送消息
    /// </summary>
    /// <param name="data"></param>
    /// <param name="iPEndPoint"></param>
    public void Send(string data, IPEndPoint iPEndPoint)
    {
        byte[] byteData = Encoding.UTF8.GetBytes(data);

        Client.Send(byteData, byteData.Length, iPEndPoint);

    }
    public UDPClient(IPEndPoint server, int port)
    {
        _selfIpEndPont = new IPEndPoint(IPAddress.Parse(IPManager.GetIP(ADDRESSFAM.IPv4)), port);

        Client = new UdpClient(_selfIpEndPont);
        Client.Connect(server);
         

    }

    public void Close()
    {
        Client.Close();
         
    }


    public void Receive()
    {
        string data = Encoding.UTF8.GetString(Client.Receive(ref ReceiveIpPoint));
        Debug.Log("收到：" + ReceiveIpPoint.Address + data);
    }

}
