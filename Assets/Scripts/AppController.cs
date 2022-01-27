using System.IO;
using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
        }

        counterTextBox.alpha = 0f;
        counterTextBox.interactable = false;
        counterTextBox.blocksRaycasts = false;
    }

    void Start()
    {
        currentCounter = startCounterFrom;

        CleanPersistentData();
    }

    public void GoToAppPhase(int phaseID)
    {
        webcamElement.StartCamera();

        LeanTween.value(gameObject, 0f, 1f, 1f).setOnComplete(()=>{
            LeanTween.alphaCanvas(appPhases[currentAppPhase], 0, 0.5f).setEaseInOutCubic().setOnStart(()=>{
                appPhases[currentAppPhase].interactable = false;
                appPhases[currentAppPhase].blocksRaycasts = false;
            });

            currentAppPhase = phaseID;

            LeanTween.alphaCanvas(appPhases[currentAppPhase], 1, 0.5f).setEaseInOutCubic().setOnStart(()=>{
                
                
            }).setOnComplete(()=>{
                
                appPhases[currentAppPhase].interactable = true;
                appPhases[currentAppPhase].blocksRaycasts = true;

                if(currentAppPhase == 1)
                {
                    StartCounter();
                }
                else if(currentAppPhase == 2)
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
        if(currentImageID == 0)
            yield return new WaitForSeconds(2f);

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

        yield return new WaitForSeconds(0.4f);

        webcamElement.StartWebcamCapture();
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

    public void GenerateVideo()
    {
        Debug.Log("CAPTURE HAS ENDED");

        GoToAppPhase(2);

        animatedImage.StartAnimation();
    }
}
