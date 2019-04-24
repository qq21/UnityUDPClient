using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetData
{
    private static NetData _instance;

    static object _lock = new object();
    public static NetData Instance
    {
        
        get
        {
            lock (_lock)
            {               
                if (_instance == null)
                {
                    _instance = new NetData();
                }
                return _instance;
            }
        }
    } 

    // TODO:同步 32 个子物体，按照Id取
    public Dictionary<string, Transform> netTransDic = new Dictionary<string, Transform>();



    public NetData()
    {

       
    }

    public void InitData()
    {
        NetInputDetecter[] netInputs = GameObject.FindObjectsOfType<NetInputDetecter>();
        for (int i = 0; i < netInputs.Length; i++)
        {
            netInputs[i].Id = i.ToString();
            netTransDic.Add(i.ToString(), netInputs[i].transform);

        }
    }

    public Vector3[] receiveTran;

    public byte[] receiveData = null;

    public string receiveMes = "";
 

}
