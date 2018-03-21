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
	public AudioClip correctSubmitSound;
	public AudioClip wrongSubmitSound;
	public AudioClip correctSkipSound;
	public AudioClip wrongSkipSound;

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
    Dictionary<string, Sprite> spriteMap;

	public string background_name;

    void Start()
    {
        approvalRatingBar = GameObject.Find("ApprovalRatingBar").GetComponent<ApprovalRatingBar>();

		targetPosition = new Vector3(0f, -2.25f, 0);
		velocity = 50;

		tweet = TweetBank.getRandomTweet ();
		action = new Action(tweet);
		AnalyticManager.addAction (action);
		saveAnalytics ();
        handle.horizontalOverflow = HorizontalWrapMode.Overflow;
			
        UpdateTweetTextString();
        AddColorTagsToTweet();

		createTime = DateTime.Now;

        spriteMap = GameObject.Find("Main Camera").GetComponent<GameController>().spriteMap;
    }

    void AddColorTagsToTweet()
    {
        string colorTags = "<color=blue></color>"; // if you change this change firstTagEnds below
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
		if (!done)
        {
			if (Input.GetKeyDown("tab"))
            {
				done = true;
				SkipTweet ();
			}
            else
            {
				if (tweetTyped)
                {				
					if (Input.GetKeyDown("return"))
                    {
						done = true;
						SendTweet ();
					} 
				}
                else
                {
					if (Input.anyKeyDown)
                    {
						Debug.Log (currentTweetText);
						while (" .,@#\"\'&".Contains(currentTweetText [0].ToString ())) {
							InsertChar ();
						}
						if (Input.inputString.ToLower() == currentTweetText [0].ToString().ToLower())
                        {
							InsertChar ();
						}
                        else
                        {
							// GetComponent<AudioSource>().PlayOneShot(errorSound);
						}
					}
				}
			}
		}
      

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, velocity * Time.deltaTime);
    }

	void InsertChar() {
		Debug.Log ("Typed the correct character: " + Input.inputString);                   
		currentTweetText = currentTweetText.Remove (0, 1);
		Debug.Log ("Remaining text: " + currentTweetText);

		ColorTypedChar ();
		charsTyped += 1;

		if (currentTweetText.Length == 0)
        {
			tweetTyped = true;
			AnalyticManager.incrementWordCount ();
			Debug.Log ("The Tweet has been typed out. Press submit.");
		}
        else if (Input.GetKeyDown("space"))
        {
			AnalyticManager.incrementWordCount ();
		}
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
		
		if(tweet.name.Equals("realDonaldTrump"))
        {
            approvalRatingBar.approvalRating += sendTweetCorrectApprovalBonus;
			approvalRatingBar.score += 5;
			GetComponent<AudioSource>().PlayOneShot(correctSubmitSound);
            Debug.Log("Your Tweet has been sent. Correct!");
			background_name = "donald_correct";
        }
        else
        {
            approvalRatingBar.approvalRating -= sendTweetIncorrectApprovalPenalty;
			GetComponent<AudioSource>().PlayOneShot(wrongSubmitSound);
			Debug.Log ("Your Tweet has been sent. Incorrect!");
			if (tweet.name.Equals("HillaryClinton")) {
				background_name = "hillary_wrong";
			} else {
				Debug.Log ("XXX");
				background_name = "fake_donald_wrong";
			}
        }
          
		DateTime actionTime = DateTime.Now;
		action.timeSpent = (float)(actionTime - createTime).TotalSeconds;
		action.action = "Sent";

		GameState.state = GameState.State.TweetSent;   

		saveAnalytics ();
    }

	public void SkipTweet()
	{
		if(tweet.name.Equals("realDonaldTrump"))
        {
            approvalRatingBar.approvalRating -= skipTweetIncorrectApprovalPenalty;
            Debug.Log("You skipped that Tweet. Incorrect!");
			background_name = "donald_wrong";
			GetComponent<AudioSource>().PlayOneShot(wrongSkipSound);
        }
        else
        {
			GetComponent<AudioSource>().PlayOneShot(correctSkipSound);
            approvalRatingBar.approvalRating += skipTweetCorrectApprovalBonus;
			approvalRatingBar.score += 1;
            Debug.Log("You skipped that Tweet. Correct!");

			if (tweet.name.Equals("HillaryClinton")) {
				background_name = "hillary_correct";
			} else {
				Debug.Log ("XXX");
				background_name = "fake_donald_correct";
			}
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
