using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentChordUI : MonoBehaviour {


    public Music music;

    public Text text;

    Chord lastChord;

    void Update()
    {
        if (music != null)
        {
            if (!music.HasStarted())
            {
                text.text = "";
            }
            else {
                Chord c = music.GetCurrentChord(true);

                if (lastChord != c)
                {
                    text.text = c.tonic.ToString() + "" + c.inversion.name;

                    lastChord = c;
                }
            }
        }
    }
}
