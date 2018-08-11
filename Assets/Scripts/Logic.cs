using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logic : MonoBehaviour {

    public Camera cam;
    public float minCamSize = 5f;
    [Header("Hands")]
    public Color[] handsColor;
    public Hand handPrefab;
    [Header("Keyboard")]
    public KeyboardKey keyboardKeyPrefab;
    [Header("Levels")]
    public Level[] levels;

    [HideInInspector]
    public Level currentLevel;
    Transform keyboardTransform;
    Transform handsTransform;

    KeyboardKey[] keys;
    Hand[] hands;

    void Start()
    {
        keyboardTransform = new GameObject("Keyboard").transform;
        handsTransform = new GameObject("Hands").transform;
        Init(levels[0]);
    }
    
	void Init (Level l)
    {
        if (l.keyboardMin > l.keyboardMax)
        {
            Debug.Log("Keyboard is impossible");
            return;
        }

        currentLevel = l;
        l.Init();
        Note from = l.keyboardMin;
        Note to = l.keyboardMax;
        keys = new KeyboardKey[to - from + 1];
            
        for (int i = 0; i < keys.Length; ++i) {
            keys[i] = Instantiate(keyboardKeyPrefab.gameObject, keyboardTransform).GetComponent<KeyboardKey>();
            keys[i].note = from + i;
            keys[i].first = i == 0;
            keys[i].last = i == keys.Length - 1;
            keys[i].Init(this);
        }

        float width = keys[keys.Length-1].transform.position.x - keys[0].transform.position.x;
        width += KeyboardKey.keyDistance * 2f;
        float ratio = Screen.width / (float)Screen.height;
        float height = Mathf.Max(minCamSize * 2f, width / ratio);

        float middle = (keys[0].transform.position.x + keys[keys.Length - 1].transform.position.x) / 2f;

        cam.transform.position = new Vector3(middle, -height / 2f + KeyboardKey.keyHeight, cam.transform.position.z);
        cam.orthographicSize = height / 2f;

        hands = new Hand[l.handPlays.Length];
        for (int i = 0; i < hands.Length; ++i) {
            hands[i] = Instantiate(handPrefab, handsTransform).GetComponent<Hand>();
            hands[i].ownHandplay = l.handPlays[i];
            hands[i].Init(this, i);
        }
    }

    void Update()
    {

    }

    public KeyboardKey GetKey(Note n) {
        return keys[n - keys[0].note];
    }

    public void HandsTouched(int handLeft, int handRight) {

    }
}
