using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ApprovalRatingBar : MonoBehaviour {

    public GameObject endGamePanel;

    public Image approvalRatingBar_Fill;
    public float approvalDecrementScalar = 0.01f;

    public float approvalRating;

    public bool isFired;

	// Use this for initialization
	void Start ()
    {
        approvalRating = 1.0f;
        isFired = false;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(!isFired)
        {
            approvalRating -= approvalDecrementScalar * Time.deltaTime;
            BoundApprovalRating();
            approvalRatingBar_Fill.fillAmount = approvalRating;
            if (approvalRating <= 0.0f)
            {
                TriggerGameEndState();
            }
        }      
    }

    void TriggerGameEndState()
    {
        isFired = true;
        endGamePanel.SetActive(true);
        Debug.Log("YOU'RE FIRED!");
    }

    void BoundApprovalRating()
    {
        if(approvalRating > 1.0f)
        {
            approvalRating = 1.0f;
        }
        else if (approvalRating < 0.0f)
        {
            approvalRating = 0.0f;
        }
    }
}
