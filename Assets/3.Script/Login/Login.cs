using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    public InputField Id_i;
    public InputField Password_i;

    public Text log;

    public void Login_btn()
    {
        if (Id_i.text.Equals(string.Empty) || Password_i.text.Equals(string.Empty))
        {
            log.text = "���̵�/��й�ȣ�� �Է����ּ���";
            return;
        }

       /* if (DBManager.instance.Login(Id_i.text, Password_i.text))
        {
            transform.gameObject.SetActive(false);
            FindObjectOfType<PunManager>().Connect();
        }
        else
        {
            log.text = "���̵� ��й�ȣ�� Ȯ�����ּ���";
        }*/
    }
}
