using Assets.Code.Scripts.Player;
using UnityEngine;

public class KillingZoneTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Body")
        {
            PlayerController.Instance.OnkilledByEnemy();
        }
    }
}
