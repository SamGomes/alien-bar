using System.Collections.Generic;

public class GameConfigsPreset
{
    public static GameConfigurations GameConfigs = new GameConfigurations()
            {
                IsDemo= true,
                ScoreMultiplier= 500,
                OrderDifficulty= 1,
                MINOrderTime= 8.0f,
                MAXOrderTime= 12.0f,
                MAXPendingOrders= 5,

                TutorialTimeMinutes= 3,
                MAXTrainingTimeMinutes= 10,
                MAXSurvivalTimeMinutes= 0.05f,
      
                SurvivalIncreaseDifficultyDelay= 60,
                SurvivalTimeChangeRate= 1.10f,

                IngredientConfigs= new List<IngredientConfigurations>
                {
                    new IngredientConfigurations(){
                        IngredientPrefabs= new List<string>{
                            "Models/Cup/Cup"
                        },
                        IngredientAttrs= new List<IngredientAttr>{
                            IngredientAttr.UTENSIL,
                            IngredientAttr.THIRPUNASOREC
                        }
                    },
                    new IngredientConfigurations(){
                        IngredientPrefabs= new List<string>{
                            "Models/Drink/Akwa/1"
                        },
                        IngredientAttrs= new List<IngredientAttr>{
                            IngredientAttr.AMERM,
                            IngredientAttr.AKWA
                        }
                    },
                    new IngredientConfigurations(){
                        IngredientPrefabs= new List<string>{
                            "Models/Drink/Aloc/1"
                        },
                        IngredientAttrs= new List<IngredientAttr>{
                            IngredientAttr.AMERM,
                            IngredientAttr.ALOC
                        }
                    },
                    new IngredientConfigurations(){
                        IngredientPrefabs= new List<string>{
                            "Models/Fruits/Orgeine/1",
                            "Models/Fruits/Orgeine/2",
                            "Models/Fruits/Orgeine/3"
                        },
                        IngredientAttrs= new List<IngredientAttr>{
                            IngredientAttr.FRUIT,
                            IngredientAttr.ORGEINE,
                            IngredientAttr.LUAHH
                        }
                    },
                    new IngredientConfigurations(){
                        IngredientPrefabs= new List<string>{
                            "Models/Fruits/Liem/1",
                            "Models/Fruits/Liem/2",
                            "Models/Fruits/Liem/3"
                        },
                        IngredientAttrs= new List<IngredientAttr>{
                            IngredientAttr.FRUIT,
                            IngredientAttr.LIEM,
                            IngredientAttr.LUAHH
                        }
                    },
                    new IngredientConfigurations(){
                        IngredientPrefabs= new List<string>{
                            "Models/Fruits/Appia/1",
                            "Models/Fruits/Appia/2",
                            "Models/Fruits/Appia/3"
                        },
                        IngredientAttrs= new List<IngredientAttr>{
                            IngredientAttr.FRUIT,
                            IngredientAttr.APPIA,
                            IngredientAttr.LUAHH
                        }
                    },
                    new IngredientConfigurations(){
                        IngredientPrefabs= new List<string>{
                            "Models/Desserts/Cup/1"
                        },
                        IngredientAttrs= new List<IngredientAttr>{
                            IngredientAttr.UTENSIL,
                            IngredientAttr.DESSERT,
                            IngredientAttr.THIRPUNASOREC
                        }
                    },
                    new IngredientConfigurations(){
                        IngredientPrefabs= new List<string>{
                            "Models/Desserts/Coleff/1",
                            "Models/Desserts/Coleff/2",
                            "Models/Desserts/Coleff/3",
                            "Models/Desserts/Coleff/4",
                            "Models/Desserts/Coleff/5"
                        },
                        IngredientAttrs= new List<IngredientAttr>{
                            IngredientAttr.COLEFF,
                            IngredientAttr.DESSERT,
                            IngredientAttr.LUAHH
                        }
                    }
                },
                
                OrderRecipesByLevel = new List<List<Recipe>>()
                    {
                        new List<Recipe>()
                        {
                            new Recipe("Akwa di Jrn",
                                new List<List<IngredientAttr>>()
                                {
                                    new List<IngredientAttr>()
                                    {
                                        IngredientAttr.AMERM,
                                        IngredientAttr.AKWA
                                    }
                                }, 1),
                            new Recipe("Aloc di Jrn",
                                new List<List<IngredientAttr>>()
                                {
                                    new List<IngredientAttr>()
                                    {
                                        IngredientAttr.AMERM,
                                        IngredientAttr.ALOC
                                    }
                                }, 1),
                            new Recipe("Thirpunasorec",
                                new List<List<IngredientAttr>>()
                                {
                                    new List<IngredientAttr>()
                                    {
                                        IngredientAttr.UTENSIL,
                                        IngredientAttr.THIRPUNASOREC
                                    }
                                }, 1),
                            new Recipe("Orgeine Luahh",
                                new List<List<IngredientAttr>>()
                                {
                                    new List<IngredientAttr>()
                                    {
                                        IngredientAttr.FRUIT,
                                        IngredientAttr.ORGEINE,
                                        IngredientAttr.LUAHH
                                    }
                                }, 1),
                            new Recipe("Liem Luahh",
                                new List<List<IngredientAttr>>()
                                {
                                    new List<IngredientAttr>()
                                    {
                                        IngredientAttr.FRUIT,
                                        IngredientAttr.LIEM,
                                        IngredientAttr.LUAHH
                                    }
                                }, 1),
                            new Recipe("Appia Luahh",
                                new List<List<IngredientAttr>>()
                                {
                                    new List<IngredientAttr>()
                                    {
                                        IngredientAttr.FRUIT,
                                        IngredientAttr.APPIA,
                                        IngredientAttr.LUAHH
                                    }
                                }, 1)
                        },
                        
                        new List<Recipe>()
                        {
                            new Recipe("Orgeine Heftt",
                                new List<List<IngredientAttr>>()
                                {
                                    new List<IngredientAttr>()
                                    {
                                        IngredientAttr.FRUIT,
                                        IngredientAttr.ORGEINE,
                                        IngredientAttr.HEFTT
                                    }
                                }, 2),
                            new Recipe("Liem Heftt",
                                new List<List<IngredientAttr>>()
                                {
                                    new List<IngredientAttr>()
                                    {
                                        IngredientAttr.FRUIT,
                                        IngredientAttr.LIEM,
                                        IngredientAttr.HEFTT
                                    }
                                }, 2),
                            new Recipe("Appia Heftt",
                                new List<List<IngredientAttr>>()
                                {
                                    new List<IngredientAttr>()
                                    {
                                        IngredientAttr.FRUIT,
                                        IngredientAttr.APPIA,
                                        IngredientAttr.HEFTT
                                    }
                                }, 2)
                        },
                        
                        new List<Recipe>()
                        {
                            new Recipe("Amerm -L'Orgeine",
                                new List<List<IngredientAttr>>()
                                {
                                    new List<IngredientAttr>()
                                    {
                                        IngredientAttr.FRUIT,
                                        IngredientAttr.ORGEINE,
                                        IngredientAttr.AMERM
                                    }
                                }, 3),
                            new Recipe("Amerm -L'Liem",
                                new List<List<IngredientAttr>>()
                                {
                                    new List<IngredientAttr>()
                                    {
                                        IngredientAttr.FRUIT,
                                        IngredientAttr.LIEM,
                                        IngredientAttr.AMERM
                                    }
                                }, 3),
                            new Recipe("Amerm -L'Appia",
                                new List<List<IngredientAttr>>()
                                {
                                    new List<IngredientAttr>()
                                    {
                                        IngredientAttr.FRUIT,
                                        IngredientAttr.APPIA,
                                        IngredientAttr.AMERM
                                    }
                                }, 3)
                        },
                        
                        new List<Recipe>()
                        {
                            new Recipe("Coleff d'Amerm Wudif",
                                new List<List<IngredientAttr>>()
                                {
                                    new List<IngredientAttr>()
                                    {
                                        IngredientAttr.DESSERT,
                                        IngredientAttr.COLEFF,
                                        IngredientAttr.AMERM,
                                        IngredientAttr.WUDIF,
                                    }
                                }, 4),
                            new Recipe("Coleff d'Amerm Krew",
                                new List<List<IngredientAttr>>()
                                {
                                    new List<IngredientAttr>()
                                    {
                                        IngredientAttr.DESSERT,
                                        IngredientAttr.COLEFF,
                                        IngredientAttr.AMERM,
                                        IngredientAttr.KREW,
                                    }
                                }, 4),
                            new Recipe("Coleff d'Amerm Frub",
                                new List<List<IngredientAttr>>()
                                {
                                    new List<IngredientAttr>()
                                    {
                                        IngredientAttr.DESSERT,
                                        IngredientAttr.COLEFF,
                                        IngredientAttr.AMERM,
                                        IngredientAttr.FRUB,
                                    }
                                }, 4)
                        },
                        
                        new List<Recipe>()
                        {
                            new Recipe("Coleff Wudif -L'Orgeine",
                                new List<List<IngredientAttr>>()
                                {
                                    new List<IngredientAttr>()
                                    {
                                        IngredientAttr.AMERM,
                                        IngredientAttr.AKWA
                                    },
                                    new List<IngredientAttr>()
                                    {
                                        IngredientAttr.AMERM,
                                        IngredientAttr.AKWA
                                    },
                                    new List<IngredientAttr>()
                                    {
                                        IngredientAttr.AMERM,
                                        IngredientAttr.AKWA
                                    },
                                    new List<IngredientAttr>()
                                    {
                                        IngredientAttr.DESSERT,
                                        IngredientAttr.COLEFF,
                                        IngredientAttr.AMERM,
                                        IngredientAttr.WUDIF,
                                    },
                                    new List<IngredientAttr>()
                                    {
                                        IngredientAttr.FRUIT,
                                        IngredientAttr.ORGEINE,
                                        IngredientAttr.AMERM
                                    }
                                }, 5),
                            new Recipe("Coleff Frub -L'Orgeine",
                                new List<List<IngredientAttr>>()
                                {
                                    new List<IngredientAttr>()
                                    {
                                        IngredientAttr.AMERM,
                                        IngredientAttr.AKWA
                                    },
                                    new List<IngredientAttr>()
                                    {
                                        IngredientAttr.DESSERT,
                                        IngredientAttr.COLEFF,
                                        IngredientAttr.AMERM,
                                        IngredientAttr.FRUB,
                                    },
                                    new List<IngredientAttr>()
                                    {
                                        IngredientAttr.FRUIT,
                                        IngredientAttr.ORGEINE,
                                        IngredientAttr.AMERM
                                    }
                                }, 5),
                            new Recipe("Coleff Krew -L'Orgeine",
                                new List<List<IngredientAttr>>()
                                {
                                    new List<IngredientAttr>()
                                    {
                                        IngredientAttr.AMERM,
                                        IngredientAttr.AKWA
                                    },
                                    new List<IngredientAttr>()
                                    {
                                        IngredientAttr.AMERM,
                                        IngredientAttr.AKWA
                                    },
                                    new List<IngredientAttr>()
                                    {
                                        IngredientAttr.DESSERT,
                                        IngredientAttr.COLEFF,
                                        IngredientAttr.AMERM,
                                        IngredientAttr.KREW,
                                    },
                                    new List<IngredientAttr>()
                                    {
                                        IngredientAttr.FRUIT,
                                        IngredientAttr.ORGEINE,
                                        IngredientAttr.AMERM
                                    }
                                }, 5),
                            new Recipe("Amerm -L'Orgappia",
                                new List<List<IngredientAttr>>()
                                {
                                    new List<IngredientAttr>()
                                    {
                                        IngredientAttr.AMERM,
                                        IngredientAttr.AKWA
                                    },
                                    new List<IngredientAttr>()
                                    {
                                        IngredientAttr.FRUIT,
                                        IngredientAttr.ORGEINE,
                                        IngredientAttr.AMERM
                                    },
                                    new List<IngredientAttr>()
                                    {
                                        IngredientAttr.FRUIT,
                                        IngredientAttr.ORGEINE,
                                        IngredientAttr.AMERM
                                    },
                                    new List<IngredientAttr>()
                                    {
                                        IngredientAttr.FRUIT,
                                        IngredientAttr.APPIA,
                                        IngredientAttr.AMERM
                                    }
                                }, 5),
                            new Recipe("Amerm -L'Liemgeine",
                                new List<List<IngredientAttr>>()
                                {
                                    new List<IngredientAttr>()
                                    {
                                        IngredientAttr.AMERM,
                                        IngredientAttr.AKWA
                                    },
                                    new List<IngredientAttr>()
                                    {
                                        IngredientAttr.FRUIT,
                                        IngredientAttr.ORGEINE,
                                        IngredientAttr.AMERM
                                    },
                                    new List<IngredientAttr>()
                                    {
                                        IngredientAttr.FRUIT,
                                        IngredientAttr.ORGEINE,
                                        IngredientAttr.AMERM
                                    },
                                    new List<IngredientAttr>()
                                    {
                                        IngredientAttr.FRUIT,
                                        IngredientAttr.LIEM,
                                        IngredientAttr.AMERM
                                    }
                                }, 5),
                            new Recipe("Amerm -L'Tut",
                                new List<List<IngredientAttr>>()
                                {
                                    new List<IngredientAttr>()
                                    {
                                        IngredientAttr.AMERM,
                                        IngredientAttr.AKWA
                                    },
                                    new List<IngredientAttr>()
                                    {
                                        IngredientAttr.FRUIT,
                                        IngredientAttr.ORGEINE,
                                        IngredientAttr.AMERM
                                    },
                                    new List<IngredientAttr>()
                                    {
                                        IngredientAttr.FRUIT,
                                        IngredientAttr.LIEM,
                                        IngredientAttr.AMERM
                                    },
                                    new List<IngredientAttr>()
                                    {
                                        IngredientAttr.FRUIT,
                                        IngredientAttr.APPIA,
                                        IngredientAttr.AMERM
                                    }
                                }, 5)
                        }
                    }
            };
 
}
