using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARTree
{
    int seed;
    int maxNumVertices;
    int numberOfSides;
    float baseRadius;
    float radiusStep;
    float minimumRadius;
    float branchRoundness;
    float segmentLength;
    float twisting;
    float branchProbability;
    float growthPercent;
    private string leafEncoded;
    private Texture2D leafText;

    public ARTree(int seed, int maxNumVertices, int numberOfSides, float baseRadius, float radiusStep, float minimumRadius, 
        float branchRoundness, float segmentLength, float twisting, float branchProbability, float growthPercent)
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
        return result;
    }

    public void ConvertToTexture(string s)
    {
        byte[] bytes = System.Convert.FromBase64String(s);
        leafText = new Texture2D(2, 2);
        leafText.LoadImage(bytes);
    }
    
    public void ConvertToString(Texture2D leaf)
    {
       leafEncoded = System.Convert.ToBase64String(leaf.EncodeToPNG());
    }
}