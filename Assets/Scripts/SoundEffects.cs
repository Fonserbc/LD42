using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffects : MonoBehaviour
{
    public AudioSource source;
    public AudioClip[] clashes;
    public AudioClip[] cheers;
    public AudioClip[] oops;
    public AudioClip[] whispers;
    public AudioSource audienceSource;
    public AudioSource whisperSource;
    public enum EffectType {
        Clash,
        Cheer,
        Oops,
        Whisper,
        Length
    }


    public void PlaySound(EffectType t) {
        switch (t) {
            case EffectType.Clash:
                source.PlayOneShot(Utilities.RandomValue(clashes));
                break;
            case EffectType.Cheer:
                audienceSource.PlayOneShot(Utilities.RandomValue(cheers));
                break;
            case EffectType.Oops:
                audienceSource.PlayOneShot(Utilities.RandomValue(oops));
                break;
            case EffectType.Whisper:
                whisperSource.PlayOneShot(Utilities.RandomValue(whispers));
                break;
            default:
                break;
        }
    }
}
