using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MapCreator : MonoBehaviour
{   
    [SerializeField] private Button buttonPrefab;
    [SerializeField] private Canvas canvas;

    private const float ButtonSize = 50f;
    private const float ButtonOffset = 60f;
    private const float CubeOffsetX = 125f;
    private const float TesseractOffsetX = 130f;
    private const float TesseractOffsetY = 130f;
    private const float ScreenOffsetX = -120f;

    private const int MaxDimension = 4;

    private Vector2 firstButtonInitialPos;
    private Vector2 lastButtonInitialPos;

    public static int NumButtons = 1;
    public static int NumLines = 1;
    public static int NumSquares = 1;
    public static int NumCubes = 1;

    public static int Columns = 1;
    public static int Rows = 1;

    public static bool IsMapCreated { get; private set; } = false;

    void Start()
    {
        StartCoroutine(WaitForMap());

        GameObject firstButton = GameObject.Find("button0.0.0.0");
        GameObject lastButton = GameObject.Find($"button{NumCubes - 1}.{NumSquares - 1}.{NumLines - 1}.{NumButtons - 1}");

        if (firstButton != null) firstButtonInitialPos = firstButton.transform.position;
        if (lastButton != null) lastButtonInitialPos = lastButton.transform.position;
    }

    IEnumerator WaitForMap()
    {
        while (!MapGeneratorScript.IsMapReady)
            yield return null;

        string rawMap = MapGeneratorScript.GetMap();
        string[] sizeTokens = MapGeneratorScript.GetMapSize().Split('x');

        NumButtons = int.Parse(sizeTokens[0]);
        if (sizeTokens.Length > 1) NumLines = int.Parse(sizeTokens[1]);
        if (sizeTokens.Length > 2) NumSquares = int.Parse(sizeTokens[2]);
        if (sizeTokens.Length > 3) NumCubes = int.Parse(sizeTokens[3]);

        Columns = Mathf.Min(NumButtons * NumSquares, 16);
        Rows = Mathf.Min(NumLines * NumCubes, 16);
        NumSquares = Mathf.Min(NumSquares, MaxDimension);

        float offsetX = ScreenOffsetX + TesseractOffsetX * (16 - Columns) / 6.5f;
        float offsetY = -10 - TesseractOffsetY * (16 - Rows) / 6.5f;

        BuildTesseract(offsetX, offsetY);
        AssignValuesToButtons(rawMap);
        IsMapCreated = true;

        Debug.Log(rawMap);
    }

    private void Update()
    {
        if (!IsMapCreated) return;

        GameObject tesseract = GameObject.Find("tesseract");
        if (tesseract == null) return;

        Vector2 move = Vector2.zero;

        if (Input.GetKeyDown(KeyCode.W) && CanMoveTesseract(Vector2.down)) move += Vector2.down * ButtonOffset;
        if (Input.GetKeyDown(KeyCode.S) && CanMoveTesseract(Vector2.up)) move += Vector2.up * ButtonOffset;
        if (Input.GetKeyDown(KeyCode.A) && CanMoveTesseract(Vector2.right)) move += Vector2.right * ButtonOffset;
        if (Input.GetKeyDown(KeyCode.D) && CanMoveTesseract(Vector2.left)) move += Vector2.left * ButtonOffset;

        tesseract.transform.Translate(move);
    }

    private bool CanMoveTesseract(Vector2 direction)
    {
        GameObject firstButton = GameObject.Find("button0.0.0.0");
        GameObject lastButton = GameObject.Find($"button{NumCubes - 1}.{NumSquares - 1}.{NumLines - 1}.{NumButtons - 1}");

        if (firstButton == null || lastButton == null) return true;

        Vector2 firstPos = firstButton.transform.position;
        Vector2 lastPos = lastButton.transform.position;

        if (direction == Vector2.right && firstPos.x >= Screen.width * 0.437f) return false;
        if (direction == Vector2.down && firstPos.y <= Screen.height * 0.9355f) return false;
        if (direction == Vector2.left && lastPos.x <= Screen.width * 0.93f) return false;
        if (direction == Vector2.up && lastPos.y >= Screen.height * 0.07f) return false;

        return true;
    }

    private void BuildTesseract(float startX, float startY)
    {
        GameObject tesseract = new GameObject("tesseract")
        {
            transform = { position = new Vector2(startX, startY) }
        };

        tesseract.transform.SetParent(canvas.transform, false);
        tesseract.transform.SetAsFirstSibling();

        for (int i = 0; i < NumCubes; i++)
        {
            float cubeY = 540 - ((ButtonOffset * NumLines + 10) * i);
            BuildCube(tesseract, $"cubo{i}", 0, cubeY, i);
        }
    }

    private void BuildCube(GameObject parent, string name, float x, float y, int cubeIndex)
    {
        GameObject cube = new GameObject(name)
        {
            transform = { position = new Vector2(x, y) }
        };

        cube.transform.SetParent(parent.transform, false);

        for (int i = 0; i < NumSquares; i++)
        {
            float squareX = (ButtonOffset * NumButtons + 10) * i + (NumSquares == 1 ? 0 : CubeOffsetX * (MaxDimension - NumSquares));
            BuildSquare(cube, $"square{cubeIndex}{i}", squareX, 0, i, cubeIndex);
        }
    }

    private void BuildSquare(GameObject parent, string name, float x, float y, int squareIndex, int cubeIndex)
    {
        GameObject square = new GameObject(name)
        {
            transform = { position = new Vector2(x, y) }
        };

        square.transform.SetParent(parent.transform, false);

        for (int i = 0; i < NumLines; i++)
        {
            BuildLine(square, $"line{cubeIndex}{squareIndex}{i}", 0, -ButtonOffset * i, cubeIndex, squareIndex, i);
        }
    }

    private void BuildLine(GameObject parent, string name, float x, float y, int cubeIndex, int squareIndex, int lineIndex)
    {
        GameObject line = new GameObject(name)
        {
            transform = { position = new Vector2(x, y) }
        };

        line.transform.SetParent(parent.transform, false);

        for (int i = 0; i < NumButtons; i++)
        {
            BuildButton(line, $"button{cubeIndex}.{squareIndex}.{lineIndex}.{i}", ButtonOffset * i, -60);
        }
    }

    private void BuildButton(GameObject parent, string name, float x, float y)
    {
        Button newButton = Instantiate(buttonPrefab);
        newButton.name = name;
        newButton.transform.position = new Vector2(x, y);
        newButton.GetComponent<RectTransform>().sizeDelta = new Vector2(ButtonSize, ButtonSize);
        newButton.transform.SetParent(parent.transform, false);
    }

    private void AssignValuesToButtons(string json)
    {
        Debug.Log($"JSON recibido: {json}");

        string[] cubos = json.Trim('[', ']').Split(new[] { "]]],[[[" }, System.StringSplitOptions.None);

        for (int i = 0; i < cubos.Length; i++)
        {
            string[] cuadrados = cubos[i].Trim('[', ']').Split(new[] { "]],[" }, System.StringSplitOptions.None);

            for (int j = 0; j < cuadrados.Length; j++)
            {
                string[] lineas = cuadrados[j].Trim('[', ']').Split(new[] { "],[" }, System.StringSplitOptions.None);

                for (int k = 0; k < lineas.Length; k++)
                {
                    string[] valores = lineas[k].Trim('[', ']').Split(',');

                    for (int l = 0; l < valores.Length; l++)
                    {
                        string buttonName = $"button{i}.{j}.{k}.{l}";
                        GameObject btnObj = GameObject.Find(buttonName);

                        if (btnObj != null && btnObj.TryGetComponent(out Button_Script script))
                        {
                            script.SetValue(int.Parse(valores[l]), false);
                        }
                        else
                        {
                            Debug.LogWarning($"Botón o script no encontrado en: {buttonName}");
                        }
                    }
                }
            }
        }
    }
}
