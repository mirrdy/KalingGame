using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalController : MonoBehaviour
{
    public Vector3 dest;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent(out PlayerControl player))
            {
                if(player.TryGetComponent(out PhotonView PV))
                {
                    if(PV.IsMine)
                    {
                        OperatePortal(player);
                    }
                }
                
            }
        }
    }

    private void OperatePortal(PlayerControl player)
    {
        player.SetPosition(dest);
        player.spawnPos = dest;
    }
}
