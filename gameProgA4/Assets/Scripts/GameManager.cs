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
        maxStage = 3;
        currentStage = 1;
        
        // check that it exists
        if (instance == null) instance = this;
        // check that it is equal to the current object
        else if (instance != this)
        {
            instance.hudManager = FindObjectOfType<HudManager>();
            instance.enemies = GameObject.Find("Enemies").transform.childCount;
            //instance.enemies = 0; // REMOVE THIS
            Destroy(gameObject);
        }
        //prevent this object from being destroyed when switching scenes
        DontDestroyOnLoad(gameObject);

        // find HUD manager object
        hudManager = FindObjectOfType<HudManager>();
        enemies = GameObject.Find("Enemies").transform.childCount;
        //enemies = 0; // REMOVE THIS
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
        if(currentStage < maxStage) enemies = GameObject.Find("Enemies").transform.childCount; // ENABLE THIS
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
        initStage();
    }

    void initStage()
    {
        if (currentStage == maxStage) AudioManager.instance.PlayGiorno();
        else AudioManager.instance.PlayBGM();
        Player.instance.score -= levelScore;
        Player.instance.facingLeft = false;
        Player.instance.health = Player.instance.maxHealth;
        levelScore = 0;
        ResetTime();
    }

    public void ResetGame()
    {
        initGame();
        if (hudManager != null) hudManager.ResetHUD();
        SceneManager.LoadScene("Stage1");
    }

    public void ResetStage()
    {
        initStage();
        if (hudManager != null) hudManager.ResetHUD();
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
        else ResetStage();
    }

    public void LoadNextStage()
    {
        // check if their are more levels
        if (currentStage < maxStage)
        {
            currentStage++;
            Player.instance.lives++;
            Player.instance.score += (int)timeLeft * 10;
            DataManagement.dataManagement.highScore = Player.instance.score;
            DataManagement.dataManagement.SaveData();
            initStage();
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
