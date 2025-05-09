using TMPro;
using UnityEngine;

public class InputText_Script : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField inputField;
    [SerializeField]
    static private int number_of_x;
    // Start is called before the first frame update
    string text;
    void Start()
    {
        text = inputField.text;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnValueChanged()
    {
        text = inputField.text;
        Debug.Log(text.Length);
        number_of_x = 0;
        if (text.Length > 0)
        {
            foreach (var chr in text)
            {
                if (chr == 'x')
                {
                    number_of_x++;
                }
            }
            if (text[0] == 'x')
            {
                inputField.text = "";
            }

            if (text.Length <= 1)
            {
                if (!int.TryParse(text.ToString(), out int num) || num == 0)
                {
                    inputField.text = "";
                }
            }
            else
            {
                for (global::System.Int32 i = text.Length - 1; i >= 1; i--)
                {
                    if ((text[i] == 'x' && text[i - 1] == 'x') || (text[i] != 'x' && !int.TryParse(text[i].ToString(), out int num)) || (text[i] == 'x' && number_of_x > 3))
                    {
                        inputField.text = inputField.text.Substring(0, inputField.text.Length - 1);
                    }
                }
            }
        }
    }

}
