using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class SelectBossButton : MonoBehaviour
{
    bool[] selection_Boss = new bool[3];
    public void SelectBoss(int bossNum)
    {
        Room room = Photon.Pun.PhotonNetwork.CurrentRoom;
        if(room == null)
        {
            return;
        }
        ExitGames.Client.Photon.Hashtable customProperties = room.CustomProperties;

        string propertyStr = $"selection_Boss{bossNum}";
        int selectionBoss = (int)customProperties[propertyStr];

        ExitGames.Client.Photon.Hashtable property = new ExitGames.Client.Photon.Hashtable
            {
                { propertyStr, selectionBoss+1 }
            };

        room.SetCustomProperties(property);
    }
}
