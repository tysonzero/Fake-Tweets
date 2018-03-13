using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ApprovalRatingBar : MonoBehaviour {

    public Image approvalRatingBar_Fill;
    public float approvalDecrementScalar = 0.01f;

    public float approvalRating;

	// Use this for initialization
	void Start () {
        approvalRating = 1.0f;
	}
	
	// Update is called once per frame
	void Update () {
        approvalRating -= approvalDecrementScalar * Time.deltaTime;
        approvalRatingBar_Fill.fillAmount = approvalRating;

    }
}
