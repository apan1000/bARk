using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

// ---------------------------------------------------------------------------------------------------------------------------
// Procedural Tree - Simple tree mesh generation - � 2015 Wasabimole http://wasabimole.com
// ---------------------------------------------------------------------------------------------------------------------------
// BASIC USER GUIDE
//
// - Choose GameObject > Create Procedural > Procedural Tree from the Unity menu
// - Select the object to adjust the tree's properties
// - Click on Rand Seed to get a new tree of the same type
// - Click on Rand Tree to change the tree type
//
// ADVANCED USER GUIDE
//
// - Drag the object to a project folder to create a Prefab (to keep a static snapshot of the tree)
// - To add a collision mesh to the object, choose Add Component > Physics > Mesh Collider
// - To add or remove detail, change the number of sides
// - You can change the default diffuse bark materials for more complex ones (with bump-map, specular, etc.)
// - Add or replace default materials by adding them to the SampleMaterials\ folder
// - You can also change the tree generation parameters in REAL-TIME from your scripts (*)
// - Use Unity's undo to roll back any unwanted changes
//
// ADDITIONAL NOTES
// 
// The generated mesh will remain on your scene, and will only be re-computed if/when you change any tree parameters.
//
// Branch(...) is the main tree generation function (called recursively), you can inspect/change the code to add new 
// tree features. If you add any new generation parameters, remember to add them to the checksum in the Update() function 
// (so the mesh gets re-computed when they change). If you add any cool new features, please share!!! ;-)
//
// To generate a new tree at runtime, just follow the example in Editor\ProceduralTreeEditor.cs:CreateProceduralTree()

// Additional scripts under ProceduralTree\Editor are optional, used to better integrate the trees into Unity.
//
// (*) To change the tree parameters in real-time, just get/keep a reference to the ProceduralTree component of the 
// tree GameObject, and change any of the public properties of the class.
//
// >>> Please visit http://wasabimole.com/procedural-tree for more information
// ---------------------------------------------------------------------------------------------------------------------------
// VERSION HISTORY
//
// 1.02 Error fixes update
// - Fixed bug when generating the mesh on a rotated GameObject
// - Fix error when building the project
//
// 1.00 First public release
// ---------------------------------------------------------------------------------------------------------------------------
// Thank you for choosing Procedural Tree, we sincerely hope you like it!
//
// Please send your feedback and suggestions to mailto://contact@wasabimole.com
// ---------------------------------------------------------------------------------------------------------------------------

namespace Wasabimole.ProceduralTree
{
    [ExecuteInEditMode]
    public class ProceduralTree : MonoBehaviour
    {
        public const int CurrentVersion = 102;

        // ---------------------------------------------------------------------------------------------------------------------------
        // Tree parameters (can be changed real-time in editor or game)
        // ---------------------------------------------------------------------------------------------------------------------------

        public int Seed; // Random seed on which the generation is based
        [Range(1024, 65000)]
        public int MaxNumVertices = 65000; // Maximum number of vertices for the tree mesh
        [Range(3, 32)]
        public int NumberOfSides = 16; // Number of sides for tree
        [Range(0.25f, 4f)]
        public float BaseRadius = 2f; // Base radius in meters
        [Range(0.75f, 0.95f)]
        public float RadiusStep = 0.9f; // Controls how quickly radius decreases
        [Range(0.01f, 0.2f)]
        public float MinimumRadius = 0.02f; // Minimum radius for the tree's smallest branches
        [Range(0f, 1f)]
        public float BranchRoundness = 0.8f; // Controls how round branches are
        [Range(0.1f, 2f)]
        public float SegmentLength = 0.5f; // Length of branch segments
        [Range(0f, 40f)]
        public float Twisting = 20f; // How much branches twist
        [Range(0f, 0.25f)]
        public float BranchProbability = 0.1f; // Branch probability
		[Range(0f, 1f)]
		public float growthPercent = 1f;

		//public AnimationCurve curve;
		[Header("Leaf")]
		public Material leafMaterial;

        // ---------------------------------------------------------------------------------------------------------------------------
        
        float checksum; // Serialized & Non-Serialized checksums for tree rebuilds only on undo operations, or when parameters change (mesh kept on scene otherwise)
        [SerializeField, HideInInspector]
        float checksumSerialized;

        List<Vector3> vertexList; // Vertex list
        List<Vector2> uvList; // UV list
        List<int> triangleList; // Triangle list

        float[] ringShape; // Tree ring shape array

        [HideInInspector, System.NonSerialized]
        public MeshRenderer Renderer; // MeshRenderer component

        MeshFilter filter; // MeshFilter component

