using System;
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


public class DirectionalButtonEvents : MonoBehaviour
{
    private void OnMouseEnter()
    {
        GameGlobals.gameManager.cursorOverlapBuffer.Add(GameGlobals.gameManager.cursorTextureDrag);
    }
    
    private void OnMouseExit()
    {
        GameGlobals.gameManager.cursorOverlapBuffer.Remove(GameGlobals.gameManager.cursorTextureDrag);
    }
}


public class TrashBinObjectEvents : 
    MonoBehaviour, 
//    IPointerClickHandler, 
    IPointerEnterHandler, 
    IPointerExitHandler
{
    private Animator _animator;
    private AudioSource _sound;

    public List<GameObject> objsToDestroy;
    
    public void Start()
    {
        objsToDestroy = new List<GameObject>();
        _sound = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        GameGlobals.gameManager.cursorOverlapBuffer.Add(GameGlobals.gameManager.cursorTextureTrashing);
    }
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        GameGlobals.gameManager.cursorOverlapBuffer.Remove(GameGlobals.gameManager.cursorTextureTrashing);
    }
    
    
    public void OnTriggerEnter(Collider other)
    {
        GameObject otherGO = other.gameObject;
        var ingEvents = otherGO.GetComponent<RecipeObjectEvents>();
        var ingEvents2 = otherGO.GetComponent<IngredientObjectEvents>();
        if (ingEvents != null  || ingEvents2 != null)
        {
            objsToDestroy.Add(otherGO);
            Vector3 pos = gameObject.transform.position;
            otherGO.transform.position = new Vector3(pos.x, 0, pos.z)
                                         + Vector3.back * 20.0f
                                         + Vector3.left * 10.0f;
        }
    }
    
    public void OnTriggerExit(Collider other)
    {
        var ingEvents = other.GetComponent<RecipeObjectEvents>();
        var ingEvents2 = other.GetComponent<IngredientObjectEvents>();
        if (ingEvents != null || ingEvents2 != null)
        {
            objsToDestroy.Remove(other.gameObject);
        }
    }
    
//    public void OnPointerClick(PointerEventData pointerEventData)
//    {
//        _animator.Play(0);
//        _animator.Rebind();
//        _animator.Update(0.0f);
//        
//        _sound.pitch = Random.Range(0.8f, 1.2f);
//        _sound.Play();
//        foreach (var obj in objsToDestroy)
//        {
//            Destroy(obj);
//        }
//        
//    }

    public void Update()
    {
        if (objsToDestroy.Count == 0)
        {
            return;
        }

        var auxObjsToDestroy = new List<GameObject>();
        foreach (var obj in objsToDestroy)
        {
            var ingEvents = obj.GetComponent<RecipeObjectEvents>();
            var ingEvents2 = obj.GetComponent<IngredientObjectEvents>();
            if ((ingEvents != null && !ingEvents.isBeingHeld) || (ingEvents2 != null && !ingEvents2.isBeingHeld))
            {
                _animator.Play(0);
                _animator.Rebind();
                _animator.Update(0.0f);

                _sound.pitch = Random.Range(0.8f, 1.2f);
                _sound.Play();
                
                auxObjsToDestroy.Add(obj);
            }
        }

        foreach (var obj in auxObjsToDestroy)
        {
            objsToDestroy.Remove(obj);
            Destroy(obj);
        }
    }
}

public class IngredientSpawner : 
    MonoBehaviour, 
    IPointerClickHandler, 
    IPointerEnterHandler, 
    IPointerExitHandler
{
    public Camera cam;
    public Ingredient template;

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        GameGlobals.gameManager.cursorOverlapBuffer.Add(GameGlobals.gameManager.cursorTexturePicking);
    }
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        GameGlobals.gameManager.cursorOverlapBuffer.Remove(GameGlobals.gameManager.cursorTexturePicking);
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
                Ingredient newIng = new Ingredient(
                     false, 
                     cam, 
                     new Vector3( 350, 
                        3 + template.GameObject.transform.position.y, 
                        100),
                     template.StateObjectPaths,
                     template.Attributes,
                     template.TimeToProcess
                     );
                newIng.GameObject.transform.position = transform.position + Vector3.back*25.0f;
            }
        }
    }
}

