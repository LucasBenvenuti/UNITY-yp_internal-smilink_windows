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

    IEnumerator TakePhoto()
    {
     yield return new WaitForEndOfFrame(); 

        Texture2D photo = new Texture2D(webcamTexture.width, webcamTexture.height);
        photo.SetPixels(webcamTexture.GetPixels());
        photo.Apply();
        
        byte[] bytes = photo.EncodeToPNG();

        AppController.instance.LoadSpriteFromBytes(bytes);

        Debug.Log("PRINTED");
    }
}
