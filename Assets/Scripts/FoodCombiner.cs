using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FoodCombinerObjectEvents : MonoBehaviour, IPointerClickHandler
{
    public FoodCombiner logic;
    private Vector3 baseScale;

    public List<IngredientObjectEvents> objectsToComb;
       
    public void Update()
    {
        objectsToComb = logic.IngredientsToCombine;
    }
    
    public void Start()
    {
        baseScale = transform.localScale;
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        //Use this to tell when the user left-clicks on the Button
        if (pointerEventData.button == PointerEventData.InputButton.Left && logic.IngredientsToCombine.Count > 0)
        {
            Recipe userRec = logic.Combine();
            GameObject newDevBag = Instantiate(logic.DeliveryBagPrefab);
            newDevBag.transform.position = transform.position;
            RecipeObjectEvents recEvents = newDevBag.AddComponent<RecipeObjectEvents>();
            recEvents.logic = userRec;

            foreach (var ing in logic.IngredientsToCombine)
            {
                Destroy(ing.gameObject);
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
            logic.IngredientsToCombine.Add(ingEvents);
        }
        
    }
    
    private void OnTriggerExit(Collider other)
    {
        transform.localScale = baseScale;
        var ingEvents = other.GetComponent<IngredientObjectEvents>();
        if (ingEvents != null)
        {
            logic.IngredientsToCombine.Remove(ingEvents);
        }
        
    }
}
    
public class FoodCombiner
{
    public List<IngredientObjectEvents> IngredientsToCombine { get; set; }
    public GameObject GameObject { get; set; }
    
    public GameObject DeliveryBagPrefab { get; set; }
    public FoodCombiner(GameObject gameObject, 
        GameObject deliveryBagPrefab)
    {
        GameObject = gameObject;
        GameObject.AddComponent<FoodCombinerObjectEvents>();
        GameObject.GetComponent<FoodCombinerObjectEvents>().logic = this;

        DeliveryBagPrefab = deliveryBagPrefab;
        IngredientsToCombine = new List<IngredientObjectEvents>();
    }

    public Recipe Combine()
    {
        List<List<IngredientAttr>> listAttrs = new List<List<IngredientAttr>>();
        string recipeName = "";
        foreach (var ing in IngredientsToCombine)
        {
            listAttrs.Add(ing.logic.Attributes);
            recipeName += ing.ToString();
        }
        Recipe recipe = new Recipe(recipeName,  listAttrs, -1);

        return recipe;
    }
}