using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //player.score of player
    public bool win;

    private int coinVal, heartVal, musicNoteVal;

    public int levelScore = 0; // player.score earned so far for the stage
    public int highScore = 0;
    public int currentStage = 1;
    // amount of stages
    public int maxStage = 2;
    //timer stuff
    public float timeLeft, timeElapsed, timePerStage;
    // static instance of GM to be accessed from anywhere
    public static GameManager instance;
    private HudManager hudManager;
    public PlayerAttr player;
    private int slimeVal = 1;

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
            instance.player = FindObjectOfType<PlayerAttr>();
            Destroy(gameObject);
        }
        //prevent this object from being destroyed when switching scenes
        DontDestroyOnLoad(gameObject);

        // find HUD manager object
        hudManager = FindObjectOfType<HudManager>();
        player = FindObjectOfType<PlayerAttr>();

    }

    public IEnumerator WaitDie()
    {
        PlayerController p = GetComponent<PlayerController>();
        p.Die();
        yield return new WaitForSeconds(2);
        Die();
    }

    private void Start()
    {
        initStats();
        print(player.ToString());
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

    public void TakeDamage(int amt)
    {
        player.health -= amt;
        if (player.health <= 0) WaitDie();
    }

    // increase player player.score
    public void EatCoin()
    {
        player.score += coinVal;
        levelScore += coinVal;
        if (hudManager != null) hudManager.ResetHUD();
        if (player.score > highScore) highScore = player.score;
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
        if (player.health < player.maxHealth) return true;
        return false;
    }

    public void IncreaseEXP(int amt)
    {
        player.exp += amt;
        if (player.exp >= player.toNextLevel) LevelUp();
        if (hudManager != null) hudManager.ResetHUD();
    }
    public void IncreaseHP(int amt)
    {
        player.health += amt;
    }

    private void LevelUp()
    {
        int diff = player.toNextLevel - player.exp;
        player.level++;
        if (player.level % 2 == 0) player.maxHealth++;
        else if (player.level % 5 == 0)
        {
            player.maxHealth++; 
            player.damage++;
        }
        else player.damage++;
        player.health = player.maxHealth;
        player.exp += diff;
        player.toNextLevel = player.toNextLevel + 10;
        if (hudManager != null) hudManager.ResetHUD();
        // probably add an effect here? sound
    }

    void initStats()
    {
        currentStage = 1;
        player.ResetStats();
        timePerStage = 120f;
        coinVal = 10;
        win = false;
        musicNoteVal = 1;
        heartVal = 1;
        timeLeft = timePerStage;
        timeElapsed = 0f;
        // reset player.score
        levelScore = 0;
    }

    public void ResetGame()
    {
        initStats();
        player.ResetStats();
        // go back to player.level 1
        if (hudManager != null) hudManager.ResetHUD();
        // load player.level 1 scene
        SceneManager.LoadScene("Stage1");
    }

    public void ResetLevel()
    {
        // remove points collected on the player.level so far
        player.score -= levelScore;
        levelScore = 0;
        ResetTime();
        // reset player.level
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
        player.lives--;
        if (player.lives <= 0) GameOver();
        else ResetLevel();
    }

    public void LoadNextStage()
    {
        // check if their are more levels
        if (currentStage < maxStage)
        {
            currentStage++;
            player.lives++;
            player.health = player.maxHealth;
            levelScore = 0;
            player.score += (int)timeLeft * 10;
            DataManagement.dataManagement.highScore = player.score;
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
