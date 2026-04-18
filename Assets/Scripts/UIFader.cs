using System.Collections;
using System.Reflection.Metadata;
using UnityEngine;
using UnityEngine.UI;

public class UIFader : MonoBehaviour
{
    Image image;

    public void FadeToBlack()
    {
        image = GetComponent<Image>();
        image.gameObject.SetActive(true);
        StartCoroutine(FadeToBlackRoutine());
    }

    IEnumerator FadeToBlackRoutine()
    {
        image.color = new Color(0, 0, 0, 0);
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime;
            image.color = new Color(0, 0, 0, t);
            yield return null;
        }
        image.gameObject.SetActive(false);
    }

    public void FadeFromBlack()
    {
        image = GetComponent<Image>();
        image.gameObject.SetActive(true);
        StartCoroutine(FadeFromBlackRoutine());
    }

    IEnumerator FadeFromBlackRoutine()
    {
        image.color = new Color(0, 0, 0, 1);
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime;
            image.color = new Color(0, 0, 0, 1 - t);
            yield return null;
        }
        image.gameObject.SetActive(false);
    }


}
