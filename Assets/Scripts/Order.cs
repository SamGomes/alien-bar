using System.Collections.Generic;
public class Recipe
{
    private List<Ingredient> _ingredients;
    private int _level;

    public List<Ingredient> Ingredients
    {
        get => _ingredients;
        set => _ingredients = value;
    }

    public int Level
    {
        get => _level;
        set => _level = value;
    }

}

public class Order
{
    private List<Recipe> _recipes;
    private int _level;

    public List<Recipe> Recipes
    {
        get => _recipes;
        set => _recipes = value;
    }

    public int Level
    {
        get
        {
            int level = 0;
            foreach(Recipe recipe in _recipes)
            {
                level += recipe.Level;
            }
            return level;
        }
    }

}