using Assets.Code.Scripts.Player;
using UnityEngine;

namespace Code.Scripts.Level.Interactables
{
    public class InteractableLadder: MonoBehaviour
    {
        public GameObject WorldModel;
        public GameObject ViewModel;
        private bool _isPlaced;

        public bool IsPlaced
        {
            get => _isPlaced;
            set
            {
                _isPlaced = value;
            }
        } 
        
        void Start()
        {
            IsPlaced = false;
        }

        public void StepSignal(Vector3 stepPosition)
        {
            if (IsPlaced)
                return;
                
            IsPlaced = true;
            InteractableItem item = GetComponent<InteractableItem>();
            if (item != null)
            {
                item.OnDropped();
                item.enabled = false;
            }
            PlayerController.Instance.ItemsController.HeldObject = null;

            if (WorldModel) WorldModel.SetActive(false);
            if (ViewModel) ViewModel.SetActive(false);

            Collider col = gameObject.GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = true;
            }
            
            MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();
            if (mr != null)
            {
                mr.enabled = true;
            }

            //transform.position = new Vector3(2.36f, 4.18f, 7.11f);
            transform.position = new Vector3(stepPosition.x - 0.05f, stepPosition.y + 4.1565f, stepPosition.z - 0.5f);
            transform.rotation = Quaternion.Euler(-102.68f, 0, 0);

            SetLayerRecursively(gameObject, LayerMask.NameToLayer("Ignore Raycast"));

        }
        private static void SetLayerRecursively(GameObject obj, int newLayer)
        {
            obj.layer = newLayer;
            foreach (Transform child in obj.transform)
            {
                SetLayerRecursively(child.gameObject, newLayer);
            }
        }
    }

}

