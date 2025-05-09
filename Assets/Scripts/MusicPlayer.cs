using System.Linq;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private string musicDirectory;

    private string[] canciones;
    private int currentSongIndex = 0;

    [SerializeField]
    private TextMeshProUGUI songNameText;

    [SerializeField]
    private Slider music_time_slider;

    [SerializeField]
    private TextMeshProUGUI music_time_text;

    [SerializeField]
    private TextMeshProUGUI music_Paused;

    [SerializeField]
    private TextMeshProUGUI music_Looped_text;

    [SerializeField]
    private bool music_Looped;

    void Start()
    {
        StartCoroutine(WaitForMusicReady());
    }

    IEnumerator WaitForMusicReady()
    {
        // Esperar hasta que la imagen esté lista
        while (!FileCreator.IsMusicReady)
        {
            yield return null; // Esto permite que Unity procese otros eventos
        }

        musicDirectory = Path.Combine(Application.persistentDataPath, "Music");

        canciones = GetCustomMusicList();

        if (canciones.Length > 0)
        {
            PlayCustomMusic(canciones[currentSongIndex]); // Reproducir la primera canción al iniciar
        }
    }

    private void PlayCustomMusic(string fileName)
    {
        string path = Path.Combine(musicDirectory, fileName);

        if (File.Exists(path))
        {
            if (IsValidMusicFile(fileName))
            {
                StartCoroutine(LoadAudioClip(path));
            }
            else
            {
                Debug.LogError("El archivo no es un formato de música válido: " + fileName);
            }
        }
        else
        {
            Debug.LogError("Archivo no encontrado: " + path);
        }
    }


    public bool IsValidMusicFile(string fileName)
    {
        string[] validExtensions = { ".mp3", ".ogg", ".wav" };
        string extension = Path.GetExtension(fileName).ToLower();
        return validExtensions.Contains(extension);
    }

    private IEnumerator LoadAudioClip(string filePath)
    {
        // Detectar el tipo de archivo según la extensión
        string extension = Path.GetExtension(filePath).ToLower();
        AudioType audioType;

        switch (extension)
        {
            case ".mp3":
                audioType = AudioType.MPEG;
                break;
            case ".ogg":
                audioType = AudioType.OGGVORBIS;
                break;
            case ".wav":
                audioType = AudioType.WAV;
                break;
            default:
                Debug.LogError($"Formato de archivo no soportado: {extension}");
                yield break;
        }

        // Crear la solicitud para cargar el archivo de audio
        UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + filePath, audioType);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error al cargar el archivo de audio: " + www.error);
        }
        else
        {
            AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
            audioSource.clip = clip;
            audioSource.Play();
            songNameText.text = Path.GetFileNameWithoutExtension(filePath);
            music_time_slider.maxValue = audioSource.clip.length;
        }
    }


    // Función para obtener la lista de canciones personalizadas en la carpeta Music
    public string[] GetCustomMusicList()
    {
        if (Directory.Exists(musicDirectory))
        {
            string[] files = Directory.GetFiles(musicDirectory)
                                      .Where(f => IsValidMusicFile(f))  // Filtra los archivos válidos.
                                      .ToArray();

            return files;  // Devuelve la lista de archivos válidos.
        }
        return new string[] { };  // Si no hay archivos, devuelve un array vacío.
    }

    public void NextSong()
    {
        if (canciones.Length > 0)
        {
            currentSongIndex = (currentSongIndex + 1) % canciones.Length;  // Aumentamos el índice y lo envolvemos
            PlayCustomMusic(canciones[currentSongIndex]);
        }
    }

    public void PreviousSong()
    {
        if (canciones.Length > 0)
        {
            currentSongIndex = (currentSongIndex - 1 + canciones.Length) % canciones.Length;  // Restamos el índice y lo envolvemos
            PlayCustomMusic(canciones[currentSongIndex]);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            NextSong();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            PreviousSong();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            PauseSong();
        }

        if (audioSource.isPlaying)
        {
            music_time_slider.enabled = true;
            music_time_text.enabled = true;
            music_time_slider.value = audioSource.time;
            music_time_text.text = FormatTime(audioSource.time) + " / " + FormatTime(audioSource.clip.length);
        }

        if (!audioSource.isPlaying && audioSource.time == 0f) // Si no está reproduciendo música
        {
            if (canciones.Length > 0)
            {
                if (music_Looped)
                {
                    PlayCustomMusic(canciones[currentSongIndex]);
                }
                else
                {
                    NextSong();
                }
            }
        }
    }

    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void PauseSong()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
            music_Paused.text = "Unpause";
        }
        else
        {
            audioSource.Play();
            music_Paused.text = "Pause";
        }
    }

    public void LoopSong()
    {
        if (music_Looped)
        {
            music_Looped = false;
            music_Looped_text.text = "Loop";
        }
        else
        {
            music_Looped = true;
            music_Looped_text.text = "Unloop";
        }
    }

    public void ShuffleSongs()
    {
        if (canciones.Length > 0)
        {
            System.Random rng = new System.Random();
            int n = canciones.Length;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                string value = canciones[k];
                canciones[k] = canciones[n];
                canciones[n] = value;
            }
            currentSongIndex = 0; // Reiniciar el índice de la canción actual
            PlayCustomMusic(canciones[currentSongIndex]); // Reproducir la primera canción en el orden barajado
        }
    }

    public void SetMusicTime()
    {
        audioSource.time = music_time_slider.value;
    }
}
