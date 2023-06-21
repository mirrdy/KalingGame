using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnChangeInputField : MonoBehaviour
{
    [Header("계정 등록 확인 버튼")]
    [SerializeField] Button btn_Confirm;

    public void OnChangeIPFText()
    {
        btn_Confirm.interactable = false;
    }
}
