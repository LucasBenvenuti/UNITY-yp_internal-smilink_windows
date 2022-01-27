using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.WebCam;

public class WebcamBehavior : MonoBehaviour
{
    //START WEBCAM PICTURE
    PhotoCapture photoCaptureObject = null;
    static readonly int TotalImagesToCapture = 1;
    int capturedImageCount = 0;
    //END WEBCAM PICTURE

    [SerializeField] RawImage rawImage;

    WebCamTexture webcamTexture;

    string filePath;

    public void StartCamera()
    {
        webcamTexture = new WebCamTexture();
        rawImage.texture = webcamTexture;
        rawImage.material.mainTexture = webcamTexture;
        webcamTexture.Play();
    }

    public void StopCamera()
    {
        webcamTexture.Stop();
    }

    public void StartWebcamCapture()
    {
        Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
        Texture2D targetTexture = new Texture2D(cameraResolution.width, cameraResolution.height);

        PhotoCapture.CreateAsync(false, delegate(PhotoCapture captureObject) {
            Debug.Log("Created PhotoCapture Object");
            photoCaptureObject = captureObject;

            CameraParameters c = new CameraParameters();
            c.hologramOpacity = 0.0f;
            c.cameraResolutionWidth = targetTexture.width;
            c.cameraResolutionHeight = targetTexture.height;
            c.pixelFormat = CapturePixelFormat.BGRA32;

            captureObject.StartPhotoModeAsync(c, delegate(PhotoCapture.PhotoCaptureResult result) {
                Debug.Log("Started Photo Capture Mode");
                
                StopCamera();

                TakePicture();
            });
        });
    }

    void TakePicture()
    {
        Debug.Log(string.Format("Taking Picture ({0}/{1})...", AppController.instance.currentImageID + 1, AppController.instance.totalImagesForVideo));
        string filename = string.Format(@"CapturedImage{0}.jpg", AppController.instance.currentImageID);
        filePath = System.IO.Path.Combine(Application.persistentDataPath, filename);

        photoCaptureObject.TakePhotoAsync(filePath, PhotoCaptureFileOutputFormat.JPG, OnCapturedPhotoToDisk);
    }

    void OnCapturedPhotoToDisk(PhotoCapture.PhotoCaptureResult result)
    {
        Debug.Log("Saved Picture To Disk! - " + result);

        photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
    }

    void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
    {
        photoCaptureObject.Dispose();
        photoCaptureObject = null;

        Debug.Log("Captured images have been saved at the following path.");
        Debug.Log(Application.persistentDataPath);

        AppController.instance.LoadSprite(filePath);

        StartCamera();
    }
}
