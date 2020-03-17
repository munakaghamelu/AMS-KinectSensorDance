using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Globalization; 
using static System.Math;

public class PositionVectors
{
    private static Dictionary<string, string> jointsAndCoordinates = new Dictionary<string, string>();
    private static List<List<float>> positionVectorList = new List<List<float>>();
    private static List<List<float>> normalisedPositionVectors = new List<List<float>>();
    public void positionVectors() {
        //Debug.Log("Initialising");
        loadCoordinatesToDictionary();
        //Debug.Log("Dictionary Created");
        createPositionVectorList();
        //Debug.Log("PV List created");
        int count = 0;
        foreach( List<float> list in positionVectorList) {
            count++;
        }
        //Debug.Log(count);
    }

    private void loadCoordinatesToDictionary() {

        List<string> listofXYCoordinates = new List<string>();
        string filePath = "DanceFiles/badtest.txt";
        string line = string.Empty;
        StreamReader fileReader = new StreamReader(filePath);

        //add to list first so we have size
        line = fileReader.ReadLine();
        string[] frameData = new string[100];
        char[] delimeters = {','};
        string[] lineAsArray = line.Split(delimeters);

        for (int i = 0; i < 25; i++) {
            listofXYCoordinates.Add("Joint: ");
        }

        while ((line = fileReader.ReadLine()) != null) {
            lineAsArray = line.Split(delimeters);
            int countX = 1;
            int countY = 2;
            int countZ = 3;

            for (int i = 0; i < lineAsArray.Length; i++) {
                if (string.Equals(lineAsArray[i], "1"))
                {
                    Array.Copy(lineAsArray, i + 2, frameData, 0, 100);
                    break;
                }
            }

            for (int j = 0; j < 25; j++) {
                listofXYCoordinates[j] += "," + frameData[countX] + "," + frameData[countY] + "," + frameData[countZ]; //adds the x and y of the joint
                countX += 4;
                countY += 4;
                countZ += 4;
                //Debug.Log(listofXYCoordinates[j]);
            }
        }
        fileReader.Close();

        //add the lines to the dictionary
        addToDictionary(listofXYCoordinates);
    }

    private void createPositionVectorList() {
        //0-17 index shows the pairs of points
        positionVectorList.Add(getPositionVector(jointsAndCoordinates["Head"], jointsAndCoordinates["Neck"]));
        positionVectorList.Add(getPositionVector(jointsAndCoordinates["Neck"], jointsAndCoordinates["SpineShoulder"]));
        positionVectorList.Add(getPositionVector(jointsAndCoordinates["SpineShoulder"], jointsAndCoordinates["ShoulderLeft"]));
        positionVectorList.Add(getPositionVector(jointsAndCoordinates["SpineShoulder"], jointsAndCoordinates["ShoulderRight"]));
        positionVectorList.Add(getPositionVector(jointsAndCoordinates["ShoulderLeft"], jointsAndCoordinates["ElbowLeft"]));
        positionVectorList.Add(getPositionVector(jointsAndCoordinates["ShoulderRight"], jointsAndCoordinates["ElbowRight"]));
        positionVectorList.Add(getPositionVector(jointsAndCoordinates["SpineMid"], jointsAndCoordinates["SpineBase"]));
        positionVectorList.Add(getPositionVector(jointsAndCoordinates["SpineBase"], jointsAndCoordinates["HipLeft"]));
        positionVectorList.Add(getPositionVector(jointsAndCoordinates["SpineBase"], jointsAndCoordinates["HipRight"]));
        positionVectorList.Add(getPositionVector(jointsAndCoordinates["HipLeft"], jointsAndCoordinates["KneeLeft"]));
        positionVectorList.Add(getPositionVector(jointsAndCoordinates["HipRight"], jointsAndCoordinates["KneeRight"]));
        positionVectorList.Add(getPositionVector(jointsAndCoordinates["KneeLeft"], jointsAndCoordinates["AnkleLeft"]));
        positionVectorList.Add(getPositionVector(jointsAndCoordinates["KneeRight"], jointsAndCoordinates["AnkleRight"]));
        positionVectorList.Add(getPositionVector(jointsAndCoordinates["AnkleLeft"], jointsAndCoordinates["FootLeft"]));
        positionVectorList.Add(getPositionVector(jointsAndCoordinates["AnkleRight"], jointsAndCoordinates["FootRight"]));
        positionVectorList.Add(getPositionVector(jointsAndCoordinates["ElbowLeft"], jointsAndCoordinates["WristLeft"]));
        positionVectorList.Add(getPositionVector(jointsAndCoordinates["ElbowRight"], jointsAndCoordinates["WristRight"]));

        normalisePositionVectors();
    }

