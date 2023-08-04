using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusicSetter : MonoBehaviour
{
    [SerializeField] AudioClip backgroundAudio;

    void Start()
    {
        AudioManager.Instance.PlayMusic(backgroundAudio, true);
    }
}
