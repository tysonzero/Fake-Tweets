using System.Collections;
using System.Collections.Generic;
using Random=System.Random;
using UnityEngine;


public static class TweetBank {

	public static Tweets tweets;
	public static Random r = new Random();

	public static Tweet getRandomTweet() {
		Debug.Log("Here");

		return tweets.data [(int)(r.NextDouble () * (tweets.data.Count - 1))];
	}
}