    public void addToDictionary(List<string> coOrds) {
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

    private List<float> getPositionVector(string firstJoint, string secondJoint) {
        char[] delimeters = {','};
        float coordOne, coordTwo, difference;
        List<float> positionVectors = new List<float>();
        
        //Debug.Log(firstJoint);
        //Debug.Log(secondJoint);

        string[] coOrdsOfFirstJoint = firstJoint.Split(delimeters);
        string[] coordsOfSecondJoint = secondJoint.Split(delimeters);

        //Debug.Log(coOrdsOfFirstJoint.Length);
        //Debug.Log(coordsOfSecondJoint.Length);

        for (int j = 1; j < coOrdsOfFirstJoint.Length; j++){
            float.TryParse(coOrdsOfFirstJoint[j], NumberStyles.Float, CultureInfo.InvariantCulture, out coordOne);
            float.TryParse(coordsOfSecondJoint[j], NumberStyles.Float, CultureInfo.InvariantCulture, out coordTwo);
            difference = coordTwo - coordOne;
            positionVectors.Add(difference);
        }

        //Debug.Log(positionVectors);

        return positionVectors;
    }

    private void normalisePositionVectors() {
        /*Debug.Log("the interpolated one");
        foreach (List<float> list in positionVectorList){
            string s = string.Join(" ", list);
            Debug.Log(s);
        }*/

        string filePathNormalisedVectors = "DanceFiles/modelDance.txt";

        //get the length of the position vectors and then reproduce a list with division of the length
        List<float> tempNormalisedCoord = new List<float>();
        float length = 0.0000f;
        foreach (List<float> coordinateXYZ in positionVectorList) {
            for (int i = 0; i < coordinateXYZ.Count(); i += 3) {
                length += Abs(coordinateXYZ[i]) + Abs(coordinateXYZ[i+1]) + Abs(coordinateXYZ[i+2]);
                tempNormalisedCoord.Add((float)Math.Round(((float)coordinateXYZ[i]/length), 4, MidpointRounding.ToEven));
                tempNormalisedCoord.Add((float)Math.Round(((float)coordinateXYZ[i+1]/length), 4, MidpointRounding.ToEven));
                tempNormalisedCoord.Add((float)Math.Round(((float)coordinateXYZ[i+2]/length), 4, MidpointRounding.ToEven));
            }
            normalisedPositionVectors.Add(tempNormalisedCoord);
            length = 0.0000f;
            tempNormalisedCoord = new List<float>();
        }
        writeToFile(filePathNormalisedVectors, normalisedPositionVectors);

        /*Debug.Log("NOT the interpolated one");
        foreach (List<float> list in normalisedPositionVectors){
            string s = string.Join(" ", list);
            Debug.Log(s);
        }*/ 

        //Debug.Log(normalisedPositionVectors);
    }

    public static void writeToFile(string filePath, List<List<float>> data) {
        if (!File.Exists(filePath)) {
            using (StreamWriter fileWriter = File.CreateText(filePath)) {
                fileWriter.WriteLine("");
            }
        }
        else {
            File.WriteAllText(filePath, string.Empty);
        }

        using (StreamWriter fileWriter = File.AppendText(filePath)) {
            foreach ( List<float> line in data) {
                string s = string.Join(" ", line);
                fileWriter.WriteLine(s);
            }
            fileWriter.Flush();
        }
    }

}
