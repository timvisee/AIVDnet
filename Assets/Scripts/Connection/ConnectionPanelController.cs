using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ConnectionPanelController : MonoBehaviour {

    public GameObject connectionPanel;
    public InfoPanelController infoPanelController;
    public GameObject progressIndicatorObject;
    public Text statusText;
    public string statusPreparing = "Voorbereiden...";
    public string statusConnecting = "Verbinden...";
    public string statusValidating = "Verifiëren...";
    public string statusSuccess = "Verbonden";
    public string statusFail = "Verbinding onveilig!";
    public float prepareDuration = 1.5f;
    public float intervalMin = 0.1f;
    public float intervalMax = 0.4f;
    public float amountMin = 0.05f;
    public float amountMax = 0.2f;
    public float amountFrom = 0.0f;
    public float amountTo = 1.0f;
    public float amountVerify = 0.6f;
    public float updateSpeed = 0.2f;
    public float updateFailChance = 0.025f;
    public float updateFailFrom = 0.05f;
    public float failDelay = 2.0f;
    public float successDelay = 0.5f;

    private Animator animator;
    private Image progressIndicatorImage;
    private Animator progressIndicatorAnimator;

    private float nextUpdateAt = -1.0f;
    private float amount = 0.0f;
    private float amountCurrent = 0.0f;
    private float waitFailUntil = -1.0f;
    private bool failed = false;
    private float successAt = -1.0f;

	// Use this for initialization
	void Start () {
	    // Get all component instances
        animator = connectionPanel.GetComponent<Animator>();
        progressIndicatorImage = progressIndicatorObject.GetComponent<Image>();
        progressIndicatorAnimator = progressIndicatorObject.GetComponent<Animator>();
    }

    void Update() {
        // Wait a while if the progress failed
        if (waitFailUntil > Time.time)
            return;

        if (successAt <= Time.time && successAt >= 0.0f) {
            // Reset the timer
            successAt = -1.0f;

            // Hide the current panel, show the info panel
            HidePanel();
            infoPanelController.ShowPanel();
            Stop();
        }

        // Check whether we should update the amount
        if (nextUpdateAt <= Time.time && nextUpdateAt >= 0.0f && successAt < 0.0f)
        {
            // Update the amount
            if (!failed) {
                // Increase the amount
                amount += Random.Range(amountMin, amountMax);

                // Check whether this update should fail
                if (updateFailChance >= Random.Range(0.0f, 1.0f) && !failed && amount >= updateFailFrom) {
                    // Flash the failure and reset the amount
                    FlashFailure();
                    amount = amountFrom;

                    // Set the fail timer and flag to true
                    waitFailUntil = Time.time + failDelay;
                    failed = true;

                    // Set the status text and return
                    SetStatusText(statusFail);
                    return;
                }
            } else {
                // Reset the amount and the fail flag
                amount = 0.0f;
                failed = false;
            }

            // Set the status text
            if (amount < amountVerify)
                SetStatusText(statusConnecting);
            else
                SetStatusText(statusValidating);

            // Set the update timer
            nextUpdateAt = Time.time + Random.Range(intervalMin, intervalMax);

            // Check whether the progress is completed
            if (amount >= amountTo && successAt < 0.0f)
            {
                // Disable the update timer
                nextUpdateAt = -1.0f;

                // Flash the success state of the progress indicator
                FlashSuccess();

                // Set the status text
                SetStatusText(statusSuccess);

                // Set the success timer
                successAt = Time.time + successDelay;
            }
        }

        // Lerp the progress indicator value
        amountCurrent = Mathf.Lerp(amountCurrent, amount, updateSpeed);
        SetAmount(amountCurrent);
    }

    /**
     * Prepare the panel.
     */
    public void Prepare() {
        // Set and apply the amount to zero
        amount = amountFrom;
        amountCurrent = amountFrom;
        SetAmount(amount);

        // Set the status text
        SetStatusText(statusPreparing);

        // Set the update timer
        nextUpdateAt = Time.time + prepareDuration;
    }

    public void Stop() {
        // Set and apply the amount to zero
        /*amount = amountFrom;
        amountCurrent = amountFrom;*/

        // Set the update timer
        nextUpdateAt = -1.0f;
    }

    /**
     * Show the main panel.
     */
    public void ShowPanel() {
        // Prepare the panel
        Prepare();

        // Trigger the spawn animation
        animator.SetTrigger("spawn");
    }

    /**
     * Hide the main panel.
     */
    public void HidePanel() {
        // Trigger the decay animation
        animator.SetTrigger("decay");
    }

    /**
     * Set the visual progress indicator amount.
     */
    private void SetAmount(float amount) {
        progressIndicatorImage.fillAmount = amount;
    }

    /**
     * Set the status text.
     */
    private void SetStatusText(string text) {
        statusText.text = text;
    }

    private void FlashSuccess() {
        progressIndicatorAnimator.SetTrigger("success");
    }

    private void FlashFailure()
    {
        progressIndicatorAnimator.SetTrigger("fail");
    }
}
