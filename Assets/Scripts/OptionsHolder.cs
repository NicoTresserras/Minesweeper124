using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsHolder : MonoBehaviour
{
    [SerializeField]
    Canvas Options;
    [SerializeField]
    Image image;
    public static bool options_enabled = false;
    void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Options");

            if (objs.Length > 1)
            {
                Destroy(this.gameObject);
            }

            DontDestroyOnLoad(this.gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Options.enabled = !Options.enabled;
            options_enabled = Options.enabled;
            image.raycastTarget = Options.enabled;
        }
    }
}
