using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip leaves, heart, coin, music, bgm, girono, jojo, chest, goal;

    public static AudioManager instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        //leaves = heartSound = coinSound = musicSound = bgm = girono = null;
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayLeaves()
    {
        if (leaves != null)
        {
            audioSource.loop = false;
            audioSource.PlayOneShot(leaves);
        }
    }
    public void PlayHeart()
    {
        audioSource.loop = false;
        if (heart != null) audioSource.PlayOneShot(heart);
    }
    public void PlayCoin()
    {
        audioSource.loop = false;
        if (coin != null) audioSource.PlayOneShot(coin);
    }
    public void PlayMusic()
    {
        audioSource.loop = false;
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
