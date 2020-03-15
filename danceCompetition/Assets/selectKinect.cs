using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine;

public class selectKinect : MonoBehaviour
{
    public Text fileName;
    private string path;

    // Start is called before the first frame update
    public void chooseFile()
    {
        path = EditorUtility.OpenFilePanel("", "", "txt");
        fileName.text = Path.GetFileName(path);
    }

    public string getPath()
    {
        return path;
    }

}
