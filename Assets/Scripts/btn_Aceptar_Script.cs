using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class btn_Aceptar_Script : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI button_map;
    [SerializeField]
    TextMeshProUGUI button_bombas;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetMap()
    {
        string mapa = button_map.text;
        if (!string.IsNullOrEmpty(mapa))
        {
            Debug.Log(button_map.text);
            string txt_bombas = button_bombas.text.Trim();
             txt_bombas = txt_bombas.Substring(0, txt_bombas.Length - 1);
            if (int.TryParse(txt_bombas, out int bombas))
            {
                Debug.Log(bombas.ToString());
                mapa = mapa.Substring(0, mapa.Length - 1);
                MapGeneratorScript.SetMap(mapa);
                MapGeneratorScript.SetBombas(bombas);
                SceneManager.LoadScene("GameScene");
            }

        }
    }
}
