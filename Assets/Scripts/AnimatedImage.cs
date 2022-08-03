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
    [SerializeField] float numberOfRepeats = 3f;

    [SerializeField] bool canRun = false;
    // [SerializeField] VideoPlayer frameAnimation;
    [SerializeField] AudioSource audioSource;
    int currentRepeatCount = 1;
    int currentImageID = 0;

    [SerializeField] float frameRateVideo = 24f;

    [SerializeField] List<CanvasGroup> dismissedCanvasGroup = new List<CanvasGroup>();
    [SerializeField] CanvasGroup fadeEffect;

    MP4Recorder mp4Recorder;
    private IClock clock;
    string pathFinished;
    Texture2D readbackTexture;

    bool videoPlayed;

    void Start()
    {
        fadeEffect.alpha = 0f;
        fadeEffect.interactable = false;
        fadeEffect.blocksRaycasts = false;
    }

    public void StartAnimation()
    {
        currentImageID = 0;

        clock = new RealtimeClock();
        
        mp4Recorder = new MP4Recorder(1080, 1920, frameRateVideo, 48000, 2);

        canRun = true;
        StartCoroutine(StartAnimationEnumerator());
    }

    IEnumerator StartAnimationEnumerator()
    {

        StartCoroutine(StartRecordEnumerator());

        while(canRun)
        {
            // frameAnimation.Play();
            FlashEffect();
            audioSource.Play();

            yield return new WaitForSeconds(delayBetweenImages);

            if(currentImageID == AppController.instance.pictures.Count - 1)
            {
                if(currentRepeatCount == numberOfRepeats)
                {
                    canRun = false;

                    for(int i = 0; i < dismissedCanvasGroup.Count; i++)
                    {
                        dismissedCanvasGroup[i].interactable = false;
                        dismissedCanvasGroup[i].blocksRaycasts = false;

                        LeanTween.alphaCanvas(dismissedCanvasGroup[i], 0, 0.5f);
                    }

                    // frameAnimation.Stop();
                    audioSource.Stop();

                    yield return new WaitForSeconds(delayBetweenImages);

                    CallFinish();

                    currentImageID = 0;
                    currentRepeatCount = 0;
                }
                else
                {
                    currentRepeatCount++;

                    currentImageID = 0;
                }
            }
            else
            {
                // frameAnimation.Stop();
                audioSource.Stop();

                currentImageID++;
            }

        }
    }

    IEnumerator StartRecordEnumerator()
    {
        while(canRun)
        {
            rawImage.texture = AppController.instance.pictures[currentImageID];

            yield return new WaitForSeconds(1f / frameRateVideo);

            readbackTexture = new Texture2D(renderTexture.width, renderTexture.height);
            RenderTexture.active = renderTexture;

            readbackTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            RenderTexture.active = null;
            rawImage.color = Color.white;
            mp4Recorder.CommitFrame(readbackTexture.GetPixels32(), clock.timestamp);

        }
    }

    public void FlashEffect()
    {
        LeanTween.alphaCanvas(fadeEffect, 0.8f, 0.1f).setEaseInCirc().setOnComplete(()=> {
            LeanTween.alphaCanvas(fadeEffect, 0f, 0.1f).setEaseOutCirc().setOnComplete(()=> {});
        });
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

    void OnAudioFilterRead(float[] sampleBuffer, int channels)
    {
        if (mp4Recorder != null && canRun)
        {
            mp4Recorder.CommitSamples(sampleBuffer, clock.timestamp);
        }
    }
}
