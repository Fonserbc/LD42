using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChordSinger : Singer {

    bool[] options;

    protected ChordSinger[] otherChordSingers;

    public int maxJump = 10;

    Note lastNote;
    bool firstNote = true;

    protected override void Init()
    {
        options = new bool[pitches.Length];

        otherChordSingers = new ChordSinger[otherSingers.Length];
        for (int i = 0; i < otherSingers.Length; ++i)
        {
            otherChordSingers[i] = otherSingers[i].GetComponent<ChordSinger>();
        }
    }

    protected override bool ChooseNewNote()
    {
        if (!music.HasStarted()) {
            return false;
        }

        Utilities.InitializeArray(ref options, false);

        Chord currentChord = music.GetCurrentChord();

        int optionCount = 0;

        for (int i = 0; i < currentChord.inversion.notes.Length; ++i) {
            Note.American chordNote = (Note.American)((int)currentChord.tonic + currentChord.inversion.notes[i]);

            for (int j = 0; j < options.Length; ++j) {
                Note.American an = (Note.American)(min + j);

                if (!firstNote && Mathf.Abs(an - lastNote) > maxJump) continue;

                if (an.key == chordNote.key && !options[j]) {
                    options[j] = true;
                    optionCount++;
                }
            }
        }

        for (int i = 0; i < otherChordSingers.Length; ++i) {
            if (otherChordSingers[i].IsSinging())
            {
                Note otherNote = otherChordSingers[i].singingNote;

                int otherNoteLocal = otherNote - min;

                if (otherNoteLocal >= 0 && otherNoteLocal < options.Length) // Remove same note
                {
                    if (options[otherNoteLocal])
                    {
                        options[otherNoteLocal] = false;
                        optionCount--;
                    }

                    if (otherChordSingers[i].min < min)
                    {
                        for (int j = 0; j < otherNoteLocal; ++j)
                        {
                            if (options[j])
                            {
                                options[j] = false;
                                optionCount--;
                            }
                        }
                    }
                    else if (otherChordSingers[i].max > max)
                    {
                        for (int j = options.Length - 1; j > otherNoteLocal; --j)
                        {
                            if (options[j])
                            {
                                options[j] = false;
                                optionCount--;
                            }
                        }
                    }
                }
            }
        }

        if (optionCount == 0) return false;

        int[] currentOptions = new int[optionCount];
        int it = 0;
        for (int i = 0; i < options.Length && it < currentOptions.Length; ++i) {
            if (options[i]) {
                currentOptions[it++] = i;
            }
        }
        
        singingNote = Utilities.RandomValue(currentOptions) + min;

        if (!firstNote && singingNote == lastNote && Random.Range(0, 2) == 0)
        { // If the note is the same as last, 50% chance to try another one
            singingNote = Utilities.RandomValue(currentOptions) + min;
        }

        lastNote = singingNote;
        firstNote = false;

        return true;
    }
}
