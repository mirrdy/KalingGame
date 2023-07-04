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
        // ���� �� ���� �޾ƿ���
        RoomPuller roomPoller = gameObject.AddComponent<RoomPuller>();
        roomPoller.OnGetRoomsInfo
        (
            (list_Room) =>
            {                
                string id = ipf_Id.text;
                string pw = ipf_Pw.text;

                if (id.Equals(string.Empty) || pw.Equals(string.Empty))
                {
                    DisplayLog("���̵�/��й�ȣ�� �Է����ּ���");
                    return;
                }
                if (!DBManager.instance.CheckValidationQueryString(id) ||
                    !DBManager.instance.CheckValidationQueryString(pw))
                {
                    DisplayLog("���̵�/��й�ȣ�� ������ ���ڸ� �Է� �����մϴ�.");
                    return;
                }

                if (DBManager.instance.Login(id, pw))
                {
                    if (CheckUserConnection(list_Room, id))
                    {
                        DisplayLog("���� �������� ���̵��Դϴ�.");
                        return;
                    }
                    Debug.Log($"�α��� ����");
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
                    DisplayLog("���̵�/��й�ȣ�� Ȯ�����ּ���");
                    return;
                }

                // �������� ������Ʈ ����
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
