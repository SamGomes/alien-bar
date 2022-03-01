using System.Collections.Generic;

public enum OrderSection
{
    Fruits = 0,
    Dessert = 1,
    Pasta = 2
}

public class Recipe
{
    public OrderSection section;
    public List<Ingredient> Ingredients { get; set; }
    public int Level { get; set; }
    
    public Recipe(List<Ingredient> ingredients, int level)
    {
        Ingredients = ingredients;
        Level = level;
    }

}

public class Order
{
    public int Level { get; set; }

    private List<Recipe> _recipes;
    public List<Recipe> Recipes
    {
        get { return _recipes; }
        private set
        {
            _recipes = value;
            int Level = 0;
            foreach(Recipe recipe in _recipes)
            {
                Level += recipe.Level;
            }
        }
    }

    public void AddRecipe(Recipe recipe)
    {
        Recipes.Add(recipe);
        Level += recipe.Level;
    }

    public Order()
    {
        Recipes = new List<Recipe>();
    }

}