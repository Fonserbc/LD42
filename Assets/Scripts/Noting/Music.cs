using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour {

    public Song song;

    [Header("Audio info")]
    public AudioSource source;
    public float songStartTime = 0f;

    float _time = 0;
    public float time {
        get {
            if (source != null)
            {
                return source.time - songStartTime;
            }
            else {
                return _time;
            }
        }
    }

    bool started = false;

	// Use this for initialization
	void Start () {
        _time = -songStartTime;
	}
	
	// Update is called once per frame
	void Update () {
        if (source == null) {
            _time += Time.deltaTime;
        }

        if (!started && time >= 0)
        {
            Debug.Log("Music Start");
            started = true;
        }
    }

    public Chord GetCurrentChord(bool prefectTiming = false) {
        int it = GetCurrentSongIterator();

        int subDiv = CurrentChordSubdivision();

        if (!prefectTiming && subDiv + 1 % song.subdivisionsPerChord == 0) { // Play next
            it = (it + 1) % song.chordProgression.Length;
        }

        return song.chordProgression[it];
    }

    public int GetCurrentSongIterator() {
        float t = Mathf.Max(0, time);

        int it = Mathf.FloorToInt(t * song.chordsPerMinute / 60f) % song.chordProgression.Length;
        return it;
    }

    public int CurrentChordSubdivision()
    {
        float t = Mathf.Max(0, time);
        
        int sub = Mathf.FloorToInt(t * song.chordsPerMinute * (float)song.subdivisionsPerChord / 60f) % song.subdivisionsPerChord;

        return sub;
    }

    public bool HasStarted() {
        return started;
    }
}
