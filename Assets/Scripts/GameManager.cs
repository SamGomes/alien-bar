using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class IngredientSpawner : MonoBehaviour, IPointerClickHandler
{
    public Camera cam;
    public GameObject cuttingTable;
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
                     new Vector3( hit.point.x, 
                        hit.point.y + template.GameObject.transform.position.y, 
                        hit.point.z),
                     cuttingTable,
                     template.StateObjects,
                     template.Attributes,
                     template.TimeToProcess
                     );
            }
        }
    }
}

public class Board : MonoBehaviour
{
    private Order _order;
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
    
    
    public Camera cam;
    public GameObject cuttingTable;

    public GameObject juiceFoodProcessorObj;
    public GameObject knifeObj;
    public GameObject burner1Obj;
    public GameObject burner2Obj;
    public GameObject burner3Obj;
    public GameObject burner4Obj;
    public GameObject coffeeMachineObj;

    public List<GameObject> orangePrefabs;
    public List<GameObject> lemonPrefabs;
    public List<GameObject> applePrefabs;
    
    public GameObject cupPrefab;
    
    public GameObject orangeSpawner;
    public GameObject lemonsSpawner;
    public GameObject applesSpawner;
    
    
    public GameObject cupsSpawner;

    public GameObject board;


    
    void InitSpawner(GameObject spawnerObj, Ingredient template)
    {
        spawnerObj.AddComponent<IngredientSpawner>();
        var spawner = spawnerObj.GetComponent<IngredientSpawner>();
        spawner.cuttingTable = cuttingTable;
        spawner.cam = cam;
        spawner.template = template;
    }

    void InitFruitSectionSpawners()
    {
        InitSpawner(orangeSpawner,
            new Ingredient(true,false,  
                cam,new Vector3(), cuttingTable, 
                orangePrefabs,
                new List<IngredientAttr> {IngredientAttr.FRUIT, IngredientAttr.ORANGE, IngredientAttr.WHOLE},
                0)
        );
        
        InitSpawner(lemonsSpawner,
            new Ingredient(true, false, 
                cam,new Vector3(), cuttingTable, 
                lemonPrefabs,
                new List<IngredientAttr> {IngredientAttr.FRUIT, IngredientAttr.LEMON, IngredientAttr.WHOLE},
                0)
        );

        InitSpawner(applesSpawner,
            new Ingredient(true, false, 
                cam,new Vector3(), cuttingTable, 
                applePrefabs, 
                new List<IngredientAttr> {IngredientAttr.FRUIT, IngredientAttr.APPLE, IngredientAttr.WHOLE}, 
                0)
        );

        InitSpawner(cupsSpawner,
            new Ingredient(true,true,  
                cam,new Vector3(), cuttingTable, new List<GameObject> {cupPrefab},
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
            new List<List<IngredientAttr>>() {orangeJuice}, 3));
        orderRecipesByLevel[2].Add(new Recipe("lemonJuice", 
            new List<List<IngredientAttr>>() {lemonJuice}, 3));
        orderRecipesByLevel[2].Add(new Recipe("appleJuice", 
            new List<List<IngredientAttr>>() {appleJuice}, 3));

        orderRecipesByLevel.Add(new List<Recipe>());
        orderRecipesByLevel[3].Add(new Recipe("citrusJuice", 
            new List<List<IngredientAttr>>() {orangeJuice,lemonJuice}, 4));
        orderRecipesByLevel[3].Add(new Recipe("tuttiFruttiJuice", 
            new List<List<IngredientAttr>>() {orangeJuice,appleJuice,lemonJuice}, 4));


    }

    void GenerateOrder()
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
    }

    void EvaluateOrder(List<Ingredient> currIngredients)
    {
        foreach (var ing in currIngredients)
        {
            
        }
    }

    

    // Start is called before the first frame update
    void Start()
    {
        var juiceFoodProcessor = new FoodProcessor(juiceFoodProcessorObj,
            new List<IngredientAttr> {IngredientAttr.FRUIT,IngredientAttr.CUT},
            new List<IngredientAttr> {IngredientAttr.FRUIT,IngredientAttr.CUT},
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
        
        InitFruitSectionSpawners();
        InitPossibleOrders();

        GenerateOrder();
//        InvokeRepeating("GenerateOrder", 
//            0.0f, 
//            Random.Range(_minOrderTime, _maxOrderTime));


    }

    public void Update()
    {
        if (currOrders.Count == _maxPendingOrders)
        {
            SceneManager.LoadScene("GameOverScene");
        }
    }
}
