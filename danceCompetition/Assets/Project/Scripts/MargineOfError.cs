using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Globalization; 
using static System.Math;

public class MargineOfError : MonoBehaviour
{

    //List of list of joints - 2 List<List<String>>, read line by line, split into array and then back to list

    // get the percentage error of the values , accumulate to a massive total percentage error and then divide by n

    // for each joint store the average percentage array in a list that can be called from unity and then displayed onto the GUI screen
    
    //eventually get the percentage accuracy of the whole skeletal

    private static List<float> listPercentageVector = new List<float>();
    public void margineOfError() {
        loadFiles();

        float overallScore = 0.0f;
        

        for (int i = 0; i < listPercentageVector.Count(); i++) {
             overallScore += listPercentageVector[i];
         }

         listPercentageVector.Add((float) overallScore / 17);

        for (int i = 0; i < listPercentageVector.Count(); i++) {
            Debug.Log(listPercentageVector[i]);
         }
    }

    //private static List<List<float>> testModelPositionVectors = new List<List<float>>();

    private void loadFiles() {
        string filePathIdealDance = "DanceFiles/idealDance.txt";
        string filePathTestDance = "DanceFiles/testDance.txt";

        string idealLine = string.Empty;
        string testLine = string.Empty;

        char[] delimeters = {' '};
        List<string> lineAsArray = new List<string>();
        List<string> testLineAsArray = new List<string>();

        StreamReader fileReaderIdeal = new StreamReader(filePathIdealDance);
        StreamReader fileReaderTest = new StreamReader(filePathTestDance);

        idealLine = fileReaderIdeal.ReadLine();

        for (int i = 0; i < 16 ; i++) {
            idealLine = fileReaderIdeal.ReadLine();
            testLine = fileReaderTest.ReadLine();

            int lengthOfIdealLine = idealLine.Trim().Split(delimeters).Count();
            int lengthOfModelLine = testLine.Trim().Split(delimeters).Count();

            int numberOfValues = -1;

            if (lengthOfIdealLine >= lengthOfModelLine) {
                numberOfValues += lengthOfModelLine;
            }
            else {
                numberOfValues += lengthOfIdealLine;
            }

            //Debug.Log("The length of line is " + numberOfValues);

            lineAsArray = idealLine.Trim().Split(delimeters).Take(numberOfValues).ToList();
            testLineAsArray = testLine.Trim().Split(delimeters).Take(numberOfValues).ToList();

            float percentageErrorOfVector = getPercentageError(lineAsArray, testLineAsArray);
            listPercentageVector.Add(percentageErrorOfVector);
        }
        fileReaderIdeal.Close();
        fileReaderTest.Close();
    }

    private float getPercentageError(List<string> idealValue, List<string> modelValue) {
        
        float totalPercentages = 0.0f;
        float idealValueFloat = 0.0f;
        float modelValueFloat = 0.0f;
        float error = 0.0f;
        float denominator = 0.0f;
        float averagePercentageError = 0.0f;
        float numerator = 0.0f;

        for (int i = 0; i < idealValue.Count(); i++) {
            float.TryParse(idealValue[i], NumberStyles.Float, CultureInfo.InvariantCulture, out idealValueFloat);
            float.TryParse(modelValue[i], NumberStyles.Float, CultureInfo.InvariantCulture, out modelValueFloat);

            numerator = idealValueFloat - modelValueFloat;
            denominator = (Abs(idealValueFloat) + Abs(modelValueFloat)) / 2;
            error = Abs(numerator / denominator) * 100;
            if (Double.IsNaN(error)) {
                error = Abs(modelValueFloat) * 100;
            }
            totalPercentages += error;
        }

        averagePercentageError = (float) totalPercentages / idealValue.Count();
        //if (Double.IsNaN(averagePercentageError)) averagePercentageError = 0;

        //averagePercentageError = (float) Math.Round(averagePercentageError, 2, MidpointRounding.ToEven);

        // Debug.Log("The percentage area for this vector is " + averagePercentageError);

        return averagePercentageError;
    }

}
