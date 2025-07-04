using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class ButtonSound : MonoBehaviour
{
    public AudioClip clickSound;
    public UnityEvent onClick; // Aquí metes la acción real del botón

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
        }
    }

    public void PlaySoundAndExecute()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }

        onClick?.Invoke();
    }
}