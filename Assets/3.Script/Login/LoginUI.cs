using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LoginUI : MonoBehaviour
{
    EventSystem system;
    public Selectable ipf_First_ID;
    public Button btn_Login;

    void Start()
    {
        system = EventSystem.current;
        // �� ó�� InputField ����(ID)
        ipf_First_ID.Select();
    }


    void Update()
    {
        // TabŰ�� �ߺ��Ǳ� ������ �߰�Ű�� �Է��ϴ� ���� ���� �˻��ؾ� ��
        if (Input.GetKeyDown(KeyCode.Tab) && Input.GetKey(KeyCode.LeftShift))
        {
            // Tab + LeftShift -> Tab�� �ݴ����
            Selectable next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp();
            if (next != null)
            {
                next.Select();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Tab))
        {
            // Tab�� �Ʒ��� Selectable ��ü�� ����
            Selectable next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
            if (next != null)
            {
                next.Select();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            // ����, �����̽� �� -> �α���
            btn_Login.onClick.Invoke();
        }
    }
}
