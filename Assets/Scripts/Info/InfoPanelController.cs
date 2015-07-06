using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class InfoPanelController : MonoBehaviour {

    public GameObject infoPanel;
    public GameObject loginPanel;
    public GameObject agentManagerObject;
    public GameObject picture;
    public GameObject optionsButton;
    public GameObject credentialsButton;
    public GameObject logoutButton;
    public NoiseAndGrain noiseAndGrain;
    public GlitchEffect glitchEffect;
    public Vignetting vignetting;
    public ScreenOverlay screenOverlay;
    public Fisheye fisheye;
    public List<GameObject> labels;
    public List<GameObject> labelValues;
    public Material textGlitchMaterial;
    public int nameLabelIndex = 0;
    public int codeLabelIndex = 1;
    public int birthDateLabelIndex = 2;
    public int streetLabelIndex = 3;
    public int postCodeLabelIndex = 4;
    public int swimCertLabelIndex = 5;

    public float panelSpawnDelay = 1.0f;
    public float glitchStopDelay = 1.0f;
    public float firstCharDelay = 1.0f;
    public float pictureDelay = 1.5f;
    public int artifactCharCount = 5;
    public float nextCharDelayMin = 0.01f;
    public float nextCharDelayMax = 0.075f;
    public float nextLabelGlitchDelayMin = 0.03f;
    public float nextLabelGlitchDelayMax = 0.15f;
    public float artifactsSpawnDuration = 3.0f;
    public float artifactsDecayDuration = 0.5f;
    public float artifactsNoiseIntensityMultiplier = 2.0f;
    public float artifactsGlitchIntensity = 0.7f;
    public float artifactsVignettingChromAber = 5.0f;
    public float artifactsVignettingBlur = 0.3f;
    public float artifactsScreenOverlayIntensity = 0.5f;
    public float artifactsFisheyeStrengthX = 0.1f;
    public float artifactsFisheyeStrengthY = 0.1f;

    private Animator infoPanelAnimator;
    private LoginPanelController loginPanelController;
    private AgentManager agentManager;
    private Image pictureImage;
    private PictureController pictureController;
    private Button optionsButtonButton;
    private Button credentialsButtonButton;
    private Button logoutButtonButton;
    private List<Text> labelsText = new List<Text>();
    private List<Text> labelValuesText = new List<Text>();
    private List<string> labelsTextDefaults = new List<string>();
    private List<Text> labelsSpawn = new List<Text>();
    private List<string> labelsSpawnValues = new List<string>();
    private int labelCount = 0;
    private int labelCountTotal = 0;

    private float spawnPanelAt = -1.0f;
    private float updateGlitchedLabelsAt = 0.0f;
    float updateNextCharAt = -1.0f;

    private bool showArtifacts = false;
    private float artifactsTime = -1.0f;

    int curLabelIndex = 0;
    int curCharIndex = 0;

    private Hashtable removeMaterialTimed = new Hashtable();
    private List<Text> glitchedLabels = new List<Text>();

    private bool panelShown = false;

	/**
     * Initialize.
     */
    void Start() {
        // Get the picture and buttons instances
        infoPanelAnimator = infoPanel.GetComponent<Animator>();
        loginPanelController = loginPanel.GetComponent<LoginPanelController>();
        agentManager = agentManagerObject.GetComponent<AgentManager>();
        pictureImage = picture.GetComponent<Image>();
        pictureController = picture.GetComponent<PictureController>();
        optionsButtonButton = optionsButton.GetComponent<Button>();
        credentialsButtonButton = credentialsButton.GetComponent<Button>();
        logoutButtonButton = logoutButton.GetComponent<Button>();

        // Get the text components from the labels
        foreach(GameObject label in labels)
            labelsText.Add(label.GetComponent<Text>());
        foreach (GameObject label in labelValues)
            labelValuesText.Add(label.GetComponent<Text>());

        // Get the label count and total label count
        labelCount = Mathf.Max(labelsText.Count, labelValuesText.Count);
        labelCountTotal = labelsText.Count + labelValuesText.Count;

        // Store the default strings
        foreach (Text text in labelsText)
            labelsTextDefaults.Add(text.text);

        // Fill the labels spawn list
        for (int i = 0; i < labelCount; i++) {
            if (labelsText.Count > i)
                labelsSpawn.Add(labelsText[i]);
            if (labelValuesText.Count > i)
                labelsSpawn.Add(labelValuesText[i]);
        }

        // Store the text values
        foreach (Text text in labelsSpawn)
            labelsSpawnValues.Add(text.text);
	}

    void Update() {
        // Panel spawn delay timer
        if (spawnPanelAt <= Time.time && spawnPanelAt >= 0) {
            // Reset the timer
            spawnPanelAt = -1.0f;

            // Show the panel
            ShowPanel();
        }

        if (updateNextCharAt <= Time.time && updateNextCharAt >= 0.0f && curLabelIndex < labelsSpawn.Count) {
            // Create some variables to store the strings and label instances in
            Text label = labelsSpawn[curLabelIndex];
            string cur = "";
            string target = labelsSpawnValues[curLabelIndex];

            int targetLength = target.Length;

            bool glitchedData = false;
            if (target.Contains("?")) {
                glitchedData = true;
                targetLength = 10;

                if (!showArtifacts)
                    ShowArtifacts();
            }

            // Get the current label string part
            if (curCharIndex >= 0 && targetLength > 0 && !glitchedData)
                cur = target.Substring(0, curCharIndex + 1);
            curCharIndex++;

            // Generate some artifact characters
            if (targetLength > 0) {
                int artifactChars = Mathf.Min(Mathf.Min(target.Length - curCharIndex, artifactCharCount), curCharIndex + artifactCharCount);

                if (glitchedData)
                    artifactChars = targetLength;
                
                for (int i = 0; i < artifactChars; i++)
                    cur += (char) Random.Range(32, 126);
            }
            
            // Set the label text
            label.text = cur;

            // Check whether we should continue with the next label
            if (curCharIndex >= targetLength) {
                curLabelIndex++;
                curCharIndex = -artifactCharCount;

                if(!glitchedData)
                    removeMaterialTimed.Add(Time.time + glitchStopDelay, label);
                else
                    glitchedLabels.Add(label);
            }

            // Set the update timer
            updateNextCharAt = Time.time + Random.Range(nextCharDelayMin, nextCharDelayMax);
        }

        List<float> removeKeys = new List<float>();
        foreach (float time in removeMaterialTimed.Keys) {
            // Skip the element if it's time isn't reached yet
            if (time > Time.time)
                continue;

            // Remove the material
            Text text = (Text)removeMaterialTimed[time];
            text.material = null;

            // Add the element to the remove list
            removeKeys.Add(time);
        }

        // Remove the elements
        foreach (float key in removeKeys)
            removeMaterialTimed.Remove(key);

        // Update glitched labels
        if (updateGlitchedLabelsAt <= Time.time)
        {
            updateGlitchedLabelsAt = Time.time + Random.Range(nextLabelGlitchDelayMin, nextLabelGlitchDelayMax);
            
            foreach (Text label in glitchedLabels) {
                string text = "";
                for (int i = 0; i < Random.Range(8, 12); i++)
                    text += (char)Random.Range(32, 126);
                label.text = text;
            }
        }

        if (Input.GetAxis("Cancel") > 0 && panelShown)
            Logout();

        if (showArtifacts) {
            // Make sure the image effects are enabled
            noiseAndGrain.enabled = true;
            glitchEffect.enabled = true;
            vignetting.enabled = true;
            screenOverlay.enabled = true;
            fisheye.enabled = true;

            // Update the image effect values
            noiseAndGrain.intensityMultiplier = Mathf.Lerp(0.0f, artifactsNoiseIntensityMultiplier, (Time.time - artifactsTime) / artifactsSpawnDuration);
            glitchEffect.intensity = Mathf.Lerp(0.0f, artifactsGlitchIntensity, (Time.time - artifactsTime) / artifactsSpawnDuration);
            vignetting.chromaticAberration = Mathf.Lerp(0.0f, artifactsVignettingChromAber, (Time.time - artifactsTime) / artifactsSpawnDuration);
            vignetting.blur = Mathf.Lerp(0.0f, artifactsVignettingBlur, (Time.time - artifactsTime) / artifactsSpawnDuration);
            screenOverlay.intensity = Mathf.Lerp(0.0f, artifactsScreenOverlayIntensity, (Time.time - artifactsTime) / artifactsSpawnDuration);
            fisheye.strengthX = Mathf.Lerp(0.0f, artifactsFisheyeStrengthX, (Time.time - artifactsTime) / artifactsSpawnDuration);
            fisheye.strengthY = Mathf.Lerp(0.0f, artifactsFisheyeStrengthY, (Time.time - artifactsTime) / artifactsSpawnDuration);

        } else {
            // Update the image effect values
            noiseAndGrain.intensityMultiplier = Mathf.Lerp(artifactsNoiseIntensityMultiplier, 0.0f, (Time.time - artifactsTime) / artifactsDecayDuration);
            glitchEffect.intensity = Mathf.Lerp(artifactsGlitchIntensity, 0.0f, (Time.time - artifactsTime) / artifactsDecayDuration);
            vignetting.chromaticAberration = Mathf.Lerp(artifactsVignettingChromAber, 0.0f, (Time.time - artifactsTime) / artifactsDecayDuration);
            vignetting.blur = Mathf.Lerp(artifactsVignettingBlur, 0.0f, (Time.time - artifactsTime) / artifactsDecayDuration);
            screenOverlay.intensity = Mathf.Lerp(artifactsScreenOverlayIntensity, 0.0f, (Time.time - artifactsTime) / artifactsDecayDuration);
            fisheye.strengthX = Mathf.Lerp(artifactsFisheyeStrengthX, 0.0f, (Time.time - artifactsTime) / artifactsSpawnDuration);
            fisheye.strengthY = Mathf.Lerp(artifactsFisheyeStrengthY, 0.0f, (Time.time - artifactsTime) / artifactsSpawnDuration);

            // Disable the timer after a while
            if(artifactsTime + artifactsDecayDuration <= Time.time) {
                noiseAndGrain.enabled = false;
                glitchEffect.enabled = false;
                vignetting.enabled = false;
                screenOverlay.enabled = false;
                fisheye.enabled = false;
                artifactsTime = -1.0f;
            }
        }
    }

    public void ShowArtifacts()
    {
        // Set the flag
        showArtifacts = true;

        // Set the artifacts timer
        artifactsTime = Time.time;
    }

    public void HideArtifacts()
    {
        // Set the flag
        showArtifacts = false;

        // Set the artifacts timer
        artifactsTime = Time.time;
    }

    public void Prepare() {
        updateNextCharAt = -1.0f;
        curLabelIndex = 0;
        curCharIndex = 0;

        // Clear all labels and set the proper material
        foreach (Text text in labelsSpawn)
        {
            text.text = "";
            text.material = textGlitchMaterial;
        }

        // Clear the list of glitched labels
        glitchedLabels.Clear();
    }

    /**
     * Show the main panel.
     */
    public void ShowPanel()
    {
        // Set the shown flag
        panelShown = true;

        // Trigger the spawn animation
        infoPanelAnimator.SetTrigger("spawn");

        // Prepare
        Prepare();

        // Update the current agent data if available
        if(agentManager.GetCurrentAgent() != null)
            SetDataFromAgent(agentManager.GetCurrentAgent());

        // Show the data
        ShowData();
    }

    /**
     * Hide the main panel.
     */
    public void HidePanel()
    {
        // Trigger the spawn animation
        infoPanelAnimator.SetTrigger("decay");

        /* // Reset the input field text
        SetInput("");

        // Enable all controls
        SetComponentsInteractable(true);

        // Focus the input field
        FocusInput();*/

        // Set the panel shown flag
        panelShown = false;
    }

    public void ShakePanel() {
        // Trigger the shake animation
        infoPanelAnimator.SetTrigger("shake");
    }

    public void SetDataFromAgent(Agent agent) {
        // Determine whether we should always glitch
        bool alwaysGlitch = agent.GetId().Equals("goederick");

        // Set the label values
        labelsSpawnValues[1] = agent.GetName();
        labelsSpawnValues[3] = agent.GetCode();
        labelsSpawnValues[5] = agent.GetBirthday();
        labelsSpawnValues[7] = agent.GetStreet();
        labelsSpawnValues[9] = agent.GetPostalCode();
        labelsSpawnValues[11] = agent.GetSwimCert();

        // Set the picture
        pictureController.Prepare();
        pictureController.LoadPicture(agent.GetId());
        pictureController.alwaysGlitch = alwaysGlitch;
    }

    public void ShowData() {
        // Set some timers
        updateNextCharAt = Time.time + firstCharDelay;
        curCharIndex = -artifactCharCount;
        pictureController.ShowPicture(pictureDelay);
    }

    public void Logout() {
        // Hide the current panel
        HidePanel();

        // Show the login panel
        loginPanelController.ShowPanel();

        // Reset the current agent
        agentManager.SetCurrentAgent("");

        // Hide any artifacts
        HideArtifacts();
    }
}
