using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class ConnetManager : MonoBehaviour
{
    private InputField inputField;
    private Client client;
    private Button button;
    private void Awake()
    {
        inputField = GetComponentInChildren<InputField>();
        button = GetComponentInChildren<Button>();
        client = GameObject.FindObjectOfType<Client>();
        inputField.text = IPManager.GetIP(ADDRESSFAM.IPv4);
        button.onClick.AddListener
        (
            () =>
            {
                if (IPManager.IsIP(inputField.text))
                {

                    if (client.OnConnectClick(inputField.text, 7789))
                    {
                        SceneManager.LoadScene(1);
                    }
                    else
                    {
                        inputField.text = "连接失败,检查服务器地址";
                    }
                }
                else
                {
                    Debug.LogWarning("输入非法");
                    inputField.text = "";
                }

            }

        );
    }

}
