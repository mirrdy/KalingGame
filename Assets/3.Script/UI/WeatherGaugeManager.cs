using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum Weather
{
    Spring, //yellow
    Summer, //blue
    Autumn  //purple
}
public class WeatherGaugeManager : MonoBehaviour
{
    public static WeatherGaugeManager instance;

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
        NetworkManager.instance.onUpdateRoom_WeatherGauge = UpdateWeatherGauge;
        NetworkManager.instance.onUpdateRoom_IsCrashed = DisplayWeatherIsCrashed;
    }
    public void UpdateWeatherGauge()
    {
        int[] weatherGauges = NetworkManager.instance.GetWeatherGauge();

        for(int i=0; i<weatherGauges.Length; i++)
        {
            gauges[i].fillAmount = (weatherGauges[i] / 1000f / 3) + (0.333f * i);
            text[i].text = weatherGauges[i].ToString();
        }

        bool[] isCrashed = NetworkManager.instance.GetWeatherCrashed();
    }
    public void DisplayWeatherIsCrashed(int index)
    {
        Debug.Log($"{(Weather)index}ÀÌ(°¡) ºØ±«Çß´Ù");
    }
    public void AddSpringValue(int value)
    {
        if(NetworkManager.instance.GetWeatherCrashed()[(int)Weather.Spring])
        {
            return;
        }
        NetworkManager.instance.AddWeatherGauge(Weather.Spring, value);
    }
    public void AddSummerValue(int value)
    {
        if (NetworkManager.instance.GetWeatherCrashed()[(int)Weather.Summer])
        {
            return;
        }
        NetworkManager.instance.AddWeatherGauge(Weather.Summer, value);
    }
    public void AddAutumnValue(int value)
    {
        if (NetworkManager.instance.GetWeatherCrashed()[(int)Weather.Autumn])
        {
            return;
        }
        NetworkManager.instance.AddWeatherGauge(Weather.Autumn, value);
    }
}
