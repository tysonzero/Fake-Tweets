using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameState{

	public enum State {Neutral, TweetSent, TweetSkipped};  

	public static State state = State.TweetSent;


}