		List<GameObject> leaves;
		int leafAmount;
		int leafCount;

#if UNITY_EDITOR
        [HideInInspector]
        public string MeshInfo; // Used in ProceduralTreeEditor to show info about the tree mesh
#endif

        // ---------------------------------------------------------------------------------------------------------------------------
        // Initialise object, make sure it has MeshFilter and MeshRenderer components
        // ---------------------------------------------------------------------------------------------------------------------------

        void OnEnable()
        {
            if (filter != null && Renderer != null) return;

            gameObject.isStatic = true;

            filter = gameObject.GetComponent<MeshFilter>();
            if (filter == null) filter = gameObject.AddComponent<MeshFilter>();
            if (filter.sharedMesh != null) checksum = checksumSerialized;
            Renderer = gameObject.GetComponent<MeshRenderer>();
            if (Renderer == null) Renderer = gameObject.AddComponent<MeshRenderer>();

			leafAmount = 52;


			if (leaves == null) {
				leaves = new List<GameObject> ();
				Debug.Log ("New leaf list created");
			}

			string holderName = "Leaf Holder";
			if (transform.FindChild (holderName)) {
				leaves.Clear ();
				DestroyImmediate (transform.FindChild (holderName).gameObject);
			}
			Transform leafHolder = new GameObject (holderName).transform;
			leafHolder.parent = transform;

			float width = 3;
			float height = 3;

			// ---------------------------------------------------------------------------------------------------------------------------
			// Leaf Generation
			// ---------------------------------------------------------------------------------------------------------------------------

			for (int i = 0; i < leafAmount; i++) {

				GameObject leaf = new GameObject ("leaf_" + i);
				MeshFilter mf = leaf.gameObject.AddComponent<MeshFilter> ();
				MeshRenderer mr = leaf.gameObject.AddComponent<MeshRenderer> ();
				mr.material = leafMaterial;

				Mesh mesh = new Mesh();
				mf.mesh = mesh;

				Vector3[] vertices = new Vector3[4];

				vertices[0] = new Vector3(-width, 0, 0);
				vertices[1] = new Vector3(width, 0, 0);
				vertices[2] = new Vector3(-width, 2*height, 0);
				vertices[3] = new Vector3(width, 2*height, 0);

				mesh.vertices = vertices;

				int[] tri = new int[6];

				tri[0] = 0;
				tri[1] = 2;
				tri[2] = 1;

				tri[3] = 2;
				tri[4] = 3;
				tri[5] = 1;

				mesh.triangles = tri;

				Vector3[] normals = new Vector3[4];

				normals[0] = -Vector3.forward;
				normals[1] = -Vector3.forward;
				normals[2] = -Vector3.forward;
				normals[3] = -Vector3.forward;

				mesh.normals = normals;

				Vector2[] uv = new Vector2[4];

				uv[0] = new Vector2(0, 0);
				uv[1] = new Vector2(1, 0);
				uv[2] = new Vector2(0, 1);
				uv[3] = new Vector2(1, 1);

				mesh.uv = uv;



				//GameObject leaf = GameObject.CreatePrimitive (PrimitiveType.Plane);
				leaf.transform.parent = leafHolder;
				leaves.Add (leaf);
				leaf.SetActive (false);
			}

			/*

			if (leaves.Count != leavesAmount) {
				Debug.Log (leavesAmount + " != " + leaves.Count);
				foreach (GameObject l in leaves) {
					if (Application.isPlaying) {
						Destroy (l); 
					} else {
						DestroyImmediate (l);
					}
				}
				leaves.Clear ();
				for (int i = 0; i < leavesAmount; i++) {
					GameObject leaf = GameObject.CreatePrimitive (PrimitiveType.Plane);
					leaf.transform.parent = leafHolder;
					leaves.Add (leaf);
					leaf.name = "leaf_" + i;
					leaf.SetActive (false);
					Debug.Log ("leaf created");
				}
			}
			*/

        }

        // ---------------------------------------------------------------------------------------------------------------------------
        // Generate tree (only called when parameters change, or there's an undo operation)
        // ---------------------------------------------------------------------------------------------------------------------------

    public void GenerateTree()
    {
        gameObject.isStatic = false;
			 
		foreach (GameObject l in leaves) {
			l.SetActive (false);
		}

        var originalRotation = transform.localRotation;
        var originalSeed = Random.seed;

        if (vertexList == null) // Create lists for holding generated vertices
        {
            vertexList = new List<Vector3>();
            uvList = new List<Vector2>();
            triangleList = new List<int>();
        }
        else // Clear lists for holding generated vertices
        {
            vertexList.Clear();
            uvList.Clear();
            triangleList.Clear();
        }

        SetTreeRingShape(); // Init shape array for current number of sides

        Random.seed = Seed;
		leafCount = leafAmount-1;

        // Main recursive call, starts creating the ring of vertices in the trunk's base
        Branch(new Quaternion(), Vector3.zero, -1, BaseRadius, 0f, 0.05f);
            
        Random.seed = originalSeed;

        transform.localRotation = originalRotation; // Restore original object rotation

        SetTreeMesh(); // Create/Update MeshFilter's mesh
    }

