using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Dette system er lavet til at mindske mængden af røg der bliver lavet, når mad bliver stegt. Vi valgte at have hver eneste mad-del til at lave den samme mængde røg.
// Derefter sørger dette system for at samle de partikler der er indenfor et område, og kun bruge én af dem, som den placerer i midten af alle mad-dele der indgår i dén "gruppe".
public class ParticleManager : MonoBehaviour
{
    // En stativ ParticleManager sørger for at andre scripts kan kalde dette script, uden at have en direkte reference til scriptet.
    public static ParticleManager instance;
    // Dette er prefab'et, som vi bruger til at indsætte, når man skal emitte røg.
    public GameObject SmokePrefab;
    // Dette er distancen mellem mad, for at de bør indgå i samme "gruppe".
    public float minSmokeDistance = 1;
    // Dette er en liste over alle røgsystemer der er i banen.
    List<GameObject> particleSystemsInScene = new List<GameObject>();

    // I starten af spillet, bliver dette script sat som instance, altså det eneste script i banen, så andre scripts kan kalde dette.
    private void Awake() {
        instance = this;
    }

    void Update() {
        // Til at starte med, vil vi nulstille grupperinger
        List<GameObject> particleSystemsInAGroup = new List<GameObject>();
        // Vi går nu igennem alle particle systemer
        foreach (GameObject go1 in particleSystemsInScene) {
            // Hvis dette particle system allerede er i en gruppe, så gå videre til næste particle system
            if (particleSystemsInAGroup.Contains(go1))
                continue;

            // For dette specifikke particle system, laver vi en liste/gruppe, som alle nærliggende partikelsystemer skal blive en del af.
            List<GameObject> particleSystemsInMyGroup = new List<GameObject>();
            // Hvis denne partikel allerede er blevet rykket, i det den har været i en tidligere gruppe, skal dens position nulstilles.
            go1.transform.position = go1.transform.parent.position;
            // Dette particle system er nu i en gruppe, da vi starter gruppering.
            particleSystemsInAGroup.Add(go1);

            // Distance check mellem denne partikel, og alle andre partikler.
            foreach (GameObject go2 in particleSystemsInScene) {
                if (go1 != go2) { // Hvis der er tale om to forskellige particlesystems
                    // Hvis et andet particle system (go2) er for tæt på, bliver dette en del af go1's gruppe.
                    if (TooClose(go1, go2, minSmokeDistance)) {
                        // go2 skal nu i go1's gruppe.
                        particleSystemsInMyGroup.Add(go2);
                        // go2 er nu også i én gruppe.
                        particleSystemsInAGroup.Add(go2);
                    }
                }
            }

            // Stop de andre particle systemer i go1's gruppe, og ryk go1 i midten af alle particle systemer i gruppen.
            Vector3 averagePosition = new Vector3(0, 0, 0);
            foreach (GameObject gameObject in particleSystemsInMyGroup) {
                StopParticle(gameObject.GetComponent<ParticleSystem>());
                averagePosition += gameObject.transform.position;
            }
            // midten af systemerne skal også have go1's position for at udregne den
            averagePosition += go1.transform.position;
            // Vi tager nu gennemsnittet af alle systemer, og plusser 1, for at tælle go1 med, da den ikke er i listen "particleSystemsInMyGroup".
            averagePosition /= particleSystemsInMyGroup.Count + 1;
            // Vi placerer go1 i midten af systemerne.
            go1.transform.position = averagePosition;

            // Afspil min egen partikel
            PlayParticle(go1.GetComponent<ParticleSystem>());

            // Destroy check
            if (!go1.GetComponent<ParticleSystem>().isPlaying) {
                // Hvis particlesystem er færdig med at afspille, skal det fjernes.
                //particleSystemsInScene.Remove(go1);
                //Destroy(go1);
            }
            
        }
    }

    // Denne funktion laver en ny røgpartikel, og gør den child af det objekt som kalder denne funktion. Funktionen bliver kaldt af mad, når den bliver stegt.
    public void AddParticle(GameObject originObject) {
        GameObject particleSystem = Instantiate(SmokePrefab, originObject.transform);
        particleSystemsInScene.Add(particleSystem);
    }

    // Denne funktion afspiller et particlesystem.
    public void PlayParticle(ParticleSystem particleSystem) {
        // Forhindrer restart af particle
        if (!particleSystem.isEmitting) {
            particleSystem.Play();
        }
    }

    // Denne funktion stopper et particlesystem.
    public void StopParticle(ParticleSystem particleSystem) {
        particleSystem.Stop();
    }

    // Denne funktion vurderer om to partikelsystemer er for tæt på hinanden
    bool TooClose(GameObject ps1, GameObject ps2, float minDistance) {
        if (Vector3.Distance(ps1.transform.position, ps2.transform.position) < minDistance) {
            return true;
        }
        return false;
    }

    // Denne funktion viser visuelt, hvor grænsen går mellem om partikelsystemer skal grupperes, og om de bliver grupperet. Dette bliver kun vist i Unity, og ikke når spillet er "buildet".
    private void OnDrawGizmos() {
        foreach (GameObject go1 in particleSystemsInScene) {
            Gizmos.DrawWireSphere(go1.transform.position, minSmokeDistance);
        }
    }
}
