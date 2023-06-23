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

        ExitGames.Client.Photon.Hashtable customProperties = room.CustomProperties;
        ExitGames.Client.Photon.Hashtable property;
        string propertyStr;
        int selectionBossCount;

        // ���� �����ߴ� �ٸ� ��ư ���ó��
        int preSelNum = System.Array.FindIndex(isSelected, (sel) => sel);
        if (preSelNum >= 0 && preSelNum != selectionNum-1)
        {
            propertyStr = $"selection_Boss{preSelNum + 1}";
            selectionBossCount = (int)customProperties[propertyStr];

            property = new ExitGames.Client.Photon.Hashtable
            {
                { propertyStr, selectionBossCount-1 }
            };
            room.SetCustomProperties(property);

            btn_Selection[preSelNum].GetComponentInChildren<Text>().text = "����";
            isSelected[preSelNum] = false;
        }

        
        Text text = btn_Selection[selectionNum - 1].GetComponentInChildren<Text>();
        int countNum;
        if (isSelected[selectionNum-1])
        {
            countNum = -1;
            text.text = "����";
        }
        else
        {
            countNum = 1;
            text.text = "���� ���";
        }

        propertyStr = $"selection_Boss{selectionNum}";
        selectionBossCount = (int)customProperties[propertyStr];

        property = new ExitGames.Client.Photon.Hashtable
            {
                { propertyStr, selectionBossCount+countNum }
            };

        room.SetCustomProperties(property);

        isSelected[selectionNum-1] = !isSelected[selectionNum-1];
    }
}