        // ---------------------------------------------------------------------------------------------------------------------------
        // Set the tree mesh from the generated vertex lists (vertexList, uvList, triangleLists)
        // ---------------------------------------------------------------------------------------------------------------------------

        private void SetTreeMesh()
        {
            // Get mesh or create one
            var mesh = filter.sharedMesh;
            if (mesh == null) 
                mesh = filter.sharedMesh = new Mesh();
            else 
                mesh.Clear();

            // Assign vertex data
            mesh.vertices = vertexList.ToArray();
            mesh.uv = uvList.ToArray();
            mesh.triangles = triangleList.ToArray();
			/*
            // Update mesh
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            ; // Do not call this if we are going to change the mesh dynamically!
			*/

#if UNITY_EDITOR
            MeshInfo = "Mesh has " + vertexList.Count + " vertices and " + triangleList.Count / 3 + " triangles";
#endif
        }

        // ---------------------------------------------------------------------------------------------------------------------------
        // Main branch recursive function to generate tree
        // ---------------------------------------------------------------------------------------------------------------------------

		void Branch(Quaternion quaternion, Vector3 position, int lastRingVertexIndex, float radius, float texCoordV, float growthCost)
        {
            var offset = Vector3.zero;
            var texCoord = new Vector2(0f, texCoordV);
            var textureStepU = 1f / NumberOfSides;
            var angInc = 2f * Mathf.PI * textureStepU;
            var ang = 0f;

            // Add ring vertices
            for (var n = 0; n <= NumberOfSides; n++, ang += angInc) 
            {
				var r = ringShape[n] * radius * growthPercent;
                offset.x = r * Mathf.Cos(ang); // Get X, Z vertex offsets
                offset.z = r * Mathf.Sin(ang);
                vertexList.Add(position + quaternion * offset); // Add Vertex position
                uvList.Add(texCoord); // Add UV coord
                texCoord.x += textureStepU;
            }

            if (lastRingVertexIndex >= 0) // After first base ring is added ...
            {
                // Create new branch segment quads, between last two vertex rings
                for (var currentRingVertexIndex = vertexList.Count - NumberOfSides - 1; currentRingVertexIndex < vertexList.Count - 1; currentRingVertexIndex++, lastRingVertexIndex++) 
                {
                    triangleList.Add(lastRingVertexIndex + 1); // Triangle A
                    triangleList.Add(lastRingVertexIndex);
                    triangleList.Add(currentRingVertexIndex);
                    triangleList.Add(currentRingVertexIndex); // Triangle B
                    triangleList.Add(currentRingVertexIndex + 1);
                    triangleList.Add(lastRingVertexIndex + 1);
                }
            }

			if(leafCount >= 0 && radius < MinimumRadius * 4 && Random.value > 0.5f) {
				leaves [leafCount].SetActive (true);
				leaves [leafCount].transform.localPosition = position;
				leaves [leafCount].transform.localScale = Vector3.one * radius * growthPercent * (Random.value + 1f);
				Quaternion q = Quaternion.Euler (new Vector3 (0, Random.value * 180, 0));
				leaves [leafCount].transform.localRotation = quaternion * q;

				leafCount--;

			}
		
            // Do we end current branch?
            radius *= RadiusStep;
            if (radius < MinimumRadius || vertexList.Count + NumberOfSides >= MaxNumVertices) // End branch if reached minimum radius, or ran out of vertices
            {
                // Create a cap for ending the branch
                vertexList.Add(position); // Add central vertex
                uvList.Add(texCoord + Vector2.one); // Twist UVs to get rings effect
                for (var n = vertexList.Count - NumberOfSides - 2; n < vertexList.Count - 2; n++) // Add cap
                {
                    triangleList.Add(n);
                    triangleList.Add(vertexList.Count - 1);
                    triangleList.Add(n + 1);
                }

				//

                return; 
            }

			//TODO:Add leaf shader, not culling backfaces of planes
			//Add leaves 





            // Continue current branch (randomizing the angle)
			//float grow = Random.value;// * Random.value * GrowthMultiplier);
            texCoordV += 0.0625f * (SegmentLength + SegmentLength / radius);
			if (growthPercent >= growthCost) { // curve.Evaluate (GrowthMultiplier)
				if (growthPercent <= growthCost * 1.1f) {
					float mult = 1f / (growthCost*1.1f - growthCost);
					position += quaternion * new Vector3 (0f, (growthPercent - growthCost) * mult * SegmentLength / (1 + growthCost), 0f);
				} else {
					position += quaternion * new Vector3 (0f, SegmentLength / (1 + growthCost), 0f);
				}
			}
            transform.rotation = quaternion; 
            var x = (Random.value - 0.5f) * Twisting;
            var z = (Random.value - 0.5f) * Twisting;
            transform.Rotate(x, 0f, z);
            lastRingVertexIndex = vertexList.Count - NumberOfSides - 1;
			//if (curve.Evaluate (GrowthMultiplier) < growth) return;
			Branch(transform.rotation, position, lastRingVertexIndex, radius, texCoordV, growthCost*1.1f); // Next segment

            // Do we branch?
			if (vertexList.Count + NumberOfSides >= MaxNumVertices || Random.value > BranchProbability) return;

            // Yes, add a new branch
            transform.rotation = quaternion;
            x = Random.value * 70f - 35f;
            x += x > 0 ? 10f : -10f;
            z = Random.value * 70f - 35f;
            z += z > 0 ? 10f : -10f;
            transform.Rotate(x, 0f, z);
			Branch(transform.rotation, position, lastRingVertexIndex, radius, texCoordV, growthCost + 0.05f);
        }

