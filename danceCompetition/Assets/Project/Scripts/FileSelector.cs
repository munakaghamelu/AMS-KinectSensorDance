using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using SimpleFileBrowser;

public class FileSelector : MonoBehaviour
{
	public static List<string> fileNames = new List<string>();
	public void CreateWindow()
	{
		FileBrowser.SetFilters(false, new FileBrowser.Filter("TextFiles", ".txt"));
		FileBrowser.SetDefaultFilter(".jpg");
		FileBrowser.SetExcludedExtensions(".lnk", ".tmp", ".zip", ".rar", ".exe");
		FileBrowser.AddQuickLink("DanceFiles", "C:\\kinectSensorDance\\DanceCompetition\\DanceFiles", null);
		StartCoroutine(ShowLoadDialogCoroutine());
	}

	// Update is called once per frame
	IEnumerator ShowLoadDialogCoroutine()
	{
		// Show a load file dialog and wait for a response from user
		// Load file/folder: file, Initial path: default (Documents), Title: "Load File", submit button text: "Load"
		yield return FileBrowser.WaitForLoadDialog(false, "C:\\kinectSensorDance\\DanceCompetition\\DanceFiles",
			"Choose Files for Merging", "Add");
		// Dialog is closed
		// Print whether a file is chosen (FileBrowser.Success)
		// and the path to the selected file (FileBrowser.Result) (null, if FileBrowser.Success is false)
		Debug.Log(FileBrowser.Success + " " + FileBrowser.Result);

		if (FileBrowser.Success)
		{
			fileNames.Add(FileBrowser.Result);
			CreateWindow();
		}
	}

	/*public void MergeSelectedFiles()
	{
		fileNames = removeDuplicates(fileNames);
		Debug.Log(fileNames.Count);
		if(fileNames.Count == 2)
		{
			TransformFunction merger = new TransformFunction();
			string path = Path.Combine(Environment.CurrentDirectory, "DanceFiles\\MergedFile.txt");
			merger.readFiles(fileNames[0], fileNames[1], path);
		}

		else
		{
			Debug.Log("Error, please select two files to merge");
		}
	} */

	private List<string> removeDuplicates(List<string> fileList)
	{
		HashSet<string> listWithoutDupes = new HashSet<string>();
		List<string> finalList = new List<string>();
		foreach( string str in fileList)
		{
			listWithoutDupes.Add(str);
		}

		foreach(string str in listWithoutDupes)
		{
			finalList.Add(str);
		}
		return finalList;
	}
}
