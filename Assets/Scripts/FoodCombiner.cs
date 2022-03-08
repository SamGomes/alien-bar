using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FoodCombinerObjectEvents : MonoBehaviour, IPointerClickHandler
{
    public FoodCombiner logic;
    private Vector3 baseScale;
    
    public void Start()
    {
        baseScale = transform.localScale;
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        //Use this to tell when the user left-clicks on the Button
        if (pointerEventData.button == PointerEventData.InputButton.Left)
        {
            Recipe userRec = logic.Combine();
            GameObject newDevBag = Instantiate(logic.DeliveryBagPrefab);
            newDevBag.transform.position = transform.position;
            RecipeObjectEvents recEvents = newDevBag.AddComponent<RecipeObjectEvents>();
            recEvents.logic = userRec;

            foreach (Ingredient ing in logic.IngredientsToCombine)
            {
                Object.Destroy(ing);
            }
            logic.IngredientsToCombine.Clear();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entered combiner");
        transform.localScale = 1.1f * baseScale;
        var ingEvents = other.GetComponent<IngredientObjectEvents>();
        if (ingEvents != null)
        {
            Ingredient otherIng = ingEvents.logic;
            logic.IngredientsToCombine.Add(otherIng);
        }
        
    }
    
    private void OnTriggerExit(Collider other)
    {
        transform.localScale = baseScale;
        var ingEvents = other.GetComponent<IngredientObjectEvents>();
        if (ingEvents != null)
        {
            Ingredient otherIng = ingEvents.logic;
            logic.IngredientsToCombine.Remove(otherIng);
        }
        
    }
}
    
public class FoodCombiner
{
    public List<Ingredient> IngredientsToCombine { get; set; }
    public GameObject GameObject { get; set; }
    
    public GameObject DeliveryBagPrefab { get; set; }
    public FoodCombiner(GameObject gameObject, 
        GameObject deliveryBagPrefab)
    {
        GameObject = gameObject;
        GameObject.AddComponent<FoodCombinerObjectEvents>();
        GameObject.GetComponent<FoodCombinerObjectEvents>().logic = this;

        DeliveryBagPrefab = deliveryBagPrefab;
        IngredientsToCombine = new List<Ingredient>();
    }

    public Recipe Combine()
    {
        List<List<IngredientAttr>> listAttrs = new List<List<IngredientAttr>>();
        string recipeName = "";
        foreach (var ing in IngredientsToCombine)
        {
            listAttrs.Add(ing.Attributes);
            recipeName += ing.ToString();
        }
        Recipe recipe = new Recipe(recipeName,  listAttrs, -1);

        return recipe;
    }
}