using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARTree
{
    private string name;
    private string barkType;
    private string plantDate;
    private string trackingImage;
    private string ownerID;
    private string leafEncoded;

    public ARTree(string name, string barkType, string plantDate, string trackingImage)
    {
        this.name = name;
        this.barkType = barkType;
        this.plantDate = plantDate;
        this.trackingImage = trackingImage;
    }

    public Dictionary<string, object> ToDictionary()
    {
        Dictionary<string, object> result = new Dictionary<string, object>();
        result["ownerID"] = ownerID;
        result["name"] = name;
        result["barkType"] = barkType;
        result["plantDate"] = plantDate;
        result["trackingImage"] = trackingImage;
        result["leaf"] = leafEncoded;
        return result;
    }

    public void ConvertToBytes(string s)
    {
        byte[] bytes = System.Convert.FromBase64String(s);
    }
    
    public void ConvertToString(Texture2D leaf)
    {
       leafEncoded = System.Convert.ToBase64String(leaf.EncodeToPNG());
    }
}
