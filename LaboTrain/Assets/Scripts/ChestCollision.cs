using UnityEngine;

public class ChestCollision : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.isTrigger && other.gameObject.CompareTag("Player"))
            GameManager.Instance.EndWinPlayer();
    }
}
