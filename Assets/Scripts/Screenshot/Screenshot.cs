using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screenshot : MonoBehaviour
{
    public KeyCode screenShotButton;
    public int resolutionWidth = 1920; // Default resolution width
    public int resolutionHeight = 1080; // Default resolution height
    public float screenshotDelay = 0.1f; // Delay before taking screenshot

    void Update()
    {
        if (Input.GetKeyDown(screenShotButton))
        {
            // Set the desired resolution
            Screen.SetResolution(resolutionWidth, resolutionHeight, false);

            // Start coroutine to capture screenshot after delay
            StartCoroutine(TakeScreenshotAfterDelay());
        }
    }

    IEnumerator TakeScreenshotAfterDelay()
    {
        // Wait for a short delay to ensure the resolution change has taken effect
        yield return new WaitForSeconds(screenshotDelay);

        // Capture screenshot
        ScreenCapture.CaptureScreenshot("screenshot.png");
        Debug.Log("A screenshot was taken!");

        // Optionally, reset the resolution to its original values
        // Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, false);
    }
}
