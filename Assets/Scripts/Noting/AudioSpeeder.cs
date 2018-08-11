using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSpeeder : MonoBehaviour {

    public AudioSource source;

    AudioClip clip;

    [Range(0, 2)]
    public float speed = 1f;
    int wantedSample = 0;
    public float minDeltaTime = 0.1f;

    int lastSample = 0;
    float lastSampleTime = 0;

	// Use this for initialization
	void Start () {
        clip = source.clip;
        lastSample = source.timeSamples;
        lastSampleTime = Time.time;
    }
    
	// Update is called once per frame
	void Update () {
        if (source.timeSamples != lastSample && Time.time - lastSampleTime >= minDeltaTime)
        {
            float deltaTime = Time.time - lastSampleTime;
            wantedSample += (int)(clip.frequency * deltaTime * speed);

            if (source.timeSamples != wantedSample && speed != 1f)
            {
                source.timeSamples = wantedSample;
            }
            lastSampleTime = Time.time;
            lastSample = source.timeSamples;
        }
	}
}
