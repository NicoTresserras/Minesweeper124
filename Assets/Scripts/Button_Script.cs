using JetBrains.Annotations;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Button_Script : MonoBehaviour, IPointerClickHandler
{
    private Position position = new Position();
    private TextMeshProUGUI Text;
    [SerializeField]
    private List<Button_Script> neighbours = new List<Button_Script>();
    GameObject[] buttons;
    static public bool Game_Won = false;
    static public bool Game_Lost = false;
    [SerializeField]
    private Button buttonGuanyar;
    [SerializeField]
    private Button buttonPerdre;
    [SerializeField]
    private Image button_sprite;

    private static Sprite button_0;
    private static Sprite button;
    private static Sprite bomb;
    private static Sprite flag;
    private static Sprite exploding_bomb;
    // Start is called before the first frame update
    void Start()
    {
        Text = this.gameObject.GetComponentInChildren<TextMeshProUGUI>();
        Text.text = string.Empty;
        Game_Lost = false;
        Game_Won = false;
        StartCoroutine(MapCreated());
        StartCoroutine(WaitForImageReady());
        buttons = GameObject.FindGameObjectsWithTag("Button");

        button_sprite.sprite = button;

        Debug.Log(this.name + ": " + transform.position.x + "/" + transform.position.y);
    }

    IEnumerator MapCreated()
    {

        // Esperar hasta que el mapa esté listo
        while (!MapCreator.IsMapCreated)
        {
            yield return null; // Esto permite que Unity procese otros eventos
        }

        if (MapCreator.NumSquares > 1)
        {
            GetNeighbors();
        }
        else
        {
            GetNeighbors2D();
        }
    }
    IEnumerator WaitForImageReady()
    {
        // Esperar hasta que la imagen esté lista
        while (!FileCreator.IsImageReady)
        {
            yield return null; // Esto permite que Unity procese otros eventos
        }
    }

    public void GetNeighbors()
    {
        // Extraer las coordenadas del nombre del botón
        string nom = this.gameObject.name.Substring(6); // Quitar el prefijo "button"
        string[] coords = nom.Split('.'); // Dividir las coordenadas por el separador '.'

        int x = int.Parse(coords[0]); // Primera coordenada = X
        int y = int.Parse(coords[1]); // Segunda coordenada = Y
        int z = int.Parse(coords[2]); // Tercera coordenada = Z
        int w = int.Parse(coords[3]); // Cuarta coordenada = W

        // Calcular los vecinos dentro de los límites del mapa
        for (int i = Mathf.Max(x - 1, 0); i <= Mathf.Min(x + 1, MapCreator.NumCubes - 1); i++) // Limitar el rango de X
        {
            for (int j = Mathf.Max(y - 1, 0); j <= Mathf.Min(y + 1, MapCreator.NumSquares - 1); j++) // Limitar el rango de Y
            {
                for (int k = Mathf.Max(z - 1, 0); k <= Mathf.Min(z + 1, MapCreator.NumLines - 1); k++) // Limitar el rango de Z
                {
                    for (int h = Mathf.Max(w - 1, 0); h <= Mathf.Min(w + 1, MapCreator.NumButtons - 1); h++) // Limitar el rango de W
                    {
                        // Construir el nombre del botón vecino
                        string buttonName = $"button{i}.{j}.{k}.{h}";
                        GameObject button = GameObject.Find(buttonName);

                        // Verificar si el botón existe y no es el mismo que el actual
                        if (button != null && this.gameObject.name != button.name)
                        {
                            Button_Script button_script = button.GetComponent<Button_Script>();
                            neighbours.Add(button_script); // Agregar a los vecinos
                            Debug.Log($"Vecino encontrado: {buttonName}");
                        }
                        else
                        {
                            if (this.gameObject.name != buttonName)
                            {
                                Debug.Log($"ERROR: No se encontró el botón {buttonName}");
                            }
                        }
                    }
                }
            }
        }
    }


    public void GetNeighbors2D()
    {
        // Extraer las coordenadas X e Y del nombre del botón
        string nom = this.gameObject.name.Substring(6); // Quitar el prefijo "button"
        string[] coords = nom.Split('.'); // Dividir las coordenadas por el separador '.'

        int x = int.Parse(coords[2]); // Tercer valor = X
        int y = int.Parse(coords[3]); // Cuarto valor = Y

        // Calcular los vecinos dentro de los límites del mapa
        for (int i = Mathf.Max(x - 1, 0); i <= Mathf.Min(x + 1, MapCreator.NumLines - 1); i++) // Limitar el rango de X
        {
            for (int j = Mathf.Max(y - 1, 0); j <= Mathf.Min(y + 1, MapCreator.NumButtons - 1); j++) // Limitar el rango de Y
            {
                // Construir el nombre del botón vecino
                string buttonName = $"button0.0.{i}.{j}";
                GameObject button = GameObject.Find(buttonName);

                // Verificar si el botón existe y no es el mismo que el actual
                if (button != null && this.gameObject.name != button.name)
                {
                    Button_Script button_script = button.GetComponent<Button_Script>();
                    neighbours.Add(button_script); // Agregar a los vecinos
                    Debug.Log($"Vecino encontrado: {buttonName}");
                }
                else
                {
                    if (this.gameObject.name != buttonName)
                    {
                        Debug.Log($"ERROR: No se encontró el botón {buttonName}");
                    }
                }
            }
        }
    }



    public static void GetImages(string path)
    {
        byte[] byte_button_0 = File.ReadAllBytes(Path.Combine(path, "button_0" + ".png"));
        byte[] byte_button = File.ReadAllBytes(Path.Combine(path, "button" + ".png"));
        byte[] byte_bomb = File.ReadAllBytes(Path.Combine(path, "bomb" + ".png"));
        byte[] byte_explodingbomb = File.ReadAllBytes(Path.Combine(path, "exploding_bomb" + ".png"));
        byte[] byte_flag = File.ReadAllBytes(Path.Combine(path, "flag" + ".png"));

        // Crear un Texture2D a partir de los bytes
        Texture2D texture_button_0 = new Texture2D(2, 2);
        Texture2D texture_button = new Texture2D(2, 2);
        Texture2D texture_bomb = new Texture2D(2, 2);
        Texture2D texture_explodingbomb = new Texture2D(2, 2);
        Texture2D texture_flag = new Texture2D(2, 2);

        texture_button_0.LoadImage(byte_button_0);
        texture_button.LoadImage(byte_button);
        texture_bomb.LoadImage(byte_bomb);
        texture_explodingbomb.LoadImage(byte_explodingbomb);
        texture_flag.LoadImage(byte_flag);

        // Convertir la textura a un Sprite
        button_0 = Sprite.Create(texture_button_0, new Rect(0, 0, texture_button_0.width, texture_button_0.height), new Vector2(0.5f, 0.5f));
        button = Sprite.Create(texture_button, new Rect(0, 0, texture_button.width, texture_button.height), new Vector2(0.5f, 0.5f));
        bomb = Sprite.Create(texture_bomb, new Rect(0, 0, texture_bomb.width, texture_bomb.height), new Vector2(0.5f, 0.5f));
        exploding_bomb = Sprite.Create(texture_explodingbomb, new Rect(0, 0, texture_explodingbomb.width, texture_explodingbomb.height), new Vector2(0.5f, 0.5f));
        flag = Sprite.Create(texture_flag, new Rect(0, 0, texture_flag.width, texture_flag.height), new Vector2(0.5f, 0.5f));

        Debug.Log("GetImagesFet");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Game_Won || Game_Lost)
        {
            return;
        }

        foreach (GameObject button in buttons)
        {
            button.GetComponent<Button_Script>().ResetColor();
        }
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftClick();
        }
        else if (eventData.button == PointerEventData.InputButton.Middle)
        {
            OnMiddleClick();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnRightClick();
        }

        foreach (GameObject button in buttons)
        {
            Button_Script button_script = button.GetComponent<Button_Script>();
            int no_visibles = 0;
            int banderas = 0;
            if (button_script.Visible && button_script.Valor > 0)
            {
                foreach (Button_Script neighbour in button_script.neighbours)
                {
                    if (!neighbour.Visible && !neighbour.Bandera)
                    {
                        no_visibles++;
                    }

                    if (neighbour.Bandera)
                    {
                        banderas++;
                    }
                }
                if (button_script.Valor == banderas && no_visibles > 0)
                {
                    button_script.ColorLightPurple();
                }
                else if (button_script.Valor - banderas == no_visibles && no_visibles > 0)
                {
                    button_script.ColorLightBlue();
                }
            }
        }

        CheckIfLost();
        CheckIfWin();

    }

    public void OnLeftClick()
    {
        if (!this.Visible)
        {
            if (!this.Bandera)
            {
                SetVisible();
                if (this.Valor == 0)
                {
                    // Realizar la recursión de manera asíncrona
                    foreach (Button_Script button in neighbours)
                    {
                        if (!button.Visible)
                        {
                            // Llamada recursiva pero usando una corutina para evitar el bloqueo
                            button.OnLeftClick();
                        }
                    }
                }
            }
        }
        else
        {
            foreach (Button_Script button in neighbours)
            {
                if (!button.Visible)
                {
                    button.GetComponent<Image>().color = new Color(255 / 255, 125 / 255, 125 / 255);
                }
            }
        }
    }


    public void OnRightClick()
    {
        foreach (GameObject button in buttons)
        {
            button.GetComponent<Button_Script>().ResetColor();
        }
        if (!this.position.visible)
        {
            ToggleBandera();
        }
    }

    public void OnMiddleClick()
    {
        if (this.position.visible)
        {

            int banderas = 0;
            foreach (Button_Script button in neighbours)
            {
                if (button.Bandera)
                {
                    banderas++;
                }
            }

            if (banderas == this.position.valor)
            {
                foreach (Button_Script button in neighbours)
                {
                    if (!button.Bandera && !button.Visible)
                    {
                        button.OnLeftClick();
                    }
                }
            }
        }
    }


    public void ColorLightPurple()
    {
        if (OptionsLoader.Settings_Game.ShowHelp)
        {
            this.GetComponent<Image>().color = new Color(255f / 255f, 125f / 255f, 255f / 255f);
        }
    }
    public void ColorLightBlue()
    {
        if (OptionsLoader.Settings_Game.ShowHelp)
        {
            this.GetComponent<Image>().color = new Color(125f / 255f, 125f / 255f, 255f / 255f);
        }
    }
    public void ColorRed()
    {
        this.GetComponent<Image>().color = new Color(255 / 255, 125 / 255, 125 / 255);
    }
    public void ResetColor()
    {
        this.GetComponent<Image>().color = new Color(255f / 255f, 255f / 255f, 255f / 255f);
    }

    public void SetVisible()
    {
        this.Visible = true;
        this.ResetColor();
    }

    public void ToggleBandera()
    {
        this.Bandera = !this.Bandera;
    }

    public void SetValue(int value, bool visible)
    {

        position.valor = value;
        position.visible = false;
        UpdateText();
    }

    public bool Visible
    {
        get { return position.visible; }
        set
        {
            position.visible = value;
            UpdateText();
        }
    }

    public bool Bandera
    {
        get
        {
            return position.bandera;
        }
        set
        {
            position.bandera = value;
            UpdateBandera();
        }
    }

    public int Valor
    {
        get { return position.valor; }
    }

    private void UpdateText()
    {
        if (position.visible)
        {
            if (position.valor == -1)
            {
                button_sprite.sprite = exploding_bomb;
                Game_Lost = true;
            }
            else if (position.valor == 0)
            {
                button_sprite.sprite = button_0;
            }
            else
            {
                Text.text = position.valor.ToString();
            }
        }


    }

    private void UpdateBandera()
    {
        if (position.bandera)
        {
            button_sprite.sprite = flag;
        }
        else
        {
            button_sprite.sprite = button;
        }
    }

    private void CheckIfLost()
    {

        if (Game_Lost)
        {
            Button button_perdre = Instantiate(buttonPerdre);
            button_perdre.transform.SetParent(GameObject.Find("Canvas").transform);
            button_perdre.transform.position = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            button_perdre.transform.lossyScale.Set((float)(Screen.width) / 1920f, (float)(Screen.height) / 1080f, 1f);

            foreach (GameObject button in buttons)
            {
                Button_Script button_Script = button.GetComponent<Button_Script>();
                if (button_Script.Valor == -1 && !button_Script.Bandera && !button_Script.Visible)
                {
                    button.GetComponent<Button_Script>().UpdateText();
                }
            }
        }
    }

    private void CheckIfWin()
    {
        if (!Game_Lost && !Game_Won)
        {
            int num_visibles = 0;
            int num_banderas = 0;
            foreach (GameObject button in buttons)
            {
                Button_Script button_script = button.GetComponent<Button_Script>();

                if (button_script.Visible)
                {
                    num_visibles++;
                }
                if (button_script.Bandera)
                {
                    num_banderas++;
                }
            }

            if ((num_visibles + num_banderas) == buttons.Length && MapGeneratorScript.bombas == num_banderas)
            {
                Button button_guanyar = Instantiate(buttonGuanyar);
                button_guanyar.transform.SetParent(GameObject.Find("Canvas").transform);
                button_guanyar.transform.position = new Vector3(Screen.width / 2, Screen.height / 2, 0);
                button_guanyar.transform.lossyScale.Set((float)(Screen.width) / 1920f, (float)(Screen.height) / 1080f, 1f);
                Game_Won = true;

                if (OptionsLoader.dbConnection)
                {
                    if (OptionsLoader.Settings_Game.UploadGamesToDB)
                    {
                        StartCoroutine(UploadWin());
                    }
                }
            }
        }
    }

    private IEnumerator UploadWin()
    {
        string url = "http://Minesweeper124.somee.com/MinesweeperAPI/api/Point";
            
            //"https://localhost:7006/api/Point";

        Points game = new Points
        {
            AccountID1 = OptionsLoader.account.AccountID1,
            AccountID2 = OptionsLoader.account.AccountID2,
            Mapa = MapGeneratorScript.mapsize,
            Bombes = MapGeneratorScript.bombas,
            TempsNecessari = TimerScript.Timer,
            Nom = OptionsLoader.Settings_Game.DefaultName,
            Date = DateTime.Now
        };

        string jsonBody = JsonConvert.SerializeObject(game);

        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            // Enviar la solicitud
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error al realizar la solicitud POST: {www.error}");
                Debug.LogError($"Respuesta del servidor: {www.downloadHandler.text}");
            }
            else
            {
                Debug.Log($"Datos del juego subidos exitosamente: {www.downloadHandler.text}");
            }
        }
    }

    public class Points
    {
        public int ID { get; set; }
        public string AccountID1 { get; set; }
        public string AccountID2 { get; set; }
        public string Mapa { get; set; }
        public int Bombes { get; set; }
        public TimeSpan TempsNecessari { get; set; }
        public string Nom { get; set; }
        public DateTime Date { get; set; }
    }
}
