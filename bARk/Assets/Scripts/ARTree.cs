using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARTree
{
    public int seed { get; set; }
    public int maxNumVertices { get; set; }
    public int numberOfSides { get; set; }
    public float baseRadius { get; set; }
    public float radiusStep { get; set; }
    public float minimumRadius { get; set; }
    public float branchRoundness { get; set; }
    public float segmentLength { get; set; }
    public float twisting { get; set; }
    public float branchProbability { get; set; }
    public float growthPercent { get; set; }
    public string leafEncoded { get; set; }
    public Texture2D leafTexture { get; set; }
    public string timeStamp { get; set; }
    public string materialName { get; set; }

    public ARTree(int seed, int maxNumVertices, int numberOfSides, float baseRadius, float radiusStep, float minimumRadius, 
        float branchRoundness, float segmentLength, float twisting, float branchProbability, float growthPercent, string timeStamp,
        string materialName)
    {
        this.seed = seed;
        this.maxNumVertices = maxNumVertices;
        this.numberOfSides = numberOfSides;
        this.baseRadius = baseRadius;
        this.radiusStep = radiusStep;
        this.minimumRadius = minimumRadius;
        this.branchRoundness = branchRoundness;
        this.segmentLength = segmentLength;
        this.twisting = twisting;
        this.branchProbability = branchRoundness;
        this.growthPercent = growthPercent;
        this.timeStamp = timeStamp;
        this.materialName = materialName;
    }

    /// <summary>
    /// Creates a tree from data snapshot
    /// dbN = database Node
    /// </summary>
    /// <param name="dbN"></param>
    public ARTree(DataSnapshot dbN)
    {
        this.seed = int.Parse(dbN.Child("seed").Value.ToString());
        this.maxNumVertices = int.Parse(dbN.Child("maxNumVertices").Value.ToString());
        this.numberOfSides = int.Parse(dbN.Child("numberOfSides").Value.ToString());
        this.baseRadius = float.Parse(dbN.Child("baseRadius").Value.ToString());
        this.radiusStep = float.Parse(dbN.Child("radiusStep").Value.ToString());
        this.minimumRadius = float.Parse(dbN.Child("minimumRadius").Value.ToString());
        this.branchRoundness = float.Parse(dbN.Child("branchRoundness").Value.ToString());
        this.segmentLength = float.Parse(dbN.Child("segmentLength").Value.ToString());
        this.twisting = float.Parse(dbN.Child("twisting").Value.ToString());
        this.branchProbability = float.Parse(dbN.Child("branchProbability").Value.ToString());
        this.growthPercent = float.Parse(dbN.Child("growthPercent").Value.ToString());
        this.leafEncoded = dbN.Child("leaf").Value.ToString();
        this.timeStamp = dbN.Child("timeStamp").Value.ToString();
        this.materialName = dbN.Child("materialName").Value.ToString();
    }

    public Dictionary<string, object> ToDictionary()
    {
        Dictionary<string, object> result = new Dictionary<string, object>();
        result["seed"] = seed.ToString();
        result["maxNumVertices"] = maxNumVertices.ToString();
        result["numberOfSides"] = numberOfSides.ToString();
        result["baseRadius"] = baseRadius.ToString();
        result["radiusStep"] = radiusStep.ToString();
        result["minimumRadius"] = minimumRadius.ToString();
        result["branchRoundness"] = branchRoundness.ToString();
        result["segmentLength"] = segmentLength.ToString();
        result["twisting"] = twisting.ToString();
        result["branchProbability"] = branchProbability.ToString();
        result["growthPercent"] = growthPercent.ToString();
        result["leaf"] = leafEncoded;
        result["timeStamp"] = timeStamp;
        result["materialName"] = materialName;
        return result;
    }

    /// <summary>
    /// Creates a Texture2D from a PNG string.
    /// Has to be done on main thread.
    /// </summary>
    /// <param name="s"></param>
    public void ConvertToTexture()
    {
        byte[] bytes = System.Convert.FromBase64String(leafEncoded);
        leafTexture = new Texture2D(2, 2); // Load image overrides the size
        leafTexture.LoadImage(bytes);
    }
    
    /// <summary>
    /// Converts a Texture2D to a PNG string
    /// </summary>
    /// <param name="leaf"></param>
    public void ConvertToString(Texture2D leaf)
    {
       leafEncoded = System.Convert.ToBase64String(leaf.EncodeToPNG());
    }

    /// <summary>
    /// Retrurns a string with information about the tree.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return
           "seed: " + seed.ToString() + "\n" +
           "maxNumVertices: " + maxNumVertices.ToString() + "\n" +
           "numberOfSides: " + numberOfSides.ToString() + "\n" +
           "baseRadius: " + baseRadius.ToString() + "\n" +
           "radiusStep: " + radiusStep.ToString() + "\n" +
           "minimumRadius: " + minimumRadius.ToString() + "\n" +
           "branchRoundness: " + branchRoundness.ToString() + "\n" +
           "segmentLength: " + segmentLength.ToString() + "\n" +
           "twisting: " + twisting.ToString() + "\n" +
           "branchProbability: " + branchProbability.ToString() + "\n" +
           "growthPercent: " + growthPercent.ToString() + "\n" +
           "leafEncoded: " + leafEncoded.ToString();
    }
}