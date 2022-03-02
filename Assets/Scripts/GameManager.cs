using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


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
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entered processor");
        var ingEvents = other.GetComponent<IngredientObjectEvents>();
        if (ingEvents != null)
        {
            Ingredient otherIng = ingEvents.logic;
            if (otherIng.IsUtensil)
            {
                // logic.ADDUtensil(otherIng);
            }
            else
            {
                // logic.SETIngredientInProcess(otherIng);
            }
        }

    }
}

public class GameManager : MonoBehaviour
{
    private int _currOrderLevel;
    
    
    private float _minOrderTime;
    private float _maxOrderTime;
    
    public List<List<Recipe>> orderRecipesByLevel;
    
    public GameObject orderPrefab;
    public GameObject orderContainer;
    public GameObject orderDisplayPositioner;
    
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
        Ingredient orangeWhole = new Ingredient(true,false,  
            cam,new Vector3(), cuttingTable, 
            new List<GameObject>{orangePrefabs[0]},
            new List<IngredientAttr> {IngredientAttr.FRUIT, IngredientAttr.ORANGE, IngredientAttr.WHOLE},
            0);
        
        Ingredient lemonWhole = new Ingredient(true,false,  
            cam,new Vector3(), cuttingTable, 
            new List<GameObject>{lemonPrefabs[0]},
            new List<IngredientAttr> {IngredientAttr.FRUIT, IngredientAttr.LEMON, IngredientAttr.WHOLE},
            0);
       
        Ingredient appleWhole = new Ingredient(true,false,  
            cam,new Vector3(), cuttingTable, 
            new List<GameObject>{applePrefabs[0]},
            new List<IngredientAttr> {IngredientAttr.FRUIT, IngredientAttr.APPLE, IngredientAttr.WHOLE},
            0);
        
        
        Ingredient orangeCut = new Ingredient(true,false,  
            cam,new Vector3(), cuttingTable, 
            new List<GameObject>{orangePrefabs[1]},
            new List<IngredientAttr> {IngredientAttr.FRUIT, IngredientAttr.ORANGE, IngredientAttr.CUT},
            0);
        
        Ingredient lemonCut = new Ingredient(true,false,  
            cam,new Vector3(), cuttingTable, 
            new List<GameObject>{lemonPrefabs[1]},
            new List<IngredientAttr> {IngredientAttr.FRUIT, IngredientAttr.LEMON, IngredientAttr.CUT},
            0);
       
        Ingredient appleCut = new Ingredient(true,false,  
            cam,new Vector3(), cuttingTable, 
            new List<GameObject>{applePrefabs[1]},
            new List<IngredientAttr> {IngredientAttr.FRUIT, IngredientAttr.APPLE, IngredientAttr.CUT},
            0);
        
        
        
        Ingredient orangeJuice = new Ingredient(true,false,  
            cam,new Vector3(), cuttingTable, 
            new List<GameObject>{orangePrefabs[2]},
            new List<IngredientAttr> {IngredientAttr.FRUIT, IngredientAttr.ORANGE, IngredientAttr.DRINK},
            0);
        
        Ingredient lemonJuice = new Ingredient(true,false,  
            cam,new Vector3(), cuttingTable, 
            new List<GameObject>{lemonPrefabs[2]},
            new List<IngredientAttr> {IngredientAttr.FRUIT, IngredientAttr.LEMON, IngredientAttr.DRINK},
            0);
       
        Ingredient appleJuice = new Ingredient(true,false,  
            cam,new Vector3(), cuttingTable, 
            new List<GameObject>{applePrefabs[2]},
            new List<IngredientAttr> {IngredientAttr.FRUIT, IngredientAttr.APPLE, IngredientAttr.DRINK},
            0);
        
        
        Ingredient cup = new Ingredient(true,true,  
            cam,new Vector3(), cuttingTable, new List<GameObject> {cupPrefab},
            new List<IngredientAttr> {IngredientAttr.CUP},
            0);


        orderRecipesByLevel = new List<List<Recipe>>();
        orderRecipesByLevel.Add(new List<Recipe>());
        orderRecipesByLevel[0].Add(new Recipe(new List<Ingredient>() {orangeWhole}, 1));
        orderRecipesByLevel[0].Add(new Recipe(new List<Ingredient>() {lemonWhole}, 1));
        orderRecipesByLevel[0].Add(new Recipe(new List<Ingredient>() {appleWhole}, 1));
        
        orderRecipesByLevel.Add(new List<Recipe>());
        orderRecipesByLevel[1].Add(new Recipe(new List<Ingredient>() {orangeCut}, 2));
        orderRecipesByLevel[1].Add(new Recipe(new List<Ingredient>() {lemonCut}, 2));
        orderRecipesByLevel[1].Add(new Recipe(new List<Ingredient>() {appleCut}, 2));
        
        orderRecipesByLevel.Add(new List<Recipe>());
        orderRecipesByLevel[2].Add(new Recipe(new List<Ingredient>() {orangeCut}, 3));
        orderRecipesByLevel[2].Add(new Recipe(new List<Ingredient>() {lemonCut}, 3));
        orderRecipesByLevel[2].Add(new Recipe(new List<Ingredient>() {appleCut}, 3));

        
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
        newOrder.PrintOrder(orderPrefab, cam, orderContainer, orderDisplayPositioner);
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

        _minOrderTime = 5;
        _maxOrderTime = 10;
        _currOrderLevel = 3;
        currOrders = new List<Order>();
        
        InitFruitSectionSpawners();
        InitPossibleOrders();
        
//        GenerateOrder();
        InvokeRepeating("GenerateOrder", 
            0.0f, 
            Random.Range(_minOrderTime, _maxOrderTime));


    }

    // public void Update()
    // {
    //     _applesSpawner.transform.Translate(new Vector3());
    // }
}
