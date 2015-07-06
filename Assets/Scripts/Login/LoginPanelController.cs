using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LoginPanelController : MonoBehaviour {

    public GameObject loginPanel;
    public ConnectionPanelController connectionPanelController;
    public GameObject infoPanelController;
    public GameObject passField;
    public GameObject loginButton;
    public GameObject agentManagerObject;

    public string loginButtonTextDefault = "Inloggen";
    public string loginButtonTextValidating = "Gegevens valideren...";
    public string loginButtonTextValidated = "Gegevens gevalideerd!";
    public string loginButtonTextInvalid = "Gegevens ongeldig!";
    public float panelSpawnDelay = 1.0f;
    public float panelDecayDelay = 0.5f;
    public float validationDuration = 2.0f;

    private Animator loginPanelAnimator;
    private InfoPanelController infoPanelControllerController;
    private InputField passFieldInput;
    private Animator passFieldAnimator;
    private Button loginButtonButton;
    private Text loginButtonText;
    private AgentManager agentManager;

    private float spawnPanelAt = -1.0f;
    private float decayPanelAt = -1.0f;
    private float validatedAt = -1.0f;

    private bool showInfoPanelOnHide = false;

    private bool isSown = false;

	/**
     * Initialize.
     */
	void Start () {
	    // Get the component instances
        loginPanelAnimator = loginPanel.GetComponent<Animator>();
        infoPanelControllerController = infoPanelController.GetComponent<InfoPanelController>();
        passFieldInput = passField.GetComponent<InputField>();
        passFieldAnimator = passField.GetComponent<Animator>();
        loginButtonButton = loginButton.GetComponent<Button>();
        loginButtonText = loginButton.GetComponentInChildren<Text>();
        agentManager = agentManagerObject.GetComponent<AgentManager>();

        // Set the text of the login button
        loginButtonText.text = loginButtonTextDefault;

        // Set some timers
        spawnPanelAt = Time.time + panelSpawnDelay;

        // Update the button state
        UpdateButtonState();
	}

    void Update() {
        // Panel spawn delay timer
        if (spawnPanelAt <= Time.time && spawnPanelAt >= 0) {
            // Reset the timer
            spawnPanelAt = -1.0f;

            // Show the panel
            ShowPanel();
        }

        // Panel decay delay timer
        if (decayPanelAt <= Time.time && decayPanelAt >= 0) {
            // Reset the timer
            decayPanelAt = -1.0f;

            // Decay the panel
            HidePanel();
        }

        // Validation timer
        if (validatedAt <= Time.time && validatedAt >= 0) {
            validatedAt = -1.0f;

            // Check whether the input is valid
            if (agentManager.IsAgentWithCode(GetInput())) {
                SetButton(loginButtonTextValidated);
                HidePanelDelayed();
                showInfoPanelOnHide = true;
                agentManager.SetCurrentAgent(GetInput());
                Debug.Log("Login with: " + GetInput());

            } else
                ShakePanelWrongPass();
        }

        // Handle the enter keypress
        if (Input.GetKeyDown(KeyCode.Return))
            DoLogin();
    }

    /**
     * Show the main panel.
     */
    public void ShowPanel() {
        // Set the shown flag
        isSown = true;

        // Trigger the spawn animation
        loginPanelAnimator.SetTrigger("spawn");

        // Reset the input field text
        SetInput("");

        // Enable all controls
        SetComponentsInteractable(true);

        // Focus the input field
        FocusInput();

        showInfoPanelOnHide = false;
    }

    /**
     * Hide the main panel.
     */
    public void HidePanel() {
        // Trigger the decay animation
        loginPanelAnimator.SetTrigger("decay");

        // Disable all the input fields
        SetComponentsInteractable(false);

        // Show the info panel
        if (showInfoPanelOnHide) {
            //infoPanelControllerController.ShowPanel();
            connectionPanelController.ShowPanel();
        }

        // Reset the shown flag
        isSown = false;
    }

    /**
     * Hide the main panel, after the specified delay.
     */
    public void HidePanelDelayed() {
        decayPanelAt = Time.time + panelDecayDelay;
    }

    /**
     * Shake the login panel because of a wrong password.
     */
    public void ShakePanelWrongPass() {
        // Set the button and component states
        loginButtonText.text = loginButtonTextDefault;
        SetComponentsInteractable(true);

        // Shake the panel and flash the input field
        loginPanelAnimator.SetTrigger("shake");
        passFieldAnimator.SetTrigger("invalid");

        // Focust the input field
        FocusInput();

        // Disable the button and set it's text
        SetButtonEnabled(false);
        SetButton(loginButtonTextInvalid);
    }

    /**
     * Enable all controls on the login panel.
     */
    private void SetComponentsInteractable(bool interactable) {
        // Enable the input field
        SetInputEnabled(interactable);
         
        // (Smart) Update the button state
        if (!interactable)
            SetButtonEnabled(false);
        else
            UpdateButtonState();
    }

    /**
     * Get the login password from the text field.
     */
    public string GetInput() {
        return passFieldInput.text;
    }

    /**
     * Focus the input field.
     */
    public void FocusInput() {
        if (!(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)) {
            passFieldInput.ActivateInputField();
            passFieldInput.Select();
        }
    }

    /**
     * Set the text of the input field.
     */
    public void SetInput(string inputText) {
        passFieldInput.text = inputText;
    }

    /**
     * Set whether the input field is enabled.
     */
    private void SetInputEnabled(bool enabled) {
        passFieldInput.interactable = enabled;
    }

    /**
     * Set the text of the login button.
     */
    public void SetButton(string buttonText) {
        loginButtonText.text = buttonText;
    }

    /**
     * Set whether the button is enabled.
     */
    private void SetButtonEnabled(bool enabled) {
        loginButtonButton.interactable = enabled;
    }










    /**
     * Called when the value in the login input field is changed
     */
    public void OnPassValueChange() {
        UpdateButtonState();
    }

    /**
     * Called when the value in the login input field is changed
     */
    public void OnPassValueEnd()
    {
        // Shake the field if no pass is entered
        /*if (GetInput().Length <= 0)
            loginPanelAnimator.SetTrigger("shake");*/
    }

    /**
     * Update the state of the login button based on the pass field.
     */
    void UpdateButtonState() {
        // Set the button state based on the input
        SetButton(loginButtonTextDefault);
        SetButtonEnabled(GetInput().Length >= 0);
    }

    public void DoLogin() {
        // Make sure there's anything in the pass field
        bool anyPass = GetInput().Length > 0;

        if (!anyPass)
            loginPanelAnimator.SetTrigger("shake");

        else {
            SetComponentsInteractable(false);
            SetButton(loginButtonTextValidating);
            validatedAt = Time.time + validationDuration;
        }
    }
}
