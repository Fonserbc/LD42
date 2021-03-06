﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public List<SpriteRenderer> ownRenderers;
    public Transform fingerTransform;
    public Transform bodyTransform;
    public Transform lineOrigin, knob;
    public LineRenderer line0, line1;
    public float minLine0Distance = 3f;
    public Sprite pressingSprite, idleSprite;

    public SpriteButton arrowButtonLeft, arrowButtonRight, toggleButton;

    public SpriteRenderer optionsFrom, optionsTo;
    public SpriteRenderer finger;

    [Header("Cues")]
    public SpriteRenderer cueMain;
    public SpriteRenderer cuePrefab;
    public Sprite[] cueMainSprites;
    public Sprite[] cueSprites;
    public Transform cueCenter;
    public float cueDistance;
    SpriteRenderer[] cues;
    public UnityEngine.UI.Image fillingImage;
    bool[] cuePlayed;

    public float fingerHeight = 2f;

    float lerpTime = 0.5f;
    float animTime = 0f;

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
            if (isPlaying)
            {
                middleNote = (ownHandplay.LowestNote() + ownHandplay.HighestNote()) / 2;
                startPos = logic.GetKeyPosition(middleNote + noteOffset, false) - Vector3.up * KeyboardKey.keyHeight / 4f + Vector3.down * (ownId / (float)logic.handsColor.Length);
                return startPos;
            }
            else {
                return lineOrigin.position + Vector3.up * 1.2f + Vector3.left * 0.8f;
            }
        }
    }
    bool isPlaying = true;

    Vector3 startPos;

    int minOffset, maxOffset;
    Transform optionsParent;

    SpriteRenderer[] options;

    float armsLength = 0;

    int workingBeats = 0;

    public void Init(Logic l, int id)
    {
        logic = l;
        ownId = id;
        currentIt = 0;
        
        finger.sprite = idleSprite;

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

        armsLength = MinArmLengthToReach(logic.GetKeyPosition(logic.keys[0].note));
        for (int i = 1; i < logic.keys.Length; ++i)
        {
            armsLength = Mathf.Max(armsLength, MinArmLengthToReach(logic.GetKeyPosition(logic.keys[i].note)));
        }

        fingerTransform.position = restPos;
        workingBeats = 0;

        cues = new SpriteRenderer[level.blockCount];
        cuePlayed = new bool[level.blockCount];
        Utilities.InitializeArray(ref cuePlayed, false);

        float startAngle = 1.5f * Mathf.PI;
        float deltaAngle = 2f * Mathf.PI / (float)level.blockCount;
        for (int i = 0; i < cues.Length; ++i) {
            cues[i] = Instantiate(cuePrefab.gameObject, cueCenter).GetComponent<SpriteRenderer>();
            cues[i].sprite = cueSprites[0];

            float angle = startAngle + deltaAngle * i;
            cues[i].transform.localPosition = new Vector3(-Mathf.Cos(angle) * cueDistance, Mathf.Sin(angle) * cueDistance, 0);
        }
        ownRenderers.AddRange(cues);

        SetColor(l.handsColor[id]);

        noteOffset = 0;
        if (maxOffset - minOffset > 0)
        {
            while (noteOffset == 0)
            {
                noteOffset = Random.Range(minOffset, maxOffset + 1);
            }
        }
        SetOffset(noteOffset);
    }

    float MinArmLengthToReach(Vector3 pos) {
        float a = (pos + (Vector3.up * fingerHeight) - lineOrigin.position).magnitude;
        a += 0.5f;
        a /= 2f;
        return a;
    }

    public bool IsPlaying() {
        return isPlaying;
    }

    public void Clean() {
        Destroy(optionsParent.gameObject);
    }

    Vector3 lastFingerPos = Vector3.zero;
    public void StopPlaying() {
        isPlaying = false;
        toggleButton.SetToggleState(false);
        lastFingerPos = fingerTransform.position;
        animTime = lerpTime;

        Utilities.InitializeArray(ref cuePlayed, false);
        workingBeats = 0;
    }

    public void StartPlaying() {
        isPlaying = true;
        lastFingerPos = fingerTransform.position;
        animTime = lerpTime;
    }

    public void BeatStarted(int beat) {
        int beatIt = beat % logic.currentLevel.blockCount;

        if (IsPlaying())
        {
            cuePlayed[beatIt] = true;
            workingBeats++;
        }

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
        if (!isPlaying || GetLerpFactor() < 0.9f) return;
        k.Pressed(ownId);
        finger.sprite = pressingSprite;
        keyPressing = k;
    }

    void ReleaseKey(KeyboardKey k) {
        if (k == null) return;
        k.Released(ownId);
        finger.sprite = idleSprite;
        keyPressing = null;
    }

    float GetLerpFactor() {
        if (animTime > 0)
        {
            return 1f - Mathf.Clamp01(animTime / lerpTime);
        }
        else return 1f;
    }

    void Update()
    {
        if (animTime > 0) {
            animTime -= Time.deltaTime;
        }
        float lerpFactor = GetLerpFactor();

        fillingImage.fillAmount = WorkingFactor();

        float currentBeatFloat = logic.CurrentBeatFloat();
        float startAngle = 1.5f * Mathf.PI;
        float deltaAngle = 2f * Mathf.PI / (float)logic.currentLevel.blockCount;

        float angle = startAngle + deltaAngle * currentBeatFloat;
        cueMain.transform.localPosition = new Vector3(-Mathf.Cos(angle) * cueDistance, Mathf.Sin(angle) * cueDistance, 0);
        cueMain.sprite = isPlaying ? cueMainSprites[1] : cueMainSprites[0];
        cueMain.color = Color.Lerp(logic.handsColor[ownId], Color.white, isPlaying? 0.8f : 0.2f);

        int currentBeat = logic.CurrentBeat() % cuePlayed.Length;
        for (int i = 0; i < cues.Length; ++i) {
            int whichSprite = cuePlayed[i] ? 1 : 0;
            cues[i].color = logic.handsColor[ownId];
            if (isPlaying && currentBeat == i)
            {
                whichSprite++;
                cues[i].color = Color.Lerp(cues[i].color, Color.white, 0.5f);
            }
            //if (cuePlayed[i])
            //{
            //}
            cues[i].sprite = cueSprites[whichSprite];
        }

        if (isPlaying)
        {
            int currentBlock = Mathf.FloorToInt(currentBeatFloat) % logic.currentLevel.blockCount;
            int nextBlock = currentBlock + 1;
            float f = currentBeatFloat - Mathf.Floor(currentBeatFloat);
            float minFactor = 0.7f;

            Vector3 currentBeatPos = restPos;
            bool active = false;

            if (ownHandplay.notes[currentIt].blockStart <= currentBlock)
            {
                currentBeatPos = logic.GetKeyPosition(ownHandplay.notes[currentIt].note + noteOffset);
                currentBeatPos.y += fingerHeight;
                active = true;
            }

            Vector3 nextBeatPos = currentBeatPos;
            if (ownHandplay.notes[currentIt].blockEnd <= nextBlock)
            {
                int nextIt = (currentIt + 1) % ownHandplay.notes.Length;
                nextBlock = nextBlock % logic.currentLevel.blockCount;
                if (ownHandplay.notes[nextIt].blockStart <= nextBlock)
                {
                    nextBeatPos = logic.GetKeyPosition(ownHandplay.notes[nextIt].note + noteOffset);
                    nextBeatPos.y += fingerHeight;
                }
                else {
                    nextBeatPos = restPos;
                }
                if (f > minFactor)
                    finger.sprite = idleSprite;
            }
            else if (!active && ownHandplay.notes[currentIt].blockStart <= nextBlock) {
                nextBeatPos = logic.GetKeyPosition(ownHandplay.notes[currentIt].note + noteOffset);
                nextBeatPos.y += fingerHeight;
            }

            Vector3 wantedPos = fingerTransform.position;
            if (f > minFactor)
            {
                wantedPos = Vector3.Lerp(currentBeatPos, nextBeatPos, (f - minFactor) / (1f - minFactor));
            }
            else wantedPos = currentBeatPos;

            fingerTransform.position = Vector3.Lerp(lastFingerPos, wantedPos, lerpFactor);
        }
        else {
            fingerTransform.position = Vector3.Lerp(lastFingerPos, restPos, lerpFactor);
        }

        if (clashTime > 0) {
            clashTime -= Time.deltaTime;
            float clashfactor = Easing.Sinusoidal.In(Mathf.Clamp01(clashTime / maxClashTime));
            fingerTransform.position += Vector3.right * 1.5f  * Mathf.Sin(Time.time * 15f) * clashfactor;
        }

        // Knob pos
        Vector3 delta = fingerTransform.position - lineOrigin.position;
        Vector3 perp = delta.normalized;
        float aux = perp.x;
        perp.x = perp.y;
        perp.y = -aux;
        if (perp.y > 0) {
            perp = perp * -1f;
        }

        float h = Mathf.Sqrt(armsLength * armsLength - ((delta.magnitude / 2f) * (delta.magnitude / 2f)));

        knob.position = Vector3.Lerp(lineOrigin.position, fingerTransform.position, 0.5f) + perp * h;

        line0.SetPosition(0, lineOrigin.transform.position);
        line0.SetPosition(1, knob.position);
        line1.SetPosition(0, knob.position);
        line1.SetPosition(1, fingerTransform.position + (knob.position - fingerTransform.position).normalized * 0.18f);
    }

    void SetColor(Color c) {
        foreach (SpriteRenderer r in ownRenderers) {
            r.color = c;
        }

        arrowButtonLeft.Init(c);
        arrowButtonRight.Init(c);
        toggleButton.Init(c);
    }


    public void TogglePressed() {
        if (toggleButton.ToggleSate())
        {
            StartPlaying();
        }
        else {
            StopPlaying();
        }
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
        workingBeats = 0;
        Utilities.InitializeArray(ref cuePlayed, false);
        animTime = lerpTime / 2f;
        lastFingerPos = fingerTransform.position;
        finger.sprite = idleSprite;
    }

    public void ChangeOffset() {
        if (noteOffset > minOffset) noteOffset--;
        if (noteOffset < maxOffset) noteOffset++;
    }

    public int GetOffset() {
        return noteOffset;
    }

    void UpdateOffsetUI() {
        arrowButtonRight.enabled = noteOffset < maxOffset;
        arrowButtonLeft.enabled = noteOffset > minOffset;

        for (int i = 0; i < options.Length; ++i) {
            options[i].color = Color.Lerp(((noteOffset - minOffset) == i) ? Color.white : Color.black, logic.handsColor[ownId], 0.5f);
        }
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(lineOrigin.position, minLine0Distance);
        Gizmos.DrawLine(fingerTransform.position, fingerTransform.position - Vector3.up * fingerHeight);
    }

    public float WorkingFactor() {
        return Mathf.Clamp01(workingBeats / (float)logic.currentLevel.blockCount);
    }


    float maxClashTime = 1.5f;
    float clashTime = 0f;
    public void Clash() {
        StopPlaying();
        clashTime = maxClashTime;
    }
}
