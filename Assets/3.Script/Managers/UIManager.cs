using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    [SerializeField] private TextMeshProUGUI text_SharedLife;
    [SerializeField] private RectTransform panel_DeathBackground;

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
}
