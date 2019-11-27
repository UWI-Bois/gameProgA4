using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //PlayerAttr.playerAttr.score of PlayerAttr.playerAttr
    public bool win;
    public int coinVal, heartVal, musicNoteVal, slimeVal, enemies;

    public int levelScore = 0; // PlayerAttr.playerAttr.score earned so far for the stage
    public int highScore = 0;
    public int currentStage = 1;
    // amount of stages
    public int maxStage = 2;
    //timer stuff
    public float timeLeft, timeElapsed, timePerStage;
    // static instance of GM to be accessed from anywhere
    public static GameManager instance;
    private HudManager hudManager;
    

    // Awake is called before the game starts 
    void Awake()
    {
        maxStage = 2;
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

        // find HUD manager object
        hudManager = FindObjectOfType<HudManager>();
        enemies = GameObject.Find("Enemies").transform.childCount;
        //print("enemies: " + enemies);
    }
    private void Start()
    {
        initGame();
        //print(Player.instance.ToString());
    }
    private void Update()
    {
        TickTime();
        enemies = GameObject.Find("Enemies").transform.childCount;
    }


    void TickTime()
    {
        timeLeft -= Time.deltaTime;
        timeElapsed += Time.deltaTime;
        if (hudManager != null) hudManager.ResetHUD();
    }

    void initGame()
    {
        currentStage = heartVal = musicNoteVal = 1;
        Player.instance.ResetStats();
        timePerStage = 120f;
        coinVal = 10;
        win = false;
        timeLeft = timePerStage;
        timeElapsed = levelScore= 0;
    }

    public void ResetGame()
    {
        initGame();
        // go back to PlayerAttr.playerAttr.level 1
        if (hudManager != null) hudManager.ResetHUD();
        // load PlayerAttr.playerAttr.level 1 scene
        SceneManager.LoadScene("Stage1");
    }

    public void ResetLevel()
    {
        // remove points collected on the PlayerAttr.playerAttr.level so far
        Player.instance.score -= levelScore;
        Player.instance.facingLeft = false;
        levelScore = 0;
        ResetTime();
        // reset PlayerAttr.playerAttr.level
        if (hudManager != null) hudManager.ResetHUD();
        // load stage scene
        SceneManager.LoadScene("Stage" + currentStage);
    }

    public void ResetTime()
    {
        timeLeft = timePerStage;
        timeElapsed = 0;
    }

    public void KillPlayer()
    {
        Player.instance.lives--;
        if (Player.instance.lives <= 0) GameOver();
        else ResetLevel();
    }

    public void LoadNextStage()
    {
        // check if their are more levels
        if (currentStage < maxStage)
        {
            currentStage++;
            Player.instance.lives++;
            Player.instance.health = Player.instance.maxHealth;
            Player.instance.facingLeft = false;
            levelScore = 0;
            Player.instance.score += (int)timeLeft * 10;
            DataManagement.dataManagement.highScore = Player.instance.score;
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
            GameOver();
        }

    }

    public void GameOver()
    {
        SceneManager.LoadScene("GameOver");
    }

}
