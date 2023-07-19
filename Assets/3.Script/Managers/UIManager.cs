using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum PotionType
{
    HP = 0,
    MP = 1
};

public class UIManager : MonoBehaviour
{
    
    public static UIManager instance;
    [SerializeField] private TextMeshProUGUI text_SharedLife;
    [SerializeField] private RectTransform panel_DeathBackground;

    [Header("Player UI")]
    [SerializeField] private Image hpBar;
    [SerializeField] private TextMeshProUGUI text_HP;
    [SerializeField] private Image mpBar;
    [SerializeField] private TextMeshProUGUI text_MP;

    [Header("PotionSlot")]
    [SerializeField] private Image[] Image_CooltimeFill;  // ��ų ��Ÿ�� �ð� ���� �̹���
    [SerializeField] private TextMeshProUGUI[] text_PotionCooltime;  // ��ų �� ���� �ð� ǥ��



    private IEnumerator hpDelay_Co;
    private IEnumerator mpDelay_Co;
    private IEnumerator[] potionDelay_Co = new IEnumerator[2];

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
            StopCoroutine(hpDelay_Co); // ���� �ڷ�ƾ ����
        }

        hpDelay_Co = HpDelay_Co(goals);
        StartCoroutine(hpDelay_Co);
        text_HP.text = currentHp + " / " + maxHp;
    }

    IEnumerator HpDelay_Co(float goals)
    {
        float timer = 0f;
        float current = hpBar.fillAmount;
        float duration = 1f; // ü�� ���� ���� �ð�

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

    public void MpCheck(int maxMp, int currentMp)
    {
        float goals = currentMp / (float)maxMp;

        if (mpDelay_Co != null)
        {
            StopCoroutine(mpDelay_Co); // ���� �ڷ�ƾ ����
        }

        mpDelay_Co = MpDelay_Co(goals);
        StartCoroutine(mpDelay_Co);
        text_MP.text = currentMp + " / " + maxMp;
    }

    IEnumerator MpDelay_Co(float goals)
    {
        float timer = 0f;
        float current = mpBar.fillAmount;
        float duration = 1f; // ü�� ���� ���� �ð�

        while (timer <= duration)
        {
            yield return null;
            timer += Time.deltaTime;
            float t = timer / duration;
            mpBar.fillAmount = Mathf.Lerp(current, goals, t);
        }

        mpBar.fillAmount = goals;
        mpDelay_Co = null;
    }

    IEnumerator PotionDelay_Co(PotionType potion)
    {
        int typeIndex = (int)potion;
        float timer = 0f;
        float current = Image_CooltimeFill[typeIndex].fillAmount;
        float coolTime = 10;
        float time = coolTime;

        while (timer <= coolTime)
        {
            timer += Time.deltaTime;
            float t = timer / coolTime;
            time -= Time.deltaTime;
            text_PotionCooltime[typeIndex].text = time.ToString("N1") + "(s)";
            Image_CooltimeFill[typeIndex].fillAmount = 1 - Mathf.Lerp(0, 1, t);
            yield return null;
        }

        Image_CooltimeFill[typeIndex].fillAmount = 0;
        potionDelay_Co[typeIndex] = null;
        text_PotionCooltime[typeIndex].text = string.Empty;
        yield break;
    }
    public bool UsePotion(PotionType potion)
    {
        if (potionDelay_Co[(int)potion] != null)
        {
            return false;
        }
        potionDelay_Co[(int)potion] = PotionDelay_Co(potion);
        StartCoroutine(potionDelay_Co[(int)potion]);
        return true;
    }
}
