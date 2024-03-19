using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

public class Data : MonoBehaviour
{
    public static T LoadJsonFromFile<T>(string filePath)
    {
        return JsonUtility.FromJson<T>(File.ReadAllText(filePath));
    }

    public static void UpdateJsonFile<T>(T data, string filePath)
    {
        File.WriteAllText(filePath, JsonUtility.ToJson(data, true));
    }
    
    public static void UpdateFieldInJsonFile<T>(string filePath, string fieldName, object newValue)
    {
        T data = LoadJsonFromFile<T>(filePath);
        Type type = typeof(T);
        PropertyInfo field = type.GetProperty(fieldName);
        if (field == null)
        {
            Debug.LogError("Field " + fieldName + " does not exist in type " + type);
            return;
        }
        field.SetValue(data, newValue);
        UpdateJsonFile(data, filePath);
    }
    
    public delegate bool ElementSelector<T>(T element);

    public static void UpdateElementInArray<T>(string filePath, ElementSelector<T> selector, Action<T> updater)
    {
        // Load the data from the JSON file
        T[] data = LoadJsonFromFile<T[]>(filePath);

        // Find the element to update
        T elementToUpdate = Array.Find(data, selector.Invoke);

        // Check if the element exists
        if (elementToUpdate == null)
        {
            Debug.LogError("Element does not exist in the JSON file");
            return;
        }

        // Update the element
        updater.Invoke(elementToUpdate);

        // Save the updated data back to the JSON file
        UpdateJsonFile(data, filePath);
    }
}
