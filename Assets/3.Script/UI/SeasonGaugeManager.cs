using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public enum Season
{
    Spring, //yellow
    Summer, //blue
    Autumn,  //purple
    None    // Not set yet
}
public class SeasonGaugeManager : MonoBehaviourPunCallbacks
{
    public static SeasonGaugeManager instance;
    [SerializeField] private Image[] gauges;
    [SerializeField] private TextMeshProUGUI[] text;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        NetworkManager.instance.onUpdateRoom_SeasonGauge = UpdateSeasonGauge;
        NetworkManager.instance.onUpdateRoom_IsCrashed = DisplaySeasonIsCrashed;
    }
    public void UpdateSeasonGauge()
    {
        int[] seasonGauges = NetworkManager.instance.GetSeasonGauge();

        for(int i=0; i<seasonGauges.Length; i++)
        {
            gauges[i].fillAmount = (seasonGauges[i] / 1000f / 3) + (0.333f * i);
            text[i].text = seasonGauges[i].ToString();
        }

        bool[] isCrashed = NetworkManager.instance.GetSeasonCrashed();
    }
    public void DisplaySeasonIsCrashed(int index)
    {
        Debug.Log($"{(Season)index}ÀÌ(°¡) ºØ±«Çß´Ù");
    }
    public void AddSpringValue(int value)
    {
        if(NetworkManager.instance.GetSeasonCrashed()[(int)Season.Spring])
        {
            return;
        }
        NetworkManager.instance.Enqueue_AddSeasonGauge(Season.Spring, value);
    }
    public void AddSummerValue(int value)
    {
        if (NetworkManager.instance.GetSeasonCrashed()[(int)Season.Summer])
        {
            return;
        }
        NetworkManager.instance.Enqueue_AddSeasonGauge(Season.Summer, value);
    }
    public void AddAutumnValue(int value)
    {
        if (NetworkManager.instance.GetSeasonCrashed()[(int)Season.Autumn])
        {
            return;
        }
        NetworkManager.instance.Enqueue_AddSeasonGauge(Season.Autumn, value);
    }
}
