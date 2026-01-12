using System;
using UnityEngine;
using UnityEngine.Video;
using Code.Scripts.Checkpoint;

namespace Code.Scripts.Level.Interactables
{
    public class InteractableTV : MonoBehaviour, IInteractable, ISaveable
    {
        [SerializeField] private string _id;
        public string id => string.IsNullOrEmpty(_id) ? gameObject.name : _id;
        
        public VideoPlayer VideoPlayer;
        public AudioSource AudioSource;
        public AudioClip TurnOnOffClip;

        public static bool TvOn;
        
        private string _videoName = "Noticia.mp4";

        private void Awake()
        {
            if (VideoPlayer != null && !string.IsNullOrEmpty(_videoName))
            {
                var path = System.IO.Path.Combine(Application.streamingAssetsPath, _videoName);
                VideoPlayer.source = VideoSource.Url;
                VideoPlayer.url = path;
            }
        }

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
            data.interactableStates.TryGetValue(id, out TvOn);
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