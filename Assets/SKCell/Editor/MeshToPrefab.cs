using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MeshToPrefab : Editor
{
    [MenuItem("SKCell/Tools/Mesh To Prefab")]
    public static void Generate()
    {
        
        List<GameObject> MeshInScene = new List<GameObject>();
        List<Transform> children = new List<Transform>();
        List<GameObject> DestroyMesh = new List<GameObject>();
        //Dictionary<string, GameObject> PrefabDone = new Dictionary<string, GameObject>();

        Queue<GameObject> PrefabInScene = new Queue<GameObject>();

        Queue<GameObject> allObjects = new Queue<GameObject>();

        GameObject rootObject = GameObject.Find("MTPRoot");  // root

        allObjects.Enqueue(rootObject);

        while(allObjects.Count != 0)
        {
            GameObject objectItem = allObjects.Dequeue();
            foreach (Transform child in objectItem.transform)
            {
                bool isNotPrefab = PrefabUtility.GetPrefabInstanceStatus(child.gameObject) == PrefabInstanceStatus.NotAPrefab;
                if (isNotPrefab)
                {
                    MeshInScene.Add(child.gameObject);
                }
                else
                {
                    PrefabInScene.Enqueue(child.gameObject);
                }
                allObjects.Enqueue(child.gameObject);
                Debug.Log(child.gameObject.name);
            }
        }

        while (PrefabInScene.Count != 0)
        {
            GameObject prefab = PrefabInScene.Dequeue();
            string prefabName = prefab.name;

            //if (PrefabDone.ContainsKey(prefab.transform.parent.gameObject.name))
            //{
            //    prefab.transform.parent = PrefabDone[prefab.transform.parent.gameObject.name].transform;
            //}

            int index = prefabName.IndexOf("_");
            if (index != -1)
            {
                string prefixPrefabName = prefabName.Substring(0, index);
                GameObject sourcePrefab = PrefabUtility.GetCorrespondingObjectFromSource(prefab);

                foreach (GameObject mesh in MeshInScene)
                {
                    int indexMesh = mesh.name.IndexOf("_");
                    if (indexMesh == -1)
                        continue;

                    string prefixMeshName = mesh.name.Substring(0, indexMesh);

                    if (prefixMeshName == prefixPrefabName)
                    {
                        //GameObject instanceObj = UnityEditor.PrefabUtility.InstantiatePrefab(Resources.Load<GameObject>(weaponPath)) as GameObject;                     

                        GameObject clonePrefab = UnityEditor.PrefabUtility.InstantiatePrefab(sourcePrefab) as GameObject;

                        clonePrefab.transform.parent = mesh.transform.parent;
                        //if (PrefabDone.ContainsKey(mesh.transform.parent.gameObject.name))
                        //{
                        //    clonePrefab.transform.parent = PrefabDone[mesh.transform.parent.gameObject.name].transform;
                        //}


                        children.Clear();

                        foreach (Transform child in mesh.transform)
                        {
                            children.Add(child);
                        }
                        foreach (Transform child in children)
                        {
                            child.parent = clonePrefab.transform;
                        }

                       
                        clonePrefab.name = mesh.name;
                        clonePrefab.transform.position = mesh.transform.position;
                        clonePrefab.transform.rotation = mesh.transform.rotation;
                        clonePrefab.transform.localScale = mesh.transform.localScale;

                        DestroyMesh.Add(mesh);
                        //PrefabDone.Add(clonePrefab.name, clonePrefab);

                        //MeshInScene.Remove(mesh);
                        //Debug.Log("Destroy" + mesh.name);
                        //DestroyImmediate(mesh);
                        //Debug.Log("done");
                    }
                }
            }
        }

        foreach (GameObject mesh in DestroyMesh)
        {
            DestroyImmediate(mesh);
        }
    }
}
