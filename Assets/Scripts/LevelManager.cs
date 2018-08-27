using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LevelManager 
{
    public class Level
    {
        // Basic level configuration
        public int colorsCount;
        public int connectorsCount;
        public float circleRotationSpeed;

        // Advanced level configuration
        // Static connectors
        public bool hasStaticConnectors;
        public float[] staticConnectorsRotation;

        // Speed changing
        public bool hasSpeedChange;
        public float newSpeed;
        public float speedChangingTime;

        // Color changing
        public bool hasColorChange;
        public float colorChangingTime;

        // Special movements
        public bool increasingSpeed;
        public bool directionChange;

        // Basic configuration constructor
        public Level(int colorsCount, int connectorsCount, float circleRotationSpeed)
        {
            this.colorsCount = colorsCount;
            this.connectorsCount = connectorsCount;
            this.circleRotationSpeed = circleRotationSpeed;
        }

        // Static connectors configuration constructor
        public Level(int colorsCount, int connectorsCount, float circleRotationSpeed, float[] staticConnectorsRotation) 
            : this(colorsCount, connectorsCount, circleRotationSpeed)
        { 
            // Set the advanced level configuration
            hasStaticConnectors = true;
            this.staticConnectorsRotation = staticConnectorsRotation;
        }

        // Speed changing configuration constructor
        public Level(int colorsCount, int connectorsCount, float circleRotationSpeed, float newSpeed, float speedChangingTime)
            : this(colorsCount, connectorsCount, circleRotationSpeed)
        {
            // Set the advanced level configuration
            hasSpeedChange = true;
            this.newSpeed = newSpeed;
            this.speedChangingTime = speedChangingTime;
        }

        // Color changing configuration constructor
        public Level(int colorsCount, int connectorsCount, float circleRotationSpeed, float colorChangingTime)
            : this(colorsCount, connectorsCount, circleRotationSpeed)
        {
            // Set the advanced level configuration
            hasColorChange = true;
            this.colorChangingTime = colorChangingTime;
        }

        // Color changing + static connectors configuration constructor
        public Level(int colorsCount, int connectorsCount, float circleRotationSpeed, float[] staticConnectorsRotation, 
            float colorChangingTime) : this(colorsCount, connectorsCount, circleRotationSpeed)
        {
            // Static connectors
            hasStaticConnectors = true;
            this.staticConnectorsRotation = staticConnectorsRotation;
            // Color changing
            hasColorChange = true;
            this.colorChangingTime = colorChangingTime;
        }

        // Speed changing + static connectors configuration constructor
        public Level(int colorsCount, int connectorsCount, float circleRotationSpeed, float[] staticConnectorsRotation,
            float newSpeed, float speedChangingTime) : this(colorsCount, connectorsCount, circleRotationSpeed)
        {
            // Static connectors
            hasStaticConnectors = true;
            this.staticConnectorsRotation = staticConnectorsRotation;
            // Speed changing
            hasSpeedChange = true;
            this.newSpeed = newSpeed;
            this.speedChangingTime = speedChangingTime;
        }

        // Speed changing + color changing configuration constructor
        public Level(int colorsCount, int connectorsCount, float circleRotationSpeed, float newSpeed, 
            float speedChangingTime, float colorChangingTime) : this(colorsCount, connectorsCount, circleRotationSpeed)
        {
            // Speed changing
            hasSpeedChange = true;
            this.newSpeed = newSpeed;
            this.speedChangingTime = speedChangingTime;
            // Color changing
            hasColorChange = true;
            this.colorChangingTime = colorChangingTime;
        }

        // Static connectors + speed changing + color changing configuration constructor
        public Level(int colorsCount, int connectorsCount, float circleRotationSpeed, float[] staticConnectorsRotation, 
            float newSpeed, float speedChangingTime, float colorChangingTime) : this(colorsCount, connectorsCount, circleRotationSpeed)
        {
            // Static connectors
            hasStaticConnectors = true;
            this.staticConnectorsRotation = staticConnectorsRotation;
            // Speed changing
            hasSpeedChange = true;
            this.newSpeed = newSpeed;
            this.speedChangingTime = speedChangingTime;
            // Color changing
            hasColorChange = true;
            this.colorChangingTime = colorChangingTime;
        }

        // Speed increasing
        public Level(int colorsCount, int connectorsCount, float circleRotationSpeed, float maxSpeed, float speedChangingTime, 
            bool increasingSpeed) : this(colorsCount, connectorsCount, circleRotationSpeed)
        {
            // Speed increasing
            this.increasingSpeed = increasingSpeed;
            // Speed changing
            this.newSpeed = maxSpeed;
            this.speedChangingTime = speedChangingTime;
        }

        // Static connectors + Speed increasing
        public Level(int colorsCount, int connectorsCount, float circleRotationSpeed, float[] staticConnectorsRotation, 
            float maxSpeed, float speedChangingTime, bool increasingSpeed) : this(colorsCount, connectorsCount, circleRotationSpeed)
        {
            // Static connectors
            hasStaticConnectors = true;
            this.staticConnectorsRotation = staticConnectorsRotation;
            // Speed increasing
            this.increasingSpeed = increasingSpeed;
            // Speed changing
            this.newSpeed = maxSpeed;
            this.speedChangingTime = speedChangingTime;
        }

        // Speed increasing + direction change
        public Level(int colorsCount, int connectorsCount, float circleRotationSpeed, float maxSpeed, float speedChangingTime,
            bool increasingSpeed, bool directionChange) : this(colorsCount, connectorsCount, circleRotationSpeed)
        {
            // Speed increasing
            this.increasingSpeed = increasingSpeed;
            // Direction change
            this.directionChange = directionChange;
            // Speed changing
            this.newSpeed = maxSpeed;
            this.speedChangingTime = speedChangingTime;
        }

        // Static connectors + Speed increasing + direction change
        public Level(int colorsCount, int connectorsCount, float circleRotationSpeed, float[] staticConnectorsRotation,
            float maxSpeed, float speedChangingTime, bool increasingSpeed, bool directionChange) : this(colorsCount, connectorsCount, circleRotationSpeed)
        {
            // Static connectors
            hasStaticConnectors = true;
            this.staticConnectorsRotation = staticConnectorsRotation;
            // Speed increasing
            this.increasingSpeed = increasingSpeed;
            // Direction change
            this.directionChange = directionChange;
            // Speed changing
            this.newSpeed = maxSpeed;
            this.speedChangingTime = speedChangingTime;
        }

        // Speed increasing + color change
        public Level(int colorsCount, int connectorsCount, float circleRotationSpeed, float maxSpeed, float speedChangingTime,
            float colorChangingTime, bool increasingSpeed) : this(colorsCount, connectorsCount, circleRotationSpeed)
        {
            // Speed increasing
            this.increasingSpeed = increasingSpeed;
            // Speed changing
            this.newSpeed = maxSpeed;
            this.speedChangingTime = speedChangingTime;
            // Color changing
            hasColorChange = true;
            this.colorChangingTime = colorChangingTime;
        }

        // Speed increasing + color change + direction change
        public Level(int colorsCount, int connectorsCount, float circleRotationSpeed, float maxSpeed, float speedChangingTime,
            float colorChangingTime, bool increasingSpeed, bool directionChange) : this(colorsCount, connectorsCount, circleRotationSpeed)
        {
            // Speed increasing
            this.increasingSpeed = increasingSpeed;
            // Speed changing
            this.newSpeed = maxSpeed;
            this.speedChangingTime = speedChangingTime;
            // Direction change
            this.directionChange = directionChange;
            // Color changing
            hasColorChange = true;
            this.colorChangingTime = colorChangingTime;
        }
    }

    // 45 levels
    public static Level[] levels =
    {
        // Basic levels (13)
        new Level(2, 2, -1f),
        new Level(2, 6, -1f),
        new Level(3, 6, -1.2f),
        new Level(3, 10, 1.5f),
        new Level(3, 15, 1.9f),
        new Level(4, 10, -2f),
        new Level(4, 11, -2.3f),
        new Level(4, 15, -2.6f),
        new Level(5, 10, 1.8f),
        new Level(5, 10, 2.1f),
        new Level(5, 15, 1.8f),
        new Level(5, 20, -1.5f),
        new Level(6, 20, 1.5f),

        // Advanced levels using static connectors (5)
        new Level(4, 10, -1.8f, new float[] {0f, 180f}),
        new Level(4, 13, -2f, new float[] {0f, 90f, 180f, 270f}),
        new Level(4, 18, 2f, new float[] {45f, 135f, 225f, 315f}),
        new Level(5, 22, 2.25f, new float[] {22.5f, 67.5f, 205.5f, 255.5f}),
        new Level(6, 10, -2.7f, new float[] {30f, 90f, 150f, 210f, 270f, 330f}),

        // Advanced levels using speed changing (4)
        new Level(4, 10, -1.4f, -2.4f, 4f),
        new Level(5, 15, -1f, -3.5f, 5f),
        new Level(6, 10, -1.5f, 3f, 5f),
        new Level(6, 15, 2.2f, -2.2f, 4f),

        // Advanced levels using color changing (3)
        new Level(6, 15, 2.2f, 4f),
        new Level(6, 8, 2.5f, 2f),
        new Level(4, 18, 2.5f, 3f),

        // Advanced levels mashup (12)
        new Level(4, 15, 2.5f, new float[] {45f, 135f, 225f, 315f}, 4f), // Connectors + color changing
        new Level(3, 15, 6f, new float[] {60f, 180f, 300f}, 2f, 3.5f), // Connectors + speed changing
        new Level(5, 12, 4f, 1f, 3f, 5f), // color changing + speed changing
        new Level(6, 6, 4f, new float[] {60f, 180f, 300f}, 1f, 3f, 5f), // Connectors + color changing + speed changing
        new Level(6, 5, 4f, new float[] {0f, 40f, 80f, 120f, 160f, 200f, 240f, 280f, 320f}, 2f), // Connectors + color changing
        new Level(3, 10, 4f, new float[] {0f, 40f, 80f, 120f, 160f, 200f, 240f, 280f, 320f}, 2f), // Connectors + color changing
        new Level(2, 8, 10f), // Basic
        new Level(5, 15, 8f, new float[] {60f, 180f, 300f}, 0.25f, 3f), // Connectors + speed changing
        new Level(4, 18, 4f, 2f, 3f, 2f), // color changing + speed changing
        new Level(6, 20, 3f), // Basic
        new Level(6, 6, 6f, new float[] {0f, 40f, 80f, 120f, 160f, 200f, 240f, 280f, 320f}, 4f, 2f, 3f), // Connectors + color changing + speed changing
        new Level(6, 6, 6f, new float[] {0f, 40f, 80f, 120f, 160f, 200f, 240f, 280f, 320f}, 1.5f), // Connectors + color changing

        // Speed increasing levels (6)
        new Level(4, 15, 2f, 4f, 6f, true),
        new Level(4, 15, 1f, 6f, 6f, true),
        new Level(5, 10, 1f, new float[] {45f, 135f, 225f, 315f}, 10f, 5f, true), // Static connectors + speed increase
        new Level(5, 10, 1f, 6f, 4f, true, true),
        new Level(5, 15, 1f, 8f, 3f, true, true),
        new Level(4, 15, 1f, new float[] {0f, 40f, 80f, 120f, 160f, 200f, 240f, 280f, 320f}, 8f, 3f, true, true), // Static connectors + speed increase + change direction

        // Advanced level mashup (2)
        new Level(5, 15, 1f, 7f, 3f, 3.5f, true), // Speed increase + color change
        new Level(4, 20, 1f, 7f, 2f, 2.2f, true, true), // Speed increase + color change + direction change
    };
}
