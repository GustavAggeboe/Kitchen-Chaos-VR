using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Dette script sørger for at tallerkener bliver flyttet på rullebåndet, for at maden kan blive bedømt.
public class Rullebånd : MonoBehaviour {
    // Hastigheden af rullebåndet
    public float speed = 30.0f;

    void OnCollisionStay(Collision collision) {
        // Sålænge et objekt med tagget "Plate" er på båndet
        if (collision.gameObject.CompareTag("Plate")) {
            //Debug.Log("mvoe plate");
            // Tallerkenen bliver tildelt en hastighed baseret på retningen "transform.forward".
            float conveyorVelocity = speed * Time.deltaTime;
            Rigidbody rigidbody = collision.gameObject.GetComponent<Rigidbody>();
            // Tallerkenens rigidbody bliver tildelt hastigheden.
            rigidbody.velocity = conveyorVelocity * transform.forward;
        }
    }
}
