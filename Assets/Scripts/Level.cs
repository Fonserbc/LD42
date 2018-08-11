using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName ="Music Level")]
public class Level : ScriptableObject {

    public Note.American keyboardMin, keyboardMax;
    public int bpm = 60;
    public int blockCount = 8;

    [System.Serializable]
    public class HandPlay {

        [System.Serializable]
        public class PlayedNote {
            public Note.American note;
            [UnityEngine.Serialization.FormerlySerializedAs("beatStart")]
            public int blockStart;
            [UnityEngine.Serialization.FormerlySerializedAs("beatLength")]
            public int blockLength = 1;
            public int beatEnd {
                get {
                    return blockStart + blockLength;
                }
            }
        }

        public PlayedNote[] notes;

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
            return highest - lowest + 1;
        }
    }

    public HandPlay[] handPlays;

    public void Init() {
        for (int i = 0; i < handPlays.Length; ++i) {
            handPlays[i].Init();
        }
    }

    public int Range()
    {
        return keyboardMax - keyboardMin + 1;
    }

}
