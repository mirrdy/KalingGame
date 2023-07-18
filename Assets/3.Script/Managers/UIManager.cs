using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    [SerializeField] private TextMeshProUGUI text_SharedLife;
    [SerializeField] private RectTransform panel_DeathBackground;

    [Header("Player UI")]
    [SerializeField] Image hpBar;
    [SerializeField] TextMeshProUGUI text_HP;
    [SerializeField] Image mpBar;
    [SerializeField] TextMeshProUGUI text_MP;



    private IEnumerator hpDelay_Co;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        NetworkManager.instance.onUpdateRoom_SharedLife = UpdateSharedLife;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void UpdateSharedLife()
    {
        text_SharedLife.text = NetworkManager.instance.GetSharedLife().ToString();
    }
    public void DisplayRespawnUI()
    {
        panel_DeathBackground.gameObject.SetActive(true);
    }
    public void HpCheck(int maxHp, int currentHp)
    {
        float goals = currentHp / (float)maxHp;

        if (hpDelay_Co != null)
        {
            StopCoroutine(hpDelay_Co); // 기존 코루틴 종료
        }

        hpDelay_Co = HpDelay_Co(goals);
        StartCoroutine(hpDelay_Co);
        text_HP.text = currentHp + " / " + maxHp;
    }

    IEnumerator HpDelay_Co(float goals)
    {
        float timer = 0f;
        float current = hpBar.fillAmount;
        float duration = 1f; // 체력 감소 지속 시간

        while (timer <= duration)
        {
            yield return null;
            timer += Time.deltaTime;
            float t = timer / duration;
            hpBar.fillAmount = Mathf.Lerp(current, goals, t);
        }

        hpBar.fillAmount = goals;
        hpDelay_Co = null;
    }
}
