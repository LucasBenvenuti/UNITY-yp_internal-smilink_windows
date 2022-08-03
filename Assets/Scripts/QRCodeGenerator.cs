using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZXing;
using ZXing.QrCode;

public class QRCodeGenerator : MonoBehaviour
{
    //Display the QR code on the screen  
    [SerializeField] RawImage image;
    //Store QR code  
    Texture2D encoded;

    // Start is called before the first frame update
    void Start()
    {
        encoded = new Texture2D(256, 256);
    }

    /// <summary>
    ///  Define method to generate QR code 
    /// </summary>
    /// <param name="textForEncoding">Need to produce the string of QR code</param>
    /// <param name="width">width</param>
    /// <param name="height">high</param>
    /// <returns></returns>       
    private static Color32[] Encode(string textForEncoding, int width, int height)
    {
        var writer = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Height = height,
                Width = width
            }
        };
       return writer.Write(textForEncoding);
    }

   /// <summary>  
    ///  Generate QR code  
   /// </summary>  
    public void Btn_CreatQr(string path)
    {
        Debug.Log("CREATING QRCODE");

        //QR code write picture    
        var color32 = Encode(path, encoded.width, encoded.height);
        encoded.SetPixels32(color32);
        encoded.Apply();
       //The generated QR code image is attached to RawImage
        image.texture = encoded;

        AppController.instance.GoToAppPhase(4);
    }
}
