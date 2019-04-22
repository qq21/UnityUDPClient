using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Text;

[System.Serializable]
public struct IpInfo
{
    public string Ip;
    public int Port;
}

public class GameSever : MonoBehaviour {

	// Use this for initialization



    public IpInfo ipInfo;
 

    void CreateTcpClient(IpInfo ipInfo)
    {
        //1新 建 socket
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        EndPoint endPoint = new IPEndPoint(IPAddress.Parse(ipInfo.Ip), ipInfo.Port);
        socket.Connect(endPoint);
        socket.Send(Encoding.UTF8.GetBytes( ""));
        Debug.Log(GetSrting(socket));

    }

    public string GetSrting(Socket socket,int dataLength=1024)
    {
        byte[] data = new byte[dataLength];
        int length = socket.Receive(data);
        string s = Encoding.UTF8.GetString(data, 0,length);

        return s;
    }

    
}
