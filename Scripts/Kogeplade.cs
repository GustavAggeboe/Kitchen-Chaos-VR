using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Dette script er til pladen som man kan varme mad på.
public class Kogeplade : MonoBehaviour
{
    // Temperaturen er til at holde styr på hvor meget maden skal steges.
    public float temperature = 20;
    // Denne float bliver ændret med det samme, når håndtaget er sat til en temperatur. Derefter nærmer "temperature" sig "desired temperature".
    public float desiredTemperature;
    // Dette objekt er den so man drejer på, når man vil skifte temperatur.
    public GameObject temperatureKnob;
    // Denne float holder styr på "temperatureKnob"'s rotation, som kan blive konverteret om til en "desired temperature".
    public float knobRotation;

    void Update()
    {
        // Knob rotation
        knobRotation = temperatureKnob.transform.localEulerAngles.y;

        // Desired temperatur findes
        if (knobRotation > 1) { // Kogeplade er tændt.
            // Desired temperatur findes ved at tage rotationen af håndtaget, som bliver remappet fra 0-360grader til 50-250grader celcius. Denne funktion har vi lavet, og ligger i bunden af scriptet "food".
            desiredTemperature = knobRotation.Remap(0, 360, 50, 250);
        } else {
            // Kogeplade er slukket, så temperatur skal være stuetemperatur.
            desiredTemperature = 20;
        }

        // Temperaturskift
        float temperatureChangeSpeed = 5f;
        // Hvis den desired temperatur er mindre end temperaturen, skal ændringen i temperatur blive negativ.
        if (desiredTemperature - temperature < 0) {
            temperatureChangeSpeed = -temperatureChangeSpeed;
        }
        // Ændre temperaturen, så den nærmer sig desiredTemperature.
        temperature += temperatureChangeSpeed * Time.deltaTime;
    }
}


