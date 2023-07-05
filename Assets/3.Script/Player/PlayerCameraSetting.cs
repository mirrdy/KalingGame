using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using StarterAssets;

public class PlayerCameraSetting : MonoBehaviourPun
{
    private Transform playerCameraRoot;

    private void Awake()
    {
        if (TryGetComponent(out ThirdPersonController tpc))
        {
            playerCameraRoot = tpc.CinemachineCameraTarget.transform;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        FindObjectOfType<CameraSetting>().SetFollowTransform(playerCameraRoot);
    }
}
