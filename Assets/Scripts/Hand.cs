using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour {

    public Sprite pressingSprite;
    public Sprite relaxedSprite;
    public SpriteRenderer ownRenderer;

    [HideInInspector]
    public Level.HandPlay ownHandplay;

    Logic logic;
    int ownId;
    int currentIt;

    [HideInInspector]
    public int noteOffset = 0;
    KeyboardKey keyPressing;

    int middleNote = 0;

    Vector3 restPos {
        get {
            return logic.GetKey(middleNote + noteOffset).transform.position;
        }
    }
    Vector3 wantedPos = Vector3.zero;

    public void Init(Logic l, int id) {
        logic = l;
        ownId = id;
        ownRenderer.color = l.handsColor[id];
        currentIt = 0;

        middleNote = ownHandplay.LowestNote() + ownHandplay.Range() / 2;
        wantedPos = restPos;
    }

    public void BeatStarted(int beat) {
        int beatIt = beat % logic.currentLevel.blockCount;

        //Debug.Log("Start " + ownId + ": " + beatIt + "=> ownHandplay.notes[" + currentIt + "].beatStart = " + ownHandplay.notes[currentIt].beatStart);

        if (ownHandplay.notes[currentIt].blockStart == beatIt) {
            PressKey(logic.GetKey(ownHandplay.notes[currentIt].note + noteOffset));
        }
    }

    public void BeatEnd(int beat) {
        int beatIt = (beat + 1) % logic.currentLevel.blockCount;

        //Debug.Log("End " + ownId + ": " + beatIt + "=> ownHandplay.notes[" + currentIt + "].beatEnd = " + (ownHandplay.notes[currentIt].beatStart + ownHandplay.notes[currentIt].beatLength));

        if (((ownHandplay.notes[currentIt].blockStart + ownHandplay.notes[currentIt].blockLength) % logic.currentLevel.blockCount) == beatIt)
        {
            ReleaseKey(keyPressing);
            currentIt = (currentIt + 1) % ownHandplay.notes.Length;
        }
    }

    void PressKey(KeyboardKey k) {
        k.Pressed(ownId);
        wantedPos = k.transform.position;
        wantedPos.y += k.isBlack ? KeyboardKey.keyHeight / 2f : KeyboardKey.keyHeight / 4f;
        transform.position = wantedPos;

        ownRenderer.sprite = pressingSprite;
        keyPressing = k;
    }

    void ReleaseKey(KeyboardKey k) {
        k.Released(ownId);
        ownRenderer.sprite = relaxedSprite;
        wantedPos = restPos;
    }

    void Update() {
        transform.position = Vector3.Lerp(transform.position, wantedPos, 5f * Time.deltaTime);
    }
}
