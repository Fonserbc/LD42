using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSinger : Singer
{
    protected override bool ChooseNewNote()
    {
        singingNote = Random.Range(0, max - min) + min;

        return true;
    }
}