using UnityEngine;
using System;

public class LevelSystem
{

    public event EventHandler onExpChanged;
    public event EventHandler onLevelChanged;

    private int level;
    private int experience;
    private int experienceToNextLevel;

    public LevelSystem()
    {
        level = PlayerPrefs.GetInt(PlayerScores.playerName + "Level", PlayerScores.playerLevel);
        experience = PlayerPrefs.GetInt(PlayerScores.playerName + "Xp",PlayerScores.playerXp);
        experienceToNextLevel = PlayerPrefs.GetInt(PlayerScores.playerName + "XpToNextLevel", 500);
    }

    public void AddExperience(int amount)
    {
        experience += amount;

        if (experience < 0)
        {
            experience = 0;
        }
        PlayerPrefs.SetInt(PlayerScores.playerName + "Xp", experience);
        PlayerScores.playerXp = experience;

        if (experience >= experienceToNextLevel)
        {

            level++;
            PlayerPrefs.SetInt(PlayerScores.playerName + "Level", level);
            PlayerScores.playerLevel = level;

            experienceToNextLevel = experienceToNextLevel + (100 * level * 2);

            //experience -= experienceToNextLevel;
            experience -= PlayerPrefs.GetInt(PlayerScores.playerName + "XpToNextLevel", 500);
            PlayerPrefs.SetInt(PlayerScores.playerName + "Xp", experience);
            PlayerScores.playerXp = experience;

            PlayerPrefs.SetInt(PlayerScores.playerName + "XpToNextLevel", experienceToNextLevel);

           

            //if (onLevelChanged != null) onLevelChanged(this, EventArgs.Empty);
        }

        if (ConnectToWallet.isWalletUser)
        {
            ConnectToWallet._CW.SaveData();
        }

        //if (onExpChanged != null) onExpChanged(this, EventArgs.Empty);
    }

    public void LoseExperience(int amount)
    {

        if (experience >= 0)
        {
            experience -= amount;
        }
        else
        {
            experience = 0;
        }
        
        
        //if (onExpChanged != null) onExpChanged(this, EventArgs.Empty);
    }

    public int GetLevelNumber()
    {
        return level;
    }public int GetExpNumber()
    {
        return experience;
    }public int GetNextLevelNumber()
    {
        return experienceToNextLevel;
    }

    public float GetExpNormalized()
    {
        return  (float)experience / experienceToNextLevel;
    }


}
