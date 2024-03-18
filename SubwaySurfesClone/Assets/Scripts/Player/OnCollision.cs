using UnityEngine;
using UnityEngine.TextCore.Text;

public class OnCollision : MonoBehaviour
{
    public PlayerController m_char;

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("Player"))
        {
            return;
        }
        m_char.OnCharacterColliderHit(other.collider);
    }
}