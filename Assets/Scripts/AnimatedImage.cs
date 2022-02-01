using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NatSuite;
using NatSuite.Recorders;
using NatSuite.Recorders.Clocks;
using AWSSDK.Examples;
using UnityEngine.Video;

public class AnimatedImage : MonoBehaviour
{
    [SerializeField] AppManager_Custom appManager_Custom;
    [SerializeField] RenderTexture renderTexture;

    [SerializeField] RawImage rawImage;
    [SerializeField] float delayBetweenImages = 1f;

    [SerializeField] bool canRun = false;
    [SerializeField] VideoPlayer frameAnimation;
    int currentImageID = 0;

    [SerializeField] List<CanvasGroup> dismissedCanvasGroup = new List<CanvasGroup>();

    MP4Recorder mp4Recorder;
    private IClock clock;
    string pathFinished;
    Texture2D readbackTexture;

    bool videoPlayed;

    public void StartAnimation()
    {
        currentImageID = 0;

        clock = new RealtimeClock();
        
        mp4Recorder = new MP4Recorder(1080, 1920, 24);

        canRun = true;
        StartCoroutine(StartAnimationEnumerator());
    }

    IEnumerator StartAnimationEnumerator()
    {

        StartCoroutine(StartRecordEnumerator());

        while(canRun)
        {
            // rawImage.texture = AppController.instance.pictures[currentImageID];
            frameAnimation.Play();

            yield return new WaitForSeconds(delayBetweenImages);

            // readbackTexture = new Texture2D(renderTexture.width, renderTexture.height);
            // RenderTexture.active = renderTexture;

            // readbackTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            // RenderTexture.active = null;

            // mp4Recorder.CommitFrame(readbackTexture.GetPixels32(), clock.timestamp);

            if(currentImageID == AppController.instance.pictures.Count - 1)
            {
                canRun = false;

                for(int i = 0; i < dismissedCanvasGroup.Count; i++)
                {
                    dismissedCanvasGroup[i].interactable = false;
                    dismissedCanvasGroup[i].blocksRaycasts = false;

                    LeanTween.alphaCanvas(dismissedCanvasGroup[i], 0, 0.5f);
                }

                frameAnimation.Stop();

                yield return new WaitForSeconds(delayBetweenImages);

                // mp4Recorder.CommitFrame(readbackTexture.GetPixels32(), clock.timestamp);


                CallFinish();

                currentImageID = 0;
            }
            else
            {
                frameAnimation.Stop();

                currentImageID++;
            }

        }
    }

    // void Update () {
    //         // Record frames from the webcam
    //         if (canRun && webCamTexture.didUpdateThisFrame) {
    //             webCamTexture.GetPixels32(pixelBuffer);
    //             recorder.CommitFrame(pixelBuffer, clock.timestamp);
    //         }
    //     }

    IEnumerator StartRecordEnumerator()
    {
        while(canRun)
        {
            rawImage.texture = AppController.instance.pictures[currentImageID];

            yield return new WaitForSeconds(1f / 24f);

            readbackTexture = new Texture2D(renderTexture.width, renderTexture.height);
            RenderTexture.active = renderTexture;

            readbackTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            RenderTexture.active = null;
            rawImage.color = Color.white;

            mp4Recorder.CommitFrame(readbackTexture.GetPixels32(), clock.timestamp);

            // if(currentImageID == AppController.instance.pictures.Count - 1)
            // {
            //     canRun = false;
            //     // frameAnimation.Stop();

            //     yield return new WaitForSeconds(1f / 24f);

            //     mp4Recorder.CommitFrame(readbackTexture.GetPixels32(), clock.timestamp);


            //     // CallFinish();

            //     // currentImageID = 0;
            // }
            // else
            // {
            //     // currentImageID++;
            // }

        }
    }

    async void CallFinish()
    {
        pathFinished = await mp4Recorder.FinishWriting();
        Debug.Log(pathFinished);

        string[] completePathArray = pathFinished.Split('/');
        string[] finalPathArray = completePathArray[completePathArray.Length - 1].Split('\\');
        string fileName = finalPathArray[finalPathArray.Length - 1];

        appManager_Custom.UploadFileToAWS(pathFinished, fileName);
    }

    IEnumerator UploadStart(string path)
    {

        yield return new WaitForSeconds(1f);

        // AWS3.instance.UploadFileToAWS3("videoTest.mp4", path);
    }
}
