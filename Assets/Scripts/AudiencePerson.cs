﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudiencePerson : MonoBehaviour {

    public Image ownImage;
    public Sprite[] idleSprites;
    public Sprite whisperSprite;
    public Sprite cheerSprite;

    public float minChange = 1f;
    public float maxChange = 4f;
    
    float time = 0;
    

    Audience controller;

    public void Init(Audience aud) {
        controller = aud;
    }

	// Update is called once per frame
	void Update () {
        time -= Time.deltaTime;

        if (time < 0) {
            ownImage.sprite = Utilities.RandomValue(idleSprites);
            float changeTime = Mathf.Lerp(minChange, maxChange, controller.CalmFactor());
            time = Random.Range(changeTime - 0.7f, changeTime + 0.7f);
            ownImage.transform.localScale = Random.Range(0, 2) == 0 ? Vector3.one : new Vector3(-1f, 1f, 1f);
            ownImage.rectTransform.sizeDelta = new Vector2(100, 100);
        }
	}

    public void Whisper() {
        float changeTime = Mathf.Lerp(maxChange, minChange, controller.CalmFactor());
        time = Random.Range(changeTime - 0.7f, changeTime + 0.7f);
        ownImage.sprite = whisperSprite;
        ownImage.transform.localScale = Random.Range(0, 2) == 0 ? Vector3.one : new Vector3(-1f, 1f, 1f);
        ownImage.rectTransform.sizeDelta = new Vector2(100, 100);
    }

    public void Cheer()
    {
        float changeTime = Mathf.Lerp(maxChange, minChange, controller.CalmFactor());
        time = Random.Range(changeTime - 0.7f, changeTime + 0.7f);
        ownImage.sprite = cheerSprite;
        ownImage.transform.localScale = Random.Range(0, 2) == 0 ? Vector3.one : new Vector3(-1f, 1f, 1f);
        ownImage.rectTransform.sizeDelta = new Vector2(100, 150);
    }
}
