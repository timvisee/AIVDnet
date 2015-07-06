using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;

public class PictureController : MonoBehaviour {

    public float startDelay = 0.0f;
    public float slideDelayMin = 0.15f;
    public float slideDelayMax = 0.5f;
    public float slideAmountMin = 0.02f;
    public float slideAmountMax = 0.08f;
    public float glitchStopDelay = 1.0f;
    public string picturesPath = "pics/";
    
    private Image imageComp;

    private float increaseAt = -1.0f;
    private float currentAmount = 0.0f;
    private float stopGlitchAt = -1.0f;

    public bool alwaysGlitch = false;

	// Use this for initialization
	void Start () {
	    // Get the image component
        imageComp = gameObject.GetComponent<Image>();

        // Prepare
        Prepare();
	}

    public void Prepare()
    {
        // Enable the glitching
        imageComp.material.SetFloat("_DispGlitchOn", 1);
        imageComp.material.SetFloat("_ColorGlitchOn", 1);

        // Set the fill amount to zero before starting
        imageComp.fillAmount = 0.0f;

        // Load the default picture
        LoadPicture("default");

        // Reset the variables
        increaseAt = -1.0f;
        currentAmount = 0.0f;
        stopGlitchAt = -1.0f;
    }

    public void LoadPicture(string id) {
        // Try to load the picture
        Texture2D pic;
        try
        {
            pic = (Texture2D)Resources.Load(picturesPath + id, typeof(Texture2D));
        } catch (System.Exception e) {
            pic = (Texture2D)Resources.Load(picturesPath + "default", typeof(Texture2D));
        }

        // Create a sprite from this texture
        Sprite sprite = Sprite.Create(pic, new Rect(0, 0, pic.width, pic.height), Vector2.zero, 100);

        // Set the picture
        imageComp.sprite = sprite;

        /*string path = "C:\\Users\\Tim\\Pictures\\Avatar\\512x512\\Avatar.png";
        byte[] data = File.ReadAllBytes(path);
        Texture2D texture = new Texture2D(512, 512, TextureFormat.ARGB32, false);
        texture.LoadImage(test.bytes);
        texture.name = Path.GetFileNameWithoutExtension(path);*/
    }
	
	// Update is called once per frame
	void Update () {
        if(increaseAt <= Time.time && increaseAt >= 0.0f) {
            currentAmount += Random.Range(slideAmountMin, slideAmountMax);

            increaseAt = Time.time + Random.Range(slideDelayMin, slideDelayMax);

            imageComp.fillAmount = currentAmount;
        }

        if (currentAmount >= 1.0f && stopGlitchAt < 0.0f)
        {
            stopGlitchAt = Time.time + glitchStopDelay;
        }

        if(stopGlitchAt <= Time.time && stopGlitchAt >= 0.0f) {
            if (!alwaysGlitch) {
                imageComp.material.SetFloat("_DispGlitchOn", 0);
                imageComp.material.SetFloat("_ColorGlitchOn", 0);
            }
            
        }
	}

    public void ShowPicture()
    {
        ShowPicture(startDelay);
    }

    public void ShowPicture(float delay)
    {
        // Set the start timer
        increaseAt = Time.time + delay;
    }
}
