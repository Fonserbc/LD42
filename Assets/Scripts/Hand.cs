using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public SpriteRenderer[] ownRenderers;
    public Transform fingerTransform;

    public SpriteButton arrowButtonLeft, arrowButtonRight, toggleButton;

    [HideInInspector]
    public Level.HandPlay ownHandplay;

    Logic logic;
    int ownId;
    int currentIt;

    [HideInInspector]
    public int noteOffset = 0;
    KeyboardKey keyPressing = null;

    int middleNote = 0;

    Vector3 restPos {
        get {
            return startPos;
        }
    }
    Vector3 wantedPos = Vector3.zero;
    public bool isPlaying = true;

    Vector3 startPos;

    int minOffset, maxOffset;

    public void Init(Logic l, int id) {
        logic = l;
        ownId = id;
        SetColor(l.handsColor[id]);
        currentIt = 0;

        middleNote = ownHandplay.LowestNote() + ownHandplay.Range() / 2;
        wantedPos = restPos;
        startPos = transform.position;

        //

        Level level = l.currentLevel;
        minOffset = level.keyboardMin - ownHandplay.LowestNote();
        maxOffset = level.keyboardMax - ownHandplay.HighestNote();

        noteOffset = Random.Range(minOffset, maxOffset + 1);
        UpdateOffsetArrows();

        Debug.Log(minOffset + "->" + maxOffset + ": " + noteOffset);
        isPlaying = false;
        toggleButton.SetToggleState(false);
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
        if (!isPlaying) return;
        k.Pressed(ownId);
        wantedPos = k.transform.position;
        wantedPos.y += k.isBlack ? KeyboardKey.keyHeight / 2f : KeyboardKey.keyHeight / 4f;
        
        keyPressing = k;
    }

    void ReleaseKey(KeyboardKey k) {
        if (k == null) return;
        k.Released(ownId);
        wantedPos = restPos;
        keyPressing = null;
    }

    void Update() {
        fingerTransform.position = wantedPos;// Vector3.Lerp(transform.position, wantedPos, 5f * Time.deltaTime);
    }

    void SetColor(Color c) {
        foreach (SpriteRenderer r in ownRenderers) {
            r.color = c;
        }
    }


    public void TogglePressed() {
        isPlaying = toggleButton.ToggleSate();
    }

    public void RightArrowPressed() {
        if (noteOffset < maxOffset) {
            noteOffset++;
            UpdateOffsetArrows();
        }
    }

    public void LeftArrowPressed() {
        if (noteOffset > minOffset)
        {
            noteOffset--;
            UpdateOffsetArrows();
        }
    }

    void UpdateOffsetArrows() {
        arrowButtonRight.enabled = noteOffset < maxOffset;
        arrowButtonLeft.enabled = noteOffset > minOffset;
    }
}