public class DeliveryBoardEvents : 
    MonoBehaviour, 
    IPointerClickHandler, 
    IPointerEnterHandler, 
    IPointerExitHandler
{
    private List<RecipeObjectEvents> _recipes;

    public void Start()
    {
        _recipes = new List<RecipeObjectEvents>();   
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        GameGlobals.gameManager.cursorOverlapBuffer.Add(GameGlobals.gameManager.cursorTextureDelivering);
    }
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        GameGlobals.gameManager.cursorOverlapBuffer.Remove(GameGlobals.gameManager.cursorTextureDelivering);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        var recEvents = other.GetComponent<RecipeObjectEvents>();
        if (recEvents != null)
        {
            _recipes.Add(recEvents);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        var recEvents = other.GetComponent<RecipeObjectEvents>();
        if (recEvents != null)
        {
            _recipes.Remove(recEvents);
        }
    }
    
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        List<Order> ordersToRemove = new List<Order>();
        GameManager gm = GameGlobals.gameManager;
        foreach (Order order in gm.currOrders)
        {
            List<RecipeObjectEvents> validRecipes = 
                gm.EvaluateOrder(order, _recipes);
            if (validRecipes.Count == order.Recipes.Count)
            {
                for (int i=0; i<order.Recipes.Count; i++)
                {
                    gm.scoreValueObj.text =
                        (int.Parse(gm.scoreValueObj.text) + 
                         GameGlobals.GameConfigs.ScoreMultiplier).ToString();
                }

                foreach (var recipe in validRecipes)
                {
                    Destroy(recipe.gameObject);
                }
                _recipes = _recipes.Except(validRecipes).ToList();
                Destroy(order.GameObject);

                ordersToRemove.Add(order);
                
            }
        }
    
        Animator anim = gm.orderContainer.GetComponent<Animator>();
        if (ordersToRemove.Count > 0)
        {
            anim.Update(0.0f);
            anim.Play("Warn");
            
            gm.currOrders = gm.currOrders.Except(ordersToRemove).ToList();
            gm.orderValidScreen
                .GetComponent<Animator>().Play("Warn");
            gm.orderDeliveredSound.Play();
        }
        else if(_recipes.Count > 0)
        {
            anim.Update(0.0f);
            anim.Play("Warn");
            
            gm.orderNotFoundScreen
                .GetComponent<Animator>().Play("Warn");
            gm.orderNotFoundSound.Play();
            
        }
        
    }
    
}

public class GameManager : MonoBehaviour
{
    public AudioSource orderReceivedSound;
    public AudioSource orderDeliveredSound;
    public AudioSource orderNotFoundSound;
    
    
    public TextMeshPro scoreValueObj;

    public GameObject orderPrefab;
    public GameObject orderContainer;
    public GameObject orderValidScreen;
    public GameObject orderNotFoundScreen;

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
    public List<GameObject> coleffMachineTubes;
    
    
    public List<GameObject> ingredientSpawners;
    public GameObject deliveryBoardObj;
    
    
    public List<GameObject> trashBins;
    
    public List<GameObject> cameraPositioners;
    private int _currCameraSection;
    public List<Button> cameraChangeButtons;
    
    public float repeatRate;

    public Button resetButton;
//    public Button quitButton;

    private float _playingTime;
    private float _initialPlayingTime;


    public List<Texture2D> cursorOverlapBuffer;
    public Texture2D cursorTextureFinger;
    public Texture2D cursorTextureStop;
    public Texture2D cursorTextureDrag;
    public Texture2D cursorTextureProcess;
    public Texture2D cursorTextureWand; 
    public Texture2D cursorTexturePicking;
    public Texture2D cursorTextureTrashing;
    public Texture2D cursorTextureDelivering;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 cursorHotSpot = Vector2.zero;
    
    
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

    IEnumerator IncreaseOrderDifficulty()
    {
        yield return new WaitForSeconds(GameGlobals.GameConfigs.SurvivalIncreaseDifficultyDelay);
        if (GameGlobals.GameConfigs.OrderDifficulty <
            GameGlobals.GameConfigs.OrderRecipesByLevel.Count())
        {
            GameGlobals.GameConfigs.OrderDifficulty++;
        }
        StartCoroutine(IncreaseOrderDifficulty());
    }


