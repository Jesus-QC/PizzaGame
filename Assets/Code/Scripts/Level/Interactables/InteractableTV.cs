using UnityEngine;
using UnityEngine.Video;

namespace Code.Scripts.Level.Interactables
{
    public class InteractableTV : MonoBehaviour, IInteractable
    {
        public static bool TvOn;

        public VideoPlayer VideoPlayer;
        public AudioSource AudioSource;
        public AudioClip TurnOnOffClip;

        public bool IsOn
        {
            get => TvOn;
            set
            {
                TvOn = value;
                if (TvOn) 
                    On();
                else 
                    Off();
                
            }
        }

        public void Interact()
        {
            IsOn = !IsOn;
        }

        private void On()
        {
            AudioSource.PlayOneShot(TurnOnOffClip);

            VideoPlayer.targetMaterialRenderer.enabled = true;
            VideoPlayer.isLooping = true;
            VideoPlayer.Play();
        }

        private void Off()
        {
            VideoPlayer.Pause();
            VideoPlayer.targetMaterialRenderer.enabled = false;
            
            AudioSource.PlayOneShot(TurnOnOffClip);
        }
    }
}