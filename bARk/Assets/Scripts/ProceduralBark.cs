using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralBark : MonoBehaviour {
    public float freq, lacunarity, gain;

    public void GenerateTexture() {
        // Setup noise
        var noiseGenerator = new FastNoise(1234);
        noiseGenerator.SetFractalOctaves(4);
        noiseGenerator.SetFrequency(freq);
        noiseGenerator.SetFractalLacunarity(lacunarity);
        noiseGenerator.SetFractalGain(gain);

        // Create texture and fill the pixels
        var texture = new Texture2D(128, 256, TextureFormat.ARGB32, false);

        for(int y = 0; y < texture.height; y++) {
            for(int x = 0; x < texture.width; x++) {
                texture.SetPixel(x, y, Color.Lerp(Color.black, Color.white, Mathf.InverseLerp(-1, 1, noiseGenerator.GetValueFractal(x, y))));
                //texture.SetPixel(x, y, Color.Lerp(Color.black, Color.white, noiseGenerator.GetValue(x, y)));
            }
        }

        // Apply all SetPixel calls
        texture.Apply();

        // connect texture to material of GameObject this script is attached to
        GetComponent<Renderer>().sharedMaterial.mainTexture = texture;
    }

	// Use this for initialization
	void Start () {
		GenerateTexture();
	}
	
	// Update is called once per frame
	void Update () {
		// GenerateTexture();
	}
}
