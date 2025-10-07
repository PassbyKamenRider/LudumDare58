using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [SerializeField] AudioSource cardAudio;
    [SerializeField] AudioSource clickAudio;
    [SerializeField] AudioSource fAudio;
    [SerializeField] AudioSource dealAudio;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayCardAudio()
    {
        cardAudio.Play();
    }

    public void PlayClickAudio()
    {
        clickAudio.Play();
    }

    public void PlayFAudio()
    {
        fAudio.Play();
    }

    public void PlayDealAudio()
    {
        dealAudio.Play();
    }
}
