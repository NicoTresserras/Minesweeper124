using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class SaveButtonImage : MonoBehaviour
{
    private string imagePath;

    void Start()
    {
        // Definir el path donde vamos a guardar la imagen
        imagePath = Path.Combine(Application.persistentDataPath, "buttonImage.png");

        // Obtener la imagen del botón (su imagen es un Sprite)
        Image buttonImage = this.gameObject.GetComponent<Image>();

        if (buttonImage != null && buttonImage.sprite != null)
        {
            // Convertir el sprite a una textura
            Texture2D texture = buttonImage.sprite.texture;

            // Guardar la textura como una imagen PNG
            SaveTextureAsPNG(texture, imagePath);
            Debug.Log("Imagen guardada en: " + imagePath);
        }
        else
        {
            Debug.LogError("El botón no tiene imagen asignada.");
        }
    }

    // Método para guardar la textura como PNG
    void SaveTextureAsPNG(Texture2D texture, string path)
    {
        // Convierte la textura en bytes PNG
        byte[] bytes = texture.EncodeToPNG();

        // Escribe los bytes en el archivo
        File.WriteAllBytes(path, bytes);
    }
}
