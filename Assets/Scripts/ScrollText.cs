// https://gist.github.com/unitycoder/19625fed364a39cb278f

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScrollText : MonoBehaviour
{
    public float InitialDelay = 0.1f;
    public float CharWait = 0.5f;
    public float CommaWait = 0.3f;
    public float PeriodWait = 0.5f;

    TextMeshProUGUI textMesh;
    string text;

    void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        text = textMesh.text;
        textMesh.text = "";
        StartCoroutine(TypeText());
    }

    IEnumerator TypeText()
    {
        yield return new WaitForSeconds(InitialDelay);
        foreach (char c in text)
        {
            textMesh.text += c;
            yield return new WaitForSeconds(CharWait);
            if (c == ',') yield return new WaitForSeconds(CommaWait);
            if (c == '.') yield return new WaitForSeconds(PeriodWait);
            if (c == '\n') yield return new WaitForSeconds(PeriodWait * 2);
        }
    }
}