using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Dette script sørger for at lave en bøf om til en hakkebøf.
public class Grinder : MonoBehaviour {
    // Hvor hakkebøffen skal laves
    public Transform outputPosition;
    // hakkebøf prefab som bliver lavet
    public GameObject choppedSteakPrefab;
    // Når et objekt bliver placeret på hakkerens input.
    private void OnTriggerEnter(Collider other) {
        // Hvis objektet er mad og har ID'et steak
        if (other.GetComponent<Food>().ID == "steak") {
            // Fjern bøffen
            Destroy(other.gameObject);
            // Lav en hakkebøf lidt under outputPosition
            Instantiate(choppedSteakPrefab, new Vector3(outputPosition.position.x, outputPosition.position.y - .05f, outputPosition.position.z), choppedSteakPrefab.transform.rotation);
        }
    }
}
