using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class IngredientConfigurations
{
    public List<string> IngredientPrefabs;
    public List<IngredientAttr> IngredientAttrs;
}

public class GameConfigurations
{
    public int CurrOrderLevel;
    public float MINOrderTime;
    public float MAXOrderTime;
    public int MAXPendingOrders;
    
    
    public List<IngredientConfigurations> IngredientConfigs;
    public List<List<Recipe>> OrderRecipesByLevel;
}


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
        var ingEvents2 = other.GetComponent<IngredientObjectEvents>();
        if (ingEvents != null || ingEvents2!= null)
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
                     cam, 
                     new Vector3( 350, 
                        0 + template.GameObject.transform.position.y, 
                        100),
                     template.StateObjectPaths,
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
        Order orderToRemove = null;
        foreach (Order order in gm.currOrders)
        {
            if (gm.EvaluateOrder(order, _recipes))
            {
                foreach (var targetRecipe in order.Recipes)
                {
                    gm.scoreValueObj.text =
                        (int.Parse(gm.scoreValueObj.text) + gm.scoreMultiplier * targetRecipe.Level).ToString();
                }

                Destroy(order.GameObject);
                foreach (var recipe in _recipes)
                {
                    Destroy(recipe.gameObject);
                }

                orderToRemove = order;
            }
        }

        if (orderToRemove != null)
        {
            gm.currOrders.Remove(orderToRemove);
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

    private GameConfigurations _gameConfig;

    public GameObject orderPrefab;
    public GameObject orderContainer;
    
    public List<Order> currOrders;
    
    
    public Camera cam;
    
    //food combiners
    public List<GameObject> foodCombinerObjs;
    public GameObject deliveryBagPrefab;

    //food processors
    public GameObject juiceFoodProcessorObj;
    public GameObject knifeObj;

    public List<GameObject> ingredientSpawners;
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
        for (int i=0; i<ingredientSpawners.Count; i++)
        {
            InitSpawner(ingredientSpawners[i],
                new Ingredient(true, 
                    cam,new Vector3(), 
                    _gameConfig.IngredientConfigs[i].IngredientPrefabs,
                    _gameConfig.IngredientConfigs[i].IngredientAttrs,
                    0)
            );
        }
    }

    Order GenerateOrder()
    {
        Order newOrder = new Order();
        int numRecipes = _gameConfig.CurrOrderLevel / Random.Range(1, _gameConfig.CurrOrderLevel + 1);
        for (int i = 0; i < numRecipes; i++)
        {
            List <Recipe> orderRecipes = _gameConfig.OrderRecipesByLevel[(_gameConfig.CurrOrderLevel / numRecipes) - 1];
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
        new FoodProcessor(juiceFoodProcessorObj,
            new List<IngredientAttr> {IngredientAttr.FRUIT,IngredientAttr.CUT},
            new List<IngredientAttr> {IngredientAttr.CUT},
            new List<IngredientAttr> {IngredientAttr.DRINK},
            new List<IngredientAttr> {IngredientAttr.CUP},
            1);
        new FoodProcessor(knifeObj,
            new List<IngredientAttr> {IngredientAttr.WHOLE},
            new List<IngredientAttr> {IngredientAttr.WHOLE},
            new List<IngredientAttr> {IngredientAttr.CUT},
            new List<IngredientAttr>(),
            1);

        currOrders = new List<Order>();
        
        _gameConfig = new GameConfigurations();
        string path = "Assets/StreamingAssets/configs.cfg";
        StreamReader reader = new StreamReader(path);
        string json = reader.ReadToEnd();
        _gameConfig = JsonConvert.DeserializeObject<GameConfigurations>(json);
        reader.Close();
        
        // _gameConfig.MINOrderTime = 1;
        // _gameConfig.MAXOrderTime = 1;
        // _gameConfig.CurrOrderLevel = 4;
        // _gameConfig.MAXPendingOrders = 5;
        //
        // // string path = "Assets/StreamingAssets/recipes.cfg";
        // // StreamReader reader = new StreamReader(path);
        // // string json = reader.ReadToEnd();
        // _gameConfig.OrderRecipesByLevel = JsonConvert.DeserializeObject<List<List<Recipe>>>(json);
       
        // path = "Assets/StreamingAssets/configs.cfg";
        // json = JsonConvert.SerializeObject(_gameConfig);
        // StreamWriter writer = new StreamWriter(path, true);
        //
        // writer.WriteLine(json);
        // writer.Close();
        //

        scoreMultiplier = 1000;

        trashBin.AddComponent<TrashBinObjectEvents>();
        
        DeliveryBoardEvents delBoardEvents = deliveryBoardObj.AddComponent<DeliveryBoardEvents>();
        delBoardEvents.gm = this;

        foreach (var combinerObj in foodCombinerObjs)
        {
            new FoodCombiner(combinerObj, deliveryBagPrefab, cam);
        }
        
        InitFruitSectionSpawners();

        // Order newOrders = GenerateOrder();
        InvokeRepeating("GenerateOrder", 
            0.0f, 
            Random.Range(_gameConfig.MINOrderTime, _gameConfig.MAXOrderTime));

    }

    public void Update()
    {
        if (_gameConfig == null)
        {
            return;
        }
        
        if (currOrders.Count == _gameConfig.MAXPendingOrders)
        {
            SceneManager.LoadScene("GameOverScene");
        }
    }
}
