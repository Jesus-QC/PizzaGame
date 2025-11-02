using UnityEngine;
using UnityEngine.Video;

namespace Code.Scripts.Level.Interactables
{
    public class InteractableTV : MonoBehaviour, IInteractable
    {
        public VideoPlayer VideoPlayer;
        public AudioSource AudioSource;
        public AudioClip TurnOnOffClip;
        private bool _isOn;

        public bool IsOn
        {
            get => _isOn;
            set
            {
                _isOn = value;
                if (_isOn) 
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