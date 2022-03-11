using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;


public class TrashBinObjectEvents : MonoBehaviour
{
    private Vector3 _baseScale;

    public void Start()
    {
        _baseScale = transform.localScale;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entered combiner");
        transform.localScale = 1.1f * _baseScale;
        var ingEvents = other.GetComponent<RecipeObjectEvents>();
        if (ingEvents != null)
        {
            Destroy(other.gameObject);
        }
        
    }
}
public class IngredientSpawner : MonoBehaviour, IPointerClickHandler
{
    public Camera cam;
    public Ingredient template;
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        //Use this to tell when the user right-clicks on the Button
        if (pointerEventData.button == PointerEventData.InputButton.Left)
        {
            Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                new Ingredient(
                     false, 
                     template.IsUtensil,
                     cam, 
                     new Vector3( 350, 
                        0 + template.GameObject.transform.position.y, 
                        100),
                     template.StateObjects,
                     template.Attributes,
                     template.TimeToProcess
                     );
            }
        }
    }
}

public class DeliveryBoardEvents : MonoBehaviour, IPointerClickHandler
{
    public GameManager gm;
    private List<RecipeObjectEvents> _recipes;
    private Vector3 _baseScale;

    public string recipesStr;

    public void Start()
    {
        _recipes = new List<RecipeObjectEvents>();   
        _baseScale = transform.localScale;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entered delivery board");
        var recEvents = other.GetComponent<RecipeObjectEvents>();
        if (recEvents != null)
        {
            _recipes.Add(recEvents);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Exited delivery board");
        var recEvents = other.GetComponent<RecipeObjectEvents>();
        if (recEvents != null)
        {
            _recipes.Remove(recEvents);
        }
    }
    
    public void OnPointerClick(PointerEventData pointerEventData)
    {
//        transform.localScale = 1.1f * _baseScale;
        if (gm.EvaluateOrder(gm.selectedOrder, _recipes))
        {
            Destroy(gm.selectedOrder.GameObject);
            foreach (var recipe in _recipes)
            {
                Destroy(recipe.gameObject);
            }
            gm.currOrders.Remove(gm.selectedOrder);
        }
    }

    void Update()
    {
        recipesStr = _recipes.Count.ToString();
    }
    
}

public class GameManager : MonoBehaviour
{
    private int _currOrderLevel;
    
    private float _minOrderTime;
    private float _maxOrderTime;
    private float _maxPendingOrders;
    
    public List<List<Recipe>> orderRecipesByLevel;
    
    public GameObject orderPrefab;
    public GameObject orderContainer;
    
    public List<Order> currOrders;
    public Order selectedOrder;
    
    
    public Camera cam;
    
    //food combiners
    public FoodCombiner fruitCombiner;
    public GameObject fruitCombinerObj;
    public GameObject deliveryBagPrefab;

    //food processors
    public GameObject juiceFoodProcessorObj;
    public GameObject knifeObj;
   
    public List<GameObject> orangePrefabs;
    public List<GameObject> lemonPrefabs;
    public List<GameObject> applePrefabs;
    
    public GameObject cupPrefab;
    
    public GameObject orangeSpawner;
    public GameObject lemonsSpawner;
    public GameObject applesSpawner;
    
    
    public GameObject cupsSpawner;
    public GameObject deliveryBoardObj;
    
    
    public GameObject trashBin;
    
    void InitSpawner(GameObject spawnerObj, Ingredient template)
    {
        spawnerObj.AddComponent<IngredientSpawner>();
        var spawner = spawnerObj.GetComponent<IngredientSpawner>();
        spawner.cam = cam;
        spawner.template = template;
    }