    IEnumerator GenerateOrder()
    {
        if (GameGlobals.IsTraining && currOrders.Count == GameGlobals.GameConfigs.MAXPendingOrders)
        {
            yield return new WaitForSeconds(repeatRate);
            StartCoroutine(GenerateOrder());
        }
        else
        {
            Order newOrder = new Order();
            int numRecipes = GameGlobals.GameConfigs.OrderDifficulty
                             / Random.Range(1, GameGlobals.GameConfigs.OrderDifficulty + 1);
            for (int i = 0; i < numRecipes; i++)
            {
                List<Recipe> orderRecipes =
                    GameGlobals.GameConfigs.
                        OrderRecipesByLevel[(GameGlobals.GameConfigs.OrderDifficulty / numRecipes) - 1];
                newOrder.AddRecipe(orderRecipes[Random.Range(0, orderRecipes.Count)]);
            }

            currOrders.Add(newOrder);
            newOrder.PrintOrder(orderPrefab, cam, 
                orderContainer.transform.GetChild(0).gameObject);

            //decrement repeatRate on survival
            if (!GameGlobals.IsTraining)
            {
                repeatRate =
                    (repeatRate > GameGlobals.GameConfigs.MINOrderTime) ? 
                        repeatRate * GameGlobals.GameConfigs.SurvivalTimeChangeRate : repeatRate;
            }
            
            orderReceivedSound.Play();
            
            yield return new WaitForSeconds(repeatRate);
            StartCoroutine(GenerateOrder());
        }
    }

    public List<RecipeObjectEvents> EvaluateOrder(Order selectedOrder, List<RecipeObjectEvents> userRecipes)
    {
        List<RecipeObjectEvents> validRecipes = new List<RecipeObjectEvents>();
        
        foreach (var orderRecipe in selectedOrder.Recipes)
        {
            foreach (var userRecipe in userRecipes)
            {
                if (!validRecipes.Contains(userRecipe) && ValidateRecipe(orderRecipe, userRecipe.logic))
                {
                    validRecipes.Add(userRecipe);
                    break;
                }
            }
        }
        
        return validRecipes;
    }

    private bool ValidateRecipe(Recipe orderRecipe, Recipe userRecipe)
    {
        if (orderRecipe.IngredientAttrs.Count != userRecipe.IngredientAttrs.Count)
        {
            return false;
        }

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

        return (numValidIngs > 0 && numValidIngs == orderRecipe.IngredientAttrs.Count);
    }


