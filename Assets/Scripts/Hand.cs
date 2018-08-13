using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public SpriteRenderer[] ownRenderers;
    public Transform fingerTransform;
    public Transform bodyTransform;
    public Transform lineOrigin, knob;
    public LineRenderer line0, line1;
    public float minLine0Distance = 3f;
    public Sprite pressingSprite, idleSprite;

    public SpriteButton arrowButtonLeft, arrowButtonRight, toggleButton;

    public SpriteRenderer optionsFrom, optionsTo;
    public SpriteRenderer finger;

    public float fingerHeight = 2f;

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
            middleNote = (ownHandplay.LowestNote() + ownHandplay.HighestNote()) / 2;
            startPos = logic.GetKeyPosition(middleNote + noteOffset, false) - Vector3.up * KeyboardKey.keyHeight / 4f + Vector3.down * (ownId / (float)logic.handsColor.Length);
            return startPos;
        }
    }
    public bool isPlaying = true;

    Vector3 startPos;

    int minOffset, maxOffset;
    Transform optionsParent;

    SpriteRenderer[] options;

    float armsLength = 0;

    public void Init(Logic l, int id)
    {
        logic = l;
        ownId = id;
        SetColor(l.handsColor[id]);
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

        armsLength = Mathf.Max(MinArmLengthToReach(logic.GetKeyPosition(logic.keys[logic.keys.Length - 1].note)), MinArmLengthToReach(logic.GetKeyPosition(logic.keys[0].note)));

        SetOffset(Random.Range(minOffset, maxOffset + 1));
        toggleButton.SetToggleState(true);
    }

    float MinArmLengthToReach(Vector3 pos) {
        float a = (pos + (Vector3.up * fingerHeight) - lineOrigin.position).magnitude;
        a += 0.5f;
        a /= 2f;
        return a;
    }

    public void Clean() {
        Destroy(optionsParent.gameObject);
    }

    public void StopPlaying() {
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
        finger.sprite = pressingSprite;
        keyPressing = k;
    }

    void ReleaseKey(KeyboardKey k) {
        if (k == null) return;
        k.Released(ownId);
        finger.sprite = idleSprite;
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
            if (f > minFactor)
            {
                fingerTransform.position = Vector3.Lerp(currentBeatPos, nextBeatPos, (f - minFactor) / (1f - minFactor));
            }
            else fingerTransform.position = currentBeatPos;
        }
        else {
            fingerTransform.position = Vector3.Lerp(fingerTransform.position, restPos, Time.deltaTime * 3f);
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

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(lineOrigin.position, minLine0Distance);
        Gizmos.DrawLine(fingerTransform.position, fingerTransform.position - Vector3.up * fingerHeight);
    }
}
