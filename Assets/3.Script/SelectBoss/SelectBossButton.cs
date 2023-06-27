using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.UI;

public class SelectBossButton : MonoBehaviour
{
    [SerializeField] private Button[] btn_Selection;
    private bool[] isSelected;

    private void Start()
    {
        isSelected = new bool[btn_Selection.Length];
    }
    public void SelectBoss(int selectionNum)
    {
        Room room = Photon.Pun.PhotonNetwork.CurrentRoom;
        if(room == null)
        {
            return;
        }
        
        Text text = btn_Selection[selectionNum - 1].GetComponentInChildren<Text>();

        // ���� �����ߴ� �ٸ� ��ư ���ó��
        int preSelNum = System.Array.FindIndex(isSelected, (sel) => sel);
        if (preSelNum >= 0 && preSelNum != selectionNum - 1)
        {
            btn_Selection[preSelNum].GetComponentInChildren<Text>().text = "����";
            isSelected[preSelNum] = false;
        }

        if (isSelected[selectionNum-1])
        {
            text.text = "����";
            NetworkManager.instance.UpdatePlayerSelectionData(DBManager.instance.info.id, -1);
        }
        else
        {
            text.text = "���� ���";
            NetworkManager.instance.UpdatePlayerSelectionData(DBManager.instance.info.id, selectionNum);
        }

        isSelected[selectionNum-1] = !isSelected[selectionNum-1];
    }
}
