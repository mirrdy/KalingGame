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
        // 현재 룸 정보 받아오기
        RoomPuller roomPoller = gameObject.AddComponent<RoomPuller>();
        roomPoller.OnGetRoomsInfo
        (
            (list_Room) =>
            {                
                string id = ipf_Id.text;
                string pw = ipf_Pw.text;

                if (id.Equals(string.Empty) || pw.Equals(string.Empty))
                {
                    DisplayLog("아이디/비밀번호를 입력해주세요");
                    return;
                }
                if (!DBManager.instance.CheckValidationQueryString(id) ||
                    !DBManager.instance.CheckValidationQueryString(pw))
                {
                    DisplayLog("아이디/비밀번호는 영문과 숫자만 입력 가능합니다.");
                    return;
                }

                if (DBManager.instance.Login(id, pw))
                {
                    if (CheckUserConnection(list_Room, id))
                    {
                        DisplayLog("현재 접속중인 아이디입니다.");
                        return;
                    }
                    Debug.Log($"로그인 성공");
                    transform.gameObject.SetActive(false);
                    NetworkManager.instance.onJoinedRoomDele = () =>
                    {
                        SceneManager.LoadScene("SelectBoss");
                    };
                    NetworkManager.instance.JoinRoom();

                    //FindObjectOfType<PunManager>().Connect();
                }
                else
                {
                    DisplayLog("아이디/비밀번호를 확인해주세요");
                    return;
                }

                // 마지막엔 컴포넌트 제거
                Destroy(roomPoller);
            }
        );

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

    public bool CheckUserConnection(List<Photon.Realtime.RoomInfo> list_Room, string id)
    {
        bool isConnecting = false;

        foreach (var room in list_Room)
        {
            if(NetworkManager.instance.CheckHereIsThisID(room, id))
            {
                return true;
            }
        }

        return isConnecting;
    }
}
