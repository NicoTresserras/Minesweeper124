using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Button_Dimensions : MonoBehaviour
{
    [SerializeField]
    TMP_InputField text_map;
    [SerializeField]
    TMP_InputField text_bombas;

    public void Set1()
    {
        text_map.text = "10";
        text_bombas.text = "2";
    }

    public void Set2()
    {
        text_map.text = "16x16";
        text_bombas.text = "10";
    }

    public void Set3()
    {
        text_map.text = "4x4x4";
        text_bombas.text = "4";
    }

    public void Set4()
    {
        text_map.text = "4x4x4x4";
        text_bombas.text = "5";
    }
}
