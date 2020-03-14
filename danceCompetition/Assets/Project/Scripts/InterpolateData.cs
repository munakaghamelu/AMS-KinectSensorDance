using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Globalization;
using static System.Math;

public class InterpolateData : MonoBehaviour
{
    public selectKinect filePath1;
    public selectKinect filePath2;
    public selectKinect filePath3;
    public selectKinect filePath4;
    private string[] newFilePaths = { "NewKinect1.txt", "NewKinect2.txt", "NewKinect3.txt", "NewKinect4.txt" };
    private string ID;
    private int numOfFiles = 0;

    List<List<string>> data = new List<List<string>>();
    List<string> timeStamps = new List<string>();
    List<string> tempList;
    List<List<string>> interpolatedData = new List<List<string>>();
    float xCoord, yCoord, zCoord;
    float nextX, nextY, nextZ;

    public void interpolate()
    {
        selectKinect[] temp = { filePath1, filePath2, filePath3, filePath4};
        List<selectKinect> filePaths = new List<selectKinect>();
        foreach (selectKinect kinect in temp)
        {
            if (kinect.getPath() != null)
            {
                filePaths.Add(kinect);
            }
        }

        numOfFiles = filePaths.Count();

        for (int i = 0; i < filePaths.Count(); i++)
        {
            string path = filePaths[i].getPath();
            interpolateFiles(path, newFilePaths[i]);
        }

    }

    private void interpolateFiles(string filePath, string filePathNew)
    {
        loadFileToArray(filePath);
        interpolateData();

        if (!File.Exists(filePathNew))
        {
            using (StreamWriter fileWriter = File.CreateText(filePathNew))
            {
                fileWriter.Write("");
            }
        }

        else
        {
            File.WriteAllText(filePathNew, string.Empty);
        }

        using (StreamWriter fileWriter = File.AppendText(filePathNew))
        {
            string head = "|kb,43271723768560000,6,25,0,0,0,0,0,1,72057594037935056,2";

            fileWriter.WriteLine(ID);
            for (int i = 0; i < interpolatedData.Count(); i++)
            {
                string headWithTime = i * 0.01 + head;
                List<string> joints = interpolatedData[i];
                //joints = invertJoints(joints);
                for (int j = 3; j < joints.Count(); j += 4)
                {
                    joints.Insert(j, "2");
                }

                joints.Insert(0, headWithTime);
                fileWriter.WriteLine(string.Join(",", joints));
            }
            fileWriter.Flush();
        }

    }

    private void loadFileToArray(string filePath)
    {
        data = new List<List<string>>();
        string mainKinect_line = string.Empty;
        StreamReader fileReader = new StreamReader(filePath);
        //List<List<string>> data = new List<List<string>>();

        ID = fileReader.ReadLine();

        while (((mainKinect_line = fileReader.ReadLine()) != null))
        {
            string[] frameData = new string[100];
            char[] delimeters = { ',' };
            string[] mainKinectAsArray = mainKinect_line.Split(delimeters);
            int lineNumber = 0;

            for (int i = 0; i < mainKinectAsArray.Length; i++)
            {

                if (string.Equals(mainKinectAsArray[i], "1"))
                {
                    Array.Copy(mainKinectAsArray, i + 2, frameData, 0, 100);
                    break;
                }
            }

            tempList = new List<string>();

            for (int j = 0; j <= 96; j += 4)
            {
                tempList.Add(frameData[j + 1]);
                tempList.Add(frameData[j + 2]);
                tempList.Add(frameData[j + 3]);
            }

            data.Add(tempList);

            string time = mainKinectAsArray[0].Split('|')[0];
            timeStamps.Add(time);
            lineNumber++;
            //Debug.Log(String.Join(",", tempList));
        }
        fileReader.Close();
        //return data;

    }

    private void interpolateData()
    {
        interpolatedData = new List<List<string>>();
        float currentTime, nextTime;
        float intervalX, intervalY, intervalZ;
        float interval = 0.01f;

        for (int i = 0; i < data.Count() - 1; i++)
        {
            float.TryParse(timeStamps[i], NumberStyles.Float, CultureInfo.InvariantCulture, out currentTime);
            float.TryParse(timeStamps[i + 1], NumberStyles.Float, CultureInfo.InvariantCulture, out nextTime);
            while (interval < nextTime)
            {
                List<string> temporaryJoints = new List<string>();
                for (int j = 0; j < data[i].Count(); j += 3)
                {
                    float.TryParse(data[i][j], NumberStyles.Float, CultureInfo.InvariantCulture, out xCoord);
                    float.TryParse(data[i][j + 1], NumberStyles.Float, CultureInfo.InvariantCulture, out yCoord);
                    float.TryParse(data[i][j + 2], NumberStyles.Float, CultureInfo.InvariantCulture, out zCoord);
                    float.TryParse(data[i + 1][j], NumberStyles.Float, CultureInfo.InvariantCulture, out nextX);
                    float.TryParse(data[i + 1][j + 1], NumberStyles.Float, CultureInfo.InvariantCulture, out nextY);
                    float.TryParse(data[i + 1][j + 2], NumberStyles.Float, CultureInfo.InvariantCulture, out nextZ);
                    intervalX = Mathf.SmoothStep(xCoord, nextX, calculateInterpPercentage(currentTime, nextTime, interval)) / numOfFiles;
                    intervalY = Mathf.SmoothStep(yCoord, nextY, calculateInterpPercentage(currentTime, nextTime, interval)) / numOfFiles;
                    intervalZ = Mathf.SmoothStep(zCoord, nextZ, calculateInterpPercentage(currentTime, nextTime, interval)) / numOfFiles;
                    temporaryJoints.Add(intervalX.ToString());
                    temporaryJoints.Add(intervalY.ToString());
                    temporaryJoints.Add(intervalZ.ToString());

                }
                interval += 0.01f;
                interpolatedData.Add(temporaryJoints);
                //Debug.Log(String.Join(",", temporaryJoints));

            }

        }

    }

    private float calculateInterpPercentage(float timestamp1, float timestamp2, float currentTimeStamp)
    {
        float totalDiff = timestamp2 - timestamp1;
        float smallDiff = currentTimeStamp - timestamp1;
        float resultingTime = smallDiff / totalDiff;
        return resultingTime;
    }
}
