using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeviceOption : MonoBehaviour
{
    [SerializeField] Button button;
    public TMP_Text buttonText;

    public void InitialSetup(int currentDevice, string deviceName)
    {
        buttonText.text = deviceName;

        button.onClick.AddListener(() => {
            SelectDeviceOption(currentDevice);
        });
    }

    public void SelectDeviceOption(int i)
    {
        AppController.instance.SetCameraDevice(i);
    }
}
