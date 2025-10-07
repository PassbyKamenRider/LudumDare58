using UnityEngine;

public class SetSelfIconByClass : MonoBehaviour
{
    [SerializeField]
    private UnityEngine.UI.Image selfImage;
    void Awake()
    {
        
        if (selfImage == null)
        {
            selfImage = GetComponent<UnityEngine.UI.Image>();
        }
    }
    void Start()
    {
        string itemName = transform.parent.name;
        string[] nameParts = itemName.Split(':');
        string className = nameParts[1].Trim();
        Sprite classSprite = Resources.Load<Sprite>($"KeyedIcons/{className}");
        if (classSprite != null)
        {
            selfImage.sprite = classSprite;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
