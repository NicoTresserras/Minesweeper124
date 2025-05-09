using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class LoadImage : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaitForImageReady());
    }

    IEnumerator WaitForImageReady()
    {
        // Esperar hasta que la imagen esté lista
        while (!FileCreator.IsImageReady)
        {
            yield return null; // Esto permite que Unity procese otros eventos
        }

        GetImage(Application.persistentDataPath + "/Image/" + gameObject.name + ".png");
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void GetImage(string path)
    {
        Image panel = GetComponent<Image>();
        if (File.Exists(path))
        {
            byte[] imageBytes = File.ReadAllBytes(path);

            // Crear un Texture2D a partir de los bytes
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(imageBytes);

            // Convertir la textura a un Sprite
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

            // Asignar el sprite al Image del panel en el que está adjunto este script
            
            if (panel != null)
            {
                panel.sprite = sprite;
                panel.enabled = true;
            }
        }
        else
        {
            Debug.LogError("La imagen no se encuentra en la ruta especificada.");
            Debug.LogError(this.gameObject.name);

            if (panel != null)
            {
                panel.color = new Color(Color.clear.r, Color.clear.g, Color.clear.b, 0);
            }
        }
    }
}
