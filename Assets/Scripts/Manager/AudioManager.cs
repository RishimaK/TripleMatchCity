using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public SaveDataJson saveDataJson;
    [Header("__________________________________")]
    private AudioClip[] ListAudio;
    public AudioSource MusicSound;
    public AudioSource SFXSound;
    public GameObject SFXList;

    private bool allowMusic;
    private bool allowSound;
    private bool pauseSound = false;
    public void TakeAudioFromResources() => ListAudio = Resources.LoadAll<AudioClip>("sound"); 

    void Awake() {
        TakeAudioFromResources();
    }

    AudioClip FindAudioClipByName(string name)
    {
        foreach (AudioClip clip in ListAudio)
        {
            if (clip.name == name) return clip;
        }
        return null;
    }

    void Start()
    {
        allowMusic = (bool)saveDataJson.GetData("Music");
        allowSound = (bool)saveDataJson.GetData("Sound");
    }

    public void ChangeMusicBackground(string txt)
    {
        MusicSound.clip = FindAudioClipByName(txt);
        PlayMusic();
    }

    public void ChangeStatusOfMusic(bool status) 
    {
        allowMusic = status;
        if(status) PlayMusic();
        else StopMusic();
    }

    public void ChangeStatusOfSound(bool status) => allowSound = status;

    public void PlaySFX(string name)
    {
        if(!allowSound) return;
        if(SFXSound.isPlaying)
        {
            foreach (Transform child in SFXList.transform)
            {
                AudioSource audio = child.GetComponent<AudioSource>();
                if(!audio.isPlaying){ 
                    SFXSound = audio;
                    break;
                }
            }
        }
        SFXSound.clip = FindAudioClipByName(name);

        if(pauseSound) WaitForSoundContinue();
        else 
        {
            SFXSound.Play();
            if(name == "fw") 
            {
                MusicSound.volume = 0.4f;
                AudioSource audio = SFXSound;
                StartCoroutine(ChangeMusicVolume(audio));
            }
        }
    }

    IEnumerator WaitForSoundContinue()
    {
        yield return new WaitForSeconds(Time.deltaTime);
        if(pauseSound) WaitForSoundContinue();
        else 
        {
            SFXSound.Play();
            if(name == "fw") 
            {
                MusicSound.volume = 0.4f;
                AudioSource audio = SFXSound;
                StartCoroutine(ChangeMusicVolume(audio));
            }
        }
    }

    IEnumerator ChangeMusicVolume(AudioSource audio) 
    {
        yield return new WaitForSeconds(Time.deltaTime);
        if(audio.isPlaying) StartCoroutine(ChangeMusicVolume(audio));
        else MusicSound.volume = 1;
    }

    public void PlayMusic()
    {
        if(!allowMusic) return;
        MusicSound.loop = true;
        MusicSound.Play();
    }

    public void StopMusic()
    {
        MusicSound.Stop();
    }

    public void PauseAllAudio()
    {
        MusicSound.Pause();

        foreach (Transform child in SFXList.transform)
        {
            child.GetComponent<AudioSource>().Pause();
        }
        pauseSound = true;
    }

    public void UnPauseAllAudio()
    {
        MusicSound.UnPause();
        foreach (Transform child in SFXList.transform)
        {
            child.GetComponent<AudioSource>().UnPause();
        }
        pauseSound = false;
    }
}
