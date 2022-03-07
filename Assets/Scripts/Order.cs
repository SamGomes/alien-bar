using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public enum OrderSection
{
    FRUITS = 0,
    DESSERTS = 1,
    PASTAS = 2
}

public class RecipeObjectEvents : MonoBehaviour
{
    public Recipe logic;

}

public class Recipe
{
    public string Name { get; set; }
    public OrderSection Section { get; set; }
    public List<List<IngredientAttr>> IngredientAttrs { get; set; }
    public int Level { get; set; }
    
    public Recipe(string name, List<List<IngredientAttr>> ingredientAttrs, int level)
    {
        Name = name;
        IngredientAttrs = ingredientAttrs;
        Level = level;
    }

}


// public class OrderObjectEvents : MonoBehaviour, IPointerClickHandler
// {
//     public Camera cam;
//     public Order order;
//     public GameManager gm;
//     public void OnPointerClick(PointerEventData pointerEventData)
//     {
//         gm.currOrder
//     }
//
// }



public class Order
{
    private GameObject _gameObject;
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
                GameObject orderContainer)
    {
    
        Transform newOrderObj = Object.Instantiate(orderPrefab,orderContainer.transform).transform;
        // var events = newOrderObj.gameObject.AddComponent<OrderObjectEvents>();
        // events.cam = cam;
        // events.logic = this;
        
        Transform orderSectionPositioner = newOrderObj.transform.GetChild(1).transform;
        
        
        foreach (Recipe recipe in Recipes)
        {
            var tmp = orderSectionPositioner.GetChild((int) recipe.Section).GetChild(1).
                    GetComponent<TextMeshPro>();
            tmp.text = (tmp.text.Length == 0)? recipe.Name: tmp.text + "," + recipe.Name;
        }
    }

}