using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class HudManager : MonoBehaviour
{
    // score text label
    public Text scoreLabel, timeLabel, healthLabel, damageLabel, expLabel, levelLabel, livesLabel;

    // Start is called before the first frame update
    void Start()
    {
        // start with the correct score
        ResetHUD();
    }

    // show up-to-date stats of the player
    // add this method: on trigger of collecting a coin, enemy, start of game
    public void ResetHUD()
    {
        timeLabel.text = "Time: " + (int)GameManager.instance.timeLeft;
        scoreLabel.text = "Score: " + GameManager.instance.score;
        healthLabel.text = "HP: " + GameManager.instance.health;
        levelLabel.text = "LVL: " + GameManager.instance.level;
        damageLabel.text = "DMG: " + GameManager.instance.damage;
        expLabel.text = "EXP: " + GameManager.instance.player.exp+ "/" + GameManager.instance.toNextLevel;
        livesLabel.text = "Lives: " + GameManager.instance.playerLives;
    }
}
