using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Code.Scripts.Player
{
    public class OpenCloseDoor : MonoBehaviour 
	{
		public float smooth = 2.0f;
		private float doorOpenAngle = -90.0f;
		private float doorCloseAngle = 0.0f;
		private float pickUpRange = 3f;
		private AudioSource asource;
		public AudioClip openDoor, closeDoor;
		private Transform door;
		private Quaternion targetRotation;
		private bool opened = false;
		private bool moving = false;

		void Update () 
		{
			if (moving && door != null)
            {
                door.localRotation = Quaternion.Slerp(door.localRotation, targetRotation, Time.deltaTime * 5 * smooth);
				if (Quaternion.Angle(door.localRotation, targetRotation) < 0.5f)
				{
					door.localRotation = targetRotation;
					moving = false;
				}
            }
		}

		public void OnDoor(InputValue value)
		{
			if (value.isPressed)
			{
				Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * pickUpRange, Color.red, 1f);

				if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, pickUpRange))
				{
					if (hit.collider.CompareTag("Door"))
					{
						door = hit.collider.transform;
						if (!opened)
							OpenDoor();
						else
							CloseDoor();
					}
					else
					{
						Debug.Log("Objeto no es una puerta: " + hit.collider.tag);
					}
				}
			}
		}

		private void OpenDoor()
		{
			moving = true;
            targetRotation = Quaternion.Euler(0f, doorOpenAngle, 0f);
			DoorSound();
		}
		
		private void CloseDoor()
		{
			moving = true;
			targetRotation = Quaternion.Euler(0f, doorCloseAngle, 0f);
			DoorSound();
		}

		public void DoorSound()
		{
			opened = !opened;
			asource = door.GetComponent<AudioSource>();
			if (asource != null)
			{
				asource.clip = opened ? openDoor : closeDoor;
				asource.Play();
			}
            else
            {
                Debug.LogWarning("No se encontró AudioSource en la puerta.");
            }
			
		}
	}
}
