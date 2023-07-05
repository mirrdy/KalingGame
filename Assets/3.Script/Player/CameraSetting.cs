using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSetting : MonoBehaviour
{
    CinemachineVirtualCamera virtualCamera;
    private void Start()
    {
        TryGetComponent(out virtualCamera);
    }
    public void SetFollowTransform(Transform t)
    {
        virtualCamera.Follow = t;
    }
}
