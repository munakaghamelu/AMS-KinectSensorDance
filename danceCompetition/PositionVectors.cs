using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Globalization;
using static System.Math;

public class PositionVectors : MonoBehaviour
{
    private static Dictionary<string, List<string>> jointsAndCoordinates = new Dictionary<string, List<string>>();
    private static List<List<float>> positionVectorList = new List<List<float>>();

    public void positionVectors() {
        loadCoordinatesToDictionary();
        createPositionVectorList();
    }
    private void loadCoordinatesToDictionary() {
        List<List<string>> listofXYCoordinates = new List<List<string>>();
        string filePath = "BodyRecording.txt";
        string line = string.Empty;
        StreamReader fileReader = new StreamReader(filePath);

        while ((line = fileReader.ReadLine()) != null) {
            string[] frameData = new string[100];
            char[] delimeters = {','};
            string[] lineAsArray = line.Split(delimeters);

            for (int i = 0; i < lineAsArray.Length; i++) {
                if (string.Equals(lineAsArray[i], "1")) {
                    Array.Copy(lineAsArray, i + 2, frameData, 0, 100);
                    //iterate through frame data to add the coordinates to the specific point
                    int countX = 0;
                    int countY = 1;

                    //Spine Base
                    listofXYCoordinates[0].Add(frameData[0]);
                    listofXYCoordinates[0].Add(frameData[1]);
                    //Spine Mid
                    listofXYCoordinates[1].Add(frameData[4]);
                    listofXYCoordinates[1].Add(frameData[5]);
                    //Neck
                    listofXYCoordinates[2].Add(frameData[9]);
                    listofXYCoordinates[2].Add(frameData[10]);
                    //Head
                    listofXYCoordinates[3].Add(frameData[12]);
                    listofXYCoordinates[3].Add(frameData[13]);
                    //add the remaining joints to the list, 0-24 index represents the joints
                    for (int j = 4; j < 25; j++) {
                        listofXYCoordinates[j].Add(frameData[countX]);
                        listofXYCoordinates[j].Add(frameData[countY]);
                        countX += 4;
                        countY += 4;
                    }
                }
            }
        }
        fileReader.Close();

        //add the lines to the dictionary
        addToDictionary(listofXYCoordinates);
    }

    private void createPositionVectorList() {
        //0-17 index shows the pairs of points
        positionVectorList[0] = getPositionVector(jointsAndCoordinates["Head"], jointsAndCoordinates["Neck"]);
        positionVectorList[1] = getPositionVector(jointsAndCoordinates["Neck"], jointsAndCoordinates["SpineShoulder"]);
        positionVectorList[2] = getPositionVector(jointsAndCoordinates["SpineShoulder"], jointsAndCoordinates["ShoulderLeft"]);
        positionVectorList[3] = getPositionVector(jointsAndCoordinates["SpineShoulder"], jointsAndCoordinates["ShoulderRight"]);
        positionVectorList[4] = getPositionVector(jointsAndCoordinates["ShoulderLeft"], jointsAndCoordinates["ElbowLeft"]);
        positionVectorList[5] = getPositionVector(jointsAndCoordinates["ShoulderRight"], jointsAndCoordinates["ElbowRight"]);
        positionVectorList[6] = getPositionVector(jointsAndCoordinates["SpineMid"], jointsAndCoordinates["SpineBase"]);
        positionVectorList[7] = getPositionVector(jointsAndCoordinates["SpineBase"], jointsAndCoordinates["HipLeft"]);
        positionVectorList[8] = getPositionVector(jointsAndCoordinates["SpineBase"], jointsAndCoordinates["HipRight"]);
        positionVectorList[9] = getPositionVector(jointsAndCoordinates["HipLeft"], jointsAndCoordinates["KneeLeft"]);
        positionVectorList[10] = getPositionVector(jointsAndCoordinates["HipRight"], jointsAndCoordinates["KneeRight"]);
        positionVectorList[11] = getPositionVector(jointsAndCoordinates["KneeLeft"], jointsAndCoordinates["AnkleLeft"]);
        positionVectorList[12] = getPositionVector(jointsAndCoordinates["KneeRight"], jointsAndCoordinates["AnkleRight"]);
        positionVectorList[13] = getPositionVector(jointsAndCoordinates["AnkleLeft"], jointsAndCoordinates["FootLeft"]);
        positionVectorList[14] = getPositionVector(jointsAndCoordinates["AnkleRight"], jointsAndCoordinates["FootRight"]);
        positionVectorList[15] = getPositionVector(jointsAndCoordinates["ElbowLeft"], jointsAndCoordinates["WristLeft"]);
        positionVectorList[16] = getPositionVector(jointsAndCoordinates["ElbowRight"], jointsAndCoordinates["WristRight"]);
    }

    public void addToDictionary(List<List<string>> coOrds) {
        jointsAndCoordinates.Add("SpineBase", coOrds[0]);
        jointsAndCoordinates.Add("SpineMid", coOrds[1]);
        jointsAndCoordinates.Add("Neck", coOrds[2]);
        jointsAndCoordinates.Add("Head", coOrds[3]);
        jointsAndCoordinates.Add("ShoulderLeft", coOrds[4]);
        jointsAndCoordinates.Add("ElbowLeft", coOrds[5]);
        jointsAndCoordinates.Add("WristLeft", coOrds[6]);
        jointsAndCoordinates.Add("HandLeft", coOrds[7]);
        jointsAndCoordinates.Add("ShoulderRight", coOrds[8]);
        jointsAndCoordinates.Add("ElbowRight", coOrds[9]);
        jointsAndCoordinates.Add("WristRight", coOrds[10]);
        jointsAndCoordinates.Add("HandRight", coOrds[11]);
        jointsAndCoordinates.Add("HipLeft", coOrds[12]);
        jointsAndCoordinates.Add("KneeLeft", coOrds[13]);
        jointsAndCoordinates.Add("AnkleLeft", coOrds[14]);
        jointsAndCoordinates.Add("FootLeft", coOrds[15]);
        jointsAndCoordinates.Add("HipRight", coOrds[16]);
        jointsAndCoordinates.Add("KneeRight", coOrds[17]);
        jointsAndCoordinates.Add("AnkleRight", coOrds[18]);
        jointsAndCoordinates.Add("FootRight", coOrds[19]);
        jointsAndCoordinates.Add("SpineShoulder", coOrds[20]);
        jointsAndCoordinates.Add("HandTipLeft", coOrds[21]);
        jointsAndCoordinates.Add("ThumbLeft", coOrds[22]);
        jointsAndCoordinates.Add("HandTipRight", coOrds[23]);
        jointsAndCoordinates.Add("ThumbRight", coOrds[24]);
    }

    private List<float> getPositionVector(List<string> firstJoint, List<string> secondJoint) {
        List<float> positionVectors = new List<float>();
        for (int i = 0; i < firstJoint.Count(); i++){
            float difference = float.Parse(secondJoint[i]) - float.Parse(firstJoint[i]);
            positionVectors.Add(difference);
        }

        Debug.Log(positionVectors);

        return positionVectors;
    }

}
