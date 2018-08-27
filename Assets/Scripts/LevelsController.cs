using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelsController : MonoBehaviour
{
    public Button levelContainer;
    public Canvas canvas;

    private RectTransform levelContainerTransform;

    private static readonly float STARTING_POSITION_X = -322.5f;
    private static readonly float STARTING_POSITION_Y = 486f;
    private static readonly float DISTANCE_BETWEEN_CONTAINERS = 37.5f;

    void Start ()
    {
        // Get rect transform
        levelContainerTransform = levelContainer.GetComponent<RectTransform>();
        // Loop through the levels
        Vector2 size = new Vector2(levelContainerTransform.rect.width, levelContainerTransform.rect.height);
        Vector2 position = new Vector2(STARTING_POSITION_X + DISTANCE_BETWEEN_CONTAINERS, STARTING_POSITION_Y - DISTANCE_BETWEEN_CONTAINERS);
        for (int i = 0; i < GamePersistence.gameData.highestLevel; i++)
        {
            // Instantiate a level controller
            Button newLevelContainer = Instantiate(levelContainer, canvas.transform);
            newLevelContainer.GetComponent<RectTransform>().anchoredPosition = position;
            // Update the number
            newLevelContainer.GetComponentInChildren<Text>().text = "" + (i + 1);
            // Add a listener to the button
            int level = i + 1;
            newLevelContainer.onClick.AddListener(delegate { OnSelectLevel(level); });
            // Update position
            position.x += size.x + DISTANCE_BETWEEN_CONTAINERS;
            if (position.x > -STARTING_POSITION_X)
            {
                position.x = STARTING_POSITION_X + DISTANCE_BETWEEN_CONTAINERS;
                position.y -= size.y + DISTANCE_BETWEEN_CONTAINERS;
            }
        }
    }
	
	void Update ()
    {
		
	}

    public void OnBackButton()
    {
        // Load the main menu scene
        SceneManager.LoadScene("MainMenu");
    }

    void OnSelectLevel(int level)
    {
        // Set the selected level
        GamePersistence.gameData.level = level;
        GamePersistence.Save();
        // Load the game scene
        SceneManager.LoadScene("Game");
    }
}
