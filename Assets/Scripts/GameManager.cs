using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;


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
                     cam, 
                     new Vector3( hit.point.x, 
                        hit.point.y + template.GETGameObject().transform.position.y, 
                        hit.point.z),
                     cuttingTable,
                     template.GETStateObjects(),
                     template.GETAttributes(),
                     template.GETName(),
                     template.GETTimeToProcess()
                     );
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
    public GameObject _lemonPrefab;
    public GameObject _applePrefab;
    
    private FoodProcessor _juiceFoodProcessor;
    private FoodProcessor _knife;
    
    public GameObject _orangeSpawner;
    public GameObject _lemonsSpawner;
    public GameObject _applesSpawner;

    
    
    // Start is called before the first frame update
    void Start()
    {
        _juiceFoodProcessor = new FoodProcessor(_juiceFoodProcessorObj,
            new List<IngredientAttr> {IngredientAttr.FRUIT,IngredientAttr.CUT},
            new List<IngredientAttr> {IngredientAttr.CUT},
            new List<IngredientAttr> {IngredientAttr.DRINK},
            1);
        _knife = new FoodProcessor(_knifeObj,
            new List<IngredientAttr> {IngredientAttr.WHOLE},
            new List<IngredientAttr> {IngredientAttr.WHOLE},
            new List<IngredientAttr> {IngredientAttr.CUT},
            1);

        _orangeSpawner.AddComponent<IngredientSpawner>();
        var orangeBowlSpawner = _orangeSpawner.GetComponent<IngredientSpawner>();
        orangeBowlSpawner.cuttingTable = _cuttingTable;
        orangeBowlSpawner.cam = _cam;
        orangeBowlSpawner.template  = new Ingredient(true, 
            _cam,new Vector3(), _cuttingTable, new List<GameObject> {_orangePrefabs[0], _orangePrefabs[1]},
            new List<IngredientAttr> {IngredientAttr.FRUIT, IngredientAttr.WHOLE},
            "orange", 0);
        
        _lemonsSpawner.AddComponent<IngredientSpawner>();
        var lemonsBowlSpawner = _lemonsSpawner.GetComponent<IngredientSpawner>();
        lemonsBowlSpawner.cuttingTable = _cuttingTable;
        lemonsBowlSpawner.cam = _cam;
        lemonsBowlSpawner.template  = new Ingredient(true, 
            _cam,new Vector3(), _cuttingTable, new List<GameObject> {_lemonPrefab},
            new List<IngredientAttr> {IngredientAttr.FRUIT, IngredientAttr.WHOLE},
            "lemon", 0);
        
        _applesSpawner.AddComponent<IngredientSpawner>();
        var applesBowlSpawner = _applesSpawner.GetComponent<IngredientSpawner>();
        applesBowlSpawner.cuttingTable = _cuttingTable;
        applesBowlSpawner.cam = _cam;
        applesBowlSpawner.template  = new Ingredient(true, 
            _cam,new Vector3(), _cuttingTable, new List<GameObject> {_applePrefab},
            new List<IngredientAttr> {IngredientAttr.FRUIT, IngredientAttr.WHOLE},
            "apple", 0);
    }

    // public void Update()
    // {
    //     _applesSpawner.transform.Translate(new Vector3());
    // }
}
