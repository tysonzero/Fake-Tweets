using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Tweet {

	public int id;
	public string status;
	public string name;

}

[System.Serializable]
public class Tweets {
	public List<Tweet> data;
}
