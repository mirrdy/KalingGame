using System.Collections;
using System.Collections.Generic;
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
    }
    public void SetWeatherGauge(Weather weather, float value)
    {
        int weatherIndex = (int)weather;
        int indexToGet = (weatherIndex + 2) % 3;
        float amount = (value / 1000 / 3);

        gauges[weatherIndex].fillAmount = amount;
        if(gauges[weatherIndex].fillAmount > 0.333f * weatherIndex)
        {
            gauges[weatherIndex].fillAmount = 0.333f * weatherIndex;
        }

        gauges[indexToGet].fillAmount -= amount;
        if (gauges[indexToGet].fillAmount > 0.333f * indexToGet)
        {
            gauges[indexToGet].fillAmount = 0.333f * indexToGet;
        }

        switch (weather)
        {
            case Weather.Spring:
                if (amount > 0.333f)
                {
                    amount = 0.333f;
                }
                break;
            case Weather.Summer:
                if (amount > 0.666f)
                {
                    amount = 0.666f;
                }
                break;
            case Weather.Autumn:
                if (amount > 1)
                {
                    amount = 1;
                }
                break;
            default:
                break;
        }
    }
    private float GetCorrectionGauge(int index, float value)
    {
        if (gauges[index].fillAmount + value > 0.333f * index)
        {
            return 0.333f * index;
        }

        return value;
    }
    public void AddSpringValue(float value)
    {
        SetWeatherGauge(Weather.Spring, value);
    }
    public void AddSummerValue(float value)
    {
        SetWeatherGauge(Weather.Summer, value);
    }
    public void AddAutumnValue(float value)
    {
        SetWeatherGauge(Weather.Autumn, value);
    }
}
