using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// Pointen med GameManageren er at holde styr på spillets tilstand - om hvad man skal lave af mad, regne point ud og lign.
public class GameManager : MonoBehaviour
{
    // Lav en static GameManager
    public static GameManager i;
    // Holder styr på, hvad spilleren holder, og sidst holdte.
    public GameObject itemHolding;
    public GameObject lastHoldItem;
    // UI som spilleren bliver vist, når de anbringer mad på transportbåndet. Kræver "using TMPro" i toppen.
    public TextMeshProUGUI pointsText;

    // Når spillet starter, sørg for der kun er én GameManager i spil
    private void Awake() {
        if (GameManager.i != null) {
            Destroy(gameObject);
        } else {
            i = this;
        }
    }

    // Denne funktion bliver kaldt af transportbåndet, når mad kommer igennem. Funktionen bliver givet en liste over al det mad som skal vurderes.
    public void FoodResult(List<GameObject> foodToJudge) {
        // Vi laver en variabel til at have roden af maden - parent over hele retten.
        GameObject baseFood = new GameObject();
        // Vi går igennem al maden
        foreach (GameObject food in foodToJudge) {
            // Det mad som ikke har nogen parent, må være roden af maden.
            if (food.transform.parent == null) {
                // Dette mad er nu sat som baseFood.
                baseFood = food;
            }
        }
        // Vi sætter værdier op til at sikre at vi har alle dele til burgeren.
        bool hasBreadTop = false;
        bool hasBreadBottom = false;
        bool hasBeef = false;

        // Tjek om den har alle dele.
        foreach (GameObject objects in foodToJudge) {
            Food food = objects.GetComponent<Food>();
            if (food.ID == "bunTop")
                hasBreadTop = true;
            if (food.ID == "bunBottom")
                hasBreadBottom = true;
            if (food.ID == "choppedSteak")
                hasBeef = true;
        }
        // Hvis den mangler en del, så får man ingen point, men bliver vist en fejlbesked i 5 sekunder, hvor der står: "Food is incomplete".
        if (!hasBreadTop || !hasBreadBottom || !hasBeef) {
            Debug.Log("Food is incomplete: top bot beef" + hasBreadTop + hasBreadBottom + hasBeef);
            pointsText.text = "Food is incomplete";
            Invoke("RemoveUI", 5);
            // Funktionen FoodResult slutter her.
            return;
        }
        // Hvis vi er nået her til, vil det sige at alle dele er der.
        // Vi opstiller en variabel til at holde styr på, hvor mange point der skal gives.
        float pointsToGive = 0;
        // Vi løber igennem al maden.
        foreach (GameObject food in foodToJudge) {
            // Vi kigger først på precisionen af burger-delene. Da den består af 3 dele, vil der være to ting der bliver lagt på, som kan bedømmes, nemlig bøffen og overbollen.
            if (food != baseFood) {
                // Vi sætter et tal til at være det antal point man max kan få.
                float maxPointsForPrecision = 50;
                // Check distancen mellem dette mad og dens parent, og give point ud fra hvor tætte de er, hvilket vil være den pæneste burger.
                pointsToGive += Mathf.Clamp(maxPointsForPrecision - Vector3.Distance(food.transform.position, food.transform.parent.position) * 250, 0, maxPointsForPrecision); // De kan ca kun blive op til .2f væk fra sin parent og stadig sidde sammen, og (.2 * 250 = 50), hvilket vil sige at, hvis maden er så langt væk fra sin parent som muligt, så får man ingen point for precision.
            }
            // Check hvor godt stegt maden er.
            // Vi sætter igen op et tal som er det antal point man max kan få.
            float maxPointsForRoasting = 100;
            // Vi laver en importanceMultiplier, for at give flere point for nogle dele i madretten.
            float importanceMultiplier = 1;
            if (food.GetComponent<Food>().ID == "choppedSteak") {
                importanceMultiplier = 2; // Bøffen er mere vigtig end brødet, så bøffen tæller dobbelt.
            }
            // Vi giver nu point ud fra, hvor godt stegt maden er. Det bedst mulige er når lvlStegt er .7f, så vi trækker fra ud fra, hvor langt væk fra .7f lvlStegt er.
            pointsToGive += Mathf.Clamp(Mathf.Clamp(maxPointsForRoasting - Mathf.Abs(.7f - food.GetComponent<Food>().lStegt) * 500, -100, maxPointsForRoasting) * importanceMultiplier, -200, maxPointsForRoasting * 2); // Værdien skal være mellem .5 og .9 for at give nogen point, da (.7f - .5f = .2f), og (.2f * 500 = 100), så 100 - 100 = 0.
            
            // Check hvor frossen maden er, og træk fra, jo mere frossen den er.
            pointsToGive -= Mathf.Clamp(food.GetComponent<Food>().lFrossen * 200, 0, 200); // For hver % frossen, bliver der minusset 2 point.
        }
        // På dette tidspunkt kan der være point mellem -500 og 500.
        if (pointsToGive < 0) {
            pointsToGive = 0;
        }
        // Der er nu mellem 0 og 500 point.

        // Giv pointene.
        ShowPoints(pointsToGive);
    }

    void ShowPoints(float points) {
        // Vis pointene i UI.
        pointsText.text = "You got " + points + " out of 500 points for your burger!";
    }

    // Denne funktion tømmer UI'en.
    void RemoveUI () {
        pointsText.text = "";
    }

}
