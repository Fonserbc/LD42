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

    float time = 0;
    float beatLength = 0;
    int beatIt = 0;

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

        FindAllCombinations();
        //
        StartPlaying();
    }

    int combIt = 0;
    void StartPlaying() {
        beatIt = 0;
        time = 0;
        beatLength = 60f / (float)currentLevel.bpm;
        SetCombination(0);
        SignalBeatStart(0);
    }

    bool beatEnded = false;
    void Update()
    {
        time += Time.deltaTime;

        if (!beatEnded && time >= beatLength - 0f)
        {
            SignalBeatEnd(beatIt);
            beatEnded = true;
        }
        if (time >= beatLength) {
            time -= beatLength;

            //SignalBeatEnd(beatIt);
            beatIt++;
            SignalBeatStart(beatIt);
            beatEnded = false;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            SetCombination((combIt + 1)%combinations.Count);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            SetCombination((combIt + combinations.Count - 1) % combinations.Count);
        }
    }

    void SetCombination(int it) {

        combIt = it;
        Debug.Log("CurrentCombination: "+combIt);
        Utilities.DebugLogList(combinations[combIt]);

        for (int i = 0; i < hands.Length; ++i) {
            hands[i].noteOffset = combinations[combIt][i];
        }
    }

    public KeyboardKey GetKey(Note n) {
        return keys[n - keys[0].note];
    }

    public void HandsTouched(int handLeft, int handRight) {
        Debug.Log("Touch! " + handLeft + " " + handRight);
    }

    void SignalBeatStart(int it)
    {
        for (int i = 0; i < hands.Length; ++i)
        {
            hands[i].BeatStarted(it);
        }
    }

    void SignalBeatEnd(int it)
    {
        for (int i = 0; i < hands.Length; ++i)
        {
            hands[i].BeatEnd(it);
        }
    }

    //

    List<List<int>> combinations;

    void FindAllCombinations()
    {
        int maxCombinations = currentLevel.Range() - currentLevel.handPlays[0].Range() + 1;
        for (int i = 1; i < currentLevel.handPlays.Length; ++i) {
            int c = currentLevel.Range() - currentLevel.handPlays[i].Range() + 1;
            maxCombinations *= c;
        }

        combinations = new List<List<int>>(maxCombinations);
        List<int> combinationAux = new List<int>(currentLevel.handPlays.Length);
        FindCombinationsRecursive(ref combinations, ref combinationAux);

        Debug.Log("Total combinations found: " + combinations.Count);
    }

    void FindCombinationsRecursive(ref List<List<int>> combinations, ref List<int> currentCombination) {
        if (currentCombination.Count == currentCombination.Capacity)
        {
            if (CheckCombination(ref currentCombination))
            {
                combinations.Add(new List<int>(currentCombination));
            }
        }
        else {
            int combIt = currentCombination.Count;
            int maxOffset = currentLevel.Range() - currentLevel.handPlays[combIt].Range();
            for (int i = 0; i <= maxOffset; ++i)
            {
                currentCombination.Add(i);
                FindCombinationsRecursive(ref combinations, ref currentCombination);
                currentCombination.RemoveAt(combIt);
            }
        }
    }

    bool CheckCombination(ref List<int> comb) {
        bool firstComb = true;
        for (int i = 0; i < currentLevel.handPlays.Length; ++i) {
            if (firstComb) firstComb &= comb[i] == 0;
            if (currentLevel.handPlays[i].HighestNote() + comb[i] > currentLevel.keyboardMax) {
                comb[i] = currentLevel.keyboardMax - (currentLevel.handPlays[i].HighestNote() + comb[i]);
            }
        }

        bool[,] used = new bool[currentLevel.Range(), currentLevel.blockCount];
        int min = currentLevel.keyboardMin;
        for (int i = 0; i < currentLevel.handPlays.Length; ++i) {
            for (int j = 0; j < currentLevel.handPlays[i].notes.Length; ++j) {
                int note = currentLevel.handPlays[i].notes[j].note - min + comb[i];
                int pos = currentLevel.handPlays[i].notes[j].blockStart;
                for (int k = 0; k < currentLevel.handPlays[i].notes[j].blockLength; ++k)
                {
                    //Debug.Log(note + "/"+used.GetLength(0) + " "+(pos + k)+ "/"+used.GetLength(1));
                    if (used[note, pos + k]) {
                        if (firstComb) {
                            Debug.Log("Block: " + (pos + k) + " Note: " + ((Note.American)(min+pos)).ToString());
                        }
                        return false;
                    }
                    else
                    {
                        used[note, pos + k] = true;
                    }
                }
            }
        }

        return true;
    }
}
