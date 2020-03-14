using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class FileManager : MonoBehaviour
{
    public static string path;
    public static string fileName;
    public Text fileStatus;
    public Button watchButton;
    public Button analyseButton;

    void uploadFile()
    {
        path = EditorUtility.OpenFilePanel("", "", "txt");
        watchButton.interactable = true;
        analyseButton.interactable = true;
        fileStatus.text = "File Selected: "+ Path.GetFileName(path); 
    }
}
