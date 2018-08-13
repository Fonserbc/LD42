using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audience : MonoBehaviour {

    public AudiencePerson personPrefab;
    public RectTransform place;
    public int rows = 10;
    public int columns = 20;

    public Logic l;

    AudiencePerson[] persons;

	// Use this for initialization
	void Start () {
        persons = new AudiencePerson[rows * columns];

        for (int i = rows-1; i >= 0; --i) {
            for (int j = 0; j < columns; ++j)
            {
                int it = i * columns + j;
                persons[it] = Instantiate(personPrefab.gameObject, place.transform).GetComponent<AudiencePerson>();
                persons[it].ownImage.color = Color.Lerp(persons[it].ownImage.color, Color.black, Mathf.Lerp(0, 0.7f, i / (float)rows));
                persons[it].Init(this);

                RectTransform rt = persons[it].GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(j / (float)(columns - 1), Easing.Quadratic.Out(i / (float)rows)) + new Vector2(i % 2 == 0 ? (1f/(float)columns) / 2f : 0f, 0f);
                rt.anchorMax = rt.anchorMin;
                rt.anchoredPosition = Random.insideUnitCircle * 10f;
            }
        }
	}

    float lastCalmFactor = 0;
    float calmness = 0f;
    float calmnessRegainTime = 2f;
    float lastError = 0f;

    float nextWhisperTime = 0f;

    float nextCheerTime = 0f;
    float lastWhisperSoundTime = -10f;
	// Update is called once per frame
	void Update () {
        float currentCalmFactor = CalmFactor();

        if (currentCalmFactor < lastCalmFactor) {
            lastError = calmnessRegainTime;
        }

        if (lastError > 0)
        {
            lastError -= Time.deltaTime;

            float f = Mathf.Clamp01(lastError / calmnessRegainTime);

            calmness = Mathf.Lerp(currentCalmFactor, 0f, f);
        }
        else {
            calmness = currentCalmFactor;
        }
        lastCalmFactor = currentCalmFactor;


        nextWhisperTime -= Time.deltaTime;

        if (nextWhisperTime <= 0 && calmness < 1f) {
            Utilities.RandomValue(persons).Whisper();

            if (Time.time - lastWhisperSoundTime > 6f)
            {
                l.sounds.PlaySound(SoundEffects.EffectType.Whisper);
                lastWhisperSoundTime = Time.time;
            }

            nextWhisperTime = Mathf.Lerp(0.1f, 2f, calmness);
        }

        if (l.FinishLevelFactor() >= 1f) {
            nextCheerTime -= Time.deltaTime;

            if (nextCheerTime <= 0) {
                l.sounds.PlaySound(SoundEffects.EffectType.Cheer);

                nextCheerTime = Random.Range(2f, 4f);
            }
        }
	}

    public float CalmFactor() {
        float f = l.FinishLevelFactor();
        return f < 1f? Easing.Circular.Out(f) : 0;
    }
}
