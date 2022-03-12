using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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
                gm.scoreValueObj.text = 
                    (int.Parse(gm.scoreValueObj.text) + gm.scoreMultiplier*recipe.logic.Level).ToString();
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
    public TextMeshPro scoreValueObj;
    public int scoreMultiplier;
    
    private int _currOrderLevel;
    
    private float _minOrderTime;
    private float _maxOrderTime;
    private float _maxPendingOrders;
    
    public SList<SList<Recipe>> orderRecipesByLevel;
    
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
                new SList<IngredientAttr> {IngredientAttr.FRUIT, IngredientAttr.ORANGE, IngredientAttr.WHOLE},
                0)
        );
        
        InitSpawner(lemonsSpawner,
            new Ingredient(true, false, 
                cam,new Vector3(), 
                lemonPrefabs,
                new SList<IngredientAttr> {IngredientAttr.FRUIT, IngredientAttr.LEMON, IngredientAttr.WHOLE},
                0)
        );

        InitSpawner(applesSpawner,
            new Ingredient(true, false, 
                cam,new Vector3(), 
                applePrefabs, 
                new SList<IngredientAttr> {IngredientAttr.FRUIT, IngredientAttr.APPLE, IngredientAttr.WHOLE}, 
                0)
        );

        InitSpawner(cupsSpawner,
            new Ingredient(true,true,  
                cam,new Vector3(), new List<GameObject> {cupPrefab},
                new SList<IngredientAttr> {IngredientAttr.CUP},
                0)
        );
    }

    void InitPossibleOrders()
    {
        SList<IngredientAttr> orangeWhole = 
            new SList<IngredientAttr>{IngredientAttr.FRUIT, IngredientAttr.ORANGE, IngredientAttr.WHOLE};
        
        SList<IngredientAttr> lemonWhole = 
            new SList<IngredientAttr> {IngredientAttr.FRUIT, IngredientAttr.LEMON, IngredientAttr.WHOLE};

        SList<IngredientAttr> appleWhole =
            new SList<IngredientAttr> {IngredientAttr.FRUIT, IngredientAttr.APPLE, IngredientAttr.WHOLE};


        SList<IngredientAttr> orangeCut = 
            new SList<IngredientAttr> {IngredientAttr.FRUIT, IngredientAttr.ORANGE, IngredientAttr.CUT};
        
        SList<IngredientAttr> lemonCut = 
            new SList<IngredientAttr> {IngredientAttr.FRUIT, IngredientAttr.LEMON, IngredientAttr.CUT};
       
        SList<IngredientAttr> appleCut = 
            new SList<IngredientAttr> {IngredientAttr.FRUIT, IngredientAttr.APPLE, IngredientAttr.CUT};
        
        
        SList<IngredientAttr> orangeJuice = 
            new SList<IngredientAttr> {IngredientAttr.FRUIT, IngredientAttr.ORANGE, IngredientAttr.DRINK};
        
        SList<IngredientAttr> lemonJuice = 
            new SList<IngredientAttr> {IngredientAttr.FRUIT, IngredientAttr.LEMON, IngredientAttr.DRINK};
       
        SList<IngredientAttr> appleJuice = 
            new SList<IngredientAttr> {IngredientAttr.FRUIT, IngredientAttr.APPLE, IngredientAttr.DRINK};

        SList<IngredientAttr> cup = new SList<IngredientAttr> {IngredientAttr.CUP};


        orderRecipesByLevel = new SList<SList<Recipe>>();
        orderRecipesByLevel.Add(new SList<Recipe>());
        orderRecipesByLevel[0].Add(new Recipe("orangeWhole", 
            new SList<SList<IngredientAttr>>() {orangeWhole}, 1));
        orderRecipesByLevel[0].Add(new Recipe("lemonWhole", 
            new SList<SList<IngredientAttr>>() {lemonWhole}, 1));
        orderRecipesByLevel[0].Add(new Recipe("appleWhole", 
            new SList<SList<IngredientAttr>>() {appleWhole}, 1));
        
        orderRecipesByLevel.Add(new SList<Recipe>());
        orderRecipesByLevel[1].Add(new Recipe("orangeCut", 
            new SList<SList<IngredientAttr>>() {orangeCut}, 2));
        orderRecipesByLevel[1].Add(new Recipe("lemonCut", 
            new SList<SList<IngredientAttr>>() {lemonCut}, 2));
        orderRecipesByLevel[1].Add(new Recipe("appleCut", 
            new SList<SList<IngredientAttr>>() {appleCut}, 2));
        
        orderRecipesByLevel.Add(new SList<Recipe>());
        orderRecipesByLevel[2].Add(new Recipe("orangeJuice", 
            new SList<SList<IngredientAttr>>() {cup,orangeJuice}, 3));
        orderRecipesByLevel[2].Add(new Recipe("lemonJuice", 
            new SList<SList<IngredientAttr>>() {cup,lemonJuice}, 3));
        orderRecipesByLevel[2].Add(new Recipe("appleJuice", 
            new SList<SList<IngredientAttr>>() {cup,appleJuice}, 3));

        orderRecipesByLevel.Add(new SList<Recipe>());
        orderRecipesByLevel[3].Add(new Recipe("citrusJuice", 
            new SList<SList<IngredientAttr>>() {cup,orangeJuice,lemonJuice}, 4));
        orderRecipesByLevel[3].Add(new Recipe("tuttiFruttiJuice", 
            new SList<SList<IngredientAttr>>() {cup,orangeJuice,appleJuice,lemonJuice}, 4));


        string json = JsonUtility.ToJson(orderRecipesByLevel);
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
            new SList<IngredientAttr> {IngredientAttr.FRUIT,IngredientAttr.CUT},
            new SList<IngredientAttr> {IngredientAttr.CUT},
            new SList<IngredientAttr> {IngredientAttr.DRINK},
            new SList<IngredientAttr> {IngredientAttr.CUP},
            1);
        var knife = new FoodProcessor(knifeObj,
            new SList<IngredientAttr> {IngredientAttr.WHOLE},
            new SList<IngredientAttr> {IngredientAttr.WHOLE},
            new SList<IngredientAttr> {IngredientAttr.CUT},
            new SList<IngredientAttr>(),
            1);

        _minOrderTime = 1;
        _maxOrderTime = 1;
        _currOrderLevel = 4;
        _maxPendingOrders = 5;
        currOrders = new List<Order>();

        scoreMultiplier = 1000;

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
