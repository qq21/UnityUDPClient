using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
///  
/// </summary>
[DisallowMultipleComponent]
public class NetInputDetecter : MonoBehaviour
{
    private CharacterController characterController;
    public string Id { get; set; }

    #region for test
    //public NetInputDetecter()
    //{
    //    Debug.Log(this.GetType().IsDefined(typeof(DisallowMultipleComponent), false));
    //}
    #endregion
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    public void Move(float h ,float v,float speed)
    {
        characterController.Move(new Vector3(h,0,v)*speed*Time.deltaTime);

    }

    private void OnMouseDown()
    {
        YEvent.EventCenter.Broadcast<NetInputDetecter>(YEvent.EventType.OnClickTran,this);
    }
}