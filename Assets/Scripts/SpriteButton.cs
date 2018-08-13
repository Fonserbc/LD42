using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteButton : MonoBehaviour {

    public UnityEngine.Events.UnityEvent onMouseDown;

    public SpriteRenderer rend;
    public Sprite enabledSprite, disabledSprite;

    public bool isToggle = false;
    bool toggleState = true;

    public float lerpColorFactor = 0f;

    const float lingerTime = 0.1f;
    float time = 0f;

    Color original;

    public void Init(Color c) {
        original = c;
        OnEnable();
    }

    public void OnEnable() {
        rend.sprite = enabledSprite;
        rend.color = Color.Lerp(original, Color.white, lerpColorFactor);

        RefreshToggle();
    }

    public void OnDisable() {
        rend.sprite = disabledSprite;
        rend.color = Color.Lerp(original, Color.black, lerpColorFactor);
    }

    public void SetToggleState(bool b) {
        toggleState = b;

        RefreshToggle();
    }

    void RefreshToggle() {
        if (isToggle) {
            if (toggleState)
            {
                rend.sprite = enabledSprite;
                rend.color = Color.Lerp(original, Color.white, lerpColorFactor);
            }
            else
            {
                rend.sprite = disabledSprite;
                rend.color = Color.Lerp(original, Color.black, lerpColorFactor);
            }
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
                rend.color = Color.Lerp(original, Color.black, lerpColorFactor);
                onMouseDown.Invoke();
            }
        }
    }

    void Update() {
        if (!isToggle && time > 0) {
            time -= Time.deltaTime;
            if (time <= 0)
            {
                if (enabled)
                {
                    rend.sprite = enabledSprite;
                    rend.color = Color.Lerp(original, Color.white, lerpColorFactor);
                }
                else
                {
                    rend.sprite = disabledSprite;
                    rend.color = Color.Lerp(original, Color.black, lerpColorFactor);
                }
            }
        }
    }

    public bool ToggleSate() {
        return toggleState;
    }
}
