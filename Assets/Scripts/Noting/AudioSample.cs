using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioSample", menuName = "Audio Sample")]
public class AudioSample : ScriptableObject {
    public AudioClip clip;
    public Note.American note;
}
