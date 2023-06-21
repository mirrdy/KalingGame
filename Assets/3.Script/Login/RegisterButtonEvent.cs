using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RegisterButtonEvent : MonoBehaviour
{
    [Header("���� ���� �Էºκ�")]
    public InputField ipf_Id;
    public InputField ipf_Pw;

    [Header("�α� UI")]
    public TextMeshProUGUI text_log;
    [Header("�α��� UI")]
    public GameObject panel_Login;

    [Header("���� ��� ��ư")]
    [SerializeField] private Button btn_Confirm;

    public void OnButton_DuplicationCheck()
    {
        string id = ipf_Id.text;
        if (id.Equals(string.Empty))
        {
            DisplayLog("ID�� �Է����ּ���");
            return;
        }
        if(!DBManager.instance.CheckValidationQueryString(id))
        {
            DisplayLog("ID�� ������ ���ڸ� �Է� �����մϴ�.");
            return;
        }

        if (!DBManager.instance.CheckDuplicationID(id))
        {
            DisplayLog("��� ������ ID �Դϴ�.");
            btn_Confirm.interactable = true;
        }
        else
        {
            DisplayLog("�̹� �����ϴ� ID�Դϴ�.");
            return;
        }
    }

    public void OnButton_Cancel()
    {
        ipf_Id.text = string.Empty;
        ipf_Pw.text = string.Empty;

        panel_Login.SetActive(true);
        gameObject.SetActive(false);
    }

    public void OnButton_Register()
    {
        // TODO...DB�� ��� �ۼ��� ��
        if(!DBManager.instance.RegisterUserInfo(ipf_Id.text, ipf_Pw.text))
        {
            DisplayLog("������ �߻��߽��ϴ�.");
            return;
        }
        DisplayLog("������� ����");
        panel_Login.SetActive(true);
        gameObject.SetActive(false);
    }
    public void tmp()
    {

    }
    private void DisplayLog(string message)
    {
        text_log.transform.parent.parent.gameObject.SetActive(true);
        text_log.text = message;
    }
}
