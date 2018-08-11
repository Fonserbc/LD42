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
    public SpriteRenderer ownRenderer;

    [HideInInspector]
    public Level.HandPlay ownHandplay;

    Logic logic;
    int ownId;

    public void Init(Logic l, int id) {
        logic = l;
        ownId = id;
        ownRenderer.color = l.handsColor[id];
    }

    public void BeatStarted(int beat) {

    }

    public void BeatEnd(int beat) {

    }
}
