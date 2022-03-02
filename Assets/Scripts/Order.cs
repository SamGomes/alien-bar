using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public enum OrderSection
{
    FRUITS = 0,
    DESSERTS = 1,
    PASTAS = 2
}

public class Recipe
{
    public OrderSection Section { get; set; }
    public List<Ingredient> Ingredients { get; set; }
    public int Level { get; set; }
    
    public Recipe(List<Ingredient> ingredients, int level)
    {
        Ingredients = ingredients;
        Level = level;
    }

}


public class OrderObjectEvents : MonoBehaviour//, IPointerClickHandler
{
    public Camera cam;
    public Order logic;
//    public void OnPointerClick(PointerEventData pointerEventData)
//    {
//        
//    }

    public void Update()
    {
        Debug.Log(logic.TimeLeft);
//        gameObject.transform.GetChild(0)
    }
}



public class Order
{
    private GameObject _gameObject;
    public float TimeLeft { get; set; }
    public int Level { get; set; }

    private List<Recipe> _recipes;
    public List<Recipe> Recipes
    {
        get { return _recipes; }
        private set
        {
            _recipes = value;
            int Level = 0;
            foreach(Recipe recipe in _recipes)
            {
                Level += recipe.Level;
            }
        }
    }

    public void AddRecipe(Recipe recipe)
    {
        Recipes.Add(recipe);
        Level += recipe.Level;
    }

    public Order()
    {
        Recipes = new List<Recipe>();
    }
    
    public void PrintOrder(GameObject orderPrefab, 
                Camera cam, 
                GameObject orderContainer, 
                GameObject orderDisplayPositioner)
    {
    
        Transform newOrderObj = Object.Instantiate(orderPrefab,orderContainer.transform).transform;
        var events = newOrderObj.gameObject.AddComponent<OrderObjectEvents>();
        events.cam = cam;
        events.logic = this;
        
        Transform orderSectionPositioner = newOrderObj.transform.GetChild(1).transform;
        
        
        foreach (Recipe recipe in Recipes)
        {
            foreach(Ingredient ing in recipe.Ingredients)
            {
                GameObject positionerObj = Object.Instantiate(orderDisplayPositioner.gameObject, 
                    orderSectionPositioner.GetChild((int) recipe.Section).GetChild(0).transform);
                Object.Instantiate(ing.GameObject, positionerObj.transform.GetChild(0).transform);
            }
        }
    }

}