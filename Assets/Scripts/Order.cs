using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public enum OrderSection
{
    FRUITS = 0,
    DESSERTS = 1,
    PASTAS = 2
}

public class RecipeObjectEvents : MonoBehaviour, IPointerClickHandler
{
    public Recipe logic; 
    public bool isBeingHeld;
    public Camera cam;

    public void Start()
    {
        string ingredAttrsText = "{\n";

        List<List<IngredientAttr>> ingredientList = logic.IngredientAttrs;
        for (int ingI = 0; ingI< ingredientList.Count; ingI++)
        {
            var ing = ingredientList[ingI]; 
            
            ingredAttrsText += "{";
            for (int ingAttrI = 0; ingAttrI< ing.Count; ingAttrI++)
            {
                var ingAttr = ing[ingAttrI]; 
                ingredAttrsText += ingAttr; 

                if (ingAttrI < ing.Count - 1)
                {
                    ingredAttrsText += ",";
                }
            }
            ingredAttrsText += "}\n";
            
            if (ingI < logic.IngredientAttrs.Count - 1)
            {
                ingredAttrsText += ",";
            }
        }
        ingredAttrsText += "}\n";
        
        gameObject.GetComponentInChildren<TextMeshPro>().text = ingredAttrsText;
    }
    
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        //Use this to tell when the user right-clicks on the Button
        if (pointerEventData.button == PointerEventData.InputButton.Left)
        {
            isBeingHeld = !isBeingHeld;
        }
    }

    public void FixedUpdate()
    {
        
        if (isBeingHeld)
        {
            Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;
    
            if (Physics.Raycast(ray, out hit))
            {
                transform.position = new Vector3(hit.point.x, transform.position.y, hit.point.z);
                //
                // _lineRenderer.startColor = Color.red;
                // _lineRenderer.endColor = Color.red;
                //
                // // set width of the renderer
                // _lineRenderer.startWidth = 0.3f;
                // _lineRenderer.endWidth = 0.3f;
                //
                // // set the position
                // _lineRenderer.SetPosition(0, transform.position);
                // _lineRenderer.SetPosition(1, new Vector3(hit.point.x, 10, hit.point.z));
            }
            
        }
    }

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
//     public Order logic;
//     public GameManager gm;
//     public void OnPointerClick(PointerEventData pointerEventData)
//     {
//         gm.currOrder
//     }
// }



public class Order
{
    public GameObject GameObject;
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
    
        GameObject = Object.Instantiate(orderPrefab,orderContainer.transform);
        // var events = newOrderObj.gameObject.AddComponent<OrderObjectEvents>();
        // events.cam = cam;
        // events.logic = this;
        
        Transform orderSectionPositioner = GameObject.transform.GetChild(1).transform;
        
        
        foreach (Recipe recipe in Recipes)
        {
            var tmp = orderSectionPositioner.GetChild((int) recipe.Section).GetChild(1).
                    GetComponent<TextMeshPro>();
            tmp.text = (tmp.text.Length == 0)? recipe.Name: tmp.text + "," + recipe.Name;
        }
    }

}