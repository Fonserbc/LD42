﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardKey : MonoBehaviour {

    public Note.American note;
    public SpriteRenderer ownRenderer;
    public Sprite wLeft, wRight, wMiddle, wFull, black;
    public const float keyDistance = 0.6f;
    public const float keyHeight = 4f;
    public AudioSample audioSample;
    public AudioSource source;

    public bool isBlack = false;
    [System.NonSerialized]
    public bool first = false, last = false;

    enum SpriteType {
        wLeft, wRight, wMiddle, wFull, black
    }

    SpriteType ownSpriteType = SpriteType.wLeft;

    Logic logic;

    const float handColorLinger = 1f;
    float[] handTimes;
    int pressingHand = -1;
    
	public void Init (Logic l) {
        logic = l;
        float pos = (note.octave * 14) * keyDistance;

        switch (note.key) {
            case Note.American.Key.C:
                ownSpriteType = SpriteType.wRight;
                break;
            case Note.American.Key.Db:
                ownSpriteType = SpriteType.black;
                isBlack = true;
                pos += keyDistance;
                break;
            case Note.American.Key.D:
                ownSpriteType = SpriteType.wMiddle;
                pos += keyDistance * 2f;
                break;
            case Note.American.Key.Eb:
                ownSpriteType = SpriteType.black;
                isBlack = true;
                pos += keyDistance * 3f;
                break;
            case Note.American.Key.E:
                ownSpriteType = SpriteType.wLeft;
                pos += keyDistance * 4f;
                break;
            case Note.American.Key.F:
                ownSpriteType = SpriteType.wRight;
                pos += keyDistance * 6f;
                break;
            case Note.American.Key.Gb:
                ownSpriteType = SpriteType.black;
                isBlack = true;
                pos += keyDistance * 7f;
                break;
            case Note.American.Key.G:
                ownSpriteType = SpriteType.wMiddle;
                pos += keyDistance * 8f;
                break;
            case Note.American.Key.Ab:
                ownSpriteType = SpriteType.black;
                isBlack = true;
                pos += keyDistance * 9f;
                break;
            case Note.American.Key.A:
                ownSpriteType = SpriteType.wMiddle;
                pos += keyDistance * 10f;
                break;
            case Note.American.Key.Bb:
                ownSpriteType = SpriteType.black;
                isBlack = true;
                pos += keyDistance * 11f;
                break;
            case Note.American.Key.B:
                ownSpriteType = SpriteType.wLeft;
                pos += keyDistance * 12f;
                break;
        }

        if (first)
        {
            switch (ownSpriteType)
            {
                case SpriteType.wMiddle:
                    ownSpriteType = SpriteType.wRight;
                    break;
                case SpriteType.wLeft:
                    ownSpriteType = SpriteType.wFull;
                    break;
                default:
                    break;
            }
        }
        else if (last) {
            switch (ownSpriteType) {
                case SpriteType.wRight:
                    ownSpriteType = SpriteType.wFull;
                    break;
                case SpriteType.wMiddle:
                    ownSpriteType = SpriteType.wLeft;
                    break;
                default:
                    break;
            }
        }

        switch (ownSpriteType) {
            case SpriteType.wLeft:
                ownRenderer.sprite = wLeft;
                break;
            case SpriteType.wRight:
                ownRenderer.sprite = wRight;
                break;
            case SpriteType.wMiddle:
                ownRenderer.sprite = wMiddle;
                break;
            case SpriteType.wFull:
                ownRenderer.sprite = wFull;
                break;
            case SpriteType.black:
                ownRenderer.sprite = black;
                break;
        }

        if (isBlack)
        {
            ownRenderer.color = Color.black;
        }

        transform.localPosition = new Vector3(pos, 0, 0);

        //

        source.clip = audioSample.clip;
        float deltaSemitone = (float) (note - audioSample.note);
        source.pitch = Mathf.Pow(2f, deltaSemitone / 12f);

        name = note.ToString();

        //
        handTimes = new float[l.currentLevel.handPlays.Length];
        Utilities.InitializeArray(ref handTimes, 0);
    }

    public void Pressed(int handId) {
        if (pressingHand >= 0)
        {
            logic.HandsTouched(pressingHand, handId);
        }
        else {
            pressingHand = handId;
            handTimes[pressingHand] = handColorLinger;
            source.Play();
        }
    }

    public void Released(int handId) {
        if (pressingHand == handId)
        {
            pressingHand = -1;
            if (source.isPlaying) source.Stop();
        }
        else {
            Debug.LogError(handId + " released an unreleased key", this);
        }
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;

        Vector3 size = new Vector3(keyDistance * 2f, keyHeight);
        Vector3 center = transform.position;
        center.y += size.y / 2f;
        Gizmos.DrawWireCube(center, size);
    }
}
