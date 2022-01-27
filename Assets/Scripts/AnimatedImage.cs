using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimatedImage : MonoBehaviour
{
    [SerializeField] RawImage rawImage;
    [SerializeField] float delayBetweenImages = 1f;

    [SerializeField] bool canRun = false;
    int currentImageID = 0;

    public void StartAnimation()
    {
        canRun = true;
        StartCoroutine(StartAnimationEnumerator());
    }

    IEnumerator StartAnimationEnumerator()
    {
        while(canRun)
        {
            rawImage.texture = AppController.instance.pictures[currentImageID];

            yield return new WaitForSeconds(delayBetweenImages);

            if(currentImageID == AppController.instance.pictures.Count - 1)
            {
                currentImageID = 0;
            }
            else
            {
                currentImageID++;
            }

        }

    }
}
