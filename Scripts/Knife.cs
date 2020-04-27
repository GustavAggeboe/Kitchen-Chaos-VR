using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Dette script sørger for at en kniv har egenskaben til at kunne skære.
public class Knife : MonoBehaviour
{
    // Disse tre punkter er punkter på kniven, som bruges til at findes krydsproduktet af retningen som kniven skal skære.
    public Transform pointA;
    public Transform pointB;
    public Transform pointC;
    // Denne markere punktet hvor kniven sidst skar i mad. Man skal derefter flytte kniven væk, længere end "cutAgainThreshold", for at skære igen. Dette -
    // forhindrer at kniven skærer mange gange, lige efter hinanden.
    Vector3 lastCutPos;
    public float cutAgainThreshold = 5;
    public bool canCutAgain;
    public bool pickedUp;
    // Når noget rammer knivens trigger, som sidder ved det skarpe stykke på kniven. 
    private void OnTriggerEnter(Collider other) {
        // Hvis tingen der rammer kniven har tagget "CutableFood".
        if (other.CompareTag("CutableFood") && canCutAgain && pickedUp) {
            // Da skæringen foregår med en usynlig plan, udregner vi hvor midten af planen er, og hvad normalvektoren er til planen er.
            // Midten ligger i midten af det skarpe på kniven.
            Vector3 centre = (pointA.position + pointB.position) / 2;
            // Normalvektoren er vektoren for krydsproduktet mellem de tre punkter i planen, med længden 1.
            Vector3 up = Vector3.Cross(pointA.position - centre, pointC.position - centre).normalized;
            // Vi beder nu cutter scriptet om at udregne, hvordan maden skal skæres.
            Cutter.Cut(other.gameObject, centre, up, null, true, true);
            // Der kan nu ikke skæres igen, før kniven er flyttet.
            canCutAgain = false;
            lastCutPos = transform.position;
        }
    }

    private void Update() {
        // Når kniven er langt nok væk fra hvor den sidst skar, kan den skære igen.
        if (Vector3.Distance(lastCutPos, transform.position) > cutAgainThreshold) {
            canCutAgain = true;
        }
    }

    // Når kniven bliver samlet op eller lagt, bliver disse funktioner kaldt.
    public void PickedUp () {
        pickedUp = true;
    }
    public void Detached() {
        pickedUp = false;
    }


    // Resten af koden sørger for, at i FixedUpdate bliver knivens position, velocity og angular velocity gemt, så når kniven rammer maden, vil den ikke blive påvirket af collisions.
    string ignoreTag = "CutableFood";
    Vector3 lastPosition;
    Vector3 lastVelocity;
    Vector3 lastAngularVelocity;
    void FixedUpdate() {
        lastPosition = transform.position;
        lastVelocity = GetComponent<Rigidbody>().velocity;
        lastAngularVelocity = GetComponent<Rigidbody>().angularVelocity;
    }
    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag(ignoreTag)) {
            transform.position = lastPosition;
            GetComponent<Rigidbody>().velocity = lastVelocity;
            GetComponent<Rigidbody>().angularVelocity = lastAngularVelocity;
            Physics.IgnoreCollision(GetComponent<MeshCollider>(), other.collider);
        }
    }
}
