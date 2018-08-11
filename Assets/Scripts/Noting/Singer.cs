using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singer : MonoBehaviour {

    public KeyCode singKeyCode;
    public AudioSource voice;
    [Header("Playing range")]
    public Note.American min;
    public Note.American max;

    [Header("Music")]
    public Music music;

    [Header("Other singers")]
    public GameObject[] otherSingers;

    [HideInInspector]
    public Note singingNote;

    public AudioSample sample;
    public float fadeOut = 0.5f;
    float stopTime = 0f;

    protected float[] pitches;

    // Use this for initialization
    void Start () {
        if (voice == null) {
            voice = GetComponent<AudioSource>();
        }

        voice.clip = sample.clip;


        pitches = new float[(int)max - (int)min + 1];

        for (int i = 0; i < pitches.Length; ++i) {
            float deltaSemitone = i + (int)min - (int)sample.note;
            pitches[i] = Mathf.Pow(2f, deltaSemitone / 12f);
        }

        singingNote = Mathf.Clamp(sample.note, min, max);

        Init();
	}

    protected virtual void Init() { }
	
	// Update is called once per frame
	protected virtual void Update () {
        if (Input.GetKeyDown(singKeyCode)) {
            Sing();
        }
        else if (Input.GetKeyUp(singKeyCode))
        {
            Stop();
        }

        if (stopTime > 0) {
            stopTime -= Time.deltaTime;

            if (stopTime <= 0 || !voice.isPlaying)
            {
                if (voice.isPlaying) voice.Stop();
                voice.volume = 1f;
            }
            else {
                voice.volume = stopTime / fadeOut;
            }
        }
	}

    protected virtual bool ChooseNewNote() {
        return false;
    }

    public void Sing() {
        if (ChooseNewNote())
        {
            int which = singingNote - min;
            if (which >= 0 && which < pitches.Length)
            {
                voice.volume = 1f;
                voice.pitch = pitches[which];
                voice.Play();
                stopTime = 0f;
            }
            else {
                Debug.LogError("Singer can\'t reach that note");
                Debug.LogError("\t" + (int)singingNote + " " + which);
            }
        }
        else {
            //Debug.Log("Cant find note to sing!");
        }
    }

    public void Stop() {
        stopTime = fadeOut;
    }

    public bool IsSinging() {
        return voice.isPlaying;
    }
}
