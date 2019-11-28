using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip leaves, heart, coin, music, bgm, girono, jojo, chest, goal;
    public AudioClip ora, oof, levelup;
    public bool playedShot;

    public static AudioManager instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this)
        {
            instance.audioSource = GetComponent<AudioSource>();
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        playedShot = false;
        if(audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        //if (playedShot) playedShot = false;
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }
    public void PlayLeaves()
    {
        if (leaves != null && !playedShot)
        {
            audioSource.PlayOneShot(leaves);
        }
        playedShot = true;
    }
    public void PlayChest()
    {
        if (chest != null) audioSource.PlayOneShot(chest);
    }
    public void PlayLevelUp()
    {
        if (levelup != null) audioSource.PlayOneShot(levelup);
    }
    public void PlayGoal()
    {
        if (goal != null) audioSource.PlayOneShot(goal);
    }
    public void PlayHeart()
    {
        if (heart != null) audioSource.PlayOneShot(heart);
    }
    public void PlayOof()
    {
        if (oof != null) audioSource.PlayOneShot(oof);
    }
    public void PlayOra()
    {
        if (ora != null) audioSource.PlayOneShot(ora);
    }
    public void PlayCoin()
    {
        if (coin != null) audioSource.PlayOneShot(coin);
    }
    public void PlayMusic()
    {
        if (music != null) audioSource.PlayOneShot(music);
    }
    public void PlayBGM()
    {
        //if (audioSource.isPlaying) audioSource.Stop();
        if (bgm != null)
        {
            audioSource.clip = bgm;
            audioSource.Play();
            audioSource.loop = true;
        }
    }
    public void PlayGiorno()
    {
        //if (audioSource.isPlaying) audioSource.Stop();
        if (girono != null)
        {
            audioSource.loop = true;
            audioSource.clip = girono;
            audioSource.Play();
        }
    }
}
