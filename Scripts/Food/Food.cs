using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour {
    // ID af maden, skulle bruges til at vurdere om maden var klar til at blive til en ret. ID'et kunne bruges til at vide, hvilke madvarer der er hvad.
    [Tooltip("Eksempler: steak/tomatoSauce/cucumber/carrot/potato/")]
    public string ID = "unknown";
    // Hvor meget stegt og frossen maden er.
    [Range(0.0f, 1.0f)]
    public float lStegt = 0;
    [Range(0.0f, 1.0f)]
    public float lFrossen = 0;
    // Temperaturen af maden.
    public float temperature = 20;
    // Omgivelsernes temperatur.
    public float surTemperature = 20;
    // Densiteten af maden.
    [Tooltip("Angivet i kg/l")]
    public float density;
    // Skal dette mads masse udregnes?
    public bool calculateMyMass = true;
    // Madens mulige farver, som afgør madens farve, angående hvor meget stegt eller frossen den er.
    public Color frossenColor;
    public Color godtStegtColor;
    public Color overstegtColor;
    public Color originColor;
    // Madsens rigidbody.
    Rigidbody rigid;
    // Om maden er på kogepladen og kogepladens information.
    public bool påKogeplade;
    public Kogeplade kogeplade;
    // Om maden er blevet "setup" ift. om det er blevet nedkølet af køleskabet.
    [HideInInspector]
    public bool hasBeenSetup;
    // Om maden kan bygges sammen med andet mad.
    public bool canBeBuildOnFromStart;
    public bool canBeBuildOn;

    void Start() {
        // Setup food
        //FoodManager.instance.MakeMePartOfTheFood(gameObject); // skulle bruges til foodmanager'en for at samle en madret
        // Sæt rigid til at være madens rigidbody
        rigid = GetComponent<Rigidbody>();
        // Hvis der skal udregnes masse af dette mad, så gør dette:
        if (calculateMyMass) {
            rigid.mass = VolumeAndMass.MassOfMesh(GetComponent<MeshFilter>().mesh, density);
        }
        // Bliv ved med at forsøge at sætte omgivelsernes temperatur til 20, så, hvis intet påvirker temperaturen af maden, vil madens temperatur gå mod rumtemperatur (20 grader c)
        InvokeRepeating("SetRoomTemperature", 0.1f, 1f);
        // Fejlbesked, hvis maden ikke har fået sat et ID.
        if (ID == "unknown") {
            Debug.LogWarning(gameObject + " has the ID of: '" + ID + "'. Is this on purpose?");
        }
        // Gør så maden ikke har noget parent, da der kommer fejl idet maden har et parent. Grunden til at maden har et parent i editoren, er for at gøre det mere overskueligt for os selv.
        gameObject.transform.parent = null;

        // Hvis maden er mad som kan starte med at bygges på, så gør så den kan bygges på.
        if (canBeBuildOnFromStart) {
            canBeBuildOn = true;
        }
    }

    void Update() {
        // Stegt farver
        Color stegtColor;
        if (lStegt <= .7) { // Hvor stegningen er mindre end 70%, skal stegningsfarven gå fra "originColor" til "overstegtColor".
            stegtColor = Color.Lerp(originColor, godtStegtColor, lStegt.Remap(0f, .7f, 0f, 1f));
        } else { // Hvor stegningen er mere end 70%, skal stegningsfarven gå fra "godtStegtColor" til "overstegtColor".
            stegtColor = Color.Lerp(godtStegtColor, overstegtColor, lStegt.Remap(.7f, 1f, 0f, 1f));
        }
        // Set farven af materialet til at være den udregnede farve, samtidig med at gå fra "stegtColor" til "frossenColor".
        GetComponent<Renderer>().material.SetColor("_Color", Color.Lerp(stegtColor, frossenColor, lFrossen));   // Ved URP skal "_Color" hedde "_BaseColor"

        // Fix levels
        if (lFrossen < 0)
            lFrossen = 0;
        if (lFrossen > 1)
            lFrossen = 1;
        if (lStegt < 0)
            lStegt = 0;
        if (lStegt > 1)
            lStegt = 1;

        // Hvis maden er på kogepladen, skal kogepladens temperatur være madens omgivelser (surroundings temperature).
        if (påKogeplade) {
            surTemperature = kogeplade.temperature;
        } else {
            // Hvis maden bliver fjernet fra kogepladen og dette mad har et smoke particles system, så stop det.
            if (GetComponentInChildren<ParticleSystem>() != null) {
                ParticleManager.instance.StopParticle(GetComponentInChildren<ParticleSystem>());
            }
        }

        // Kald funktionerne SetFrozenLvl() og AffectTemperature().
        SetFrozenLvl();
        AffectTemperature();

        // Hvis dette mad kan bygges, så kald funktionen BuildingFood().
        /*if (canBeBuild && hasConnectorsActivated) {
            BuildingFood();
        } else if (connectors != null) {
            foreach (GameObject connector in connectors) {
                connector.SetActive(false);
            }
        }*/

        // Vi sikrer os her at, hvis maden ikke er del af en madret
        if (transform.parent == null) {
            // Sørg for at madens rigidbody kan bevæge sig og bliver påvirket af tyngdekraft.
            rigid.isKinematic = false;
            rigid.useGravity = true;
        }
    }

    private void SetRoomTemperature() {
        // Sæt surroundings temperatur til 20, som den vil forblive, med mindre surroundings bliver påvirket af enten kogeplade eller køleskab.
        surTemperature = 20;
    }

    private void OnCollisionEnter(Collision collision) {
        // Hvis maden kolliderer med kogepladen, så er: påKogeplade = true.
        if (collision.gameObject.name == "Kogeplade") {
            kogeplade = collision.gameObject.GetComponent<Kogeplade>();
            påKogeplade = true;
        }
        // Hvis dette mad kolliderer med buildable food, og dette mad var det som spilleren sidst holdte.
        if (collision.gameObject.GetComponent<Food>() && gameObject == GameManager.i.lastHoldItem) {
            Food otherFood = collision.gameObject.GetComponent<Food>();
            // Hvis det andet mad kan bygges på, og dette mad ikke er del af en madret.
            if (otherFood.canBeBuildOn && gameObject.transform.parent == null && gameObject.transform.childCount == 0) {
                // så gør dette mad til et child af det andet mad.
                gameObject.transform.parent = otherFood.transform;
                // Vi ændrer rigidbody physics, så dette mad fuldstændigt følger dens parent.
                rigid.isKinematic = true;
                rigid.useGravity = false;
                // Nu kan dette mad også bygges på.
                canBeBuildOn = true;
            }
        }
    }
    // Hvis maden ikke længere kolliderer med kogepladen, så er: påKogeplade = false.
    private void OnCollisionExit(Collision collision) {
        if (collision.gameObject.name == "Kogeplade") {
            påKogeplade = false;
        }
    }

    void SetFrozenLvl () {
        // Fryse farve. Frossen level er hele tiden afhængig af temperaturen. Derfor kan en bøf både være stegt, og derefter lagt til køl.
        lFrossen = Mathf.Clamp(temperature.Remap(20, -20, 0, 1), 0f, 1f);
    }

    // Denne funktion bliver kaldt Update(), og gør at temperaturen hele tiden nærmer sig "surrounding temperature".
    public void AffectTemperature() {
        float modifier = 60;
        // Temperaturskift
        temperature += (surTemperature - temperature) * Time.deltaTime / modifier;
        if (surTemperature > 60) {
            // Når madens temperatur er over 60 grader, skal den begynde at stege.
            Steg(surTemperature);
        }
    }

    // Denne funktion bliver kaldt når maden skal stege.
    private void Steg(float power) {
        float modifier = 10000;
        // Stegning level går op. Den går hurtigere, hvis power er højere. Power er den "surrounding temperature" der påvirker maden.
        Mathf.Clamp(lStegt += power * Time.deltaTime / modifier, 0f, 1f);
        // Hvis dette mad ikke har et smoke particles system, så giv det et.
        if (GetComponentInChildren<ParticleSystem>() == null) {
            ParticleManager.instance.AddParticle(gameObject);
        }
        // Afspil det particles system som maden har.
        ParticleManager.instance.PlayParticle(GetComponentInChildren<ParticleSystem>());
    }

    // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // Dette næste stykke kode var det gamle "approach" til at samle mad. Det gik ud på at der var bestemte steder som man kunne ligges sammen, og at maden "snappede" til dette sted (connector).   -
    // Vi brugte lang tid på dette kode, indtil det gik op for os at der var en bedre løsning, som er den vi har nu.                                                                                 -
    // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    /*
    // Punkter, hvor andet mad kan tilsluttes dette mad.
    public List<GameObject> connectors = new List<GameObject>();
    void BuildingFood() {
        if (GameManager.i.itemHolding != null) {
            // Liste over al mad i banen.
            Food[] allFood = FindObjectsOfType<Food>();
            // Gennemgå egne connectors.
            foreach (GameObject connector in connectors) {
                bool foundAMatch = false; // Bool til at holde styr på om næste forloop har fundet et match.
                foreach (Food food in allFood) {
                    // Der tjekkes for om noget andet mad er tæt nok på dette mad.
                    if (CheckForConnectionRequirements(gameObject, food)) {
                        // Hvis noget andet mad er tæt på, så vis at det kan ligges på dette mad.
                        connector.SetActive(true);
                        foundAMatch = true;
                        //Debug.Log("Found: " + food.gameObject);
                    }
                }
                // Hvis denne connector før havde fundet et match, men nu ikke har, så gør connectoren inaktiv.
                if (!foundAMatch) {
                    connector.SetActive(false);
                }
            }
        }
    }

    // I denne funktion tjekkes om der er forhold til at mad kan samles.
    public float connectionDistanceCheck = .25f;
    bool CheckForConnectionRequirements(GameObject meFood, Food otherFood) {
        // Hvis maden er indenfor rækkevidde, ikke er sig selv, ikke allerede er en del af det samlede mad (child af objektet) og er mad som har egenskaben til at samle sig.
        if (Vector3.Distance(meFood.transform.position, otherFood.gameObject.transform.position) < connectionDistanceCheck && otherFood.gameObject != meFood && !otherFood.gameObject.transform.IsChildOf(transform) && otherFood.canBeBuild && !meFood.transform.IsChildOf(otherFood.gameObject.transform)) {
            return true;
        }
        return false;
    }

    // I denne funktion vil sluppet mad prøve at finde noget andet den kan connecte til, hvis den har: canBeBuild.
    void TryToConnect() {
        // Al mad i banen gemmes i et array.
        Food[] allFood = FindObjectsOfType<Food>();
        bool foundAContestant = false;
        Food closestFood = new Food();
        // Gennemgår al maden.
        foreach (Food food in allFood) {
            // Tjekker om maden kan bygges.
            if (food.canBeBuild && food.hasConnectorsActivated) {
                if (CheckForConnectionRequirements(gameObject, food)) {
                    if (!foundAContestant) {
                        // Det er det første mad der kan forbindes til.
                        closestFood = food;
                        foundAContestant = true;
                    } else {
                        // Der er allerede fundet noget mad som kan forbindes til, derfor skal denne være tættere på for at blive maden der skal forbindes til.
                        if (Vector3.Distance(food.gameObject.transform.position, gameObject.transform.position) < Vector3.Distance(closestFood.transform.position, gameObject.transform.position)) {
                            closestFood = food;
                        }
                    }
                }
            }
        }
        if (foundAContestant) {
            Debug.Log("found " + closestFood.gameObject.name);
            GameObject closestConnector = new GameObject();
            bool foundAConnector = false;
            // Gennemgår connectors i det tætteste mad.
            foreach (GameObject connector in closestFood.connectors) {
                if (!foundAConnector) {
                    closestConnector = connector;
                    foundAConnector = true;
                } else {
                    if (Vector3.Distance(connector.transform.position, gameObject.transform.position) < Vector3.Distance(closestConnector.transform.position, gameObject.transform.position)) {
                        closestConnector = connector;
                    }
                }
            }
            // Hvis vilkårende er rigtige, så gør dette mad et child af det andet mad.
            gameObject.transform.parent = closestFood.transform;
            // Placerer maden ved connectoren.
            gameObject.transform.position = closestConnector.transform.position;
            rigid.isKinematic = true;
            hasConnectorsActivated = true;
        }


    }*/


    // Bliver kaldt når spilleren tager fat i objektet.
    public void SetHoldingItem() {
        // Sætter dette mad som det objekt spilleren holder. 
        GameManager.i.itemHolding = gameObject;
        // Der er nu ikke noget objekt som "sidst" var blevet holdt, da der nu holdes noget.
        GameManager.i.lastHoldItem = null;
        // Dette mad har nu ikke noget parent
        gameObject.transform.parent = null;
        // Hvis dette mad ikke er mad som starter en "kæde" af mad, så går vi ud fra at maden nu er blevet fjernet fra hvad den sidst sad fast til, så nu kan der ikke bygges mere på dette.
        if (!canBeBuildOnFromStart) {
            canBeBuildOn = false;
        }
    }
    // Bliver kaldt når spilleren slipper objektet.
    public void RemoveHoldingItem() {
        // Fjerner dette mad som det objekt spilleren holder.
        GameManager.i.itemHolding = null;
        // Nu er dette objekt det som spilleren holdte sidst.
        GameManager.i.lastHoldItem = gameObject;

        // Til det gamle system.
        /*
        // Hvis maden kan bygges, så forsøg at connect med andet mad.
        if (canBeBuild) {
            //TryToConnect();
        }*/
    }
}

public static class ExtensionMethods {
    // Remap en værdi til at istedet for at gå fra a-b, så gå fra c-d.
    public static float Remap(this float value, float from1, float to1, float from2, float to2) {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}

