using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu_Button_Script : MonoBehaviour
{
    [SerializeField]
    Canvas options;

    [SerializeField]
    Canvas songs;

    [SerializeField]
    Canvas CC;

    [SerializeField]
    Image Blocker;

    [SerializeField]
    Canvas ChangeAccount;

    [SerializeField]
    Canvas AddAccount;

    [SerializeField]
    Canvas Tutorial;

    [SerializeField]
    TextMeshProUGUI newID;

    [SerializeField]
    TextMeshProUGUI changeAccountID;

    [SerializeField]
    TextMeshProUGUI newIDInputText;

    [SerializeField]
    TextMeshProUGUI text1;

    [SerializeField]
    TextMeshProUGUI text2;

    [SerializeField]
    TextMeshProUGUI text3;

    [SerializeField]
    TextMeshProUGUI text4;

    [SerializeField]
    Image TutorialBlocker;

    [SerializeField]
    string link;
    // Start is called before the first frame update
    void Start()
    {
        options = GameObject.Find("Options").GetComponent<Canvas>();
        songs = GameObject.Find("Songs").GetComponent<Canvas>();
        CC = GameObject.Find("CC").GetComponent<Canvas>();
        Blocker = GameObject.Find("Blocker").GetComponent<Image>();
        ChangeAccount = GameObject.Find("ChangeAccount").GetComponent<Canvas>();
        AddAccount = GameObject.Find("AddAccount").GetComponent<Canvas>();
        if (SceneManager.GetActiveScene().name == "Start")
        {
            TutorialBlocker = GameObject.Find("TutorialBlocker").GetComponent<Image>();
            Tutorial = GameObject.Find("Tutorial").GetComponent<Canvas>();
        }
    }

    public void OpenLink()
    {
        System.Diagnostics.Process.Start(link);
    }
    public void PlayGame()
    {
        SceneManager.LoadScene("Menu");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void PlayTutorial()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void AcceptPreviousID()
    {
        if (!OptionsLoader.DatabaseError)
        {
            string text = newID.text.Trim((char)8203);
            if (!String.IsNullOrEmpty(text))
            {
                // Create an instance of OptionsLoader to call the instance method
                OptionsLoader optionsLoaderInstance = FindObjectOfType<OptionsLoader>();
                if (optionsLoaderInstance != null)
                {
                    StartCoroutine(optionsLoaderInstance.GetAndUpdateAccount(newID.text));
                }
                else
                {
                    changeAccountID.text = "The new ID has not been found";
                }
            }
            else
            {
                changeAccountID.text = "Please enter a new ID";
            }
        }
        else
        {
            changeAccountID.text = "There is a database error";
        }
    }

    public void GotoSongs()
    {
        if (songs is null || Blocker is null)
        {
            songs = GameObject.Find("Songs").GetComponent<Canvas>();
            Blocker = GameObject.Find("Blocker").GetComponent<Image>();
        }
        songs.enabled = !songs.enabled;
        Blocker.raycastTarget = !Blocker.raycastTarget;
    }

    public void GotoTutorial()
    {
        if (Tutorial is null || Blocker is null)
        {
            Tutorial = GameObject.Find("Tutorial").GetComponent<Canvas>();
            Blocker = GameObject.Find("Blocker").GetComponent<Image>();
        }
        Tutorial.enabled = !Tutorial.enabled;
        TutorialBlocker.raycastTarget = !TutorialBlocker.raycastTarget;
    }

    public void Options()
    {
        if (options is null || Blocker is null)
        {
            options = GameObject.Find("Options").GetComponent<Canvas>();
            Blocker = GameObject.Find("Blocker").GetComponent<Image>();
        }
        options.enabled = !options.enabled;
        Blocker.raycastTarget = !Blocker.raycastTarget;
    }

    public void OptionsMenu()
    {
        if (SceneManager.GetActiveScene().name == "Start")
        {
            options.enabled = !options.enabled;
            Blocker.raycastTarget = !Blocker.raycastTarget;
        }
        else
        {
            options.enabled = false;
            Blocker.raycastTarget = false;
            SceneManager.LoadScene("Start");
        }
    }

    public void GotoCC()
    {
        if (CC is null)
        {
            CC = GameObject.Find("CC").GetComponent<Canvas>();
        }
        CC.enabled = !CC.enabled;
    }

    public void GotoChangeAccount()
    {
        if (ChangeAccount is null)
        {
            ChangeAccount = GameObject.Find("ChangeAccount").GetComponent<Canvas>();
        }
        ChangeAccount.enabled = !ChangeAccount.enabled;
    }

    public void GotoAddAccount()
    {
        if (AddAccount is null)
        {
            AddAccount = GameObject.Find("AddAccount").GetComponent<Canvas>();
        }
        AddAccount.enabled = !AddAccount.enabled;
    }

    public void ChangeAccountBack()
    {
        if (ChangeAccount is null)
        {
            ChangeAccount = GameObject.Find("ChangeAccount").GetComponent<Canvas>();
        }
        ChangeAccount.enabled = false;
    }

    public void AddAccountBack()
    {
        if (ChangeAccount is null)
        {
            ChangeAccount = GameObject.Find("ChangeAccount").GetComponent<Canvas>();
        }
        ChangeAccount.enabled = false;

        if (AddAccount is null)
        {
            AddAccount = GameObject.Find("AddAccount").GetComponent<Canvas>();
        }
        AddAccount.enabled = false;
    }

    public void CCBack()
    {
        if (CC is null)
        {
            CC = GameObject.Find("CC").GetComponent<Canvas>();
        }
        CC.enabled = false;
    }

    public void TutorialBack()
    {
        if (Tutorial is null)
        {
            Tutorial = GameObject.Find("Tutorial").GetComponent<Canvas>();
        }
        Tutorial.enabled = false;
        TutorialBlocker.raycastTarget = false;
    }

    public void GoToSubTutorial()
    {
        GameObject hola = GameObject.Find(this.gameObject.name.Substring(6));
        if (hola is not null)
        {
            hola.GetComponent<Canvas>().enabled = true;
        }
        else
        {
            Debug.Log("No se ha encontrado el objeto: " + this.gameObject.name.Substring(6));
        }
    }

    public void SubTutorialBack()
    {
        this.gameObject.GetComponentInParent<Canvas>().enabled = false;
    }

    public void OpenSongFolder()
    {
        string path = Application.persistentDataPath + "/Music";
        System.Diagnostics.Process.Start(path);
    }

    public void OpenImageFolder()
    {
        string path = Application.persistentDataPath + "/Image";
        System.Diagnostics.Process.Start(path);
    }

    public void CopyID()
    {
        TextEditor te = new TextEditor();
        te.text = SystemInfo.deviceUniqueIdentifier;
        te.SelectAll();
        te.Copy();

        Debug.Log(SystemInfo.deviceUniqueIdentifier);
        changeAccountID.text = "ID Copied to Clipboard";
    }

    public void ResetID()
    {
        string ID = SystemInfo.deviceUniqueIdentifier;

        // Create an instance of OptionsLoader to call the instance method
        OptionsLoader optionsLoaderInstance = FindObjectOfType<OptionsLoader>();
        if (optionsLoaderInstance != null)
        {
            StartCoroutine(optionsLoaderInstance.GetAndUpdateAccount(ID));
            newIDInputText.text = "ID has been reset";
        }
    }

    public void ToggleTextHelp()
    {
        text1.enabled = !text1.enabled;
        text2.enabled = !text2.enabled;
        text3.enabled = !text3.enabled;
        text4.enabled = !text4.enabled;
    }
}
