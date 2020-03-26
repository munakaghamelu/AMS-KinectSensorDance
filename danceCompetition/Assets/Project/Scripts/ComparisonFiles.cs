using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class ComparisonFiles : MonoBehaviour
{
    public static string path1;
    public static string path2;
    public static string fileName;
    public Text fileStatus1;
    public Text fileStatus2;
    public Button analyseButton;

    public void uploadFile1()
    {
        path1 = EditorUtility.OpenFilePanel("", "", "txt");
        fileStatus1.text = "Filename: " + Path.GetFileName(path1);
        if(path2 != null)
        {
            analyseButton.interactable = true;
        }
    }

    public void uploadFile2()
    {
        path2 = EditorUtility.OpenFilePanel("", "", "txt");
        analyseButton.interactable = true;
        fileStatus2.text = "Filename: " + Path.GetFileName(path2);
        if (path1 != null)
        {
            analyseButton.interactable = true;
        }
    }
}
