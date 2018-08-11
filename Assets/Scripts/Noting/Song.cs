using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SongName", menuName = "Song")]
public class Song : ScriptableObject
{
    public float chordsPerMinute = 60f;
    public int subdivisionsPerChord = 8;

    public Chord[] chordProgression;
}
