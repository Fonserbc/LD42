using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualSinger : Singer {
    	
	// Update is called once per frame
	protected override void Update () {
        base.Update();

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            singingNote = Mathf.Clamp(singingNote + 1, (int)min, (int)max);
            if (IsSinging())
            {
                Stop();
                Sing();
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            singingNote = Mathf.Clamp(singingNote - 1, (int)min, (int)max);
            if (IsSinging())
            {
                Stop();
                Sing();
            }
        }
    }
}
