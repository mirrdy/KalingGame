using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LoginButtonEvent : MonoBehaviour
{
    [Header("로그인 정보 입력부분")]
    public InputField ipf_Id;
    public InputField ipf_Pw;

    [Header("로그 UI")]
    public TextMeshProUGUI text_log;
    [Header("계정등록 UI")]
    public GameObject panel_Register;

    public void OnButton_Login()
    {
        string id = ipf_Id.text;
        string pw = ipf_Pw.text;

        if (id.Equals(string.Empty) || pw.Equals(string.Empty))
        {
            DisplayLog("아이디/비밀번호를 입력해주세요");
            return;
        }
        if(!DBManager.instance.CheckValidationQueryString(id) || 
            !DBManager.instance.CheckValidationQueryString(pw))
        {
            DisplayLog("아이디/비밀번호는 영문과 숫자만 입력 가능합니다.");
            return;
        }

        if (DBManager.instance.Login(id, pw))
        {
            Debug.Log($"로그인 성공");
            transform.gameObject.SetActive(false);
            SceneManager.LoadScene("SelectBoss");

            //FindObjectOfType<PunManager>().Connect();
        }
        else
        {
            DisplayLog("아이디/비밀번호를 확인해주세요");
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
