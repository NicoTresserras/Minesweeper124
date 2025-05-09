using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Newtonsoft.Json;
using static System.Net.WebRequestMethods;

public class OptionsLoader : MonoBehaviour
{
    [SerializeField]
    public Slider musicSlider;

    [SerializeField]
    public AudioSource musicPlayer;
    [SerializeField]
    public Slider brightnessSlider;
    [SerializeField]
    public Image overlay;
    [SerializeField]
    TextMeshProUGUI newID;
    [SerializeField]
    TextMeshProUGUI changeAccountID;


    public static Toggle helpToggle;
    public static Toggle nameToggle;
    public static TMP_InputField nameField;
    public static Canvas DatabaseError;
    [SerializeField]
    public static bool dbConnection = true;

    public static Settings Settings_Game;
    public static Account_Account account = new Account_Account();
    // Start is called before the first frame update

    public static readonly string apiUrl = "http://Minesweeper124.somee.com/MinesweeperAPI/api/";

        //"https://localhost:7006/api/";

    void Start()
    {
        Settings_Game = new Settings();
        DatabaseError = GameObject.Find("DatabaseError").GetComponent<Canvas>();
        if (helpToggle == null || nameToggle == null)
        {
            Toggle[] toggles = GameObject.FindObjectsOfType<Toggle>();

            foreach (Toggle toggle in toggles)
            {
                if (toggle.name == "Toggle Help")
                {
                    helpToggle = toggle;
                }
                else if (toggle.name == "Toggle Name")
                {
                    nameToggle = toggle;
                }
            }

            TMP_InputField[] inputFields = GameObject.FindObjectsOfType<TMP_InputField>();

            foreach (TMP_InputField inputField in inputFields)
            {
                if (inputField.name == "DefaultName")
                {
                    nameField = inputField;
                }
            }
        }
        if (dbConnection)
        {
            StartCoroutine(GetAccountData(SystemInfo.deviceUniqueIdentifier));
        }

    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetString("AccountID", Settings_Game.AccountID);
        PlayerPrefs.SetFloat("Volume", Settings_Game.Volume);
        PlayerPrefs.SetFloat("Brightness", Settings_Game.Brightness);
        PlayerPrefs.SetInt("Help", Settings_Game.ShowHelp ? 1 : 0);
        PlayerPrefs.SetInt("UploadGamesToDB", Settings_Game.UploadGamesToDB ? 1 : 0);
        PlayerPrefs.SetString("Name", Settings_Game.DefaultName ?? "");
        PlayerPrefs.Save();

        if (dbConnection)
        {
            StartCoroutine(PutSettings(Settings_Game));
        }
    }

