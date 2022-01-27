using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NatSuite.Recorders;
using NatSuite.Recorders.Clocks;

public class RecordTest : MonoBehaviour
{
    [SerializeField] RawImage rawImage;
    public List<Texture2D> pictures = new List<Texture2D>();
    [SerializeField] bool canRun = false;
    [SerializeField] float delayBetweenImages = 1f;

    int currentImageID = 0;

    private IClock clock;

    string pathFinished;

    MP4Recorder mp4Recorder;

    void Start()
    {
        StartTest();
    }

    public void StartTest()
    {
        clock = new RealtimeClock();

        mp4Recorder = new MP4Recorder(1280, 800, 1);
        StartAnimation();
    }

    public void StartAnimation()
    {
        canRun = true;
        StartCoroutine(StartAnimationEnumerator());
    }

    IEnumerator StartAnimationEnumerator()
    {
        while(canRun)
        {

            Texture2D textureTest = new Texture2D(1280, 800);

            // textureTest.LoadImage(pictures[currentImageID].);

            // rawImage.texture = textureTest;

            // Debug.Log(textureTest.GetPixels32());
            // Debug.Log(clock.timestamp);

            // mp4Recorder.CommitFrame(textureTest.GetPixels32(), clock.timestamp);

            yield return new WaitForSeconds(delayBetweenImages);

            if(currentImageID == pictures.Count - 1)
            {
                canRun = false;

                // currentImageID = 0;
                CallFinish();
            }
            else
            {
                currentImageID++;
            }

        }

    }

    async void CallFinish()
    {
        // pathFinished = await mp4Recorder.FinishWriting();

        // Debug.Log(pathFinished);
    }
}
