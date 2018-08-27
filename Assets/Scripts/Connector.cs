using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Connector : MonoBehaviour
{
    public Image mainCircle;
    public GameController gameController;

    private bool translating, connected, translatingDeath;
    private Vector3 translatingTo;
    private float rotationSpeed;
    private Color color;
    private int colorIndex;
    private LineRenderer lineRenderer;
    private bool isStatic;

    private RectTransform rectTransform;

    private static readonly float CONNECTING_SPEED = 2500f;
    private static readonly float DYING_SPEED = 1500f;
    private static readonly float LINE_WIDTH = 0.02f;
    private static readonly Vector2 POSITION_FIXED = new Vector2(0f, -180f);
    private static readonly Vector3 ROTATION_AXIS = new Vector3(0f, 0f, 1f);

    void Awake ()
    {
        // Reset values
        connected = false;
        translatingDeath = false;
        lineRenderer = null;
        colorIndex = 0;
        // Get components
        rectTransform = GetComponent<RectTransform>();
    }
	
	void Update ()
    {
        // Are translating ?
        if(translating)
        {
            // Translate
            if (translatingDeath)
                rectTransform.anchoredPosition += Vector2.up * DYING_SPEED * Time.deltaTime;
            else
                rectTransform.anchoredPosition = Vector2.MoveTowards(rectTransform.anchoredPosition, translatingTo,
                    CONNECTING_SPEED * Time.deltaTime);

            // Update line renderer
            if (lineRenderer != null)
            {
                lineRenderer.transform.position = mainCircle.transform.position;
                lineRenderer.SetPosition(0, mainCircle.transform.position);
                lineRenderer.SetPosition(1, transform.position);
            }        
            // Did we reach destination ?
            if (rectTransform.anchoredPosition.y == translatingTo.y)
                translating = false;
        }
        // Are we connected ?
        if (connected && (!gameController.IsGameOver() || gameController.IsWon()))
        {
            // Rotate along with the circle
            rectTransform.RotateAround(mainCircle.rectTransform.position, ROTATION_AXIS, rotationSpeed);
        }
	}

    private void OnTriggerEnter2D(Collider2D collider)
    {
        // Make sure this connector is not static and the game is not over yet
        if (isStatic || gameController.IsGameOver())
            return;

        // Did we collide with a connector ?
        if (collider.tag.Equals("Connector"))
        {
            // Play death animation on ourselves
            GetComponent<Animation>().Play(PlayMode.StopAll);
            // Play death animation on our collider
            collider.GetComponent<Animation>().Play(PlayMode.StopAll);
            // Lose the game
            gameController.LoseGame();
        }
        // If we collide with a circle then stop connecting
        else if (collider.tag.Equals("Circle") && !connected)
        {
            // Set connected flag
            connected = true;
            // Fix position
            rectTransform.anchoredPosition = POSITION_FIXED;
            // Create a new line
            GameObject newLine = new GameObject("Line");
            newLine.transform.SetParent(transform);
            // Add the line renderer and setup it
            lineRenderer = newLine.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
            lineRenderer.startWidth = LINE_WIDTH;
            lineRenderer.endWidth = LINE_WIDTH;
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
            lineRenderer.positionCount = 2;
            lineRenderer.useWorldSpace = false;
            // Set the line's position
            lineRenderer.SetPosition(0, mainCircle.transform.position);
            lineRenderer.SetPosition(1, transform.position);
            // Connector on connect
            gameController.OnConnectorConnect(this);
        }
        // Stop translating
        translating = false;
    }

    public void Connect()
    {
        // Translate to circle
        TranslateTo(POSITION_FIXED);
    }

    public void TranslateTo(Vector2 destination)
    {
        // Set translating
        translating = true;
        translatingTo = destination;
    }

    public void Setup(float rotationSpeed, Color color, int colorIndex)
    {
        this.rotationSpeed = rotationSpeed;
        this.color = color;
        this.colorIndex = colorIndex;
        // Set color
        GetComponent<Image>().color = color;
    }

    public void SetupAsStatic(float rotationSpeed, float startingRotation)
    {
        // Setup the static connector
        this.rotationSpeed = rotationSpeed;
        this.color = Color.white;
        isStatic = true;
        connected = true;
        // Set color
        GetComponent<Image>().color = color;
        // Set our position
        rectTransform.anchoredPosition = POSITION_FIXED;
        // Rotate to its startig rotation
        rectTransform.RotateAround(mainCircle.rectTransform.position, ROTATION_AXIS, startingRotation);
        // Create a new line
        GameObject newLine = new GameObject("Line");
        newLine.transform.SetParent(transform);
        // Add the line renderer and setup it
        lineRenderer = newLine.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
        lineRenderer.startWidth = LINE_WIDTH;
        lineRenderer.endWidth = LINE_WIDTH;
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = false;
        // Set the line's position
        lineRenderer.SetPosition(0, mainCircle.transform.position);
        lineRenderer.SetPosition(1, transform.position);
    }

    public void SetRotationSpeed(float rotationSpeed)
    {
        // Update rotation speed
        this.rotationSpeed = rotationSpeed;
    }

    public void SetColor(Color color)
    {
        this.color = color;
        // Update colors
        GetComponent<Image>().color = color;
        if (lineRenderer != null)
        {
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
        }
    }

    public void Restore(Vector2 position)
    {
        // Restore position
        rectTransform.anchoredPosition = position;
        // Destroy line renderer
        if(lineRenderer != null)
            Destroy(lineRenderer.gameObject);

        // Reset instances
        lineRenderer = null;
        connected = false;
    }

    public int GetColorIndex()
    {
        return colorIndex;
    }

    public Color GetColor()
    {
        return color;
    }

    public bool IsTranslating()
    {
        return translating;
    }

    public bool IsConnected()
    {
        return connected;
    }
}
