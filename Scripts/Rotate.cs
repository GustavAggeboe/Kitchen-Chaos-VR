using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Dette script bliver ikke brugt mere. Det var til at teste nogle physics. Scriptet gør så et objekt roterer på x-aksen mod (-90 + angle), hvor angle bliver sat i editoren.
public class Rotate : MonoBehaviour
{
    public float angle;
    public float speed;

    void Update() {
        angle += Time.deltaTime * speed;
        transform.rotation = Quaternion.Euler(-90 + angle, 0, 0);
    }
}
