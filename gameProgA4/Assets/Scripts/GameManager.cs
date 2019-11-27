using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //PlayerAttr.playerAttr.score of PlayerAttr.playerAttr
    public bool win;

    private int coinVal, heartVal, musicNoteVal;

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
            Destroy(gameObject);
        }
        //prevent this object from being destroyed when switching scenes
        DontDestroyOnLoad(gameObject);

        // find HUD manager object
        hudManager = FindObjectOfType<HudManager>();

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
        initGame();
        print(PlayerAttr.playerAttr.ToString());
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
        PlayerAttr.playerAttr.health -= amt;
        if (PlayerAttr.playerAttr.health <= 0) WaitDie();
    }

    // increase PlayerAttr.playerAttr PlayerAttr.playerAttr.score
    public void EatCoin()
    {
        PlayerAttr.playerAttr.score += coinVal;
        levelScore += coinVal;
        if (hudManager != null) hudManager.ResetHUD();
        if (PlayerAttr.playerAttr.score > highScore) highScore = PlayerAttr.playerAttr.score;
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
        if (PlayerAttr.playerAttr.health < PlayerAttr.playerAttr.maxHealth) return true;
        return false;
    }

    public void IncreaseEXP(int amt)
    {
        PlayerAttr.playerAttr.exp += amt;
        if (PlayerAttr.playerAttr.exp >= PlayerAttr.playerAttr.toNextLevel) LevelUp();
        if (hudManager != null) hudManager.ResetHUD();
    }
    public void IncreaseHP(int amt)
    {
        PlayerAttr.playerAttr.health += amt;
    }

    private void LevelUp()
    {
        int diff = PlayerAttr.playerAttr.toNextLevel - PlayerAttr.playerAttr.exp;
        PlayerAttr.playerAttr.level++;
        if (PlayerAttr.playerAttr.level % 2 == 0) PlayerAttr.playerAttr.maxHealth++;
        else if (PlayerAttr.playerAttr.level % 5 == 0)
        {
            PlayerAttr.playerAttr.maxHealth++; 
            PlayerAttr.playerAttr.damage++;
        }
        else PlayerAttr.playerAttr.damage++;
        PlayerAttr.playerAttr.health = PlayerAttr.playerAttr.maxHealth;
        PlayerAttr.playerAttr.exp += diff;
        PlayerAttr.playerAttr.toNextLevel = PlayerAttr.playerAttr.toNextLevel + 10;
        if (hudManager != null) hudManager.ResetHUD();
        // probably add an effect here? sound
    }

    void initGame()
    {
        currentStage = 1;
        PlayerAttr.playerAttr.ResetStats();
        timePerStage = 120f;
        coinVal = 10;
        win = false;
        musicNoteVal = 1;
        heartVal = 1;
        timeLeft = timePerStage;
        timeElapsed = 0f;
        // reset PlayerAttr.playerAttr.score
        levelScore = 0;
    }

    public void ResetGame()
    {
        initGame();
        PlayerAttr.playerAttr.ResetStats();
        // go back to PlayerAttr.playerAttr.level 1
        if (hudManager != null) hudManager.ResetHUD();
        // load PlayerAttr.playerAttr.level 1 scene
        SceneManager.LoadScene("Stage1");
    }

    public void ResetLevel()
    {
        // remove points collected on the PlayerAttr.playerAttr.level so far
        PlayerAttr.playerAttr.score -= levelScore;
        levelScore = 0;
        ResetTime();
        // reset PlayerAttr.playerAttr.level
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
        PlayerAttr.playerAttr.lives--;
        if (PlayerAttr.playerAttr.lives <= 0) GameOver();
        else ResetLevel();
    }

    public void LoadNextStage()
    {
        // check if their are more levels
        if (currentStage < maxStage)
        {
            currentStage++;
            PlayerAttr.playerAttr.lives++;
            PlayerAttr.playerAttr.health = PlayerAttr.playerAttr.maxHealth;
            levelScore = 0;
            PlayerAttr.playerAttr.score += (int)timeLeft * 10;
            DataManagement.dataManagement.highScore = PlayerAttr.playerAttr.score;
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
