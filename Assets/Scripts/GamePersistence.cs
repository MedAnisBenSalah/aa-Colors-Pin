using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System;
using System.Text;

public static class GamePersistence
{
    [Serializable]
    public class GameData
    {
        [XmlElement("Level")]
        public int level;

        [XmlElement("HighestLevel")]
        public int highestLevel;

        [XmlElement("GamesPlayed")]
        public long gamesPlayed;

        [XmlElement("Muted")]
        public bool muted;

        public GameData()
        {
            level = 1;
            gamesPlayed = 0;
            highestLevel = 1;
            muted = false;
        }
    }

    public static GameData gameData;

    // Keys
    private static readonly string SAVE_DATA_FILE = "aaColorSwitch.oap";

    public static void Load()
    {
        try
        {
            // Create an XML serialize instance
            XmlSerializer serializer = new XmlSerializer(typeof(GameData));
            // Open the save file
            using (FileStream stream = new FileStream(Application.persistentDataPath + SAVE_DATA_FILE, FileMode.Open))
            {
                // Get data
                byte[] encodedXmlData = new byte[stream.Length];
                stream.Read(encodedXmlData, 0, (int)stream.Length);
                // Decode data
                byte[] xmlData = System.Convert.FromBase64String(Encoding.UTF8.GetString(encodedXmlData));
                // Deserialize the data
                using (TextReader reader = new StringReader(Encoding.UTF8.GetString(xmlData)))
                {
                    gameData = serializer.Deserialize(reader) as GameData;
                }
            }
        }
        catch (Exception)
        {
            // Save an empty file
            Save();
        }
    }

    public static void Save()
    {
        // Create the game data if its empty
        if (gameData == null)
            gameData = new GameData();

        try
        {
            // Create an XML serialize instance
            XmlSerializer serializer = new XmlSerializer(typeof(GameData));
            // Open the save file
            using (FileStream stream = new FileStream(Application.persistentDataPath + SAVE_DATA_FILE, FileMode.Create))
            {
                byte[] xmlData;
                int length;
                using (StringWriter writer = new StringWriter())
                {
                    // Serialize the data
                    serializer.Serialize(writer, gameData);
                    // Get data
                    xmlData = Encoding.UTF8.GetBytes(writer.ToString());
                    // Get length
                    length = writer.ToString().Length;
                }
                // Base64 encode for the data
                String encodedData = System.Convert.ToBase64String(xmlData);
                // Persist data
                stream.Write(Encoding.UTF8.GetBytes(encodedData), 0, encodedData.Length);
            }
        }
        catch (Exception)
        {
        }
    }
}