using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuPiece : MonoBehaviour {


    public Image[] hands;
    public Text title;
    public Button ownButton;
    public RectTransform ownRt;

    public void Init(Level l) {
        title.text = l.name;

        for (int i = 0; i < hands.Length; ++i) {
            hands[i].gameObject.SetActive(i < l.handPlays.Length);
        }
    }
}
