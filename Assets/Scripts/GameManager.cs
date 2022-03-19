using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
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
    
    public void Start()
    {
        gameObject.GetComponentInChildren<TextMeshPro>().text = JsonConvert.SerializeObject(template.Attributes);
    }

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
                        (int.Parse(gm.scoreValueObj.text) + GameGlobals.GameConfigs.ScoreMultiplier * targetRecipe.Level).ToString();
                }
                
                foreach (var recipe in _recipes)
                {
                    Destroy(recipe.gameObject);
                }
                _recipes.Clear();
                Destroy(order.GameObject);

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

    public GameObject orderPrefab;
    public GameObject orderContainer;
    
    public List<Order> currOrders;
    
    
    public Camera cam;
    
    //food combiners
    public List<GameObject> foodCombinerObjs;
    public GameObject deliveryBagPrefab;

    //food processors
    public GameObject juiceBlenderObj;
    public GameObject juiceKnifeObj;

    public GameObject dessertBlenderObj;
    public GameObject dessertKnifeObj;
    public GameObject coleffMachineSmallObj;
    public GameObject coleffMachineMediumObj;
    public GameObject coleffMachineLargeObj;

    
    public List<GameObject> ingredientSpawners;
    public GameObject deliveryBoardObj;
    
    
    public GameObject trashBin;
    
    public List<GameObject> cameraPositioners;
    private int _currCameraSection;
    public List<Button> cameraChangeButtons;
    
    public float repeatRate;

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
                    GameGlobals.GameConfigs.IngredientConfigs[i].IngredientPrefabs,
                    GameGlobals.GameConfigs.IngredientConfigs[i].IngredientAttrs,
                    0)
            );
        }
    }

    IEnumerator GenerateOrder()
    {
        Order newOrder = new Order();
        int numRecipes = GameGlobals.GameConfigs.OrderDifficulty 
                         / Random.Range(1, GameGlobals.GameConfigs.OrderDifficulty + 1);
        for (int i = 0; i < numRecipes; i++)
        {
            List <Recipe> orderRecipes = 
                GameGlobals.GameConfigs.OrderRecipesByLevel[(GameGlobals.GameConfigs.OrderDifficulty / numRecipes) - 1];
            newOrder.AddRecipe(orderRecipes[Random.Range(0, orderRecipes.Count)]);
        }

        currOrders.Add(newOrder);
        newOrder.PrintOrder(orderPrefab, cam, orderContainer);
        
        //decrement repeatRate on survival
        if(!GameGlobals.IsTraining)
        {
            repeatRate = 
                (repeatRate > GameGlobals.GameConfigs.MINOrderTime) ? repeatRate * 0.9f : repeatRate;
        }

        //decrement repeatRate on survival
        if (GameGlobals.IsTraining && currOrders.Count == GameGlobals.GameConfigs.MAXPendingOrders)
        {
            yield return null;
        }
        else
        {
            yield return new WaitForSeconds(repeatRate);
            StartCoroutine(GenerateOrder());
        }
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


    public void IncreaseCameraSection()
    {
        _currCameraSection =
            (_currCameraSection < (cameraPositioners.Count - 1)) ? _currCameraSection + 1 : _currCameraSection;
        cam.transform.parent = cameraPositioners[_currCameraSection].transform;
        cam.transform.localPosition = new Vector3();
    }
    public void DecreaseCameraSection()
    {
        _currCameraSection =
            (_currCameraSection > 0) ? _currCameraSection - 1 : _currCameraSection;
        cam.transform.parent = cameraPositioners[_currCameraSection].transform;
        cam.transform.localPosition = new Vector3();

    }
    
    
    
    // public void Awake()
    // {
    //     if (!gameConfig.IsTraining)
    //     {
    //         if (!PlayerPrefs.HasKey("firstTime") || PlayerPrefs.GetInt("firstTime") != 69)
    //         {
    //             PlayerPrefs.SetInt("firstTime", 69);
    //             PlayerPrefs.Save();
    //         }
    //         else if (PlayerPrefs.GetInt("firstTime") == 69)
    //         {
    //             Application.Quit();
    //         }
    //     }
    // }
    
    
    // Start is called before the first frame update
    void Start()
    {
        
        new FoodProcessor(juiceBlenderObj,
            new List<IngredientAttr> {IngredientAttr.FRUIT,IngredientAttr.CUT},
            new List<IngredientAttr> {IngredientAttr.CUT},
            new List<IngredientAttr> {IngredientAttr.DRINK},
            new List<IngredientAttr> {IngredientAttr.CUP},
            1);
        new FoodProcessor(juiceKnifeObj,
            new List<IngredientAttr> {IngredientAttr.FRUIT,IngredientAttr.WHOLE},
            new List<IngredientAttr> {IngredientAttr.WHOLE},
            new List<IngredientAttr> {IngredientAttr.CUT},
            new List<IngredientAttr>(),
            1);

        
        new FoodProcessor(dessertBlenderObj,
            new List<IngredientAttr> {IngredientAttr.COLEFF,IngredientAttr.WHOLE},
            new List<IngredientAttr> {IngredientAttr.WHOLE},
            new List<IngredientAttr> {IngredientAttr.CUT},
            new List<IngredientAttr>(),
            1);
        new FoodProcessor(coleffMachineSmallObj,
            new List<IngredientAttr> {IngredientAttr.COLEFF,IngredientAttr.CUT},
            new List<IngredientAttr> {IngredientAttr.CUT},
            new List<IngredientAttr> {IngredientAttr.DRINK,IngredientAttr.SMALL},
            new List<IngredientAttr> {IngredientAttr.DESSERT,IngredientAttr.CUP},
            1);
        new FoodProcessor(coleffMachineMediumObj,
            new List<IngredientAttr> {IngredientAttr.COLEFF,IngredientAttr.DRINK, IngredientAttr.SMALL},
            new List<IngredientAttr> {IngredientAttr.SMALL},
            new List<IngredientAttr> {IngredientAttr.MEDIUM},
            new List<IngredientAttr> {IngredientAttr.DESSERT,IngredientAttr.CUP},
            2);
        new FoodProcessor(coleffMachineLargeObj,
            new List<IngredientAttr> {IngredientAttr.COLEFF,IngredientAttr.DRINK, IngredientAttr.MEDIUM},
            new List<IngredientAttr> {IngredientAttr.MEDIUM},
            new List<IngredientAttr> {IngredientAttr.LARGE},
            new List<IngredientAttr> {IngredientAttr.DESSERT,IngredientAttr.CUP},
            3);
        new FoodProcessor(dessertKnifeObj,
            new List<IngredientAttr> {IngredientAttr.CAKE,IngredientAttr.WHOLE},
            new List<IngredientAttr> {IngredientAttr.WHOLE},
            new List<IngredientAttr> {IngredientAttr.CUT},
            new List<IngredientAttr>(),
            1);
        
        
        currOrders = new List<Order>();
        
        
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

//        scoreMultiplier = 1000;

        trashBin.AddComponent<TrashBinObjectEvents>();
        
        DeliveryBoardEvents delBoardEvents = deliveryBoardObj.AddComponent<DeliveryBoardEvents>();
        delBoardEvents.gm = this;

        foreach (var combinerObj in foodCombinerObjs)
        {
            new FoodCombiner(combinerObj, deliveryBagPrefab, cam);
        }
        
        InitFruitSectionSpawners();

        repeatRate = (GameGlobals.IsTraining) ? 0.25f: GameGlobals.GameConfigs.MAXOrderTime;
        
        StartCoroutine(GenerateOrder()); //Random.Range(gameConfig.MINOrderTime, gameConfig.MAXOrderTime));

        _currCameraSection = 0;
        cam.transform.parent = cameraPositioners[0].transform;
        cameraChangeButtons[0].onClick.AddListener(IncreaseCameraSection);
        cameraChangeButtons[1].onClick.AddListener(DecreaseCameraSection);

    }

    public void Update()
    {
        /**/
        if((GameGlobals.IsTraining && (int.Parse(scoreValueObj.text) == 1000)) ||
           (!GameGlobals.IsTraining && (currOrders.Count > GameGlobals.GameConfigs.MAXPendingOrders)))
        { /**/
            GameGlobals.Score = float.Parse(scoreValueObj.text);
            
            if (GameGlobals.IsTraining)
            {
                SceneManager.LoadScene("EndSceneTraining");
            }
            else
            {
                SceneManager.LoadScene("EndSceneSurvival");
            }
            /**/
        }
        /**/
    }
}
