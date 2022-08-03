using Amazon.S3.Model;
using UnityEngine;
using UnityEngine.UI;
using ZXing;
using ZXing.QrCode;

namespace AWSSDK.Examples
{
    public class AppManager_Custom : MonoBehaviour
    {
        #region VARIABLES 

        [Header("Infos")]
        [SerializeField] private string S3BucketName;
        [Tooltip("Folder name with / at the end. E.G smilink_test/")] [SerializeField] private string folderNameOnBucket;

        [SerializeField] RawImage qrCodeImage;
        Texture2D encoded;

        #endregion

        #region METHODS MONOBEHAVIOUR

        void Start()
        {
            encoded = new Texture2D(256, 256);

            S3Manager.Instance.OnResultGetObject += GetObjectBucket;
        }

        #endregion

        #region METHODS CREATED

        private void ListBuckets()
        {
            // resultTextOperation.text = "Fetching all the Buckets";

            S3Manager.Instance.ListBuckets((result, error) =>
            {
                // resultTextOperation.text += "\n";

                if (string.IsNullOrEmpty(error))
                {
                    // resultTextOperation.text += "Got Response \nPrinting now \n";

                    result.Buckets.ForEach((bucket) =>
                    {
                        // resultTextOperation.text += string.Format("bucket = {0}\n", bucket.BucketName);
                    });
                }
                else
                {
                    print("Get Error:: " + error);
                    // resultTextOperation.text += "Got Exception \n";
                }
            });
        }

        private void ListObjectsBucket()
        {
            // resultTextOperation.text = "Fetching all the Objects from " + S3BucketName;

            S3Manager.Instance.ListObjectsBucket(S3BucketName, (result, error) =>
            {
                // resultTextOperation.text += "\n";
                if (string.IsNullOrEmpty(error))
                {
                    // resultTextOperation.text += "Got Response \nPrinting now \n";
                    result.S3Objects.ForEach((file) =>
                    {
                        // resultTextOperation.text += string.Format("File: {0}\n", file.Key);
                    });
                }
                else
                {
                    print("Get Error:: " + error);
                    // resultTextOperation.text += "Got Exception \n";
                }
            });
        }

        private void GetObjectBucket(GetObjectResponse resultFinal = null, string errorFinal = null)
        {
            // resultTextOperation.text = string.Format("fetching {0} from bucket {1}", folderNameOnBucket + fileNameOnBucket, S3BucketName);

            if(errorFinal != null)
            {
                // resultTextOperation.text += "\n";
                // resultTextOperation.text += "Get Data Error";
                print("Get Error:: " + errorFinal);
                return;
            }
        }

        private void UploadObjectForBucket(string pathFile, string S3BucketName, string fileNameOnBucket)
        {
            AppController.instance.loadingText.text = "Processando vídeo";

            Debug.Log("Retrieving the file");
            Debug.Log("\nCreating request object");
            Debug.Log("\nMaking HTTP post call");

            S3Manager.Instance.UploadObjectForBucket(pathFile, S3BucketName, fileNameOnBucket, (result, error) =>
            {
                if(string.IsNullOrEmpty(error))
                {
                    Debug.Log("\nUpload Success");

                    string completePathToQRCode = "https://s3." + S3Manager.Instance.s3Region + ".amazonaws.com/" + S3BucketName + "/" + fileNameOnBucket;

                    Debug.Log(completePathToQRCode);

                    Btn_CreatQr(completePathToQRCode);

                    //TROCAR TELA AQUI
                }
                else
                {
                    Debug.Log("\nUpload Failed");
                    Debug.LogError("Get Error:: " + error);
                }
            });
        }

        private void DeleteObjectOnBucket()
        {
        }

        public void UploadFileToAWS(string completeFilePath, string nameOnBucket)
        {
            UploadObjectForBucket(completeFilePath, S3BucketName, folderNameOnBucket + nameOnBucket);
        }

        #endregion

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

        public void Btn_CreatQr(string path)
        {
            Debug.Log("CREATING QRCODE");

            //QR code write picture    
            var color32 = Encode(path, encoded.width, encoded.height);
            encoded.SetPixels32(color32);
            encoded.Apply();
        //The generated QR code image is attached to RawImage
            qrCodeImage.texture = encoded;

            AppController.instance.GoToAppPhase(4);
        }
    }
}