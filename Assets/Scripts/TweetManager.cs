using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TweetManager : MonoBehaviour {

    public Text tweetText;
	public Tweet tweet;
//    public Button sendButton;
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

    // These are needed for changing which letters are colored as they are typed
    private int tagStringLength; // The first real letter of the tweet exists at this index
    private int firstTagEnds; // Typed letters get moved here so they become colored

    void Start()
    {

		targetPosition = new Vector3(0.2056804f, -2.582315f, 0);
		velocity = 80;

		tweet = TweetBank.getRandomTweet ();

        UpdateTweetTextString();
        AddColorTagsToTweet();
//        sendButton.interactable = false;
    }

    void AddColorTagsToTweet()
    {
        string colorTags = "<color=blue></color>";
        tagStringLength = colorTags.Length;
        firstTagEnds = 12; // Change this if you change colorTags - should be the index of the second "<"
        Debug.Log(tagStringLength);
        tweetText.text = colorTags + tweetText.text;
    }

    // Call this on Start, whenever a new Tweet is loaded
    void UpdateTweetTextString()
    {
		tweetText.text = tweet.status;
		currentTweetText = tweetText.text;
        tweetTyped = false;
		done = false;
//        sendButton.interactable = false;
        charsTyped = 0;
    }

    void Update()
    {
		if (!done) {
			if (Input.GetKeyDown("tab")) {
				done = true;
				Debug.Log ("SKIP");
				SkipTweet ();
			} else {
				if (tweetTyped) {
					
					if (Input.GetKeyDown("return")) {
						done = true;
						Debug.Log ("SUBMIT");
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
								// sendButton.interactable = true;
								Debug.Log ("The Tweet has been typed out. Press submit.");
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
        Debug.Log("Your Tweet has been sent.");
		GameState.state = GameState.State.TweetSent;

    }


	public void SkipTweet()
	{
		
		Debug.Log("Your Tweet has been skipped.");
		GameState.state = GameState.State.TweetSkipped;

	}
}
