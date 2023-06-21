using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LoginButtonEvent : MonoBehaviour
{
    [Header("�α��� ���� �Էºκ�")]
    public InputField ipf_Id;
    public InputField ipf_Pw;

    [Header("�α� UI")]
    public TextMeshProUGUI text_log;
    [Header("������� UI")]
    public GameObject panel_Register;

    public void OnButton_Login()
    {
        string id = ipf_Id.text;
        string pw = ipf_Pw.text;

        if (id.Equals(string.Empty) || pw.Equals(string.Empty))
        {
            DisplayLog("���̵�/��й�ȣ�� �Է����ּ���");
            return;
        }
        if(!DBManager.instance.CheckValidationQueryString(id) || 
            !DBManager.instance.CheckValidationQueryString(pw))
        {
            DisplayLog("���̵�/��й�ȣ�� ������ ���ڸ� �Է� �����մϴ�.");
            return;
        }

        if (DBManager.instance.Login(id, pw))
        {
            Debug.Log($"�α��� ����");
            transform.gameObject.SetActive(false);
            SceneManager.LoadScene("SelectBoss");

            //FindObjectOfType<PunManager>().Connect();
        }
        else
        {
            DisplayLog("���̵�/��й�ȣ�� Ȯ�����ּ���");
            return;
        }
    }

    public void OnButton_Register()
    {
        panel_Register.SetActive(true);
        gameObject.SetActive(false);
    }

    private void DisplayLog(string message)
    {
        text_log.transform.parent.parent.gameObject.SetActive(true);
        text_log.text = message;
    }
}