    public void ChangeBrightness()
    {
        Settings_Game.Brightness = 1 - brightnessSlider.value;
        overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, Settings_Game.Brightness);
    }

    public void ChangeVolumne()
    {
        Settings_Game.Volume = musicSlider.value;
        musicPlayer.volume = Settings_Game.Volume;
    }

    public void ToggleHelp()
    {
        Settings_Game.ShowHelp = helpToggle.isOn;
    }

    public void ToggleDefaultName()
    {
        Settings_Game.UploadGamesToDB = nameToggle.isOn;
    }

    public IEnumerator GetAccountData(string deviceId)
    {
        string url = $"{apiUrl}Account_Account/{deviceId}";
        Debug.Log($"URL generada: {url}");

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error al realizar la solicitud: {www.error}");
                Debug.LogError($"Código de estado: {www.responseCode}");

                if (www.responseCode == 404)
                {
                    Debug.Log("Cuenta no encontrada. Creando una nueva cuenta...");
                    StartCoroutine(CreateAccount(deviceId));
                    StartCoroutine(PostSettings(Settings_Game));
                }
                else
                {
                    dbConnection = false;
                    DatabaseError.enabled = true;
                }

                UsePlayerPrefs();
            }
            else
            {
                string response = www.downloadHandler.text;
                Debug.Log($"Respuesta de la API: {response}");

                try
                {
                    Account_Account _account = JsonConvert.DeserializeObject<Account_Account>(response);

                    if (_account != null)
                    {
                        account = _account;
                        Settings_Game.AccountID = account.AccountID2;
                        StartCoroutine(GetSettings(account.AccountID2));
                    }
                    else
                    {
                        Debug.LogError("No se encontró ninguna cuenta.");
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error al deserializar la respuesta: {ex.Message}");
                }
            }
        }
    }

    

    private IEnumerator CreateAccount(string deviceId)
    {
        string accountAccountUrl = apiUrl + "Account_Account";
        string accountUrl = apiUrl + "Account";

        

        // Crear el objeto Account con el deviceId
        Account newAccount = new Account
        {
            AccountID = deviceId
        };

        // Serializar el objeto Account a JSON
        string jsonBodyAccount = JsonConvert.SerializeObject(newAccount);

        // Configurar la solicitud POST para Account
        using (UnityWebRequest www = new UnityWebRequest(accountUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBodyAccount);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            // Enviar la solicitud
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error al realizar la solicitud POST a Account: {www.error}");
                dbConnection = false;
                DatabaseError.enabled = true;
            }
            else
            {
                Debug.Log($"Cuenta Account creada exitosamente: {www.downloadHandler.text}");
            }
        }

        // Crear el objeto Account_Account con el deviceId en ambas partes
        Account_Account newAccountAccount = new Account_Account
        {
            AccountID1 = deviceId,
            AccountID2 = deviceId
        };

        // Serializar el objeto Account_Account a JSON
        string jsonBodyAccountAccount = JsonConvert.SerializeObject(newAccountAccount);

        // Configurar la solicitud POST para Account_Account
        using (UnityWebRequest www = new UnityWebRequest(accountAccountUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBodyAccountAccount);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            // Enviar la solicitud
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error al realizar la solicitud POST a Account_Account: {www.error}");
                dbConnection = false;
                DatabaseError.enabled = true;
                yield break;
            }
            else
            {
                Debug.Log($"Cuenta Account_Account creada exitosamente: {www.downloadHandler.text}");
            }
        }
    }


    public IEnumerator GetAndUpdateAccount(string deviceId)
    {
        string url = $"{apiUrl}Account_Account/{deviceId}";

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error al realizar la solicitud GET: {www.error}");
                dbConnection = false;
                DatabaseError.enabled = true;
            }
            else
            {
                string response = www.downloadHandler.text;
                Debug.Log($"Respuesta de la API: {response}");

                try
                {
                    // Deserializar la respuesta en un objeto Account_Account
                    Account_Account accountData = JsonConvert.DeserializeObject<Account_Account>(response);

                    if (accountData != null)
                    {
                        Debug.Log($"Cuenta encontrada: AccountID2 = {accountData.AccountID2}");
                        // Llamar a UpdateAccount con el AccountID2 obtenido
                        StartCoroutine(UpdateAccount(accountData.AccountID2));
                    }
                    else
                    {
                        Debug.LogError("No se encontró ninguna cuenta con el device ID proporcionado.");
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error al deserializar la respuesta: {ex.Message}");
                }
            }
        }
    }

    public IEnumerator UpdateAccount(string accountID2)
    {
        string url = apiUrl + "Account_Account";

        // Crear el objeto Account_Account con los valores actualizados
        Account_Account updatedAccount = new Account_Account
        {
            AccountID1 = SystemInfo.deviceUniqueIdentifier, // Convertir el hash del deviceId a un entero
            AccountID2 = accountID2
        };

        // Serializar el objeto a JSON
        string jsonBody = JsonConvert.SerializeObject(updatedAccount);

        // Configurar la solicitud PUT
        using (UnityWebRequest www = new UnityWebRequest(url, "PUT"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            // Enviar la solicitud
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                dbConnection = false;
                DatabaseError.enabled = true;
            }
            else
            {
                account.AccountID2 = accountID2;
                Settings_Game.AccountID = account.AccountID2;
                StartCoroutine(GetSettings(accountID2));
            }
        }
    }

    public void UsePlayerPrefs()
    {
        Settings_Game.AccountID = PlayerPrefs.GetString("AccountID", SystemInfo.deviceUniqueIdentifier);
        Settings_Game.Volume = PlayerPrefs.GetFloat("Volume", 0.2f);
        Settings_Game.Brightness = PlayerPrefs.GetFloat("Brightness", 0.5f);
        Settings_Game.ShowHelp = PlayerPrefs.GetInt("Help", 1) == 1;
        Settings_Game.UploadGamesToDB = PlayerPrefs.GetInt("UploadGamesToDB", 0) == 1;
        Settings_Game.DefaultName = PlayerPrefs.GetString("Name", "");

        ChangeSettingsValues();
    }

    public void ChangeSettingsValues()
    {
        musicSlider.value = Settings_Game.Volume;
        brightnessSlider.value = 1 - Settings_Game.Brightness;
        overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, Settings_Game.Brightness);
        helpToggle.isOn = Settings_Game.ShowHelp;
        nameToggle.isOn = Settings_Game.UploadGamesToDB;
        nameField.text = Settings_Game.DefaultName;
        musicPlayer.volume = Settings_Game.Volume;
    }

    public void ChangeDefaultName()
    {
        Settings_Game.DefaultName = nameField.text;
    }


    private IEnumerator GetSettings(string accountID)
    {
        string url = $"{apiUrl}Settings/{accountID}";

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error al realizar la solicitud GET: {www.error}");
                Debug.LogError(url);
                UsePlayerPrefs();
                if (www.responseCode == 404)
                {
                    StartCoroutine(PostSettings(Settings_Game));
                }
                ChangeSettingsValues();
            }
            else
            {
                string response = www.downloadHandler.text;
                Debug.Log($"Respuesta de la API (GET): {response}");

                try
                {
                    // Deserializar la respuesta en un objeto Settings
                    Settings_Game = JsonConvert.DeserializeObject<Settings>(response);
                    Settings_Game.AccountID = SystemInfo.deviceUniqueIdentifier;

                    // Actualizar los sliders y toggles
                    ChangeSettingsValues();
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error al deserializar la respuesta GET: {ex.Message}");


                }
            }
        }
    }

    private IEnumerator PostSettings(Settings settings)
    {
        string url = $"{apiUrl}" + "Settings";

        // Serializar el objeto Settings a JSON
        string jsonBody = JsonConvert.SerializeObject(settings);

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
                dbConnection = false;
                DatabaseError.enabled = true;
            }
            else
            {
                Debug.Log($"Configuración creada exitosamente: {www.downloadHandler.text}");

                try
                {
                    // Deserializar la respuesta en un objeto Settings
                    Settings_Game = JsonConvert.DeserializeObject<Settings>(www.downloadHandler.text);
                    Settings_Game.AccountID = SystemInfo.deviceUniqueIdentifier;

                    // Actualizar los sliders y toggles
                    ChangeSettingsValues();
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error al deserializar la respuesta POST: {ex.Message}");
                }
            }
        }
    }

    private IEnumerator PutSettings(Settings settings)
    {
        string url = $"{apiUrl}" + "Settings/" + settings.AccountID;

        // Serializar el objeto Settings a JSON
        string jsonBody = JsonConvert.SerializeObject(settings);

        using (UnityWebRequest www = new UnityWebRequest(url, "PUT"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error al realizar la solicitud PUT: {www.error}");
                dbConnection = false;
                DatabaseError.enabled = true;
            }
            else
            {
                Debug.Log($"Configuración actualizada exitosamente: {www.downloadHandler.text}");
            }
        }
    }



    public class Account_Account
    {
        public string AccountID1 { get; set; }
        public string AccountID2 { get; set; }
    }

    public class Account
    {
        public string AccountID { get; set; }


    }

    public class Settings
    {
        public Settings()
        {
            AccountID = SystemInfo.deviceUniqueIdentifier;
            Volume = 0.05f;
            Brightness = 0.5f;
            ShowHelp = true;
            UploadGamesToDB = false;
            DefaultName = "";
        }

        public string AccountID { get; set; }
        public float Volume { get; set; }
        public float Brightness { get; set; }
        public bool ShowHelp { get; set; }
        public bool UploadGamesToDB { get; set; }
        public string DefaultName { get; set; }
    }
}
