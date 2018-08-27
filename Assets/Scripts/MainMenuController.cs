using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class MainMenuController : MonoBehaviour
{
    public Text levelText;

	void Start () 
    {
	    // Load the game
        GamePersistence.Load();
        //GamePersistence.gameData.highestLevel = LevelManager.levels.Length;
        // Set the level text
        levelText.text = "Level " + GamePersistence.gameData.level;
        // Initialize ads
        AdsController.Initialize();
        // Show banner ad
        AdsController.ShowBannerAd();
    }

    void Update () 
    {
		
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

    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
            // Notify application focus on ads
            AdsController.OnResume();
    }
}
