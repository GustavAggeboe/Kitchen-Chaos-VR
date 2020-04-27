using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Dette script bliver brugt til at fixe nogle physics.
// Når der er mad i en beholder, starter maden som et child af beholderen, for at følge beholderen, når den bliver flyttet på.
public class FoodContainer : MonoBehaviour {
    public List<GameObject> contentList = new List<GameObject>();

    // Når maden forlader beholderen, bliver de fjernet som child af beholderen.
    private void OnTriggerExit(Collider other) {
        if (contentList.Contains(other.gameObject)) {
            other.transform.parent = null;
            contentList.Remove(gameObject);
        }
    }
}
