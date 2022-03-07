using System.Collections.Generic;
using UnityEngine;

public class FoodCombinerObjectEvents : MonoBehaviour
{
    public FoodCombiner logic;
}
    
public class FoodCombiner
{
    private GameObject _gameObject;
    
    
    public GameObject GameObject
    {
        get => _gameObject;
        set => _gameObject = value;
    }

    public FoodCombiner(GameObject gameObject, 
        List<List<IngredientAttr>> attributes)
    {
        _gameObject = gameObject;
        _gameObject.AddComponent<FoodCombinerObjectEvents>();
        _gameObject.GetComponent<FoodCombinerObjectEvents>().logic = this;
    }
}