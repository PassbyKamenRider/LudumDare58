using UnityEngine;

public class CardInspect : MonoBehaviour
{
    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            float scaleFactor = 0.33f;
            Vector3 targetScale = transform.localScale + Vector3.one * scroll * scaleFactor;
            if (targetScale.x >= 1.0f && targetScale.x <= 1.5f)
                transform.localScale = targetScale;
        }
    }
}
