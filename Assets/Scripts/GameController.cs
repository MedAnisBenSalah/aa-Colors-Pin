using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public Image mainCircle, mainConnector, speedProgressBar, colorProgressBar, secondChanceCounterImage;
    public Canvas canvas;
    public Text connectorsCountText, levelText, speedText, colorText, connectorsLeftText;
    public GameObject secondChanceLayer;

    private ArrayList circles, connectors, staticConnectors;
    private int cirlceColors, connectorsCount;
    private float rotationSpeed;
    private Image currentConnector, lastConnector;
    private int currentConnectorIndex, lastConnectorIndex;
    private bool gameOver, gameOverAnimationPlaying, won;
    private float gameOverCounter;
    private LevelManager.Level currentLevel;
    private bool defaultSpeed;
    private float timeSinceLastSpeedChange, timeSinceLastColorChange;
    public static bool hasWon;
    public static int levelAttempts = 0;

    private bool secondChanceShowing, hasPlayedSecondChance;
    private float secondChanceTimer;

    private int[] connectorsColorCount;
    private int maxConnectorsCountPerColor, connectorsAllowedColorsBeyondMax;

    private Vector3 rotationVector;

    private static readonly Vector2 CONNECTOR_POSITION = new Vector2(0f, -280f);
    private static readonly float DISTANCE_BETWEEN_CONNECTORS = 70f;
    private static readonly float GAME_OVER_TIME = 2f;
    private static readonly float GAME_OVER_ANIMATION_TIME = 1f;
    private static readonly float SECOND_CHANCE_TIME = 4f;
    private static readonly float WIN_ROTATION_SPEED = 10f;
    public static readonly Color LOSE_COLOR = new Color(1f, 0.0941f, 0.3058f);
    public static readonly Color WIN_COLOR = new Color(0f, 1f, 0.1921f);

    public static readonly Color[] colors =
    {
        new Color(1f, 0f, 0.5f), // Pink
        new Color(0.2078f, 0.8862f, 0.949f), // Blue
        new Color(0.549f, 0.0705f, 0.9843f), // Purple
        new Color(0.9568f, 0.8745f, 0.0549f), // Yellow
        new Color(0.1764f, 0.7647f, 0.1568f), // Green
        new Color(0.9372f, 0.4156f, 0.1921f), // Orange
    };

    public Color[] currentColors;

	void Start ()
    {
        // Temp
        //GamePersistence.Load();
        //GamePersistence.gameData.level = LevelManager.levels.Length;


        // Create rotation vector
        rotationVector = new Vector3();
        // Create arrays
        circles = new ArrayList();
        connectors = new ArrayList();
        staticConnectors = new ArrayList();
        currentColors = new Color[colors.Length];
        // Add the main circle
        circles.Add(mainCircle);
        // Reset values
        gameOver = false;
        defaultSpeed = true;
        connectorsColorCount = null;
        gameOverCounter = 0f;
        timeSinceLastSpeedChange = 0f;
        timeSinceLastColorChange = 0f;
        gameOverAnimationPlaying = false;
        secondChanceShowing = false;
        hasPlayedSecondChance = false;
        secondChanceTimer = 0f;
        lastConnector = null;
        lastConnectorIndex = 0;
        // Start the game
        StartGame();
    }

    void Update()
    {
        // Are we seeing second chance UI ?
        if(secondChanceShowing)
        {
            // Did we finish the video ?
            if(AdsController.IsRewardedVideoAdFinished())
            {
                // Restore hit
                RestoreLastHit();
                return;
            }
            // Skip if we're already watching ad
            if (hasPlayedSecondChance)
                return;

            // Update timer
            secondChanceTimer += Time.deltaTime;
            // Update counter image
            secondChanceCounterImage.fillAmount = (SECOND_CHANCE_TIME - secondChanceTimer) / SECOND_CHANCE_TIME;
            // Did we finish second chance ?
            if (secondChanceTimer >= SECOND_CHANCE_TIME)
                // Lose the game
                LoseGame();

            return;
        }
        // Rotate all circles
        if (!gameOver)
        {
            // Update connectors
            UpdateConnectors();
            // Update circles
            foreach (Image circle in circles)
                circle.rectTransform.Rotate(rotationVector);           
        }
        else
        {
            // Rotate if we won the game
            if(won)
            {
                foreach (Image circle in circles)
                    circle.rectTransform.Rotate(rotationVector);
            }
            // Did we finish game over ?
            gameOverCounter += Time.deltaTime;
            if(gameOverCounter >= GAME_OVER_TIME)
                // Load death scene
                SceneManager.LoadScene("Death");
            else if(gameOverCounter >= GAME_OVER_ANIMATION_TIME && !gameOverAnimationPlaying)
            {
                // Connectors on game over
                foreach (Image connector in connectors)
                    connector.GetComponent<Connector>().SetRotationSpeed(0f);

                // Static connectors on game over
                foreach (Image connector in staticConnectors)
                    connector.GetComponent<Connector>().SetRotationSpeed(0f);

                // Set game over animation flag
                gameOverAnimationPlaying = true;
            }
        }
        // Update UI
        UpdateUI();
    }

    private void UpdateConnectors()
    {
        // Do we need to change the rotation speed ?
        if (currentLevel.hasSpeedChange)
        {
            // Update time
            timeSinceLastSpeedChange += Time.deltaTime;
            // Check time
            if (timeSinceLastSpeedChange >= currentLevel.speedChangingTime)
            {
                // Update speed
                rotationSpeed = defaultSpeed ? currentLevel.newSpeed : currentLevel.circleRotationSpeed;
                rotationVector.z = rotationSpeed;
                // Connectors rotation speed change
                foreach (Image connector in connectors)
                    connector.GetComponent<Connector>().SetRotationSpeed(rotationSpeed);

                // Static connectors rotation speed change
                foreach (Image connector in staticConnectors)
                    connector.GetComponent<Connector>().SetRotationSpeed(rotationSpeed);

                // Change the default speed flag
                defaultSpeed = !defaultSpeed;
                // Reset time
                timeSinceLastSpeedChange = 0f;
            }
        }
        // Do we need to increase speed ?
        else if (currentLevel.increasingSpeed)
        {
            // Update time
            timeSinceLastSpeedChange += Time.deltaTime;
            // Check time
            if (timeSinceLastSpeedChange >= currentLevel.speedChangingTime)
            {
                // Do we need to change direction ?
                if (currentLevel.directionChange)
                    // Update speed
                    rotationSpeed = (rotationSpeed > 0f ? -currentLevel.circleRotationSpeed : currentLevel.circleRotationSpeed);
                else
                    // Update speed
                    rotationSpeed = currentLevel.circleRotationSpeed;

                rotationVector.z = rotationSpeed;
                // Connectors rotation speed change
                foreach (Image connector in connectors)
                    connector.GetComponent<Connector>().SetRotationSpeed(rotationSpeed);

                // Static connectors rotation speed change
                foreach (Image connector in staticConnectors)
                    connector.GetComponent<Connector>().SetRotationSpeed(rotationSpeed);

                // Reset counter
                timeSinceLastSpeedChange = 0f;
            }
            else
            {
                // Update speed
                if(rotationSpeed > 0f)
                    rotationSpeed += Mathf.Abs(currentLevel.newSpeed - currentLevel.circleRotationSpeed) / 
                        currentLevel.speedChangingTime * Time.deltaTime;
                else
                    rotationSpeed -= Mathf.Abs(currentLevel.newSpeed - currentLevel.circleRotationSpeed) /
                        currentLevel.speedChangingTime * Time.deltaTime;

                rotationVector.z = rotationSpeed;
                // Connectors rotation speed change
                foreach (Image connector in connectors)
                    connector.GetComponent<Connector>().SetRotationSpeed(rotationSpeed);

                // Static connectors rotation speed change
                foreach (Image connector in staticConnectors)
                    connector.GetComponent<Connector>().SetRotationSpeed(rotationSpeed);
            }
        }
       
        // Do we need to change the colors ?
        if (currentLevel.hasColorChange)
        {
            // Update time
            timeSinceLastColorChange += Time.deltaTime;
            // Check time
            if (timeSinceLastColorChange >= currentLevel.colorChangingTime)
            {
                // Refresh colors
                RefreshColors();
                // Update circle colors
                int i = 0;
                foreach (Image circle in circles)
                {
                    circle.color = currentColors[i];
                    i++;
                }
                // Update connectors
                foreach (Image connector in connectors)
                {
                    Connector script = connector.GetComponent<Connector>();
                    script.SetColor(currentColors[script.GetColorIndex()]);
                }
                // Reset time
                timeSinceLastColorChange = 0f;
            }
            // Flush
            if (timeSinceLastColorChange >= currentLevel.colorChangingTime - 0.05f)
            {
                // Update connectors
                foreach (Image connector in connectors)
                    connector.GetComponent<Connector>().SetColor(Color.white);

                // Update circle colors
                foreach (Image circle in circles)
                    circle.color = Color.white;             
            }
        }

        // Did we tap the screen ?
        if (currentConnector != null && Input.GetMouseButtonDown(0))
        {
            // Connect the connector
            currentConnector.GetComponent<Connector>().Connect();
            // Set last connector
            lastConnector = currentConnector;
            lastConnectorIndex = currentConnectorIndex;
            // Did we finish all the connectors ?
            if (currentConnectorIndex < connectors.Count)
            {
                // Get the current connector instance
                currentConnector = (Image)connectors[currentConnectorIndex];
                currentConnectorIndex++;
            }
            else
            {
                currentConnector = null;
                currentConnectorIndex++;
            }
        }
    }
    
    private void UpdateUI()
    {
        // Update connectors count text
        if(!gameOver)
            connectorsCountText.text = "" + (connectorsCount - currentConnectorIndex + 1);

        // Update speed UI
        if (currentLevel.hasSpeedChange || currentLevel.increasingSpeed)
            speedProgressBar.fillAmount = timeSinceLastSpeedChange / currentLevel.speedChangingTime;

        // Update color UI
        if (currentLevel.hasColorChange)
            colorProgressBar.fillAmount = timeSinceLastColorChange / currentLevel.colorChangingTime;
    }

    private void StartGame()
    {
        // Load level
        LoadLevel();
        // Calculate the fill amount
        float fillAmount = 1f / cirlceColors;
        float rotation = 360f / cirlceColors;
        // Set the main circle's properties
        mainCircle.color = currentColors[0];
        mainCircle.fillAmount = fillAmount;
        // Loop through the circles      
        for (int i = 1; i < cirlceColors; i++)
        {
            // Instantiate a new circle
            Image newCircle = Instantiate(mainCircle, canvas.transform);
            newCircle.color = currentColors[i];
            // Set it's fill amount
            newCircle.fillAmount = fillAmount;
            newCircle.rectTransform.Rotate(0f, 0f, rotation * i);
            // Add it to the circles array
            circles.Add(newCircle);
        }
        // Add circle collider to the main circle
        CircleCollider2D collider = mainCircle.gameObject.AddComponent<CircleCollider2D>();
        collider.radius = 270f;
        // Update the level's text
        levelText.text = "Level\n" + GamePersistence.gameData.level;
        // Increase the games played
        GamePersistence.gameData.gamesPlayed++;
        GamePersistence.Save();
        // Hide second chance UI
        secondChanceLayer.SetActive(false);
    }

    private void LoadLevel()
    {
        // Get the level configuration
        currentLevel = LevelManager.levels[GamePersistence.gameData.level - 1];
        // Reset values
        currentConnectorIndex = 0;
        // Set game properties
        cirlceColors = currentLevel.colorsCount;
        connectorsCount = currentLevel.connectorsCount;
        rotationSpeed = currentLevel.circleRotationSpeed;
        // Update the rotation's vector
        rotationVector.z = rotationSpeed;
        // Create connectors color count array
        connectorsColorCount = new int[cirlceColors];
        for (int i = 0; i < cirlceColors; i++)
            connectorsColorCount[i] = 0;

        // Setup current colors
        RefreshColors();
        // Calculate the connectors count per color
        maxConnectorsCountPerColor = connectorsCount / cirlceColors;
        connectorsAllowedColorsBeyondMax = connectorsCount % cirlceColors;
        // Create connectors
        for(int i = 0; i < (connectorsCount > 3 ? 3 : connectorsCount); i++)
        {
            // Instantiate a new connector
            Image newConnector = Instantiate(mainConnector, canvas.transform);
            Vector2 position = new Vector2(CONNECTOR_POSITION.x, CONNECTOR_POSITION.y - DISTANCE_BETWEEN_CONNECTORS * i);
            // Generate a random color index
            int colorIndex;
            bool allowBeyondMax = false;
            do
            {
                // Get random color index within the allowed range
                colorIndex = (int)Random.Range(0, cirlceColors);
                // Check colors beyond maximum allowed
                if(connectorsColorCount[colorIndex] >= maxConnectorsCountPerColor && connectorsAllowedColorsBeyondMax > 0)
                {
                    // Decrease the allowed colors beyond max count
                    connectorsAllowedColorsBeyondMax--;
                    allowBeyondMax = true;
                }
            } while (!allowBeyondMax && connectorsColorCount[colorIndex] >= maxConnectorsCountPerColor);

            // Increase the chosen color count
            connectorsColorCount[colorIndex]++;
            // Get the specified color
            Color color = currentColors[colorIndex];
            // Setup connector
            newConnector.rectTransform.anchoredPosition = position;       
            newConnector.GetComponent<Connector>().Setup(rotationSpeed, color, colorIndex);
            // Add it to the connectors array
            connectors.Add(newConnector);
        }
        // Get the current connector instance
        currentConnector = (Image)connectors[currentConnectorIndex];
        currentConnectorIndex++;
        // Do we have static connectors ?
        if(currentLevel.hasStaticConnectors)
        {
            // Create static connectors
            for (int i = 0; i < currentLevel.staticConnectorsRotation.Length; i++)
            {
                // Instantiate a new connector
                Image newConnector = Instantiate(mainConnector, canvas.transform);
                // Setup connector
                newConnector.GetComponent<Connector>().SetupAsStatic(rotationSpeed, currentLevel.staticConnectorsRotation[i]);
                // Add it to the connectors array
                staticConnectors.Add(newConnector);
            }
        }
        // Set the default speed flag
        defaultSpeed = true;
        timeSinceLastSpeedChange = 0f;
        timeSinceLastColorChange = 0f;
        // Toggle speed and color UI
        speedProgressBar.gameObject.SetActive(currentLevel.hasSpeedChange || currentLevel.increasingSpeed);
        speedText.gameObject.SetActive(currentLevel.hasSpeedChange || currentLevel.increasingSpeed);
        colorProgressBar.gameObject.SetActive(currentLevel.hasColorChange);
        colorText.gameObject.SetActive(currentLevel.hasColorChange);
        // Update speed text
        if (currentLevel.increasingSpeed)
            speedText.text = "Speed\nIncreasing";
    }

    private void RefreshColors()
    {
        // Loop through the circle colors
        for (int i = 0; i < cirlceColors; i++)
        {
            // Generate a random color index
            int colorIndex;
            bool foundColor;
            do
            {
                foundColor = true;
                colorIndex = (int)Random.Range(0, cirlceColors);               
                // Did we chose the color yet ?
                if (i > 0)
                {
                    // Loop through the chosen colors
                    for (int j = 0; j < i; j++)
                    {
                        // Do we have the color is our pool ?
                        if (currentColors[j] == colors[colorIndex])
                        {
                            foundColor = false;
                            break;
                        }
                    }
                }
            } while (!foundColor);

            // Set the color
            currentColors[i] = colors[colorIndex];
        }
    }

    public void OnConnectorConnect(Connector connector)
    {
        RectTransform connectorTransform = connector.GetComponent<RectTransform>();
        // Check if we did hit any other connectors
        bool hit = false;
        float width = mainConnector.rectTransform.rect.width;
        foreach(Image otherConnector in connectors)
        {
            // Skip current connector
            if (otherConnector.rectTransform == connectorTransform)
                continue;

            // Check distance
            if (Vector2.Distance(otherConnector.rectTransform.anchoredPosition, connectorTransform.anchoredPosition)
                <= 40f)
            {
                hit = true;
                // Play death animation on ourselves
                connector.GetComponent<Animation>().Play(PlayMode.StopAll);
                // Play death animation on our collider
                otherConnector.GetComponent<Animation>().Play(PlayMode.StopAll);
                break;
            }
        }
        // Check if we did hit any static connectors
        if (!hit)
        {
            foreach (Image otherConnector in staticConnectors)
            {
                // Check distance
                if (Vector2.Distance(otherConnector.rectTransform.anchoredPosition, connectorTransform.anchoredPosition)
                    <= 40f)
                {
                    hit = true;
                    // Play death animation on ourselves
                    connector.GetComponent<Animation>().Play(PlayMode.StopAll);
                    // Play death animation on our collider
                    otherConnector.GetComponent<Animation>().Play(PlayMode.StopAll);
                    break;
                }
            }
        }
        // Did we lose ?
        if (hit)
            LoseGame();
        else
        {
            // Get circle rotation
            float angle = mainCircle.transform.eulerAngles.z;
            // Is it the right color ?
            Color hitColor = GetColorByAngle(angle);
            if (hitColor == connector.GetColor())
            {
                // Did we succesfully connected all connectors ?
                if (currentConnectorIndex > connectors.Count)
                    WinGame();
                else
                {
                    // Translate the remaining connectors
                    if (currentConnector != null)
                    {
                        // Translate to the top
                        currentConnector.GetComponent<Connector>().TranslateTo(CONNECTOR_POSITION);
                        // Translate the next one
                        if (currentConnectorIndex < connectors.Count)
                        {
                            ((Image)connectors[currentConnectorIndex]).GetComponent<Connector>().TranslateTo(
                                new Vector2(CONNECTOR_POSITION.x, CONNECTOR_POSITION.y - DISTANCE_BETWEEN_CONNECTORS));
                        }
                    }

                    // Do we need to create a new connector
                    if (connectors.Count < connectorsCount)
                    {
                        // Instantiate a new connector
                        Image newConnector = Instantiate(mainConnector, canvas.transform);
                        Vector2 position = new Vector2(CONNECTOR_POSITION.x, CONNECTOR_POSITION.y - DISTANCE_BETWEEN_CONNECTORS * 3);
                        // Generate a random color index
                        int colorIndex;
                        bool allowBeyondMax = false;
                        do
                        {
                            // Get random color index within the allowed range
                            colorIndex = (int)Random.Range(0, cirlceColors);
                            // Check colors beyond maximum allowed
                            if (connectorsColorCount[colorIndex] >= maxConnectorsCountPerColor && connectorsAllowedColorsBeyondMax > 0)
                            {
                                // Decrease the allowed colors beyond max count
                                connectorsAllowedColorsBeyondMax--;
                                allowBeyondMax = true;
                            }
                        } while (!allowBeyondMax && connectorsColorCount[colorIndex] >= maxConnectorsCountPerColor);

                        // Increase the chosen color count
                        connectorsColorCount[colorIndex]++;
                        // Get the specified color
                        Color color = currentColors[colorIndex];
                        // Setup the connector
                        newConnector.rectTransform.anchoredPosition = position;
                        newConnector.GetComponent<Connector>().Setup(rotationSpeed, color, colorIndex);
                        // Translate it to the last position
                        newConnector.GetComponent<Connector>().TranslateTo(
                            new Vector2(CONNECTOR_POSITION.x, CONNECTOR_POSITION.y - DISTANCE_BETWEEN_CONNECTORS * 2));

                        // Add it to the connectors array
                        connectors.Add(newConnector);
                    }
                }
            }
            else
            {
                // Play death animation on ourselves
                connector.GetComponent<Animation>().Play(PlayMode.StopAll);
                // Lose the game
                LoseGame();
            }
        }
        
    }

    public void LoseGame()
    {
        // Show second chance UI
        if (!secondChanceShowing && !hasPlayedSecondChance && AdsController.IsRewardedVideoAdLoaded())
        {
            // Set second chance showing
            secondChanceShowing = true;
            gameOver = true;
            // Show second chance UI
            secondChanceLayer.SetActive(true);
            secondChanceLayer.transform.SetAsLastSibling();
            // Reset second chance counter
            secondChanceTimer = 0;
        }
        else
        {
            // Set game over flag
            gameOver = true;
            won = false;
            hasWon = false;
            secondChanceShowing = false;
            // Hide second chance UI
            secondChanceLayer.SetActive(false);
            // Change connectors count text
            connectorsCountText.text = "Level Failed!";
            connectorsCountText.color = LOSE_COLOR;
            // Hide color and speed ui
            speedProgressBar.gameObject.SetActive(false);
            speedText.gameObject.SetActive(false);
            colorProgressBar.gameObject.SetActive(false);
            colorText.gameObject.SetActive(false);
            connectorsLeftText.gameObject.SetActive(false);
            // Increase the level attempts
            levelAttempts++;
        }
    }

    void WinGame()
    {
        // Set game over flags
        gameOver = true;
        won = true;
        hasWon = true;
        // Increase the rotation speed
        rotationSpeed = WIN_ROTATION_SPEED * (rotationSpeed < 0 ? -1 : 1);
        rotationVector.z = rotationSpeed;
        // Connectors on game over
        foreach (Image connector in connectors)
            connector.GetComponent<Connector>().SetRotationSpeed(rotationSpeed);

        // Static connectors on game over
        foreach (Image connector in staticConnectors)
            connector.GetComponent<Connector>().SetRotationSpeed(rotationSpeed);

        // Change connectors count text
        connectorsCountText.text = "Level Completed!";
        connectorsCountText.color = WIN_COLOR;
        // Hide color and speed ui
        speedProgressBar.gameObject.SetActive(false);
        speedText.gameObject.SetActive(false);
        colorProgressBar.gameObject.SetActive(false);
        colorText.gameObject.SetActive(false);
        connectorsLeftText.gameObject.SetActive(false);
        // Increase the current level
        GamePersistence.gameData.level++;
        if (GamePersistence.gameData.level > GamePersistence.gameData.highestLevel)
            GamePersistence.gameData.highestLevel = GamePersistence.gameData.level;

        // If we finished the game then reset the current level
        if (GamePersistence.gameData.level > LevelManager.levels.Length)
            GamePersistence.gameData.level = 1;

        GamePersistence.Save();
        // Reset level attempts
        levelAttempts = 0;
    }

    public bool IsGameOver()
    {
        return gameOver;
    }

    public bool IsWon()
    {
        return won;
    }

    private Color GetColorByAngle(float angle)
    {
        // Get angle friction
        float circleAngle = 360f / cirlceColors;
        // Get circle's color
        return currentColors[cirlceColors - 1 - (int)(angle / circleAngle)];
    }

    public void OnSecondChanceButton()
    {
        // Play rewarded ad video
        AdsController.ShowRewardedVideoAd();
    }

    private void RestoreLastHit()
    {
        // Restore the current connector
        currentConnector = lastConnector;
        currentConnectorIndex = lastConnectorIndex;
        // Set it back to its previous position
        currentConnector.GetComponent<Connector>().Restore(CONNECTOR_POSITION);
        // Hide second chance UI
        secondChanceLayer.SetActive(false);
        // Reset flags
        secondChanceShowing = false;
        hasPlayedSecondChance = true;
        gameOver = false;
        // Stop all connectors animations
        foreach(Image connector in connectors)
        {
            // Stop any animation
            connector.GetComponent<Animation>().Stop();
            // Restore size
            connector.rectTransform.localScale = new Vector3(1f, 1f, 1f);
        }
        // Stop all static connectors animations
        foreach (Image connector in staticConnectors)
        {
            // Stop any animation
            connector.GetComponent<Animation>().Stop();
            // Restore size
            connector.rectTransform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
            // Notify application focus on ads
            AdsController.OnResume();
    }
}
