using UnityEngine;

public class SpikeTrigger : MonoBehaviour
{


    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision detected with: " + collision.gameObject.name);
    }
}
