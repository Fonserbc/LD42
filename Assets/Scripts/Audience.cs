using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audience : MonoBehaviour {

    public AudiencePerson personPrefab;
    public RectTransform place;
    public int rows = 10;
    public int columns = 20;

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
	
	// Update is called once per frame
	void Update () {
		
	}

    public float CalmFactor() {
        return (Mathf.Sin(Time.time) + 1f) / 2f;
    }
}
