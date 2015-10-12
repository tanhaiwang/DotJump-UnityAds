using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Advertisements;

public class UIController : MonoBehaviour {

	// Use this for initialization


	public TextMesh scoreTextMesh,ScoreShadowTextMesh,finalScoreTextMesh,BestScoreTextMesh;
	int inGameScore =0;
	public GameObject newScoreObj,ScoreBoard,MainMenuContainer,Tutorial,QuitButton;
	public Camera UICamera;
	public static bool isRestartPressed  = false;
	bool readyToPlay  = false;


	void OnEnable(){
		
		playerController.IncreaseScore += OnScoreIncrease;
		playerController.PlayerDead += OnPlayerDead;
		MainMenuContainer.SetActive(true);
		newScoreObj.SetActive(false);
		ScoreBoard.SetActive(false);

		//on game end scoreboard menu ,if user presses on restart ,
		//we need to disable the mainmenu and changing player to readyto play.
		if(isRestartPressed)
		{ MainMenuContainer.SetActive(false);
			isRestartPressed = false;
			readyToPlay = true;
		} 
		else {
			playerController.currentState = playerController.playerStates.idle;
            }

	}
	 
	// Update is called once per frame
	RaycastHit hit ;
	void Update () {

		if(Input.GetKeyDown(KeyCode.Mouse0) )
		{

//			Debug.Log(playerController.currentState);
			Ray R = UICamera.ScreenPointToRay(Input.mousePosition);
			if( readyToPlay && playerController.currentState == playerController.playerStates.idle)
			{
				playerController.currentState = playerController.playerStates.alive;
				readyToPlay = false;
				iTween.ScaleTo(Tutorial,iTween.Hash("scale",Vector3.zero,"time",0.5f,"easetype",iTween.EaseType.linear));
				iTween.FadeTo(Tutorial,iTween.Hash("alpha",0,"time",0.2f,"easetype",iTween.EaseType.linear));
			}
			if(Physics.Raycast(R,out hit,100))
			{
				iTween.ScaleTo(hit.collider.gameObject,iTween.Hash("scale",new Vector3(0.45f,0.45f,0.45f),"time",0.5f,"easetype",iTween.EaseType.easeInOutBounce));
				SoundController.Static.PlayClickSound();
				switch(hit.collider.name)
				{
					
				 
				case "Play":
					MainMenuContainer.SetActive(false);
					Tutorial.SetActive(true);
					readyToPlay = true;
				 
					break;
				}
			 
			}





		}

		if(Input.GetKeyUp(KeyCode.Mouse0) )
		{
			 
			Ray R = UICamera.ScreenPointToRay(Input.mousePosition);
			
			if(Physics.Raycast(R,out hit,100))
			{

				switch(hit.collider.name)
				{
					
				case "Home":
					
					restart();
					break;

                case "DoubleScore":
                        ShowRewardedAd();                      
                        break;
					
				case "Replay":
					isRestartPressed = true;
					restart();
					playerController.currentState = playerController.playerStates.idle;
					break;

				case "More":					
					Application.OpenURL("https://unity3d.com/services/ads");					
					break;

				case "Quit":

					Application.Quit();

					break;
				}
			}
			
		}

	
		//to handle escape key
		if(Input.GetKeyUp(KeyCode.Escape))
		{
			if( playerController.currentState ==playerController.playerStates.idle)
			{
				
				Application.Quit();
			}
			else {
				
				restart();
			}
			
		}
	}


	public void restart()
	{
		Application.LoadLevel(Application.loadedLevelName);
	}
	void OnScoreIncrease(System.Object obj,EventArgs args) 
	{
		inGameScore++;
        scoreTextMesh.text =""+ inGameScore;
		ScoreShadowTextMesh.text=""+inGameScore;
		SoundController.Static.PlayScoreIncrease();
	}
	void OnPlayerDead(System.Object obj,EventArgs args)
	{       

        ScoreBoard.transform.localScale = Vector3.zero;
		ScoreBoard.SetActive(true);

      
	 	iTween.ScaleTo(ScoreBoard,iTween.Hash("scale",Vector3.one,"time",1.5f,"easetype",iTween.EaseType.easeOutSine,"delay",1.0f));
		scoreTextMesh.GetComponent<Renderer>().enabled = false;
		ScoreShadowTextMesh.GetComponent<Renderer>().enabled= false;
		finalScoreTextMesh.text = "Score : "+inGameScore;

		if(PlayerPrefs.GetInt("BestScore",0) < inGameScore ){
			PlayerPrefs.SetInt("BestScore",inGameScore);
			newScoreObj.SetActive(true);
		}

		BestScoreTextMesh.text = "Best : " + PlayerPrefs.GetInt("BestScore",inGameScore);
		
	}
	void OnDisable()
	{

		playerController.IncreaseScore -= OnScoreIncrease;
		playerController.PlayerDead -= OnPlayerDead;

		 
	}

    public void ShowRewardedAd()
    {
        if (Advertisement.IsReady("rewardedVideo"))
        {
            var options = new ShowOptions { resultCallback = HandleShowResult };
            Advertisement.Show("rewardedVideo", options);
        }
    }

    private void HandleShowResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                Debug.Log("The ad was successfully shown.");
                inGameScore = inGameScore * 2;
                finalScoreTextMesh.text = "Score : " + inGameScore;
                if (PlayerPrefs.GetInt("BestScore", 0) < inGameScore)
                {
                    PlayerPrefs.SetInt("BestScore", inGameScore);
                    newScoreObj.SetActive(true);
                }

                BestScoreTextMesh.text = "Best : " + PlayerPrefs.GetInt("BestScore", inGameScore);
                break;
            case ShowResult.Skipped:
                Debug.Log("The ad was skipped before reaching the end.");
                break;
            case ShowResult.Failed:
                Debug.LogError("The ad failed to be shown.");
                break;
        }
    }
}
