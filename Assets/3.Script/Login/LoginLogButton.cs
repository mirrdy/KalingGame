using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginLogButton : MonoBehaviour
{
    public void Confirm_Log()
    {
        transform.parent.gameObject.SetActive(false);
    }
}
