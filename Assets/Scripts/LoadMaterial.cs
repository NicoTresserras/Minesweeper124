using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LoadMaterial : MonoBehaviour
{
    [SerializeField]
    MeshRenderer meshRenderer;
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
        if (File.Exists(path))
        {
            byte[] imageBytes = File.ReadAllBytes(path);

            // Crear un Texture2D a partir de los bytes
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(imageBytes);

            // Crear un Material y asignar la textura
            Material material = new Material(Shader.Find("Standard"));
            material.mainTexture = texture;


            if (meshRenderer != null)
            {
                meshRenderer.material = material;
            }
            else
            {
                Debug.LogError("MeshRenderer no encontrado en el objeto.");
                Debug.LogError(this.gameObject.name);
            }
        }
        else
        {
            Debug.LogError("La imagen no se encuentra en la ruta especificada.");
        }
    }
}
