using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Dette script er ikke lavet som et system der kan bruges i flere tilfælde, men som specifikt til dens ene brug, nemlig at holde knivene.
public class KnifeHolder : MonoBehaviour
{
    // Her har vi informationer om knivene
    public GameObject knifeBig;
    public GameObject knifeMed;
    public GameObject knifeSma;
    // Her har vi informationer om knivenes holdere
    public GameObject holderBig;
    public GameObject holderMed;
    public GameObject holderSma;
    // Afstanden kniven skal have fra sin holder, for at man bliver visuelt vist, om man kan placere kniven.
    public float minSnapDistance = 2;
    // Materials der skal ændre udseendet på holderne.
    public Material normalHolder;
    public Material highlightHolder;

    void Update()
    {
        // Vi kalder denne funktion for at ændre udseendet på holderen.
        IsKnifeCloseToSnapZone();
    }

    public void PickingUpKnife(GameObject theKnife) {
        // Når en kniven bliver samlet op, bliver denne funktion kaldt
        // I denne funktion bliver kinematic slået fra kniven, så den bliver påvirket af physics.
        theKnife.GetComponent<Rigidbody>().isKinematic = false;
    }

    public void DroppingKnife() {
        // Når en kniven bliver sluppet, bliver denne funktion kaldt.
        // Tjek om knivene er i nærheden af deres holder. Hvis dette er sandt, skal kniven "snappes" til holderen.
        if (CheckDistance(knifeBig, holderBig)) {
            Snap(knifeBig, holderBig);
        }
        if (CheckDistance(knifeMed, holderMed)) {
            Snap(knifeMed, holderMed);
        }
        if (CheckDistance(knifeSma, holderSma)) {
            Snap(knifeSma, holderSma);
        }
    }

    void Snap (GameObject theKnife, GameObject theHolder) {
        // I denne funktion bliver kniven anbragt i holderen, og kniven bliver kinematic, så den ikke bliver påvirker af tyngdekraft. Vi bruger også kinematic, for at -
        // holde styr på om kniven allerede er i holderen.
        theKnife.GetComponent<Rigidbody>().isKinematic = true;
        theKnife.transform.position = theHolder.transform.position;
        theKnife.transform.rotation = theHolder.transform.rotation;
    }

    void IsKnifeCloseToSnapZone() {
        // Tjek om knivene er i nærheden af deres holder og ikke allerede er i holderen. Hvis dette er sandt, skal holderen highlightes, og vise at spilleren kan ligge kniven i holderen.
        if (CheckDistance(knifeBig, holderBig) && !knifeBig.GetComponent<Rigidbody>().isKinematic) {
            ChangeMaterial(holderBig, highlightHolder);
        } else {
            ChangeMaterial(holderBig, normalHolder);
        }
        if (CheckDistance(knifeMed, holderMed) && !knifeMed.GetComponent<Rigidbody>().isKinematic) {
            ChangeMaterial(holderMed, highlightHolder);
        } else {
            ChangeMaterial(holderMed, normalHolder);
        }
        if (CheckDistance(knifeSma, holderSma) && !knifeSma.GetComponent<Rigidbody>().isKinematic) {
            ChangeMaterial(holderSma, highlightHolder);
        } else {
            ChangeMaterial(holderSma, normalHolder);
        }
    }

    // Denne funktion tjekker om en kniv er tæt nok på sin holder.
    bool CheckDistance(GameObject theKnife, GameObject theHolder) {
        if (Vector3.Distance(theKnife.transform.position, theHolder.transform.position) <= minSnapDistance) {
            return true;
        }
        return false;
    }
    // Denne funktion ændrer matterial af holder, for at visualisere om kniven kan anbringes i den.
    void ChangeMaterial(GameObject gameObject, Material mat) {
        gameObject.GetComponent<Renderer>().material = mat;
    }
}
