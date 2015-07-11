using UnityEngine;
using System.Collections;

public class CursorController : MonoBehaviour {

    public bool useCustomCursor = true;
    public Texture2D cursorTexture;
    public Vector2 cursorHotspot;
    public CursorMode cursorMode = CursorMode.Auto;

	// Use this for initialization
	void Start () {
	    // Keep the controller when switching scenes
        KeepController();

        // Set cursor
        SetCursor();
	}

    /**
     * Keep the controller object when switching scenes.
     */
    private void KeepController() {
        DontDestroyOnLoad(gameObject);
    }

    /**
     * Set up the cursor.
     */
    private void SetCursor() {
        // Determine whether to show the cursor based on the platform, show a status message
        bool showCursor = (!(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)) && useCustomCursor;
        Debug.Log("Using custom cursor: " + (showCursor ? "True" : "False") + " (based on current platform)");

        // Show or hide the cursor
        Cursor.visible = true;
        if (showCursor)
            Cursor.SetCursor(this.cursorTexture, this.cursorHotspot, this.cursorMode);
        else
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}
