using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {
	public GameObject tweet_prefab;

	public Sprite donald_correct;
	public Sprite donald_wrong;
	public Sprite fake_donald_correct;
	public Sprite fake_donald_wrong;
	public Sprite hillary_correct;
	public Sprite hillary_wrong;

	public Dictionary<string, Sprite> spriteMap;

	public GameObject current_tweet;
	public Queue<GameObject> timeline_tweets;
	public Queue<GameObject> skipped_tweets;
	public bool downloaded;

	public DateTime startTime;

	void Start () {
		timeline_tweets = new Queue<GameObject>();
		skipped_tweets = new Queue<GameObject>();
		downloaded = false;
		startTime = DateTime.Now;
		spriteMap = GetSpriteMap ();
		StartCoroutine(DownloadTweets());

	}

	IEnumerator DownloadTweets()
	{
		WWW w = new WWW("https://raw.githubusercontent.com/metinsay/Fake-Tweets/master/Assets/Data/tweets.json");
		yield return w;
		if (w.error != null) {
			Debug.Log("Error .. " +w.error);
		} else {
			Debug.Log("Found ... ==>" +w.text.Length +"<==");
		}


		TweetBank.tweets = JsonUtility.FromJson<Tweets>("{\"data\":" + w.text + "}");
		downloaded = true;


	}

    public void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

	void Update () {

		if (!downloaded) {
			return;
		}
		if (GameState.state == GameState.State.TweetSent) {
				GameState.state = GameState.State.Neutral;
				if (timeline_tweets.Count > 3) {
					Destroy(timeline_tweets.Dequeue ());
				}



				if (current_tweet != null) {
					timeline_tweets.Enqueue (current_tweet);
				}

			current_tweet = (GameObject)Instantiate (tweet_prefab);

		} else if (GameState.state == GameState.State.TweetSkipped) {
			GameState.state = GameState.State.Neutral;
			
			if (skipped_tweets.Count > 3) {
				Destroy(skipped_tweets.Dequeue ());
			}



			if (current_tweet != null) {
				skipped_tweets.Enqueue (current_tweet);
			}

			current_tweet = (GameObject)Instantiate (tweet_prefab);


		}

		GameObject[] ts = timeline_tweets.ToArray ();
		for (int i = 0; i < ts.Length; i++) {
			TweetManager t = ts [i].GetComponent<TweetManager>();
			t.targetPosition = new Vector3 (-11.12909f, (float) (ts.Length - i - 1) * -4.612444f + 1.942444f, 0);
			t.backgroundCanvas.GetComponent<Image> ().sprite = spriteMap [t.background_name];
			t.handle.text = t.tweet.name + "\n@" + t.tweet.name;
 		}

		GameObject[] ss = skipped_tweets.ToArray ();
		for (int i = 0; i < ss.Length; i++) {
			TweetManager t = ss [i].GetComponent<TweetManager>();
			t.targetPosition = new Vector3 (11.12909f, (float) (ss.Length - i - 1) * -4.612444f + 1.942444f, 0);
			Debug.Log (t.background_name);
			t.backgroundCanvas.GetComponent<Image> ().sprite = spriteMap [t.background_name];
			t.handle.text = t.tweet.name + "\n@" + t.tweet.name;
		}

		DateTime now = DateTime.Now;
		AnalyticManager.setTimeSpent((float)(now - startTime).TotalSeconds);
	}


	Dictionary<string, Sprite> GetSpriteMap() {
		Dictionary<string, Sprite> s = new Dictionary<string, Sprite>();
		s.Add("donald_correct", donald_correct);
		s.Add("donald_wrong", donald_wrong);
		s.Add("fake_donald_correct", fake_donald_correct);
		s.Add("fake_donald_wrong", fake_donald_wrong);
		s.Add("hillary_correct", hillary_correct);
		s.Add("hillary_wrong", hillary_wrong);
		return s;
	}

		

}
