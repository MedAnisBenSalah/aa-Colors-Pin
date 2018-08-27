using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DeathController : MonoBehaviour
{
    public Text resultText, hintText, levelText;
    public Button skipLevelButton;

    private static int gamesPlayedSinceStart = 0;
    private static readonly int GAMES_TO_SHOW_INTERSTITIAL_AD = 4;

    private static readonly int SKIP_LEVEL_ATTEMPS = 6;

	void Start ()
    {
        // Set level text
        levelText.text = "Level " + GamePersistence.gameData.level;
	    // Did we win the level ?
        if (GameController.hasWon)
        {
            // Change texts
            resultText.text = "PASS!";
            hintText.text = "NEXT LEVEL ...";
            // Change text colors
            resultText.color = hintText.color = GameController.WIN_COLOR;
            // Hide skip level button
            skipLevelButton.gameObject.SetActive(false);
        }
        else
        {
            // Check if we need to show the skip level button
            bool visible = GameController.levelAttempts >= SKIP_LEVEL_ATTEMPS && GamePersistence.gameData.level == GamePersistence.gameData.highestLevel
                && AdsController.IsRewardedVideoAdLoaded();

            skipLevelButton.gameObject.SetActive(visible);
        }
        // Increase the games played count
        gamesPlayedSinceStart++;
        // Show ads
        if (gamesPlayedSinceStart % GAMES_TO_SHOW_INTERSTITIAL_AD == 0)
            AdsController.ShowInterstitialAd();

        // Show banner ad
        AdsController.ShowBannerAd();

    }
	
	void Update () 
    {
	    // Did we finish the rewarded ad video ?
        if(AdsController.IsRewardedVideoAdFinished())
        {
            // Increase highest level
            GamePersistence.gameData.highestLevel++;
            GamePersistence.gameData.level = GamePersistence.gameData.highestLevel;
            // Save
            GamePersistence.Save();
            // Start the game scene
            SceneManager.LoadScene("Game");
        }
	}

    public void OnPlayButton()
    {
        // Load the game scene
        SceneManager.LoadScene("Game");
    }

    public void OnLevelsButton()
    {
        // Load the levels scene
        SceneManager.LoadScene("Levels");
    }

    public void OnSkipLevelButton()
    {
        // Reset level attempts flag
        GameController.levelAttempts = 0;
        // Play a rewarded ad video
        AdsController.ShowRewardedVideoAd();
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
            // Notify application focus on ads
            AdsController.OnResume();
    }
}
