using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public static class AnalyticManager {

	public static Analytics an = new Analytics();

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
		string json = JsonUtility.ToJson(an);
		File.WriteAllText ("data_saved.json", json);
	}
}

[System.Serializable]
public class Analytics {
	public int numWords;
	public float totalTimeSpent;
	public List<Action> actions = new List<Action>();
}