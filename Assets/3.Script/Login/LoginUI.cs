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
        // 맨 처음 InputField 선택(ID)
        ipf_First_ID.Select();
    }


    void Update()
    {
        // Tab키가 중복되기 때문에 추가키를 입력하는 쪽을 먼저 검사해야 함
        if (Input.GetKeyDown(KeyCode.Tab) && Input.GetKey(KeyCode.LeftShift))
        {
            // Tab + LeftShift -> Tab의 반대방향
            Selectable next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp();
            if (next != null)
            {
                next.Select();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Tab))
        {
            // Tab은 아래의 Selectable 객체를 선택
            Selectable next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
            if (next != null)
            {
                next.Select();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            // 엔터, 스페이스 바 -> 로그인
            btn_Login.onClick.Invoke();
        }
    }
}
