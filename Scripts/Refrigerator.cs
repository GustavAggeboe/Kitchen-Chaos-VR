using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Dette script gør at maden bliver kold, når det er i køleskabet. Scriptet kan også bruges til fryseren, men fryseren blev ikke nødvendig at gøre færdig, i forhold til, hvad vi kunne nå med dette projekt.
public class Refrigerator : MonoBehaviour
{
    // Hvad er det koldeste køleskabet kan være
    public float startingTemperature = 2; // Fryser skal være -18
    // Dette er den konkrete temperatur af køleskabet, som kan ændre sig.
    public float temperature;
    // Denne varibel holder informationer om køleskabsdøren.
    public GameObject door;

    // Ved starten af spillet skal køleskabet være koldest
    void Start() {
        temperature = startingTemperature;
    }

    void Update()
    {
        // Hvis døren er mere end 3 grader åben
        if (door.transform.localEulerAngles.y < 357f && door.transform.localEulerAngles.y > 90) {
            // Køleskabets temperatur går mod 20 grader (stuetemperatur).
            temperature += (20 - temperature) * Time.deltaTime / 10;            
        } else {
            // Hvis døren er lukket, skal temperaturen i køleskabet gå mod 2 grader.
            temperature += (2 - temperature) * Time.deltaTime / 10;
        }
    }

    // Denne bliver kaldt hver frame der er noget i køleskabet.
    public void OnTriggerStay(Collider other) {
        // Hvis det er mad
        if (other.GetComponent<Food>() != null) {
            // Setup
            Food food = other.GetComponent<Food>();
            // Til at starte med skal maden have skiftet temperatur, som om det har ligget i køleskabet længe.
            if (!food.hasBeenSetup) {
                food.temperature = temperature;
                food.hasBeenSetup = true;
            } else {
                // Hvis det ikke er starten af spillet, skal maden have 'surroundings temperature' til køleskabets temperatur.
                // Inde i food scriptet, vil madens temperatur så gå mod 'surroundings temperature'.
                food.surTemperature = temperature;
            }
        }
    }
}
