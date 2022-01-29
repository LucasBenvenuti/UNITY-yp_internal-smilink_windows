using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NatSuite.Recorders;
using NatSuite.Recorders.Clocks;

public class AnimatedImage : MonoBehaviour
{
    [SerializeField] RenderTexture renderTexture;

    [SerializeField] RawImage rawImage;
    [SerializeField] float delayBetweenImages = 1f;

    [SerializeField] bool canRun = false;
    int currentImageID = 0;

    MP4Recorder mp4Recorder;
    private IClock clock;
    string pathFinished;

    Texture2D readbackTexture;

    public void StartAnimation()
    {
        currentImageID = 0;

        clock = new RealtimeClock();
        
        mp4Recorder = new MP4Recorder(1280, 800, 1);

        canRun = true;
        StartCoroutine(StartAnimationEnumerator());
    }

    IEnumerator StartAnimationEnumerator()
    {
        while(canRun)
        {
            rawImage.texture = AppController.instance.pictures[currentImageID];

            yield return new WaitForSeconds(delayBetweenImages);

            readbackTexture = new Texture2D(renderTexture.width, renderTexture.height);
            RenderTexture.active = renderTexture;

            readbackTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            RenderTexture.active = null;

            mp4Recorder.CommitFrame(readbackTexture.GetPixels32(), clock.timestamp);

            if(currentImageID == AppController.instance.pictures.Count - 1)
            {
                canRun = false;

                yield return new WaitForSeconds(delayBetweenImages);

                mp4Recorder.CommitFrame(readbackTexture.GetPixels32(), clock.timestamp);

                CallFinish();

                currentImageID = 0;
            }
            else
            {
                currentImageID++;
            }

        }
    }

    async void CallFinish()
    {
        pathFinished = await mp4Recorder.FinishWriting();
        Debug.Log(pathFinished);

        StartCoroutine(UploadStart(pathFinished));
    }

    IEnumerator UploadStart(string path)
    {
        yield return new WaitForSeconds(1f);

        // AWS3.instance.UploadFileToAWS3("videoTest.mp4", path);
    }
}
