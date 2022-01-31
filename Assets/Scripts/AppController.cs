using System.IO;
using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class AppController : MonoBehaviour
{
    public static AppController instance;

    [SerializeField] List<CanvasGroup> appPhases = new List<CanvasGroup>();
    [SerializeField] int startAppPhase = 0;
    [SerializeField] int currentAppPhase = 0;

    public int totalImagesForVideo = 5;
    public List<Texture2D> pictures = new List<Texture2D>();
    public int currentImageID = 0;
    [SerializeField] int startCounterFrom = 3;
    [SerializeField] int currentCounter = 0;
    [SerializeField] TMP_Text counterTextElement;
    [SerializeField] CanvasGroup counterTextBox;
    [SerializeField] WebcamBehavior webcamElement;
    [SerializeField] AnimatedImage animatedImage;
    public TMP_Text poseText;
    public CanvasGroup poseTextBox;
    [SerializeField] CanvasGroup fadeImage;
    public TMP_Text loadingText;

    public TMP_Text fullySavedText;

    public int currentDeviceInUse = 0;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }

        for(int i = 0; i < appPhases.Count; i++)
        {
            if(i != startAppPhase)
            {
                appPhases[i].alpha = 0f;
                appPhases[i].interactable = false;
                appPhases[i].blocksRaycasts = false;
            }
            else
            {
                appPhases[i].alpha = 1f;
                appPhases[i].interactable = true;
                appPhases[i].blocksRaycasts = true;
            }
        }

        counterTextBox.alpha = 0f;
        counterTextBox.interactable = false;
        counterTextBox.blocksRaycasts = false;

        poseTextBox.alpha = 0f;
        poseTextBox.interactable = false;
        poseTextBox.blocksRaycasts = false;

        fadeImage.alpha = 1f;
        fadeImage.interactable = true;
        fadeImage.blocksRaycasts = true;
    }

    void Start()
    {
        LeanTween.value(gameObject, 0f, 1f, 1f).setOnComplete(()=> {
            LeanTween.alphaCanvas(fadeImage, 0f, 0.5f).setEaseInOutCubic().setOnComplete(()=> {
                fadeImage.interactable = false;
                fadeImage.blocksRaycasts = false;
            });
        });

        currentCounter = startCounterFrom;

        currentDeviceInUse = PlayerPrefs.GetInt("DeviceInUse");

        CleanPersistentData();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.C) && currentAppPhase == 0)
        {
            if(currentAppPhase == 0)
            {
                GoToAppPhase(3);   
            }
            else
            {
                Debug.Log("Você precisa estar na etapa inicial do jogo para abrir o menu de configurações");
            }
        }        
    }

    public void GoToAppPhase(int phaseID)
    {
        if(phaseID == 1)
        {
            webcamElement.StartCamera();
        }
        else if(phaseID == 2)
        {
            loadingText.text = "Estamos criando seu vídeo para\ncompartilhamento";
        }
        else if(phaseID == 3)
        {
            webcamElement.StartConfigCamera();
        }

        LeanTween.value(gameObject, 0f, 1f, 1f).setOnComplete(()=>{

            appPhases[currentAppPhase].interactable = false;
            appPhases[currentAppPhase].blocksRaycasts = false;

            LeanTween.alphaCanvas(appPhases[currentAppPhase], 0, 0.5f).setEaseInOutCubic().setOnStart(()=>{});

            currentAppPhase = phaseID;

            LeanTween.alphaCanvas(appPhases[currentAppPhase], 1, 0.5f).setEaseInOutCubic().setOnStart(()=>{
                
                
            }).setOnComplete(()=>{
                
                appPhases[currentAppPhase].interactable = true;
                appPhases[currentAppPhase].blocksRaycasts = true;

                if(currentAppPhase == 1)
                {
                    StartCounter();
                }
                else if(currentAppPhase == 0)
                {
                    webcamElement.StopCamera();
                    webcamElement.StopConfigCamera();
                }
                else if(currentAppPhase == 4)
                {
                    webcamElement.StopCamera();
                }
                
            });

        });
    }

    public void StartCounter()
    {
        StartCoroutine(StartCounterEnumerator());
    }

    IEnumerator StartCounterEnumerator()
    {
        poseText.text = "Prepare sua pose " + (currentImageID + 1) + " / " + totalImagesForVideo;

        if(currentImageID == 0)
        {
            LeanTween.alphaCanvas(poseTextBox, 1f, 0.5f).setEaseInOutCubic().setOnComplete(()=> {});

            yield return new WaitForSeconds(2f);
        }
        else
        {
            LeanTween.alphaCanvas(poseTextBox, 1f, 0.5f).setEaseInOutCubic().setOnComplete(()=> {});
        }

        LeanTween.alphaCanvas(counterTextBox, 1f, 0.5f).setEaseInOutCubic();

        yield return new WaitForSeconds(0.5f);

        while(currentCounter > 0)
        {
            counterTextElement.text = currentCounter.ToString();
            
            yield return new WaitForSeconds(1f);
            
            currentCounter--;
        }

        Debug.Log("End Counter");

        LeanTween.alphaCanvas(counterTextBox, 0f, 0.2f).setEaseInOutCubic().setOnComplete(()=> {
            currentCounter = startCounterFrom;
            counterTextElement.text = currentCounter.ToString();
        });

        LeanTween.alphaCanvas(poseTextBox, 0f, 0.2f).setEaseInOutCubic().setOnComplete(()=> {});

        yield return new WaitForSeconds(0.4f);

        webcamElement.StartTakePhoto();
        // webcamElement.StartWebcamCapture();
    }

    public void CleanPersistentData()
    {
        string[] filePaths = Directory.GetFiles(Application.persistentDataPath);

        foreach(string filePath in filePaths)
        {
            File.Delete(filePath);
        }
    }

    public void LoadSprite(string path)
    {
        if (string.IsNullOrEmpty(path))
            Debug.Log("Caminho vazio ou nulo");

        if (System.IO.File.Exists(path))
        {
            byte[] bytes = File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(1280, 800, TextureFormat.RGB24, false);
            texture.filterMode = FilterMode.Trilinear;
            texture.LoadImage(bytes);

            pictures.Add(texture);

            currentImageID++;

            if(currentImageID < totalImagesForVideo)
            {
                StartCounter();
            }
            else
            {
                GenerateVideo();
            }
        }
    }

    public void LoadSpriteFromBytes(byte[] byteArray)
    {
        if (byteArray.Length == 0)
        {
            Debug.Log("Byte vazio ou nulo");

            return;
        }

        Texture2D texture = new Texture2D(1280, 800, TextureFormat.RGB24, false);
        texture.filterMode = FilterMode.Trilinear;
        texture.LoadImage(byteArray);

        pictures.Add(texture);

        currentImageID++;

        if(currentImageID < totalImagesForVideo)
        {
            StartCounter();
        }
        else
        {
            GenerateVideo();
        }
    }

    public void GenerateVideo()
    {
        Debug.Log("CAPTURE HAS ENDED");

        GoToAppPhase(2);

        animatedImage.StartAnimation();
    }

    public void SetCameraDevice(int i)
    {
        Debug.Log(i);
        

        if(i < webcamElement.devicesList.Count)
        {
            currentDeviceInUse = i;
            
            webcamElement.webcamConfigTexture.Stop();
            webcamElement.webcamConfigTexture.deviceName = webcamElement.devicesList[currentDeviceInUse];
            webcamElement.webcamConfigTexture.Play();

            PlayerPrefs.SetInt("DeviceInUse", currentDeviceInUse);
        }
        else
        {
            Debug.Log("No device detected!");
        }
    }

    public void RestartScene()
    {
        LeanTween.alphaCanvas(fadeImage, 1f, 0.5f).setEaseInOutCubic().setOnStart(()=> {
            fadeImage.interactable = false;
            fadeImage.blocksRaycasts = false;
        }).setOnComplete(()=> {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });
    }
}
