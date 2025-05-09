using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerScript : MonoBehaviour
{
    private static int seconds = 0;
    private static int minutes = 0;
    private static int hours = 0;
    private static int days = 0;

    private bool isTimerRunning = false;
    private float elapsedTime = 0f; 

    [SerializeField]
    private TextMeshProUGUI timerText;

    public static TimeSpan Timer {
        get {
            return new TimeSpan(days, hours, minutes, seconds);
        } 
    }

    void Start()
    {
        days = 0;
        hours = 0;
        minutes = 0;
        seconds = 0;
        StartCoroutine(CheckMapReady());
    }

    IEnumerator CheckMapReady()
    {
        // Espera hasta que el mapa esté listo
        while (!MapGeneratorScript.IsMapReady)
        {
            yield return null;
        }

        isTimerRunning = true;
        StartCoroutine(StartTimer());
    }

    IEnumerator StartTimer()
    {
        while (isTimerRunning)
        {
            // Solo incrementamos el tiempo si las condiciones son adecuadas
            if (!OptionsHolder.options_enabled && !Button_Script.Game_Won && !Button_Script.Game_Lost)
            {

                elapsedTime += Time.deltaTime;

                if (elapsedTime >= 1f)
                {
                    IncrementTime();
                    elapsedTime -= 1f; 
                }
            }

            yield return null; 
        }
    }

    private void IncrementTime()
    {
        seconds++;

        if (seconds >= 60)
        {
            seconds = 0;
            minutes++;
        }

        if (minutes >= 60)
        {
            minutes = 0;
            hours++;
        }

        if (hours >= 24)
        {
            hours = 0;
            days++;
        }

        timerText.text = days.ToString().PadLeft(2, '0') + ":" + hours.ToString().PadLeft(2, '0') + ":" +
            minutes.ToString().PadLeft(2, '0') + ":" + seconds.ToString().PadLeft(2, '0');
    }
}
