using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //score of player
    public int health, maxHealth;
    public int damage;
    public bool win;
    public int score, exp, level;
    public int toNextLevel = 10;

    private int coinVal, heartVal, musicNoteVal;

    public int levelScore = 0; // score earned so far for the stage
    public int playerLives = 3;
    public int defaultLives = 3;
    public int highScore = 0;
    public int currentStage = 1;
    // amount of stages
    public int maxStage = 1;
    //timer stuff
    public float timeLeft, timeElapsed, timePerStage;
    // static instance of GM to be accessed from anywhere
    public static GameManager instance;
    private HudManager hudManager;
    private int slimeVal = 1;

    // Awake is called before the game starts
    void Awake()
    {
        maxStage = 1;
        currentStage = 1;
        // check that it exists
        if (instance == null) instance = this;
        // check that it is equal to the current object
        else if (instance != this)
        {
            instance.hudManager = FindObjectOfType<HudManager>();
            Destroy(gameObject);
        }
        //prevent this object from being destroyed when switching scenes
        DontDestroyOnLoad(gameObject);

        // find hud manager object
        hudManager = FindObjectOfType<HudManager>();
    }

    private void Start()
    {
        initStats();
    }

    private void Update()
    {
        TickTime();
    }

    void TickTime()
    {
        timeLeft -= Time.deltaTime;
        timeElapsed += Time.deltaTime;
        if (hudManager != null) hudManager.ResetHUD();
    }

    void LoadMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    // increase player score
    public void EatCoin()
    {
        score += coinVal;
        levelScore += coinVal;
        if (hudManager != null) hudManager.ResetHUD();
        if (score > highScore) highScore = score;
    }

    public void EatMusicNote()
    {
        IncreaseEXP(musicNoteVal);
        timeLeft += musicNoteVal + 2;
        timeElapsed -= musicNoteVal + 2;
    }
    public void EatHeart()
    {
        IncreaseHP(heartVal);
    }

    public void KillSlime()
    {
        IncreaseEXP(slimeVal);
    }

    public bool isDamaged()
    {
        if (health < maxHealth) return true;
        return false;
    }

    public void IncreaseEXP(int amt)
    {
        exp += amt;
        if (exp >= toNextLevel) LevelUp();
        if (hudManager != null) hudManager.ResetHUD();
    }
    public void IncreaseHP(int amt)
    {
        health += amt;
    }

    private void LevelUp()
    {
        int diff = toNextLevel - exp;
        level++;
        if (level % 2 == 0) maxHealth++;
        else if (level % 5 == 0)
        {
            maxHealth++; 
            damage++;
        }
        else damage++;
        health = maxHealth;
        exp += diff;
        toNextLevel = toNextLevel + 10;
        if (hudManager != null) hudManager.ResetHUD();
        // probably add an effect here? sound
    }

    void initStats()
    {
        currentStage = 1; // check this
        playerLives = 3;
        timePerStage = 120f;
        coinVal = 10;
        win = false;
        exp = 0;
        toNextLevel = 10;
        level = musicNoteVal = 1;
        damage = 1;
        maxHealth = 4;
        health = maxHealth;
        score = 0;
        timeLeft = timePerStage;
        timeElapsed = 0f;
        // reset score
        levelScore = 0;
        playerLives = defaultLives;
    }

    public void ResetGame()
    {
        initStats();
        // go back to level 1
        if (hudManager != null) hudManager.ResetHUD();
        // load level 1 scene
        SceneManager.LoadScene("Stage1");
    }

    public void ResetLevel()
    {
        // remove points collected on the level so far
        score -= levelScore;
        levelScore = 0;
        ResetTime();
        // reset level
        if (hudManager != null) hudManager.ResetHUD();
        // load stage scene
        SceneManager.LoadScene("Stage" + currentStage);
    }

    private void ResetTime()
    {
        timeLeft = timePerStage;
        timeElapsed = 0;
    }

    public void Die()
    {
        playerLives--;
        if (playerLives <= 0) GameOver();
        else ResetLevel();
    }

    public void LoadNextStage()
    {
        // check if their are more levels
        if (currentStage < maxStage)
        {
            currentStage++;
            playerLives++;
            health = maxHealth;
            levelScore = 0;
            score += (int)timeLeft * 10;
            DataManagement.dataManagement.highScore = score;
            DataManagement.dataManagement.SaveData();
            try
            {
                SceneManager.LoadScene("Stage" + currentStage);
            }
            catch (Exception)
            { 
                throw;
            }
        }
        else
        { // finish the game
            // go back to start
            currentStage = 1;
            GameOver();
        }

    }

    public void GameOver()
    {
        initStats();
        SceneManager.LoadScene("GameOver");
    }

}
