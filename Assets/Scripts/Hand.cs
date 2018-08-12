﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public SpriteRenderer[] ownRenderers;
    public Transform fingerTransform;
    public Transform bodyTransform;

    public SpriteButton arrowButtonLeft, arrowButtonRight, toggleButton;

    public SpriteRenderer optionsFrom, optionsTo;

    [HideInInspector]
    public Level.HandPlay ownHandplay;

    Logic logic;
    int ownId;
    int currentIt;

    int noteOffset = 0;
    KeyboardKey keyPressing = null;

    int middleNote = 0;

    Vector3 restPos {
        get {
            return startPos;
        }
    }
    public bool isPlaying = true;

    Vector3 startPos;

    int minOffset, maxOffset;
    Transform optionsParent;

    SpriteRenderer[] options;

    public void Init(Logic l, int id) {
        logic = l;
        ownId = id;
        SetColor(l.handsColor[id]);
        currentIt = 0;

        middleNote = ownHandplay.LowestNote() + ownHandplay.Range() / 2;
        startPos = transform.position;

        //

        Level level = l.currentLevel;
        minOffset = level.keyboardMin - ownHandplay.LowestNote();
        maxOffset = level.keyboardMax - ownHandplay.HighestNote();

        Debug.Log(ownHandplay.name + " has " + (maxOffset - minOffset + 1) + " positions");
        isPlaying = false;
        toggleButton.SetToggleState(false);

        optionsParent = new GameObject("Options").transform;
        optionsParent.SetParent(optionsFrom.transform.parent, false);
        optionsParent.transform.localScale = Vector3.one;

        int numOptions = maxOffset - minOffset + 1;

        options = new SpriteRenderer[numOptions];
        float deltaSpace = optionsTo.transform.localPosition.x - optionsFrom.transform.localPosition.x;
        deltaSpace /= (float)numOptions;
        float minScale = 0.200f;
        for (int i = 0; i < numOptions; ++i) {
            options[i] = Instantiate(optionsFrom, optionsParent).GetComponent<SpriteRenderer>();
            options[i].transform.localPosition = new Vector3(optionsFrom.transform.localPosition.x + deltaSpace * i + deltaSpace * 0.5f, optionsFrom.transform.localPosition.y, optionsFrom.transform.localPosition.z);
            //if (deltaSpace < minScale) {
            options[i].transform.localScale = new Vector3(deltaSpace / minScale, 1f, 1f);
            //}
            options[i].gameObject.SetActive(true);
        }

        SetOffset(Random.Range(minOffset, maxOffset + 1));
    }

    public void Clean() {
        Destroy(optionsParent.gameObject);
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
        
        keyPressing = k;
    }

    void ReleaseKey(KeyboardKey k) {
        if (k == null) return;
        k.Released(ownId);
        keyPressing = null;
    }

    void Update()
    {
        if (isPlaying)
        {
            float currentBeatFloat = logic.CurrentBeatFloat();

            int currentBlock = Mathf.FloorToInt(currentBeatFloat) % logic.currentLevel.blockCount;
            int nextBlock = currentBlock + 1;
            float f = currentBeatFloat - Mathf.Floor(currentBeatFloat);

            Vector3 currentBeatPos = restPos;

            if (ownHandplay.notes[currentIt].blockStart <= currentBlock)
            {
                currentBeatPos = logic.GetKeyPosition(ownHandplay.notes[currentIt].note + noteOffset);
            }
            Vector3 nextBeatPos = currentBeatPos;
            if (ownHandplay.notes[currentIt].blockEnd <= nextBlock)
            {
                int nextIt = (currentIt + 1) % ownHandplay.notes.Length;
                nextBlock = nextBlock % logic.currentLevel.blockCount;
                if (ownHandplay.notes[nextIt].blockStart >= nextBlock)
                {
                    nextBeatPos = logic.GetKeyPosition(ownHandplay.notes[nextIt].note + noteOffset);
                }
                else {
                    nextBeatPos = restPos;
                }
            }

            float minFactor = 0.7f;
            if (f > minFactor)
            {
                fingerTransform.position = Vector3.Lerp(currentBeatPos, nextBeatPos, (f - minFactor) / (1f - minFactor));
            }
            else fingerTransform.position = currentBeatPos;
        }
        else {
            fingerTransform.position = Vector3.Lerp(fingerTransform.position, restPos, Time.deltaTime * 3f);
        }
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
            SetOffset(noteOffset + 1);
        }
    }

    public void LeftArrowPressed() {
        if (noteOffset > minOffset)
        {
            SetOffset(noteOffset - 1);
        }
    }

    public void SetOffset(int off) {
        noteOffset = Mathf.Clamp(off, minOffset, maxOffset);
        UpdateOffsetUI();
    }

    void UpdateOffsetUI() {
        arrowButtonRight.enabled = noteOffset < maxOffset;
        arrowButtonLeft.enabled = noteOffset > minOffset;

        for (int i = 0; i < options.Length; ++i) {
            options[i].color = Color.Lerp(((noteOffset - minOffset) == i) ? Color.white : Color.black, logic.handsColor[ownId], 0.5f);
        }
    }
}
