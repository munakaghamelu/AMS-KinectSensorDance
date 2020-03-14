using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class PopulateButton : MonoBehaviour
{

    public GameObject toggleFile;

    public void Populate()
    {
        int count = 0;
        GameObject newObj;
        GameObject toggles = GameObject.Find("LoadDance/CloudPanel/BlobList/Viewport/Content");

        foreach (Transform child in toggles.transform)
        {
            count += 1;
            Debug.Log(count);
            GameObject.Destroy(child.gameObject);
        }
        UploadDownload uploader = new UploadDownload();

        int numButtons = uploader.getNumBlobs().Result;
        List<string> blobNames = uploader.getBlobNames().Result;
        for (int i = 0; i < numButtons; i++)
        {
            newObj =
            (GameObject)Instantiate(toggleFile, transform);

            Toggle button = newObj.GetComponent<Toggle>();
            button.GetComponentInChildren<Text>().text = blobNames[i];
            /* GameObject window = GameObject.Find("Canvas/BlobList/Viewport");
             RectTransform rectTransform = button.GetComponent<RectTransform>();
             rectTransform.sizeDelta = new Vector2(window.GetComponent<RectTransform>().sizeDelta.x, rectTransform.sizeDelta.y);*/
        }

    }


}
