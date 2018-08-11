using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingerNoteUI : MonoBehaviour {

    public ChordSinger singer;

    public Text text;

    Note.American lastNote;
	
	// Update is called once per frame
	void Update () {
        if (singer != null)
        {
            if (!singer.IsSinging())
            {
                text.text = "";
                lastNote.octave = -1;
            }
            else {
                Note.American n = (Note.American)singer.singingNote;

                if (n != lastNote)
                {
                    text.text = n.key.ToString() + "" + n.octave;
                    lastNote = n;
                }
            }
        }
	}
}
