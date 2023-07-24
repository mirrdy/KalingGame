using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartBossButton : MonoBehaviour
{
    public void StartBoss()
    {
        NetworkManager.instance.StartGameByButton();
    }
}
