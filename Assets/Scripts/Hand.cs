using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour {

    public enum Finger {
        Thumb = 0,
        Index = 1,
        Middle = 2,
        Ring = 3,
        Little = 4
    }
    
    [System.Serializable]
    public class HandFingerSprite {
        public Finger finger;
        public Sprite pWhite, pBlack;
    }

    public HandFingerSprite[] fingerSprites;
    public Sprite relaxedHand;
    public SpriteRenderer ownRenderer;

    [HideInInspector]
    public Level.HandPlay ownHandplay;

    Logic logic;
    int ownId;
    int currentIt;

    [HideInInspector]
    public int noteOffset = 0;

    public void Init(Logic l, int id) {
        logic = l;
        ownId = id;
        ownRenderer.color = l.handsColor[id];
        currentIt = 0;
    }

    public void BeatStarted(int beat) {
        int beatIt = beat % logic.currentLevel.blockCount;

        //Debug.Log("Start " + ownId + ": " + beatIt + "=> ownHandplay.notes[" + currentIt + "].beatStart = " + ownHandplay.notes[currentIt].beatStart);

        if (ownHandplay.notes[currentIt].blockStart == beatIt) {
            PressKey(logic.GetKey(ownHandplay.notes[currentIt].note));
        }
    }

    public void BeatEnd(int beat) {
        int beatIt = (beat + 1) % logic.currentLevel.blockCount;

        //Debug.Log("End " + ownId + ": " + beatIt + "=> ownHandplay.notes[" + currentIt + "].beatEnd = " + (ownHandplay.notes[currentIt].beatStart + ownHandplay.notes[currentIt].beatLength));

        if (((ownHandplay.notes[currentIt].blockStart + ownHandplay.notes[currentIt].blockLength) % logic.currentLevel.blockCount) == beatIt)
        {
            ReleaseKey(logic.GetKey(ownHandplay.notes[currentIt].note));
            currentIt = (currentIt + 1) % ownHandplay.notes.Length;
        }
    }

    void PressKey(KeyboardKey k) {
        k.Pressed(ownId);
        Vector3 wantedPos = k.transform.position;
        wantedPos.y += k.isBlack ? KeyboardKey.keyHeight / 2f : KeyboardKey.keyHeight / 4f;
        transform.position = wantedPos;

        ownRenderer.sprite = fingerSprites[1].pWhite;
    }

    void ReleaseKey(KeyboardKey k) {
        k.Released(ownId);
        ownRenderer.sprite = relaxedHand;
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
    }
}
