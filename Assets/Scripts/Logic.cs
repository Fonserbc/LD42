using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logic : MonoBehaviour {

    public Camera cam;
    public float minCamSize = 5f;
    [Header("Keyboard")]
    public KeyboardKey keyboardKeyPrefab;
    [Header("Levels")]
    public Level[] levels;

    Level currentLevel;
    Transform keyboard;

    KeyboardKey[] keys;

    void Start() {
        Init(levels[0]);
    }
    
	void Init (Level l) {
        if (l.keyboardMin <= l.keyboardMax)
        {
            Note from = l.keyboardMin;
            Note to = l.keyboardMax;
            keyboard = new GameObject("Keyboard").transform;
            keys = new KeyboardKey[to - from + 1];
            
            for (int i = 0; i < keys.Length; ++i) {
                keys[i] = Instantiate(keyboardKeyPrefab.gameObject, keyboard).GetComponent<KeyboardKey>();
                keys[i].note = from + i;
                keys[i].first = i == 0;
                keys[i].last = i == keys.Length - 1;
                keys[i].Init();
            }

            float width = keys[keys.Length-1].transform.position.x - keys[0].transform.position.x;
            width += KeyboardKey.keyDistance * 2f;
            float ratio = Screen.width / (float)Screen.height;
            float height = Mathf.Max(minCamSize * 2f, width / ratio);

            float middle = (keys[0].transform.position.x + keys[keys.Length - 1].transform.position.x) / 2f;

            cam.transform.position = new Vector3(middle, -height / 2f + KeyboardKey.keyHeight, cam.transform.position.z);
            cam.orthographicSize = height / 2f;
        }
	}
}