    public void IncreaseCameraSection(Button invoker)
    {
        invoker.gameObject.GetComponent<AudioSource>().Play();
        _currCameraSection =
            (_currCameraSection < (cameraPositioners.Count - 1)) ? _currCameraSection + 1 : _currCameraSection;
        cam.transform.parent = cameraPositioners[_currCameraSection].transform;
        cam.transform.localPosition = new Vector3();
    }
    public void DecreaseCameraSection(Button invoker)
    {
        invoker.gameObject.GetComponent<AudioSource>().Play();
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
    
    
    public void MockedStartScene()
    {
        string path =  Application.streamingAssetsPath + "/configs.cfg";
        StreamReader reader = new StreamReader(path);
        string json = reader.ReadToEnd();
        GameGlobals.GameConfigs = 
            JsonConvert.DeserializeObject<GameConfigurations>(json);
        reader.Close();
        
        GameGlobals.IsTraining = true;
    }
    

    // Start is called before the first frame update
    void Start()
    {
        _initialPlayingTime = Time.time;
        if (GameGlobals.GameConfigs == null)
        {
            MockedStartScene();
        }

        GameGlobals.gameManager = this;
        cursorOverlapBuffer.Add(cursorTextureFinger);
        
        resetButton.gameObject.SetActive(GameGlobals.IsTutorial);
//        quitButton.gameObject.SetActive(GameGlobals.HasControls);
        if (GameGlobals.IsTutorial)
        {
            resetButton.onClick.AddListener(delegate
            {
                QuitMainScene();
            });
        }

        new FoodProcessor(juiceBlenderObj,
            new List<IngredientAttr> {IngredientAttr.FRUIT,IngredientAttr.HEFTT},
            new List<IngredientAttr> {IngredientAttr.HEFTT},
            new List<IngredientAttr> {IngredientAttr.AMERM},
            new List<IngredientAttr> {IngredientAttr.THIRPUNASOREC},
            1,
            new List<FoodProcessor>(),
            3);
        new FoodProcessor(juiceKnifeObj,
            new List<IngredientAttr> {IngredientAttr.FRUIT,IngredientAttr.LUAHH},
            new List<IngredientAttr> {IngredientAttr.LUAHH},
            new List<IngredientAttr> {IngredientAttr.HEFTT},
            new List<IngredientAttr>(),
            1,
            new List<FoodProcessor>(),
            0);

        
        new FoodProcessor(dessertBlenderObj,
            new List<IngredientAttr> {IngredientAttr.COLEFF,IngredientAttr.LUAHH},
            new List<IngredientAttr> {IngredientAttr.LUAHH},
            new List<IngredientAttr> {IngredientAttr.HEFTT},
            new List<IngredientAttr>(),
            1,
            new List<FoodProcessor>(),
            0);
        
        foreach (GameObject coleffMachineTube in coleffMachineTubes)
        {
            FoodProcessor coleffProc1 = new FoodProcessor(coleffMachineTube,
                new List<IngredientAttr> {IngredientAttr.COLEFF, IngredientAttr.AMERM, IngredientAttr.WUDIF},
                new List<IngredientAttr> {IngredientAttr.WUDIF},
                new List<IngredientAttr> {IngredientAttr.KREW},
                new List<IngredientAttr>(),
                1,
                new List<FoodProcessor>(),
                5);
            FoodProcessor coleffProc2 = new  FoodProcessor(coleffMachineTube,
                new List<IngredientAttr> {IngredientAttr.COLEFF, IngredientAttr.AMERM, IngredientAttr.KREW},
                new List<IngredientAttr> {IngredientAttr.KREW},
                new List<IngredientAttr> {IngredientAttr.FRUB},
                new List<IngredientAttr>(),
                1,
                new List<FoodProcessor>(),
                5);
            
            new FoodProcessor(coleffMachineTube,
                new List<IngredientAttr> {IngredientAttr.COLEFF, IngredientAttr.HEFTT},
                new List<IngredientAttr> {IngredientAttr.HEFTT},
                new List<IngredientAttr> {IngredientAttr.AMERM, IngredientAttr.WUDIF},
                new List<IngredientAttr> {IngredientAttr.DESSERT, IngredientAttr.THIRPUNASOREC},
                1,
                new List<FoodProcessor>(){coleffProc1, coleffProc2},
                5);
        }
        
        new FoodProcessor(dessertKnifeObj,
            new List<IngredientAttr> {IngredientAttr.CAKE,IngredientAttr.LUAHH},
            new List<IngredientAttr> {IngredientAttr.LUAHH},
            new List<IngredientAttr> {IngredientAttr.HEFTT},
            new List<IngredientAttr>(),
            1,
            new List<FoodProcessor>(),
            0);
        
        
        currOrders = new List<Order>();
        

        foreach (var trashBin in trashBins)
        {
            trashBin.AddComponent<TrashBinObjectEvents>();
        }
        
        deliveryBoardObj.AddComponent<DeliveryBoardEvents>();
        
        foreach (var combinerObj in foodCombinerObjs)
        {
            new FoodCombiner(combinerObj, deliveryBagPrefab, cam);
        }
        
        InitFruitSectionSpawners();

        repeatRate = (GameGlobals.IsTraining) ? 0.25f: GameGlobals.GameConfigs.MAXOrderTime;
        
        StartCoroutine(GenerateOrder()); //Random.Range(gameConfig.MINOrderTime, gameConfig.MAXOrderTime));
        
        //increase difficulty on survival
        if (!GameGlobals.IsTraining)
        {
            StartCoroutine(IncreaseOrderDifficulty());
        }
        

        _currCameraSection = 0;
        cam.transform.parent = cameraPositioners[0].transform;

        foreach (var button in cameraChangeButtons)
        {
            button.gameObject.AddComponent<DirectionalButtonEvents>();
        }
        cameraChangeButtons[0].onClick.AddListener(() => IncreaseCameraSection(cameraChangeButtons[0]));
        cameraChangeButtons[1].onClick.AddListener(() => DecreaseCameraSection(cameraChangeButtons[1]));
    }

    public void QuitMainScene()
    {
        GameGlobals.PlayingTime = _playingTime;
        GameGlobals.Score = float.Parse(scoreValueObj.text);
            
        if (GameGlobals.IsTraining)
        {
            SceneManager.LoadScene("EndSceneTraining");
        }
        else
        {
            SceneManager.LoadScene("EndSceneSurvival");
        }
    }
    public void Update()
    {
        Cursor.SetCursor(cursorOverlapBuffer[cursorOverlapBuffer.Count - 1],
            cursorHotSpot,
            cursorMode);
        
        _playingTime = Time.time - _initialPlayingTime;
        if(GameGlobals.IsTraining && _playingTime >= GameGlobals.GameConfigs.TrainingTimeMinutes*60.0f ||
           !GameGlobals.IsTraining && currOrders.Count > GameGlobals.GameConfigs.MAXPendingOrders ||
           !GameGlobals.IsTraining && _playingTime > GameGlobals.GameConfigs.MAXSurvivalTimeMinutes*60.0f)
        { 
            QuitMainScene();
        }
    }
}
