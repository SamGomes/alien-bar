using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FoodCombinerObjectEvents : 
    MonoBehaviour, 
    IPointerClickHandler,
    IPointerEnterHandler, 
    IPointerExitHandler
{
    public FoodCombiner logic;
    private Vector3 _baseScale;
    public Camera cam;

    public List<IngredientObjectEvents> objectsToComb;

    private ParticleSystem _particleSystem;
    private AudioSource _sound;

    
    public void Start()
    {
        _sound = GetComponent<AudioSource>();
        _baseScale = transform.localScale;
        _particleSystem = GetComponentInChildren<ParticleSystem>();
    }
    
    public void Update()
    {
        objectsToComb = logic.IngredientsToCombine;
    }
    
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        GameGlobals.GameManager.cursorOverlapBuffer.Add(GameGlobals.GameManager.cursorTextureWand);
    }
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        GameGlobals.GameManager.cursorOverlapBuffer.Remove(GameGlobals.GameManager.cursorTextureWand);
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        //Use this to tell when the user left-clicks on the Button
        if (pointerEventData.button == PointerEventData.InputButton.Left && logic.IngredientsToCombine.Count > 0)
        {
            _particleSystem.Play();
            _sound.Play();
            Recipe userRec = logic.Combine();
            GameObject newDevBag = Instantiate(logic.DeliveryBagPrefab);
            newDevBag.transform.position = transform.position;
            RecipeObjectEvents recEvents = newDevBag.AddComponent<RecipeObjectEvents>();
            recEvents.logic = userRec;
            recEvents.cam = cam;

            foreach (var ing in logic.IngredientsToCombine)
            {
                if(ing != null)
                    Destroy(ing.gameObject);
            }
            logic.IngredientsToCombine.Clear();
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        var ingEvents = other.GetComponent<IngredientObjectEvents>();
        if (ingEvents != null)
        {
            logic.IngredientsToCombine.Add(ingEvents);
//            ingEvents.isBeingHeld = false;
//            float translation = logic.IngredientsToCombine.Count* 10.0f;
//            ingEvents.gameObject.transform.position = gameObject.transform.position + 
//                                                      new Vector3( translation - 30.0f, 0, 0);
        }
        if(logic.IngredientsToCombine.Count > 0)
            transform.localScale = 1.1f * _baseScale;
    }
    
    private void OnTriggerExit(Collider other)
    {
        var ingEvents = other.GetComponent<IngredientObjectEvents>();
        if (ingEvents != null)
        {
            logic.IngredientsToCombine.Remove(ingEvents);
        }
        
        if(logic.IngredientsToCombine.Count < 1)
            transform.localScale = _baseScale;
    }
}

public class FoodCombiner
{
    public List<IngredientObjectEvents> IngredientsToCombine { get; set; }
    public GameObject GameObject { get; set; }
    
    public GameObject DeliveryBagPrefab { get; set; }
    public FoodCombiner(GameObject gameObject, 
        GameObject deliveryBagPrefab,
        Camera cam)
    {
        GameObject = gameObject;
        GameObject.AddComponent<FoodCombinerObjectEvents>();
        GameObject.GetComponent<FoodCombinerObjectEvents>().logic = this;
        GameObject.GetComponent<FoodCombinerObjectEvents>().cam = cam;
        
        DeliveryBagPrefab = deliveryBagPrefab;
        IngredientsToCombine = new List<IngredientObjectEvents>();
    }

    public Recipe Combine()
    {
        List<List<IngredientAttr>> listAttrs = new List<List<IngredientAttr>>();
        string recipeName = "";
        foreach (var ing in IngredientsToCombine)
        {
            listAttrs.Add(ing.logic.Attributes);
            recipeName += ing.ToString();
        }
        Recipe recipe = new Recipe(recipeName,  listAttrs, -1);

        return recipe;
    }
}