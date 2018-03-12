using System.Collections;
using System.Collections.Generic;

public class Candidate {

	public string name;
	public int votes;

	public Candidate(string name) {
		this.name = name;
		this.votes = 0;
	}

	public void addRandomVotes() {
		// Should add random amount of votes to the candidate
	}

	public void addVotes() {
		// Should add specific amount of votes to the candidate
	}

	public string getTweetImageFilePath() {
		// Should return the file in Image folder that will be '<NAME>_tweet.png', where <NAME> 
		// is name field in lower case and space replaced with '_'.
		return "";
	}

	public string getProfileImageFilePath() {
		// Should return the file in Image folder that will be '<NAME>.png', where <NAME> 
		// is name field in lower case and space replaced with '_'.
		return "";
	}
}
