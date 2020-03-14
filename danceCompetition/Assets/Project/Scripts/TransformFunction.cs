using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Globalization;
using static System.Math;


public class TransformFunction : MonoBehaviour
{
    private float roomLength = 0f;
    private float roomWidth = 3.133f; // length is parallel to main Kinect, width is perpendicular
    private float xOffset = 0.116f;
    private int numFiles;
    private string[] filePaths = { "NewKinect1.txt", "NewKinect2.txt", "NewKinect3.txt", "NewKinect4.txt" };
    char[] delimeters = { ',' };
    private static Dictionary<string, string[]> mergedData = new Dictionary<string, string[]>();

    private void setNumFiles()
    {
        numFiles = 0;
        foreach (string path in filePaths)
        {
            if (File.Exists(path))
            {
                numFiles += 1;
            }
        }
    }
    private void deleteTempFiles()
    {
        foreach (string path in filePaths)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }

    public void mergeFiles(string savePath)
    {
        setNumFiles();
        foreach (string path in filePaths)
        {
            if (File.Exists(path))
            {
                Debug.Log(path);
                StreamReader fileReader = new StreamReader(path);
                readKinect(fileReader);
            }
        }



        writeToFile("DanceFiles\\" + savePath);
        deleteTempFiles();
    }

    /* public void readFiles(string filePath, string savePath)
     {

         StreamReader fileReader_1 = new StreamReader(filePathK1);
         //StreamReader fileReader_2 = new StreamReader(filePathK2);

         //creating or clearing the text file to hold combined coordinates.


         //Read kinect data and merge into single file

         readKinect(fileReader_1);
         readKinect(fileReader_2);

         writeToFile(savePath);

     }*/

    private void addKinectData(string key, string[] frameData)
    {
        // Add frame data to hashmap

        if (mergedData.ContainsKey(key))
        {
            mergedData[key] = combineArrays(frameData, mergedData[key]);
        }
        else
        {
            mergedData.Add(key, frameData);
        }
    }

    private void readKinect(StreamReader fileReader)
    {
        string kinect_line = string.Empty;

        string kinectID = fileReader.ReadLine();
        while (((kinect_line = fileReader.ReadLine()) != null))
        {
            string[] kinectAsArray = kinect_line.Split(delimeters);
            string[] frameData = new string[100];

            for (int i = 0; i < kinectAsArray.Length; i++)
            {
                if (string.Equals(kinectAsArray[i], "1"))
                {

                    Array.Copy(kinectAsArray, i + 2, frameData, 0, 100);
                    break;
                }
            }

            string key = kinectAsArray[0];

            frameData = transformData(frameData, kinectID);
            addKinectData(key, frameData);
        }
        fileReader.Close();
    }

    private string[] transformData(string[] frameData, string kinectID)
    {
        if (string.Equals(kinectID, "2") || string.Equals(kinectID, "3")  || string.Equals(kinectID, "4"))
        {
            for (int i = 0; i < frameData.Length / 4; i++)
            {
                float tempX;
                float.TryParse(frameData[4 * i + 1], NumberStyles.Float, CultureInfo.InvariantCulture, out tempX);
                float currentX;
                float.TryParse(frameData[4 * i + 1], NumberStyles.Float, CultureInfo.InvariantCulture, out currentX);
                float currentZ;
                float.TryParse(frameData[4 * i + 3], NumberStyles.Float, CultureInfo.InvariantCulture, out currentZ);


                float resultX = transformX(currentX, currentZ, roomLength / numFiles, xOffset / numFiles, kinectID);
                frameData[4 * i + 1] = resultX.ToString();
                float resultZ = transformZ(tempX, currentZ, roomWidth / numFiles, kinectID);
                frameData[4 * i + 3] = resultZ.ToString();
            }
        }

        return frameData;
    }



    public void writeToFile(string savePath)
    {
        if (!File.Exists(savePath))
        {
            using (StreamWriter fileWriter = File.CreateText(savePath))
            {
                fileWriter.WriteLine("");
            }
        }
        else
        {
            File.WriteAllText(savePath, string.Empty);
        }


        var keys = mergedData.Keys.ToList();

        string head = ",43271723768560000,6,25,0,0,0,0,0,1,72057594037935056,2";

        using (StreamWriter fileWriter = File.AppendText(savePath))
        {
            foreach (string key in keys)
            {
                //Debug.Log(key);
                fileWriter.WriteLine(key + head + string.Join(",", mergedData[key]));

            }
            fileWriter.Flush();
        }


    }

    // Average 2 arrays together
    public string[] combineArrays(string[] masterData, string[] additionalKinectData)
    {

        for (int i = 0; i < masterData.Length; i++)
        {
            float firstVal;
            float secondVal;

            float.TryParse(masterData[i], NumberStyles.Float, CultureInfo.InvariantCulture, out firstVal);
            float.TryParse(additionalKinectData[i], NumberStyles.Float, CultureInfo.InvariantCulture, out secondVal);

            float average = (firstVal + secondVal);
            masterData[i] = average.ToString();
        }


        return masterData;
    }


    public float transformX(float x, float z,float length, float offset, string kinectID)
    {
        int kinectNum;
        int.TryParse(kinectID, NumberStyles.Integer, CultureInfo.InvariantCulture, out kinectNum);
        double theta = Math.PI * (kinectNum - 1) * 0.5;
        if (kinectNum == 1 || kinectNum == 3)
        { length = 0; }
        else if (kinectNum == 2)
        { length = length / 2; }
        else if (kinectNum == 4)
        { length = -1 * (length / 2); }
        double newX = ((x * Cos(theta) - z * Sin(theta)) + length) + offset;
        return (float)newX;
    }

    public float transformZ(float x, float z, float width, string kinectID)
    {
        int kinectNum;
        int.TryParse(kinectID, NumberStyles.Integer, CultureInfo.InvariantCulture, out kinectNum);
        double theta = Math.PI * (kinectNum - 1) * 0.5;
        if(kinectNum == 1)
        {width = 0;}
        else if(kinectNum == 2 || kinectNum == 4)
        {width = width / 2;}
        double newZ = ((x * Sin(theta) + z * Cos(theta)) + width);
        return (float)newZ;
    }
}