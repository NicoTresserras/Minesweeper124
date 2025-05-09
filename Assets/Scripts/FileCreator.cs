using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileCreator : MonoBehaviour
{

    [SerializeField]
    private string musicDirectory;
    [SerializeField]
    private string ImageDirectory;

    public static bool IsImageReady = false;
    public static bool IsMusicReady = false;
    // Start is called before the first frame update
    void Start()
    {
        musicDirectory = Path.Combine(Application.persistentDataPath, "Music");

        if (!Directory.Exists(musicDirectory))
        {
            Directory.CreateDirectory(musicDirectory);
            CopyMusicFiles();
        }

        IsMusicReady = true;

        ImageDirectory = Path.Combine(Application.persistentDataPath, "Image");

        if (!Directory.Exists(ImageDirectory))
        {
            Directory.CreateDirectory(ImageDirectory);
            CopyImageFiles();
        }

        Button_Script.GetImages(Path.Combine(Application.persistentDataPath, "Image"));


        IsImageReady = true;

        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void CopyImageFiles()
    {
        string sourceDirectory = Path.Combine(Application.streamingAssetsPath, "Image");
        if (Directory.Exists(sourceDirectory))
        {
            string[] imageFiles = Directory.GetFiles(sourceDirectory);
            foreach (string file in imageFiles)
            {
                if (file.EndsWith(".meta"))
                {
                    continue;
                }
                string fileName = Path.GetFileName(file);
                string destinationPath = Path.Combine(ImageDirectory, fileName);
                if (!File.Exists(destinationPath))
                {
                    File.Copy(file, destinationPath);
                    Debug.Log($"Archivo copiado: {fileName}");
                }
            }
        }
    }

    private void CopyMusicFiles()
    {
        string sourceDirectory = Path.Combine(Application.streamingAssetsPath, "Music"); // Directorio de origen en "Assets/StreamingAssets/Music"

        if (Directory.Exists(sourceDirectory))
        {
            // Obtiene todos los archivos de música en la carpeta de streaming assets
            string[] musicFiles = Directory.GetFiles(sourceDirectory);

            foreach (string file in musicFiles)
            {
                // Ignorar archivos .meta
                if (file.EndsWith(".meta"))
                {
                    continue;
                }

                string fileName = Path.GetFileName(file);
                string destinationPath = Path.Combine(musicDirectory, fileName);

                // Copiar los archivos al directorio de destino (Application.persistentDataPath/Music)
                if (!File.Exists(destinationPath))
                {
                    File.Copy(file, destinationPath);
                    Debug.Log($"Archivo copiado: {fileName}");
                }
            }
        }
        else
        {
            Debug.LogError("La carpeta de música no existe: " + sourceDirectory);
        }
    }
}
