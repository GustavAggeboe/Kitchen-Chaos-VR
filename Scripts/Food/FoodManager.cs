using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Dette script var til at kombinere ingredienser, og lave en større ting ud af dette.
// Scriptet nåede ikke at blive færdigt og bliver derfor ikke brugt. Vi har derfor ikke kommenteret på hele scriptet.
public class FoodManager : MonoBehaviour
{
    public static FoodManager instance;

    // Til at starte med, og hver gang der bliver lavet ny mad, vil maden gøre sig en del af denne liste, ved at kalde "MakeMePartOfTheFood()".
    public List<GameObject> allFoodInScene = new List<GameObject>();


    private void Awake() {
        instance = this;
    }

    void Start()
    {
        
    }

    void Update()
    {
        CombineFood();
    }

    public void MakeMePartOfTheFood (GameObject gameObject) {
        allFoodInScene.Add(gameObject);
    }

    void CombineFood() {
        // Conditions for a specific combinations
        List<GameObject> ingredients = new List<GameObject>();

        // pasta sauce
        int requiredIngredients = 2;
        int nrOfIngredients = 0;
        /*ingredients.
        if (Conditions("tomatoSauce", 20, 50, 110, .5f, .85f, 0.1f) != null) {
            ingredients.AddRange(Conditions("tomatoSauce", 20, 50, 110, .5f, .85f, 0.1f));
            nrOfIngredients += 1;
        }
        if (Conditions("steak", 10, 50, 110, .5f, .85f, .1f, 1) != null) {
            ingredients.AddRange(Conditions("steak", 10, 50, 110, .5f, .85f, .1f, 1));
            nrOfIngredients += 1;
        }
        if (ValidateFood(ingredients1, ingredients2)) {
            // Get centrum af ingredienser
            Vector3 center = GetCenter(ingredients1, ingredients2);
            // Slet gamle ingredienser
            DeleteOldIngredients(ingr1);
            DeleteOldIngredients(ingr2);
            // Make pasta sauce
            MakeFood("pastaSauce", center);
        }*/
        

    }

    List<GameObject> Conditions(string ID = "unknown", int amount = 2, float minTemp = -20, float maxTemp = 1000, float minLvlStegt = 0, float maxLvlStegt = 1, float maxLvlFrozen = 1, float maxVolume = .1f) {
        bool conditionsAreOkay = false;
        List<GameObject> oldIngredients = new List<GameObject>();
        // Tjek for ID'er i banen

        // Tjek om der er ting i nærheden af et objekt i et loop, som er indenfor conditions, hvis der er, så giv true
        if (minLvlStegt < 0) {
            conditionsAreOkay = true;
        }

        // Return
        if (conditionsAreOkay)
            return oldIngredients;
        else
            return null;
    }

    void DeleteOldIngredients (List<GameObject> oldIngredients) {
        foreach (GameObject gameObject in oldIngredients) {
            Destroy(gameObject);
        }
    }

    bool ValidateFood(List<GameObject> ingr1, List<GameObject> ingr2) {
        if (ingr1 != null && ingr2 != null) {
            return true;
        } else {
            return false;
        }
    }

    Vector3 GetCenter (List<GameObject> ingr1, List<GameObject> ingr2) {
        Vector3 center = new Vector3();
        foreach (GameObject gameObject in ingr1)
            center += gameObject.transform.position;
        foreach (GameObject gameObject in ingr2)
            center += gameObject.transform.position;
        center /= ingr1.Count + ingr2.Count;
        return center;
    }

    void MakeFood(string foodID, Vector3 position) {
        // Lav det nye objekt af mad

        // Slet de gamle
    }

    /*public List<GameObject> TryToAdd(this List<GameObject> list, List<GameObject> ingr1) {
        //return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        return list;
    }*/
}
