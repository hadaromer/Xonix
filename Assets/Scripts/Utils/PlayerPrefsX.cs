using UnityEngine;
using System.Collections.Generic;

public static class PlayerPrefsX
{
    // Save an array of integers to PlayerPrefs
    public static void SetIntArray(string key, int[] intArray)
    {
        PlayerPrefs.SetInt(key + "_Length", intArray.Length); // Save the length of the array
        for (int i = 0; i < intArray.Length; i++)
        {
            PlayerPrefs.SetInt(key + "_" + i, intArray[i]); // Save each element of the array
        }
    }

    // Retrieve an array of integers from PlayerPrefs
    public static int[] GetIntArray(string key, int defaultValue = 0, int defaultSize = 0)
    {
        int arrayLength = PlayerPrefs.GetInt(key + "_Length", defaultSize); // Get the length of the array
        int[] intArray = new int[arrayLength];
        for (int i = 0; i < arrayLength; i++)
        {
            intArray[i] = PlayerPrefs.GetInt(key + "_" + i, defaultValue); // Get each element of the array
        }
        return intArray;
    }

    // Save an array of strings to PlayerPrefs
    public static void SetStringArray(string key, string[] stringArray)
    {
        PlayerPrefs.SetInt(key + "_Length", stringArray.Length); // Save the length of the array
        for (int i = 0; i < stringArray.Length; i++)
        {
            PlayerPrefs.SetString(key + "_" + i, stringArray[i]); // Save each element of the array
        }
    }

    // Retrieve an array of strings from PlayerPrefs
    public static string[] GetStringArray(string key, string defaultValue = "", int defaultSize = 0)
    {
        int arrayLength = PlayerPrefs.GetInt(key + "_Length", defaultSize); // Get the length of the array
        string[] stringArray = new string[arrayLength];
        for (int i = 0; i < arrayLength; i++)
        {
            stringArray[i] = PlayerPrefs.GetString(key + "_" + i, defaultValue); // Get each element of the array
        }
        return stringArray;
    }
}