using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    [Header("General attributes")]
    public AnimationCurve fadeCurve;
    [Range(0.001f, 10f)] public float fadeTime = 10f;
    [Range(0f, 1f)] public float maxVolume = 0.1f;
    [Range(1, 50)] public int numSFXPlayers = 10;

    [Header("Playlist")]
    public AudioClip[] playlist;
    private AudioClip[] randomizedPlaylist;
    public AudioClip draggingSoundClip;
    public AudioClip[] coinSounds;

    [Header("Mixer")]
    public AudioMixerGroup mixer;
    
    private int curIdx = 0;
    private AudioClip inSound;

    private AudioSource mainSound;
    private AudioSource secondarySound;
    private AudioSource draggingSound;

    private bool fading;
    private bool isDragging;
    private float startedFadingAt;

    private AudioSource[] sfxPlayers;
    private int cur_sfx_idx = 0;


    // Use this for initialization
    void Start()
    {
        mainSound = gameObject.AddComponent<AudioSource>();
        secondarySound = gameObject.AddComponent<AudioSource>();

        draggingSound = gameObject.AddComponent<AudioSource>();
        this.sfxPlayers = new AudioSource[numSFXPlayers];

        for (int i=0; i < this.numSFXPlayers; i++)
        {
            sfxPlayers[i] = gameObject.AddComponent<AudioSource>();
        }

        mainSound.outputAudioMixerGroup = mixer;
        secondarySound.outputAudioMixerGroup = mixer;

        draggingSound.clip = draggingSoundClip;
        draggingSound.loop = true;
        draggingSound.Play();

        mainSound.Play();
        secondarySound.Play();

        fading = false;

        // checks every second if the mainSound is behaving as it should
        InvokeRepeating("clampVolume", 0f, 1f);
        
        curIdx = Random.Range(0, playlist.Length - 1);
        PlayRandomMusicIndefinitely();

    }


    public void playSFX(AudioClip c)
    {
        cur_sfx_idx = cur_sfx_idx % numSFXPlayers;
        AudioSource player = sfxPlayers[cur_sfx_idx];

        player.clip = c; // set the clip to secondary audio player
        player.time = 0f;
        player.Play();
    }

    // Update is called once per frame
    void Update()
    {
        Fade();

        // Fade in dragging or fade to no dragging
        if (isDragging)
        {
            draggingSound.volume = Mathf.Lerp(draggingSound.volume, 1.0f, Time.deltaTime * 20.0f);
        } else
        {
            draggingSound.volume = Mathf.Lerp(draggingSound.volume, 0.0f, Time.deltaTime * 20.0f);
        }
    }

    public void startDragging()
    {
        isDragging = true;
    }

    public void stopDragging()
    {
        isDragging = false;
    }

    private void Fade()
    {

        // if we are not currently fading
        if (!fading)
        {
            // if we get a fade request
            if (inSound != null)
            {
                startedFadingAt = Time.time; // save the time we started fading
                secondarySound.clip = inSound; // set the clip to secondary audio player
                secondarySound.time = 0f;
                secondarySound.Play();
                inSound = null; // reset the in sound to null so we wont do it again
                fading = true;
            }

        }
        else
        {
            float curveTime = (Time.time - startedFadingAt) / fadeTime;
            float curveValue = fadeCurve.Evaluate(curveTime);

            mainSound.volume = (1f - curveValue) * maxVolume;
            secondarySound.volume = (curveValue) * maxVolume;

            // if we have finished our fading  
            if (curveTime >= 1f)
            {
                fading = false;

                // switch mainSound and secondarySound
                mainSound.clip = secondarySound.clip;
                mainSound.volume = maxVolume;
                mainSound.Play();
                mainSound.time = secondarySound.time;

                secondarySound.clip = null;
                secondarySound.volume = 0f;
                secondarySound.time = 0f;
            }
        }
    }

    private void clampVolume()
    {
        mainSound.volume = Mathf.Min(maxVolume, mainSound.volume);
    }

    // Use this function from outside to fade another track in 
    public void MakeFade(AudioClip inMusic)
    {
        inSound = inMusic;
    }

    public void PlayCoinSound()
    {
        AudioClip curSound = coinSounds[Random.Range(0, coinSounds.Length)];
        playSFX(curSound);
    }

    public void ScorePoints(float scoredPoints)
    {
        float time = 0f;
        for (int i=0; i < Mathf.Min(scoredPoints, 5); i++)
        {
            Invoke("PlayCoinSound", time);
            time += Random.Range(0.01f, 0.2f);
        }

    }

    public void PlayRandomMusicIndefinitely()
    {
		curIdx = (curIdx + 1) % playlist.Length;
        // pick next
        AudioClip luckyOne = playlist[curIdx];

        this.MakeFade(luckyOne);

        Invoke("PlayRandomMusicIndefinitely", this.inSound.length - this.fadeTime);
    }
}