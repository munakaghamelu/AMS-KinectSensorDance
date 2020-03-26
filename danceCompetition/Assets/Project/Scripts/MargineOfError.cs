using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Globalization;
using static System.Math;

public class MargineOfError : MonoBehaviour
{
    public Slider headSlider;
    public Slider shoulderSlider;
    public Slider torsoSlider;
    public Slider armsSlider;
    public Slider hipsSlider;
    public Slider legsSlider;
    public Text headPercent;
    public Text shoulderPercent;
    public Text torsoPercent;
    public Text armsPercent;
    public Text hipsPercent;
    public Text legsPercent;
    public Text score;

    //List of list of joints - 2 List<List<String>>, read line by line, split into array and then back to list

    // get the percentage error of the values , accumulate to a massive total percentage error and then divide by n

    // for each joint store the average percentage array in a list that can be called from unity and then displayed onto the GUI screen

    //eventually get the percentage accuracy of the whole skeletal

    private static List<float> listPercentageVector = new List<float>();
    public void margineOfError()
    {
        loadFiles();
        processErrorData();
    }

    private void processErrorData()
    {
        float headError = (listPercentageVector[0] + listPercentageVector[1]) / 2;
        float shoulderError = (listPercentageVector[2] + listPercentageVector[3]) / 2;
        float armsError = (listPercentageVector[4] + listPercentageVector[5] + listPercentageVector[15] + listPercentageVector[16]) / 4;
        float torsoError = (listPercentageVector[6] + listPercentageVector[7] + listPercentageVector[8]) / 3;
        float hipsError = (listPercentageVector[9] + listPercentageVector[10]) / 2;
        float legsError = (listPercentageVector[13] + listPercentageVector[14]) / 2;

        float average = (headError + shoulderError + armsError + torsoError + hipsError + legsError) / 6;
        float overall = 100 - (int)(average/4);

        headSlider.value = headError / 500;
        shoulderSlider.value = shoulderError / 500;
        armsSlider.value = armsError / 500;
        torsoSlider.value = torsoError / 500;
        hipsSlider.value = hipsError / 500;
        legsSlider.value = legsError / 500;

        headPercent.text = Math.Round((double)headError, 1).ToString() + "%";
        shoulderPercent.text = Math.Round((double)shoulderError, 1).ToString() + "%";
        armsPercent.text = Math.Round((double)armsError, 1).ToString() + "%";
        torsoPercent.text = Math.Round((double)torsoError, 1).ToString() + "%";
        hipsPercent.text = Math.Round((double)hipsError, 1).ToString() + "%";
        legsPercent.text = Math.Round((double)legsError, 1).ToString() + "%";
        score.text = overall.ToString() + "/100";

    }

    //private static List<List<float>> testModelPositionVectors = new List<List<float>>();

    private void loadFiles()
    {
        string filePathIdealDance = "DanceFiles/idealVectors.txt";
        string filePathTestDance = "DanceFiles/badVectors.txt";

        string idealLine = string.Empty;
        string testLine = string.Empty;

        char[] delimeters = { ' ' };
        List<string> lineAsArray = new List<string>();
        List<string> testLineAsArray = new List<string>();

        StreamReader fileReaderIdeal = new StreamReader(filePathIdealDance);
        StreamReader fileReaderTest = new StreamReader(filePathTestDance);

        fileReaderIdeal.ReadLine();
        fileReaderTest.ReadLine();

        for (int i = 0; i < 17; i++)
        {
            idealLine = fileReaderIdeal.ReadLine();
            testLine = fileReaderTest.ReadLine();

            int lengthOfIdealLine = idealLine.Trim().Split(delimeters).Count();
            int lengthOfModelLine = testLine.Trim().Split(delimeters).Count();

            int numberOfValues = -1;

            if (lengthOfIdealLine >= lengthOfModelLine)
            {
                numberOfValues += lengthOfModelLine;
            }
            else
            {
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
        File.Delete(filePathIdealDance);
        File.Delete(filePathTestDance);
    }

    private float getPercentageError(List<string> idealValue, List<string> modelValue)
    {

        float totalPercentages = 0.0f;
        float idealValueFloat = 0.0f;
        float modelValueFloat = 0.0f;
        float error = 0.0f;
        float denominator = 0.0f;
        float averagePercentageError = 0.0f;
        float numerator = 0.0f;

        for (int i = 0; i < idealValue.Count(); i++)
        {
            float.TryParse(idealValue[i], NumberStyles.Float, CultureInfo.InvariantCulture, out idealValueFloat);
            float.TryParse(modelValue[i], NumberStyles.Float, CultureInfo.InvariantCulture, out modelValueFloat);

            numerator = idealValueFloat - modelValueFloat;
            denominator = (Abs(idealValueFloat) + Abs(modelValueFloat)) / 2;
            error = Abs(numerator / denominator) * 100;
            if (Double.IsNaN(error))
            {
                error = Abs(modelValueFloat) * 100;
            }
            totalPercentages += error;
        }

        averagePercentageError = (float)totalPercentages / idealValue.Count();
        //if (Double.IsNaN(averagePercentageError)) averagePercentageError = 0;

        //averagePercentageError = (float) Math.Round(averagePercentageError, 2, MidpointRounding.ToEven);

        // Debug.Log("The percentage area for this vector is " + averagePercentageError);

        return averagePercentageError;
    }

}