using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Level Clips")]
    [SerializeField] AudioClip _winLevelClip;
    [SerializeField] AudioClip _endGameClip;
    [SerializeField] AudioClip _loseLevelClip;

    [Header("Audio Sources")]
    [SerializeField] AudioSource _hoverSource;
    [SerializeField] AudioSource _clickSource;
    AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayLevelWin()
    {
        _audioSource.PlayOneShot(_winLevelClip);
    }

    public void PlayLevelLose()
    {
        _audioSource.PlayOneShot(_loseLevelClip);
    }

    public void PlayGameEnd()
    {
        _audioSource.PlayOneShot(_endGameClip);
    }

    public void PlayHover()
    {
        _hoverSource.Play();
    }

    public void PlayContinue()
    {
        _clickSource.Play();
    }
}