    void InitFruitSectionSpawners()
    {
        InitSpawner(orangeSpawner,
            new Ingredient(true,false,  
                cam,new Vector3(), 
                orangePrefabs,
                new List<IngredientAttr> {IngredientAttr.FRUIT, IngredientAttr.ORANGE, IngredientAttr.WHOLE},
                0)
        );
        
        InitSpawner(lemonsSpawner,
            new Ingredient(true, false, 
                cam,new Vector3(), 
                lemonPrefabs,
                new List<IngredientAttr> {IngredientAttr.FRUIT, IngredientAttr.LEMON, IngredientAttr.WHOLE},
                0)
        );

        InitSpawner(applesSpawner,
            new Ingredient(true, false, 
                cam,new Vector3(), 
                applePrefabs, 
                new List<IngredientAttr> {IngredientAttr.FRUIT, IngredientAttr.APPLE, IngredientAttr.WHOLE}, 
                0)
        );

        InitSpawner(cupsSpawner,
            new Ingredient(true,true,  
                cam,new Vector3(), new List<GameObject> {cupPrefab},
                new List<IngredientAttr> {IngredientAttr.CUP},
                0)
        );
    }

    void InitPossibleOrders()
    {
        List<IngredientAttr> orangeWhole = 
            new List<IngredientAttr> {IngredientAttr.FRUIT, IngredientAttr.ORANGE, IngredientAttr.WHOLE};
        
        List<IngredientAttr> lemonWhole = 
            new List<IngredientAttr> {IngredientAttr.FRUIT, IngredientAttr.LEMON, IngredientAttr.WHOLE};

        List<IngredientAttr> appleWhole =
            new List<IngredientAttr> {IngredientAttr.FRUIT, IngredientAttr.APPLE, IngredientAttr.WHOLE};


        List<IngredientAttr> orangeCut = 
            new List<IngredientAttr> {IngredientAttr.FRUIT, IngredientAttr.ORANGE, IngredientAttr.CUT};
        
        List<IngredientAttr> lemonCut = 
            new List<IngredientAttr> {IngredientAttr.FRUIT, IngredientAttr.LEMON, IngredientAttr.CUT};
       
        List<IngredientAttr> appleCut = 
            new List<IngredientAttr> {IngredientAttr.FRUIT, IngredientAttr.APPLE, IngredientAttr.CUT};
        
        
        List<IngredientAttr> orangeJuice = 
            new List<IngredientAttr> {IngredientAttr.FRUIT, IngredientAttr.ORANGE, IngredientAttr.DRINK};
        
        List<IngredientAttr> lemonJuice = 
            new List<IngredientAttr> {IngredientAttr.FRUIT, IngredientAttr.LEMON, IngredientAttr.DRINK};
       
        List<IngredientAttr> appleJuice = 
            new List<IngredientAttr> {IngredientAttr.FRUIT, IngredientAttr.APPLE, IngredientAttr.DRINK};

        List<IngredientAttr> cup = new List<IngredientAttr> {IngredientAttr.CUP};


        orderRecipesByLevel = new List<List<Recipe>>();
        orderRecipesByLevel.Add(new List<Recipe>());
        orderRecipesByLevel[0].Add(new Recipe("orangeWhole", 
            new List<List<IngredientAttr>>() {orangeWhole}, 1));
        orderRecipesByLevel[0].Add(new Recipe("lemonWhole", 
            new List<List<IngredientAttr>>() {lemonWhole}, 1));
        orderRecipesByLevel[0].Add(new Recipe("appleWhole", 
            new List<List<IngredientAttr>>() {appleWhole}, 1));
        
        orderRecipesByLevel.Add(new List<Recipe>());
        orderRecipesByLevel[1].Add(new Recipe("orangeCut", 
            new List<List<IngredientAttr>>() {orangeCut}, 2));
        orderRecipesByLevel[1].Add(new Recipe("lemonCut", 
            new List<List<IngredientAttr>>() {lemonCut}, 2));
        orderRecipesByLevel[1].Add(new Recipe("appleCut", 
            new List<List<IngredientAttr>>() {appleCut}, 2));
        
        orderRecipesByLevel.Add(new List<Recipe>());
        orderRecipesByLevel[2].Add(new Recipe("orangeJuice", 
            new List<List<IngredientAttr>>() {cup,orangeJuice}, 3));
        orderRecipesByLevel[2].Add(new Recipe("lemonJuice", 
            new List<List<IngredientAttr>>() {cup,lemonJuice}, 3));
        orderRecipesByLevel[2].Add(new Recipe("appleJuice", 
            new List<List<IngredientAttr>>() {cup,appleJuice}, 3));

        orderRecipesByLevel.Add(new List<Recipe>());
        orderRecipesByLevel[3].Add(new Recipe("citrusJuice", 
            new List<List<IngredientAttr>>() {cup,orangeJuice,lemonJuice}, 4));
        orderRecipesByLevel[3].Add(new Recipe("tuttiFruttiJuice", 
            new List<List<IngredientAttr>>() {cup,orangeJuice,appleJuice,lemonJuice}, 4));


    }

