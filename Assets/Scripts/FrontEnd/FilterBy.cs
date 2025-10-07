using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FilterBy : MonoBehaviour
{
    public enum FilterType
    {
        CardClass,
        CardRarity
    }

    [SerializeField]
    private FilterType filterType;
    [SerializeField]
    private TMP_Dropdown dropdown;

    void Awake()
    {
        dropdown = GetComponent<TMP_Dropdown>();
        dropdown.ClearOptions();
        List<string> options;
        string[] choices = null;
        if (filterType == FilterType.CardClass)
        {
            choices = System.Enum.GetNames(typeof(CardClass));
        }
        else if (filterType == FilterType.CardRarity)
        {
            choices = System.Enum.GetNames(typeof(CardRarity));
        }
        options = new List<string>(choices);
        options.Insert(0, "All");
        dropdown.AddOptions(options);
    }

    void OnEnable()
    {
        dropdown.value = 0;
        dropdown.RefreshShownValue();
    }
}
