using UnityEngine;
using TMPro;
using System.Collections;

public class TypeWriter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] GameObject canvas;

    void Start()
    {
        StartCoroutine(TypeText());
    }

    private IEnumerator TypeText()
    {
        text.ForceMeshUpdate();

        int totalVisibleCharacters = text.textInfo.characterCount;
        int counter = 0;

        while (counter <= totalVisibleCharacters)
        {
            text.maxVisibleCharacters = counter;
            counter++;
            yield return new WaitForSeconds(typingSpeed);
        }
        
        canvas.SetActive(false);
        text.gameObject.SetActive(false);
    }
}