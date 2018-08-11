using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName ="Music Level")]
public class Level : ScriptableObject {

    public Note.American keyboardMin, keyboardMax;
    public int bpm = 60;

    [System.Serializable]
    public class HandPlay {

        [System.Serializable]
        public class PlayedNote {
            public Note.American note;
            public int beatStart;
            public int beatLength = 1;
            public int beatEnd {
                get {
                    return beatStart + beatLength;
                }
            }
        }

        public PlayedNote[] notes;

        public int beatCount = 0;

        Note.American lowest, highest;

        public void Init()
        {
            lowest = Note.American.Highest;
            highest = Note.American.Lowest;
            for (int i = 0; i < notes.Length; ++i) {
                if (notes[i].note > highest) {
                    highest = notes[i].note;
                }
                if (notes[i].note < lowest)
                {
                    lowest = notes[i].note;
                }
            }
        }

        public Note LowestNote()
        {
            return lowest;
        }

        public Note HighestNote()
        {
            return highest;
        }

        public int Range()
        {
            return highest - lowest;
        }
    }

    public HandPlay[] handPlays;

    public void Init() {
        for (int i = 0; i < handPlays.Length; ++i) {
            handPlays[i].Init();
        }
    }
}
