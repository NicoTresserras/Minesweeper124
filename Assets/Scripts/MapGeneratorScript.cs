using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Position
{
    public int valor;
    public bool visible;
    public bool bandera;
}

public class MapGeneratorScript : MonoBehaviour
{
    [SerializeField]
    private static object mapa;
    [SerializeField]
    public static string mapsize = "4x4x4x4";
    [SerializeField]
    public static int bombas = 4;
    public static bool IsMapReady { get; set; } = false;

    // Start is called before the first frame update
    void Start()
    {
        IsMapReady = false;
        MapGenerator(mapsize);
        SetNegativeValues(bombas);
        UpdateNeighborValues(mapa, new List<int>());
        IsMapReady = true;
    }

    private void OnDestroy()
    {
        IsMapReady = false;
    }



    public static void SetBombas(int Bombas)
    {
        bombas = Bombas;
    }

    public static void SetMap(string map_size)
    {
        mapsize = map_size;
    }

    static void MapGenerator(string tamaño)
    {
        if (string.IsNullOrEmpty(tamaño))
        {
            return;
        }

        string[] dimensiones = tamaño.Split('x');
        Array.Reverse(dimensiones);
        tamaño = string.Join("x", dimensiones);

        mapa = CreateNestedList(dimensiones, 0);
        PlaceValues(mapa, dimensiones);

    }

    static object CreateNestedList(string[] dimensiones, int index)
    {
        System.Random random = new System.Random();
        if (index >= dimensiones.Length)
        {
            return new Position { valor = 0, visible = false };
        }

        int tamañoDimensión;
        if (int.TryParse(dimensiones[index], out tamañoDimensión))
        {
            List<object> nestedList = new List<object>();
            for (int i = 0; i < tamañoDimensión; i++)
            {
                nestedList.Add(CreateNestedList(dimensiones, index + 1));
            }
            return nestedList;
        }

        return null;
    }

    static string EncodeMap(object nestedList)
    {
        if (nestedList is List<object> list)
        {
            List<string> encodedItems = new List<string>();
            foreach (var item in list)
            {
                encodedItems.Add(EncodeMap(item));
            }
            return "[" + string.Join(",", encodedItems) + "]";
        }
        else if (nestedList is Position position)
        {
            string value = $"{position.valor}:{position.visible}";
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(bytes);
        }
        return "";
    }

    static void PrettyShowMapRecursive(object nestedList, int depth, StringBuilder sb, int dimension)
    {
        if (nestedList is List<object> list)
        {
            if (dimension > 0)
            {
                sb.AppendLine(new string(' ', depth * 2) + "[");
            }
            for (int i = 0; i < list.Count; i++)
            {
                PrettyShowMapRecursive(list[i], depth + 1, sb, dimension + 1);
                if (dimension == 1 && i < list.Count - 1)
                {
                    sb.AppendLine();
                }
            }
            if (dimension > 0)
            {
                sb.AppendLine(new string(' ', depth * 2) + "]");
            }
        }
        else if (nestedList is Position position)
        {
            sb.Append(new string(' ', depth * 2) + position.valor.ToString() + " ");
        }
    }

    static object DecodeMap(string encodedMap)
    {
        if (encodedMap.StartsWith("[") && encodedMap.EndsWith("]"))
        {
            string innerContent = encodedMap.Substring(1, encodedMap.Length - 2);
            List<object> nestedList = new List<object>();
            int bracketCount = 0;
            int startIndex = 0;
            for (int i = 0; i < innerContent.Length; i++)
            {
                if (innerContent[i] == '[') bracketCount++;
                if (innerContent[i] == ']') bracketCount--;
                if (innerContent[i] == ',' && bracketCount == 0)
                {
                    nestedList.Add(DecodeMap(innerContent.Substring(startIndex, i - startIndex)));
                    startIndex = i + 1;
                }
            }
            nestedList.Add(DecodeMap(innerContent.Substring(startIndex)));
            return nestedList;
        }
        else
        {
            byte[] bytes = Convert.FromBase64String(encodedMap);
            string decodedValue = Encoding.UTF8.GetString(bytes);
            string[] parts = decodedValue.Split(':');
            if (parts.Length == 2 && int.TryParse(parts[0], out int valor) && bool.TryParse(parts[1], out bool visible))
            {
                return new Position { valor = valor, visible = visible };
            }
        }
        return null;
    }

