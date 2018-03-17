using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public static class AnalyticManager {

	public static Analytics an = new Analytics();
	public static string creationTime = DateTime.Now.ToString().Replace(":", "_").Replace("/", "_");

	public static void addAction(Action a) {
		an.actions.Add (a);
	}

	public static void incrementWordCount() {
		an.numWords += 1;
	}

	public static void setTimeSpent(float t) {
		an.totalTimeSpent = t;
	}

	public static void save() {

		if (!Directory.Exists(Application.dataPath + "/Analytics")) {
			Directory.CreateDirectory(Application.dataPath + "/Analytics");
		}
				

		string json = JsonUtility.ToJson(an);
		Debug.Log (Application.dataPath);
		var file = File.CreateText (Application.dataPath + "/Analytics/data_saved " + creationTime + ".json");
		file.WriteLine(json);
		file.Close();
	}
}

[System.Serializable]
public class Analytics {
	public int numWords;
	public float totalTimeSpent;
	public List<Action> actions = new List<Action>();
}