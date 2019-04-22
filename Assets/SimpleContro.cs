using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleContro : MonoBehaviour {

    public static SimpleContro Instance;
    // Use this for initialization
    public bool isMove = false;
    private void Awake()
    {
        Instance = this;
    }
    void Start ()
    {
        
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
     

        if (h!=0||v!=0)
        {
            GetComponent<CharacterController>().Move(transform.right * h);
            GetComponent<CharacterController>().Move(transform.forward * v);
            isMove = true;
            //同步数据
        }
        else
        {
            isMove = false;
        }

        

    }
}
