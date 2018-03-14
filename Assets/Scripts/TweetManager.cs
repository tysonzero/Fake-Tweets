using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

public class TweetManager : MonoBehaviour {

    public Text tweetText;
	public Tweet tweet;
    public ApprovalRatingBar approvalRatingBar;

	public Text handle;
	public CanvasRenderer backgroundCanvas;
    public string currentTweetText; // The untyped text of the tweet on screen
    public bool tweetTyped;
	public bool done;
	public int velocity;
	public AudioClip tweetSound;
	public AudioClip errorSound;

	public Vector3 targetPosition;

    public int charsTyped;

    [Header("ApprovalBar Balance Knobs")]
    public float sendTweetCorrectApprovalBonus = 0.3f;
    public float sendTweetIncorrectApprovalPenalty = 0.1f;
    public float skipTweetCorrectApprovalBonus = 0.05f;
    public float skipTweetIncorrectApprovalPenalty = 0.15f;

    // These are needed for changing which letters are colored as they are typed
    private int tagStringLength; // The first real letter of the tweet exists at this index
    private int firstTagEnds; // Typed letters get moved here so they become colored

	private static DateTime createTime;

	private Action action;

    void Start()
    {
        approvalRatingBar = GameObject.Find("ApprovalRatingBar").GetComponent<ApprovalRatingBar>();

		targetPosition = new Vector3(0.2056804f, -2.582315f, 0);
		velocity = 80;

		tweet = TweetBank.getRandomTweet ();
		action = new Action(tweet);
		AnalyticManager.addAction (action);
		saveAnalytics ();
			
        UpdateTweetTextString();
        AddColorTagsToTweet();

		createTime = DateTime.Now;
    }

    void AddColorTagsToTweet()
    {
        string colorTags = "<color=blue></color>";
        tagStringLength = colorTags.Length;
        firstTagEnds = 12; // Change this if you change colorTags - should be the index of the second "<"
        tweetText.text = colorTags + tweetText.text;
    }

    // Call this on Start, whenever a new Tweet is loaded
    void UpdateTweetTextString()
    {
		tweetText.text = tweet.status;
		currentTweetText = tweetText.text;
        tweetTyped = false;
		done = false;
        charsTyped = 0;
    }

    void Update()
    {
		if (!done) {
			if (Input.GetKeyDown("tab")) {
				done = true;
				SkipTweet ();
			} else {
				if (tweetTyped) {
					
					if (Input.GetKeyDown("return")) {
						done = true;
						SendTweet ();
					} 
				} else {
					if (Input.anyKeyDown) {
						Debug.Log (currentTweetText);
						if (Input.inputString == currentTweetText [0].ToString ()) {
							Debug.Log ("Typed the correct character: " + Input.inputString);                   
							currentTweetText = currentTweetText.Remove (0, 1);
							Debug.Log ("Remaining text: " + currentTweetText);

							ColorTypedChar ();
							charsTyped += 1;

							if (currentTweetText.Length == 0) {
								tweetTyped = true;
								AnalyticManager.incrementWordCount ();
								Debug.Log ("The Tweet has been typed out. Press submit.");
							} else if (Input.GetKeyDown("space")) {
								AnalyticManager.incrementWordCount ();
							}


						} else {
							//						GetComponent<AudioSource>().PlayOneShot(errorSound);
						}
					}
				}
			}
		}


		transform.position = Vector3.MoveTowards(transform.position, targetPosition, velocity * Time.deltaTime);
    }

    void ColorTypedChar()
    {
        // Moves typed char within color style tag
        char typedChar = tweetText.text[tagStringLength + charsTyped];
        tweetText.text = tweetText.text.Remove(tagStringLength + charsTyped, 1);
        tweetText.text = tweetText.text.Insert(firstTagEnds + charsTyped, typedChar.ToString());
    }

    // Called when Enter Key is pressed
    public void SendTweet()
    {
		GetComponent<AudioSource>().PlayOneShot(tweetSound);
        if(tweet.name == "realDonaldTrump")
        {
            approvalRatingBar.approvalRating += sendTweetCorrectApprovalBonus;
            Debug.Log("Your Tweet has been sent. Correct!");
        }
        else
        {
            approvalRatingBar.approvalRating -= sendTweetIncorrectApprovalPenalty;
			Debug.Log ("Your Tweet has been sent. Incorrect!");
        }
          
		DateTime actionTime = DateTime.Now;
		action.timeSpent = (float)(actionTime - createTime).TotalSeconds;
		action.action = "Sent";

		GameState.state = GameState.State.TweetSent;   

		saveAnalytics ();
    }

	public void SkipTweet()
	{
        if(tweet.name == "realDonaldTrump")
        {
            approvalRatingBar.approvalRating -= skipTweetIncorrectApprovalPenalty;
            Debug.Log("You skipped that Tweet. Incorrect!");
        }
        else
        {
            approvalRatingBar.approvalRating += skipTweetCorrectApprovalBonus;
            Debug.Log("You skipped that Tweet. Correct!");
        }

		DateTime actionTime = DateTime.Now;
		action.timeSpent = (float)(actionTime - createTime).TotalSeconds;
		action.action = "Skipped";

		GameState.state = GameState.State.TweetSkipped;

		saveAnalytics ();
	}

	private void saveAnalytics() {
		AnalyticManager.save ();
	}
}