    static string ShowMap(object nestedList)
    {
        if (nestedList is List<object> list)
        {
            List<string> encodedItems = new List<string>();
            foreach (var item in list)
            {
                encodedItems.Add(ShowMap(item));
            }
            return "[" + string.Join(",", encodedItems) + "]";
        }
        else if (nestedList is Position position)
        {
            return position.valor.ToString();
        }
        return "";
    }

    static int CountPositions(object nestedList)
    {
        if (nestedList is List<object> list)
        {
            int count = 0;
            foreach (var item in list)
            {
                count += CountPositions(item);
            }
            return count;
        }
        else if (nestedList is Position)
        {
            return 1; // Contar el elemento base
        }
        return 0;
    }

    static int GetDimensions(object nestedList)
    {
        if (nestedList is List<object> list && list.Count > 0)
        {
            return 1 + GetDimensions(list[0]);
        }
        return 0;
    }

    static void PlaceValues(object mapaDecodificado, string[] dimensiones)
    {
        mapa = CreateNestedList(dimensiones, 0);
    }

    static void SetNegativeValues(int cantidad)
    {
        List<Position> posiciones = new List<Position>();
        CollectPositions(mapa, posiciones);

        System.Random random = new System.Random();
        for (int i = 0; i < cantidad && posiciones.Count > 0; i++)
        {
            int index = random.Next(posiciones.Count);
            posiciones[index].valor = -1;
            posiciones.RemoveAt(index);
        }
    }

    static void CollectPositions(object nestedList, List<Position> posiciones)
    {
        if (nestedList is List<object> list)
        {
            foreach (var item in list)
            {
                CollectPositions(item, posiciones);
            }
        }
        else if (nestedList is Position position)
        {
            posiciones.Add(position);
        }
    }

    static void UpdateNeighborValues(object nestedList, List<int> indices)
    {
        if (nestedList is List<object> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                List<int> newIndices = new List<int>(indices) { i };
                UpdateNeighborValues(list[i], newIndices);
            }
        }
        else if (nestedList is Position position && position.valor != -1)
        {
            int neighborCount = CountNegativeNeighbors(indices);
            position.valor = neighborCount;
        }
    }

    static int CountNegativeNeighbors(List<int> indices)
    {
        int count = 0;
        int dimensions = indices.Count;
        int[] deltas = { -1, 0, 1 };

        List<int[]> neighbors = new List<int[]>();
        GenerateNeighbors(new int[dimensions], 0, deltas, neighbors);

        foreach (var neighbor in neighbors)
        {
            if (IsValidNeighbor(neighbor, indices))
            {
                object neighborValue = GetNestedListValue(mapa, neighbor);
                if (neighborValue is Position neighborPosition && neighborPosition.valor == -1)
                {
                    count++;
                }
            }
        }

        return count;
    }

    static void GenerateNeighbors(int[] current, int dimension, int[] deltas, List<int[]> neighbors)
    {
        if (dimension == current.Length)
        {
            if (Array.Exists(current, delta => delta != 0))
            {
                neighbors.Add((int[])current.Clone());
            }
            return;
        }

        foreach (int delta in deltas)
        {
            current[dimension] = delta;
            GenerateNeighbors(current, dimension + 1, deltas, neighbors);
        }
    }

    static bool IsValidNeighbor(int[] neighborIndices, List<int> originalIndices)
    {
        for (int i = 0; i < neighborIndices.Length; i++)
        {
            neighborIndices[i] += originalIndices[i];
        }


        object current = mapa;
        foreach (int index in neighborIndices)
        {
            if (current is List<object> list && index >= 0 && index < list.Count)
            {
                current = list[index];
            }
            else
            {
                return false;
            }
        }

        return true;
    }

    static object GetNestedListValue(object nestedList, int[] indices)
    {
        object current = nestedList;

        foreach (int index in indices)
        {
            if (current is List<object> list && index >= 0 && index < list.Count)
            {
                current = list[index];
            }
            else
            {
                return null;
            }
        }

        return current;
    }

    static public string GetMap()
    {
        return ShowMap(mapa);
    }

    static public string GetMapSize()
    {
        return mapsize;
    }
}