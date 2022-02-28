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
                     template.IsUtensil(),
                     cam, 
                     new Vector3( hit.point.x, 
                        hit.point.y + template.GameObject.transform.position.y, 
                        hit.point.z),
                     cuttingTable,
                     template.GETStateObjects(),
                     template.GETAttributes(),
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
            if (otherIng.IsUtensil())
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
    public Camera _cam;
    public GameObject _cuttingTable;

    public GameObject _juiceFoodProcessorObj;
    public GameObject _knifeObj;
    public GameObject _burner1Obj;
    public GameObject _burner2Obj;
    public GameObject _burner3Obj;
    public GameObject _burner4Obj;
    public GameObject _coffeeMachineObj;
    public GameObject _freezerObj;

    public List<GameObject> _orangePrefabs;
    public List<GameObject> _lemonPrefabs;
    public List<GameObject> _applePrefabs;
    
    public GameObject _cupPrefab;
    
    private FoodProcessor _juiceFoodProcessor;
    private FoodProcessor _knife;
    
    public GameObject _orangeSpawner;
    public GameObject _lemonsSpawner;
    public GameObject _applesSpawner;
    
    
    public GameObject _cupsSpawner;

    
    public GameObject _board;

    void InitSpawner(GameObject spawnerObj, Ingredient template)
    {
        spawnerObj.AddComponent<IngredientSpawner>();
        var spawner = spawnerObj.GetComponent<IngredientSpawner>();
        spawner.cuttingTable = _cuttingTable;
        spawner.cam = _cam;
        spawner.template = template;
    }
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        _juiceFoodProcessor = new FoodProcessor(_juiceFoodProcessorObj,
            new List<IngredientAttr> {IngredientAttr.FRUIT,IngredientAttr.CUT},
            new List<IngredientAttr> {IngredientAttr.FRUIT,IngredientAttr.CUT},
            new List<IngredientAttr> {IngredientAttr.DRINK},
            new List<IngredientAttr> {IngredientAttr.CUP},
            1);
        _knife = new FoodProcessor(_knifeObj,
            new List<IngredientAttr> {IngredientAttr.WHOLE},
            new List<IngredientAttr> {IngredientAttr.WHOLE},
            new List<IngredientAttr> {IngredientAttr.CUT},
            new List<IngredientAttr>(),
            1);

        InitSpawner(_orangeSpawner,
        new Ingredient(true,false,  
            _cam,new Vector3(), _cuttingTable, 
            _orangePrefabs,
            new List<IngredientAttr> {IngredientAttr.FRUIT, IngredientAttr.ORANGE, IngredientAttr.WHOLE},
            0)
        );
        
        InitSpawner(_lemonsSpawner,
            new Ingredient(true, false, 
                _cam,new Vector3(), _cuttingTable, 
                _lemonPrefabs,
                new List<IngredientAttr> {IngredientAttr.FRUIT, IngredientAttr.LEMON, IngredientAttr.WHOLE},
                0)
        );

        InitSpawner(_applesSpawner,
            new Ingredient(true, false, 
                _cam,new Vector3(), _cuttingTable, 
                _applePrefabs, 
                new List<IngredientAttr> {IngredientAttr.FRUIT, IngredientAttr.APPLE, IngredientAttr.WHOLE}, 
                0)
        );

        InitSpawner(_cupsSpawner,
            new Ingredient(true,true,  
                _cam,new Vector3(), _cuttingTable, new List<GameObject> {_cupPrefab},
                new List<IngredientAttr> {IngredientAttr.CUP},
                0)
        );
        
        
    }

    // public void Update()
    // {
    //     _applesSpawner.transform.Translate(new Vector3());
    // }
}
