using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class kinectButtonScript : MonoBehaviour
{
    // Start is called before the first frame update
    public Button kinect1, kinect2, kinect3, kinect4;
    public Text currentKinect;
    private List<Button> buttonList = new List<Button>();
    public static int kinectNum = 0;


    public void checkStates()
    {
        buttonList.Add(kinect1);
        buttonList.Add(kinect2);
        buttonList.Add(kinect3);
        buttonList.Add(kinect4);
        foreach (Button kinect in buttonList)
        {
            kinect.interactable = false;
            if (kinect.tag == kinectNum.ToString())
            {
                Debug.Log(kinectNum);
                currentKinect.text = "Recording Kinect: " + kinectNum.ToString();       
            }
            
        }         
    } 

    public void setKinectNum(int num)
    {
        kinectNum = num;
    }

    public int getKinectNum()
    {
        return kinectNum;
    }
}
