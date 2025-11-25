using UnityEngine;
using UnityEngine.Video;
using Code.Scripts.Checkpoint;

namespace Code.Scripts.Level.Interactables
{
    public class InteractableTV : MonoBehaviour, IInteractable, ISaveable
    {
        [SerializeField] private string _id;
        public string id => _id;
        
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
        
        public void Save(GameStateData data)
        {
            data.interactableStates[id] = TvOn;
        }
        
        public void Load(GameStateData data)
        {
            if (data.interactableStates.ContainsKey(id))
            {
                TvOn = data.interactableStates[id];

                if (TvOn)
                {
                    VideoPlayer.targetMaterialRenderer.enabled = true;
                    VideoPlayer.isLooping = true;
                    VideoPlayer.Play();
                }
                else
                {
                    VideoPlayer.Pause();
                    VideoPlayer.targetMaterialRenderer.enabled = false;
                }
            }
        }
    }
}