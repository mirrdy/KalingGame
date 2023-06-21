using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RegisterButtonEvent : MonoBehaviour
{
    [Header("계정 정보 입력부분")]
    public InputField ipf_Id;
    public InputField ipf_Pw;

    [Header("로그 UI")]
    public TextMeshProUGUI text_log;
    [Header("로그인 UI")]
    public GameObject panel_Login;

    [Header("계정 등록 버튼")]
    [SerializeField] private Button btn_Confirm;

    public void OnButton_DuplicationCheck()
    {
        string id = ipf_Id.text;
        if (id.Equals(string.Empty))
        {
            DisplayLog("ID를 입력해주세요");
            return;
        }
        if(!DBManager.instance.CheckValidationQueryString(id))
        {
            DisplayLog("ID는 영문과 숫자만 입력 가능합니다.");
            return;
        }

        if (!DBManager.instance.CheckDuplicationID(id))
        {
            DisplayLog("사용 가능한 ID 입니다.");
            btn_Confirm.interactable = true;
        }
        else
        {
            DisplayLog("이미 존재하는 ID입니다.");
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
        // TODO...DB에 등록 작성할 것
        if(!DBManager.instance.RegisterUserInfo(ipf_Id.text, ipf_Pw.text))
        {
            DisplayLog("오류가 발생했습니다.");
            return;
        }
        DisplayLog("계정등록 성공");
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
