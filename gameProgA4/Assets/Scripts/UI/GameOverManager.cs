using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    // access to the text element to show the score value
    public Text playerScoreValue;
    // access to the high score value
    public Text highScoreValue;
    public Text winOrLose;

    // Start is called before the first frame update
    void Start()
    {
        // set the text property of the PlayerAttr.playerAttr score 
        playerScoreValue.text = PlayerAttr.playerAttr.score.ToString();
        // set the text of high score
        highScoreValue.text = GameManager.instance.highScore.ToString();
        DetermineWin();
    }

    // send you to level 1
    public void ResetGame(){
        GameManager.instance.ResetGame();
    }

    public void DetermineWin(){
        string text;
        if(GameManager.instance.win) text = "You Win!";
        else text = "You Lose :(";
        winOrLose.text = text;
    }
}
