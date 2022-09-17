using System.Collections.Generic;
using UnityEngine;

public class EntitySounds : MonoBehaviour
{
    [SerializeField] private bool _walkClipNotMoving = false;
    [SerializeField] private List<AudioClip> _walkingClips;

    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }
    public void PlayWalk()
    {
        _audioSource.PlayOneShot(_walkingClips[Random.Range(0, _walkingClips.Count)]);
    }
}