    Order GenerateOrder()
    {
        Order newOrder = new Order();
        int numRecipes = _currOrderLevel / Random.Range(1, _currOrderLevel + 1);
        for (int i = 0; i < numRecipes; i++)
        {
            List <Recipe> orderRecipes = orderRecipesByLevel[(_currOrderLevel / numRecipes) - 1];
            newOrder.AddRecipe(orderRecipes[Random.Range(0, orderRecipes.Count)]);
        }

        currOrders.Add(newOrder);
        newOrder.PrintOrder(orderPrefab, cam, orderContainer);
        
        return newOrder;
    }

    public bool EvaluateOrder(Order selectedOrder, List<RecipeObjectEvents> userRecipes)
    {
        int numValidRecepies = 0;
        foreach (var orderRecipe in selectedOrder.Recipes)
        {
            foreach (var userRecipe in userRecipes)
            {
                if (ValidateRecipe(orderRecipe, userRecipe.logic))
                {
                    numValidRecepies++;
                    break;
                }
            }
        }
        
        return (numValidRecepies == selectedOrder.Recipes.Count);
    }

    private bool ValidateRecipe(Recipe orderRecipe, Recipe userRecipe)
    {
        int numValidIngs = 0;
        foreach (var orderIng in orderRecipe.IngredientAttrs)
        {
            foreach (var userIng in userRecipe.IngredientAttrs)
            {
                bool isEqual = !orderIng.Except(userIng).Any();
                if (isEqual)
                {
                    numValidIngs++;
                    break;
                }
            }
        }
        return (numValidIngs == orderRecipe.IngredientAttrs.Count);
    }


    // Start is called before the first frame update
    void Start()
    {
        var juiceFoodProcessor = new FoodProcessor(juiceFoodProcessorObj,
            new List<IngredientAttr> {IngredientAttr.FRUIT,IngredientAttr.CUT},
            new List<IngredientAttr> {IngredientAttr.CUT},
            new List<IngredientAttr> {IngredientAttr.DRINK},
            new List<IngredientAttr> {IngredientAttr.CUP},
            1);
        var knife = new FoodProcessor(knifeObj,
            new List<IngredientAttr> {IngredientAttr.WHOLE},
            new List<IngredientAttr> {IngredientAttr.WHOLE},
            new List<IngredientAttr> {IngredientAttr.CUT},
            new List<IngredientAttr>(),
            1);

        _minOrderTime = 1;
        _maxOrderTime = 1;
        _currOrderLevel = 4;
        _maxPendingOrders = 5;
        currOrders = new List<Order>();

        trashBin.AddComponent<TrashBinObjectEvents>();
        
        DeliveryBoardEvents delBoardEvents = deliveryBoardObj.AddComponent<DeliveryBoardEvents>();
        delBoardEvents.gm = this;

        fruitCombiner = new FoodCombiner(fruitCombinerObj, deliveryBagPrefab, cam);
        
        InitFruitSectionSpawners();
        InitPossibleOrders();

        Order newOrders = GenerateOrder();
//        InvokeRepeating("GenerateOrder", 
//            0.0f, 
//            Random.Range(_minOrderTime, _maxOrderTime));

        selectedOrder = newOrders;

    }

    public void Update()
    {
        if (currOrders.Count == _maxPendingOrders)
        {
            SceneManager.LoadScene("GameOverScene");
        }
    }
}
