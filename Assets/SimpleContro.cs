using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class SimpleContro : MonoBehaviour {

    public static SimpleContro Instance;
    // Use this for initialization
    public bool isMove = false;

    /// <summary>
    /// 当前 操作 的物体
    /// </summary>
    public NetInputDetecter currentNetDeceter;

    SimpleFollow simpleFollow;


    private void Awake()
    {
        Instance = this;
        simpleFollow = GetComponent<SimpleFollow>();

       
        YEvent.EventCenter.AddListener(YEvent.EventType.OnClickTran,
            (NetInputDetecter detector) =>
            {
                this.currentNetDeceter = detector;
                simpleFollow.player = currentNetDeceter.transform;
            });

        NetData.Instance.InitData();

    }

    private void OnDestroy()
    {
             //TODO 移除监听 处理
    }
 
    
    // Update is called once per frame
	void Update ()
    {
        Move();
    }

    void Move()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");


        if (h != 0 || v != 0)
        {
            if (currentNetDeceter)
            {
                currentNetDeceter.Move(h, v, 5);
                isMove = true;

                //同步数据       发送同步 事件
                YEvent.EventCenter.Broadcast(YEvent.EventType.OnSendTransInfo, currentNetDeceter.transform, currentNetDeceter.Id);
            }
        }
        else
        {
            isMove = false;
        }
    }
}
