using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DismissElements : MonoBehaviour
{
    [SerializeField] List<CanvasGroup> canvasGroups = new List<CanvasGroup>();

    public void Dismiss()
    {
        foreach(CanvasGroup element in canvasGroups)
        {
            LeanTween.alphaCanvas(element, 0, 0.35f).setEaseInOutCubic().setOnStart(()=>{
                element.interactable = false;
                element.blocksRaycasts = false;
            });
        }
    }
}
