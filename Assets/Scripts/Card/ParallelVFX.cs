using UnityEngine;

public class ParallelVFX : MonoBehaviour
{
    public RectTransform holoRect;
    public float speed = 200f;

    public Vector2 startPos;
    public Vector2 endPos;

    void Start()
    {
        holoRect.anchoredPosition = startPos;
    }

    void Update()
    {
        holoRect.anchoredPosition += (endPos - startPos).normalized * speed * Time.deltaTime;

        if ((holoRect.anchoredPosition - endPos).sqrMagnitude < 2 * speed * Time.deltaTime)
        {
            holoRect.anchoredPosition = startPos;
        }
    }
}
