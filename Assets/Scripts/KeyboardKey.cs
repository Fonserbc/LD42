using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardKey : MonoBehaviour {

    public Note.American note;
    public SpriteRenderer ownRenderer;
    public Sprite wLeft, wRight, wMiddle, wFull, black;
    public const float keyDistance = 0.6f;
    public const float keyHeight = 5f;
    public AudioSample audioSample;
    public AudioSource source;

    public Color blackColor = Color.black;
    public Color whiteColor = Color.white;

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
    Color originalColor;

    const float FADEOUT_TIME = 0.1f;
    float fadeoutTime = 0;
    
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
            ownRenderer.color = blackColor;
        }
        else
        {
            ownRenderer.color = whiteColor;
        }
        originalColor = ownRenderer.color;

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
            Released(pressingHand);
        }
        else {
            pressingHand = handId;
            handTimes[pressingHand] = handColorLinger;
            if (source.isPlaying) {
                source.Stop();
                source.volume = 1;
            }
            source.Play();
            fadeoutTime = 0;
        }
    }

    public void Released(int handId) {
        if (pressingHand == handId)
        {
            pressingHand = -1;
            if (source.isPlaying) fadeoutTime = FADEOUT_TIME;
        }
        else {
            //Debug.LogError(handId + " released an unreleased key "+name, this);
        }
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;

        Vector3 size = new Vector3(keyDistance * 2f, keyHeight);
        Vector3 center = transform.position;
        center.y += size.y / 2f;
        Gizmos.DrawWireCube(center, size);
    }

    void Update() {
        for (int i = 0; i < handTimes.Length; ++i) {
            if (i != pressingHand && handTimes[i] > 0) {
                handTimes[i] = Mathf.Max(0, handTimes[i] - Time.deltaTime);
            }
        }

        if (pressingHand >= 0)
        {
            ownRenderer.color = logic.handsColor[pressingHand];
        }
        else {
            Vector3 color = new Vector3(originalColor.r, originalColor.g, originalColor.b);
            float count = 1;
            for (int i = 0; i < handTimes.Length; ++i)
            {
                if (handTimes[i] > 0)
                {
                    float f = handTimes[i] / handColorLinger;
                    color.x += f * logic.handsColor[i].r;
                    color.y += f * logic.handsColor[i].g;
                    color.z += f * logic.handsColor[i].b;
                    count += f;
                }
            }
            color /= (float)count;

            ownRenderer.color = new Color(color.x, color.y, color.z);
        }

        if (source.isPlaying && fadeoutTime > 0) {
            fadeoutTime -= Time.deltaTime;
            float f = Mathf.Clamp01(fadeoutTime / FADEOUT_TIME);
            source.volume = f;

            if (f <= 0) {
                source.Stop();
                source.volume = 1;
            }
        }
    }
}
