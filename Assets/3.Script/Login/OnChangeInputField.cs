using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnChangeInputField : MonoBehaviour
{
    [Header("���� ��� Ȯ�� ��ư")]
    [SerializeField] Button btn_Confirm;

    public void OnChangeIPFText()
    {
        btn_Confirm.interactable = false;
    }
}
