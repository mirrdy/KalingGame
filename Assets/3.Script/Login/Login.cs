using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Login : MonoBehaviour
{
    public InputField ipf_Id;
    public InputField ipf_Pw;

    public TextMeshProUGUI text_log;

    public void Login_btn()
    {
        if (ipf_Id.text.Equals(string.Empty) || ipf_Pw.text.Equals(string.Empty))
        {
            text_log.text = "���̵�/��й�ȣ�� �Է����ּ���";
            return;
        }

        if (DBManager.instance.Login(ipf_Id.text, ipf_Pw.text))
        {
            Debug.Log($"�α��� ����");
            transform.gameObject.SetActive(false);
            //FindObjectOfType<PunManager>().Connect();
        }
        else
        {
            text_log.transform.parent.parent.gameObject.SetActive(true);
            text_log.text = "���̵� ��й�ȣ�� Ȯ�����ּ���";
        }
    }
}
