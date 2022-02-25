using System.Collections;
using System.Collections.Generic;
using FoodProcessorStuff;
using IngredientStuff;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject _juiceFoodProcessorObj;
    public GameObject _knife;
    public GameObject _burner1Obj;
    public GameObject _burner2Obj;
    public GameObject _burner3Obj;
    public GameObject _burner4Obj;
    public GameObject _coffeeMachineObj;
    public GameObject _freezerObj;

    public GameObject _orangePrefab;
    public GameObject _lemonPrefab;
    public GameObject _applePrefab;
    
    private FoodProcessor _juiceFoodProcessor;
    private Ingredient _orange;
    
    // Start is called before the first frame update
    void Start()
    {
        _juiceFoodProcessor = new FoodProcessor(_juiceFoodProcessorObj,
            new List<IngredientAttr> {IngredientAttr.FRUIT,IngredientAttr.CUT},
            new List<IngredientAttr> {IngredientAttr.FRUIT,IngredientAttr.DRINK},
            10);
        _knife = new FoodProcessor(_juiceFoodProcessorObj,
            new List<IngredientAttr> {IngredientAttr.WHOLE},
            new List<IngredientAttr> {IngredientAttr.CUT},
            10);
        _orange = new Ingredient(new List<GameObject> {_orangePrefab},
            new List<IngredientAttr> {IngredientAttr.FRUIT,IngredientAttr.WHOLE},
            ,"orange",2)
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
