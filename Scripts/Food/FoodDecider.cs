using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Dette script er tilegnet enden af rullebåndet, hvor den færdige ret bliver placeret.
public class FoodDecider : MonoBehaviour {
    // Dette starter som en tom liste, som er en liste over alle de maddele som skal vurderes.
    List<GameObject> foodToSend = new List<GameObject>();
    // Når et objekt kommer ind i triggeren
    private void OnTriggerEnter(Collider other) {
        // Hvis det er mad
        if (other.GetComponent<Food>()) {
            // så tilføj objektet til listen over mad der skal vurderes
            foodToSend.Add(other.gameObject);
        }
    }
    // Når en objekt forlader triggeren
    private void OnTriggerExit(Collider other) {
        // Hvis dette er tallerkenen, så ved vi at al maden er blevet scannet ind, som lå på tallerkenen, da det kun er tallerkenen der bevæger sig
        if (other.gameObject.CompareTag("Plate")) {
            // Vi sender maden der skal vurderes til GameManager, som vurderer retten i sin funktion "FoodResult()".
            GameManager.i.FoodResult(foodToSend);
            // Vi sletter indholdet i listen, så der er klar til endnu en omgang at blive sendt.
            foodToSend.Clear();
        }
    }
}