        // ---------------------------------------------------------------------------------------------------------------------------
        // Try to get shared mesh for new prefab instances
        // ---------------------------------------------------------------------------------------------------------------------------

#if UNITY_EDITOR
        bool CanGetPrefabMesh()
        {
            // Return false if we are not instancing a new procedural tree prefab
            if (PrefabUtility.GetPrefabType(this) != PrefabType.PrefabInstance) return false;
            if (filter.sharedMesh != null) return true;

            // Try to get mesh from an existing instance
            var parentPrefab = PrefabUtility.GetPrefabParent(this);
            var list = (ProceduralTree[])FindObjectsOfType(typeof(ProceduralTree));
            foreach (var go in list)
                if (go != this && PrefabUtility.GetPrefabParent(go) == parentPrefab)
                {
                    filter.sharedMesh = go.filter.sharedMesh;
                    return true;
                }
            return false;
        }
#endif

        // ---------------------------------------------------------------------------------------------------------------------------
        // Set tree shape, by computing a random offset for every ring vertex
        // ---------------------------------------------------------------------------------------------------------------------------

        private void SetTreeRingShape()
        {
            ringShape = new float[NumberOfSides + 1];
            var k = (1f - BranchRoundness) * 0.5f;
            // Randomize the vertex offsets, according to BranchRoundness
            Random.seed = Seed;
            for (var n = 0; n < NumberOfSides; n++) ringShape[n] = 1f - (Random.value - 0.5f) * k;
            ringShape[NumberOfSides] = ringShape[0];
        }

        // ---------------------------------------------------------------------------------------------------------------------------
        // Update function will return, unless the tree parameters have changed
        // ---------------------------------------------------------------------------------------------------------------------------

        public void Update()
        {
            // Tree parameter checksum (add any new parameters here!)
            var newChecksum = (Seed & 0xFFFF) + NumberOfSides + SegmentLength + BaseRadius + MaxNumVertices +
				RadiusStep + MinimumRadius + Twisting + BranchProbability + BranchRoundness + growthPercent;

            // Return (do nothing) unless tree parameters change
            if (newChecksum == checksum && filter.sharedMesh != null) return;

            checksumSerialized = checksum = newChecksum;

#if UNITY_EDITOR
            if (!CanGetPrefabMesh()) 
#endif
                GenerateTree(); // Update tree mesh
        }

        // ---------------------------------------------------------------------------------------------------------------------------
        // Destroy procedural mesh when object is deleted
        // ---------------------------------------------------------------------------------------------------------------------------

#if UNITY_EDITOR
        void OnDisable()
        {
            if (filter.sharedMesh == null) return; // If tree has a mesh
            if (PrefabUtility.GetPrefabType(this) == PrefabType.PrefabInstance) // If it's a prefab instance, look for siblings
            {   
                var parentPrefab = PrefabUtility.GetPrefabParent(this);
                var list = (ProceduralTree[])FindObjectsOfType(typeof(ProceduralTree));
                foreach (var go in list)
                    if (go != this && PrefabUtility.GetPrefabParent(go) == parentPrefab)
                        return; // Return if there's another prefab instance still using the mesh
            }
            DestroyImmediate(filter.sharedMesh, true); // Delete procedural mesh
        }
#endif
    }
}