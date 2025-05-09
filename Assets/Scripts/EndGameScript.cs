using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameScript : MonoBehaviour
{
    public float holdThrow = 2f;
    float holdTimer;

    void Start()
    {
        holdTimer = holdThrow;
    }

    void Update()
    {
        //Si he ganado/perdido, el reinicio es instantaneo

        if (Input.GetKey(KeyCode.R))
        {
            if (Button_Script.Game_Won || Button_Script.Game_Lost)
            {
                MapGeneratorScript.IsMapReady = false;
                SceneManager.LoadScene("GameScene");
            }
            else
            {
                holdTimer -= Time.deltaTime;
                if (holdTimer < 0)
                {
                    MapGeneratorScript.IsMapReady = false;
                    SceneManager.LoadScene("GameScene");
                }
            }
        }
        else
        {
            holdTimer = holdThrow;
        }
    }

    private void FixedUpdate()
    {

    }
}
