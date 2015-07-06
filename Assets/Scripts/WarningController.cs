using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WarningController : MonoBehaviour {

    const float WARNING_TIME = 8.0f;
    const float FADE_DELAY = 1.0f;
    const float FADE_DURATION = 2.0f;

    public GameObject warningTextObj;
    public GameObject loadingTextObj;

    private float fadeInAt;
    private float fadeOutAt;
    private float nextSceneAt;
    private Text warningTextComp;
    private Text loadingTextComp;
    private bool showLoading = false;

	void Start () {
        // Set the fade and next scene time
        fadeInAt = Time.time + FADE_DELAY;
        fadeOutAt = Time.time + WARNING_TIME - FADE_DURATION;
        nextSceneAt = Time.time + WARNING_TIME;

        // Get the text objects
        warningTextComp = warningTextObj.GetComponent<Text>();
        loadingTextComp = loadingTextObj.GetComponent<Text>();

        // Fade out (instantly)
        warningTextComp.CrossFadeAlpha(0.0f, 0.0f, false);
        loadingTextComp.CrossFadeAlpha(0.0f, 0.0f, false);
	}

    void Update() {
        // Fade in
        if (fadeInAt <= Time.time && fadeInAt >= 0) {
            fadeInAt = -1;
            warningTextComp.CrossFadeAlpha(1.0f, FADE_DURATION, false);
        }

        // Fade out
        if (fadeOutAt <= Time.time && fadeOutAt >= 0) {
            fadeOutAt = -1;
            warningTextComp.CrossFadeAlpha(0.0f, FADE_DURATION, false);
        }

        // Go to the next scene at the specified time
        if ((nextSceneAt - 0.75f) <= Time.time && !showLoading) {
            showLoading = true;
            loadingTextComp.CrossFadeAlpha(1.0f, 0.3f, false);
        }

        // Go to the next scene at the specified time
        if (nextSceneAt <= Time.time && nextSceneAt >= 0) {
            nextSceneAt = -1;
            Application.LoadLevel(1);
        }
	}
}
