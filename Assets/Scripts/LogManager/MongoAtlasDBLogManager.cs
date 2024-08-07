﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using UnityEngine;
using UnityEngine.Networking;



[Serializable]
public class QueryObject
{
    public string find;
    public string sort;
    public int limit;
}

//Debug log manager
public class MongoAtlasLogManager : LogManager
{
    private MonoBehaviour monoBehaviourObject;
    private MongoClient client;

    
    public override void InitLogs(MonoBehaviour monoBehaviourObject)
    {
        this.monoBehaviourObject = monoBehaviourObject;
        this.client = new MongoClient("");
    }
    

    public override IEnumerator WriteToLog(string database, string table, 
        Dictionary<string,string> argsNValues, bool justHeaders)
    {
        //"justHeaders" functionality not implemented!
        var databaseObj = client.GetDatabase(database);
        var document = BsonDocument.Parse(StringifyDictionaryForJsonLogs(argsNValues));
        databaseObj.GetCollection<BsonDocument>(table).InsertOne(document);
        yield return null;
    }

    public override IEnumerator GetFromLog(string database, string table, string query, Func<string, int> yieldedReactionToGet)
    {
        var databaseObj = client.GetDatabase(database);
        var collection = databaseObj.GetCollection<BsonDocument>(table);

        QueryObject queryObj = JsonUtility.FromJson<QueryObject>(query);
        
        var queryRes = collection.Find(queryObj.find).Sort(queryObj.sort).Limit(queryObj.limit).ToList();
        //remove _id attributes
        foreach(var queryItem in queryRes)
        {
            queryItem.RemoveAt(0);
        }
        yield return yieldedReactionToGet(queryRes.ToJson());
    }

    public override IEnumerator UpdateLog(string database, string table, string query, Dictionary<string, string> argsNValues)
    {
        var databaseObj = client.GetDatabase(database);
        var collection = databaseObj.GetCollection<BsonDocument>(table);
        
//        collection.FindOneAndReplace(user => user._id == newModelUser._id, newModelUser);
        return null;
    }


    public override IEnumerator EndLogs()
    {
        return null;
    }
}

