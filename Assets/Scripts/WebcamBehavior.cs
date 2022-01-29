using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
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

    public WebCamTexture webcamTexture;
    public WebCamTexture webcamConfigTexture;

    public List<string> devicesList = new List<string>();

    string filePath;

    [SerializeField] RawImage rawImage_Config;

    [SerializeField] GameObject deviceButtonsContainer;
    [SerializeField] DeviceOption deviceButtonPrefab;

    void Start()
    {
        webcamTexture = new WebCamTexture();
        webcamConfigTexture = new WebCamTexture();

        WebCamDevice[] devices = WebCamTexture.devices;
        for (int i = 0; i < devices.Length; i++)
        {
            devicesList.Add(devices[i].name);
            Debug.Log(devices[i].name);

            DeviceOption deviceOption = Instantiate(deviceButtonPrefab, deviceButtonsContainer.transform);
            deviceOption.InitialSetup(i, devicesList[i]);
        }
    }

    public void StartCamera()
    {
        webcamTexture = new WebCamTexture();
        rawImage.texture = webcamTexture;
        webcamTexture.deviceName = devicesList[AppController.instance.currentDeviceInUse];
        rawImage.material.mainTexture = webcamTexture;
        webcamTexture.Play();
    }

    public void StartConfigCamera()
    {
        webcamConfigTexture = new WebCamTexture();
        rawImage_Config.texture = webcamConfigTexture;
        webcamConfigTexture.deviceName = devicesList[AppController.instance.currentDeviceInUse];
        rawImage_Config.material.mainTexture = webcamConfigTexture;
        webcamConfigTexture.Play();
    }

    public void StopCamera()
    {
        webcamTexture.Stop();
    }

    public void StopConfigCamera()
    {
        webcamConfigTexture.Stop();
    }

    public void StartTakePhoto()
    {
        StartCoroutine(TakePhoto());
    }

    IEnumerator TakePhoto()  // Start this Coroutine on some button click
    {

    // NOTE - you almost certainly have to do this here:

     yield return new WaitForEndOfFrame(); 

    // it's a rare case where the Unity doco is pretty clear,
    // http://docs.unity3d.com/ScriptReference/WaitForEndOfFrame.html
    // be sure to scroll down to the SECOND long example on that doco page 

        Texture2D photo = new Texture2D(webcamTexture.width, webcamTexture.height);
        photo.SetPixels(webcamTexture.GetPixels());
        photo.Apply();

        //Encode to a PNG
        byte[] bytes = photo.EncodeToPNG();

        AppController.instance.LoadSpriteFromBytes(bytes);

        Debug.Log("PRINTED");
    }

    // public void StartWebcamCapture()
    // {
    //     Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
    //     Texture2D targetTexture = new Texture2D(cameraResolution.width, cameraResolution.height);

    //     PhotoCapture.CreateAsync(false, delegate(PhotoCapture captureObject) {
    //         Debug.Log("Created PhotoCapture Object");
    //         photoCaptureObject = captureObject;

    //         CameraParameters c = new CameraParameters();
    //         c.hologramOpacity = 0.0f;
    //         c.cameraResolutionWidth = targetTexture.width;
    //         c.cameraResolutionHeight = targetTexture.height;
    //         c.pixelFormat = CapturePixelFormat.BGRA32;

    //         captureObject.StartPhotoModeAsync(c, delegate(PhotoCapture.PhotoCaptureResult result) {
    //             Debug.Log("Started Photo Capture Mode");
                
    //             StopCamera();

    //             TakePicture();
    //         });
    //     });
    // }

    // void TakePicture()
    // {
    //     Debug.Log(string.Format("Taking Picture ({0}/{1})...", AppController.instance.currentImageID + 1, AppController.instance.totalImagesForVideo));
    //     string filename = string.Format(@"CapturedImage{0}.jpg", AppController.instance.currentImageID);
    //     filePath = System.IO.Path.Combine(Application.persistentDataPath, filename);

    //     photoCaptureObject.TakePhotoAsync(filePath, PhotoCaptureFileOutputFormat.JPG, OnCapturedPhotoToDisk);
    // }

    // void OnCaptureToMemoryCallback(PhotoCapture.PhotoCaptureResult result)
    // {
    //     Debug.Log("Saved Picture! - " + result);

    //     // photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
    // }

    // void OnCapturedPhotoToDisk(PhotoCapture.PhotoCaptureResult result)
    // {
    //     Debug.Log("Saved Picture To Disk! - " + result);

    //     photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
    // }

    // void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
    // {
    //     photoCaptureObject.Dispose();
    //     photoCaptureObject = null;

    //     Debug.Log("Captured images have been saved at the following path.");
    //     Debug.Log(Application.persistentDataPath);

    //     AppController.instance.LoadSprite(filePath);

    //     StartCamera();
    // }
}
