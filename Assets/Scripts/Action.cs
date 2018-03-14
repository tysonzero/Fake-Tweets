using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Action {
	public Tweet tweet;
	public string action;
	public float timeSpent;

	public Action(Tweet tweet) {
		this.action = "None";
		this.tweet = tweet;
	}
}
