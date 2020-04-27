using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Dette script er en hurtigt implementering af at starte spillet forfra.
public class RestartButton : MonoBehaviour {
    // Hvis restart button rammer den trigger som dette script er forbundet til, skal scenen startes forfra.
    private void OnTriggerEnter(Collider other) {
        if (other.name == "RestartButton") {
            Debug.Log("Restart game");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
