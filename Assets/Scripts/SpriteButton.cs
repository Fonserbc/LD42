using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteButton : MonoBehaviour {

    public UnityEngine.Events.UnityEvent onMouseDown;

    public SpriteRenderer rend;
    public Sprite enabledSprite, disabledSprite;

    public bool isToggle = false;
    bool toggleState = true;

    const float lingerTime = 0.1f;
    float time = 0f;

    public void OnEnable() {
        if (isToggle)
        {
            rend.sprite = toggleState ? enabledSprite : disabledSprite;
        }
        else {
            rend.sprite = enabledSprite;
        }
    }

    public void OnDisable() {
        rend.sprite = disabledSprite;
    }

    public void SetToggleState(bool b) {
        toggleState = b;

        RefreshToggle();
    }

    void RefreshToggle() {
        if (isToggle) {
            rend.sprite = toggleState ? enabledSprite : disabledSprite;
        }
    }

    void OnMouseDown() {
        if (isToggle)
        {
            toggleState = !toggleState;
            RefreshToggle();
            onMouseDown.Invoke();
        }
        else {
            if (enabled)
            {
                time = lingerTime;
                rend.sprite = disabledSprite;
                onMouseDown.Invoke();
            }
        }
    }

    void Update() {
        if (!isToggle && time > 0) {
            time -= Time.deltaTime;
            if (time <= 0) {
                rend.sprite = enabled? enabledSprite : disabledSprite;
            }
        }
    }

    public bool ToggleSate() {
        return toggleState;
    }
}
