//Gong Xueyuan
//2016/9/7
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.SceneManagement;

class MyModel
{
    public Transform transform;
    public Mesh mesh;
    public MeshRenderer MR;
    public SkinnedMeshRenderer SMR;
    public string objName;
    public int layer;
    public bool isEnabled;
    public bool is2Export;

    public MyModel()
    {
        transform = null;
        mesh = null;
        MR = null;
        SMR = null;
        objName = string.Empty;
        layer = 0;
        isEnabled = true;
        is2Export = true;
    }

    public MyModel(Transform _transform, string _objName, int _layer)
    {
        transform = _transform;
        objName = _objName;
        layer = _layer;
        isEnabled = false;
        is2Export = false;
    }

    public MyModel(Transform _transform, Mesh _mesh, MeshRenderer _MR, string _objName, int _layer, bool _isEnabled)
    {
        transform = _transform;
        mesh = _mesh;
        MR = _MR;
        objName = _objName;
        layer = _layer;
        isEnabled = _isEnabled;
        is2Export = _isEnabled;
    }

    public MyModel(Transform _transform, Mesh _mesh, SkinnedMeshRenderer _SMR, string _objName, int _layer, bool _isEnabled)
    {
        transform = _transform;
        mesh = _mesh;
        SMR = _SMR;
        objName = _objName;
        layer = _layer;
        isEnabled = _isEnabled;
        is2Export = _isEnabled;
    }

    public void print()
    {
        Debug.Log("Name: " + objName + " Layer: " + layer);
    }
}

public class ModelExporter : EditorWindow
{
    static ModelExporter window;
    static List<MyModel> myModels;

    //static Camera cameraPrefab;
    //static Camera cameraC;
    //static RenderTexture RT;

    [MenuItem("Gizmos/ModelWindowExporter", false, 30)]
    static void Init()
    {
        window = GetWindow(typeof(ModelExporter)) as ModelExporter;
        //ModelExporter window = GetWindowWithRect(typeof(ModelExporter), new Rect(0, 0, 800, 600), true, "WindowExportModels") as ModelExporter;
        window.Show();

        findModels();
        
        //cameraPrefab = Camera.main;

        //if(cameraPrefab != null)
        //{
        //    cameraC = Instantiate<Camera>(cameraPrefab);
        //    RT = new RenderTexture(640, 360, 24);
        //    cameraC.targetTexture = RT;
        //}
    }

    bool showPosition = true;
    string status = "Select a GameObject";

    Vector2 scrollPos;
    void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        drawToggleTree();
        EditorGUILayout.EndScrollView();
        //if(RT != null)
        //{
        //    EditorGUI.DrawPreviewTexture(new Rect(0, 200, 320, 180), RT);
        //}

        if (GUILayout.Button("Reset to Initial State"))
        {
            reset();
        }
        if (GUILayout.Button("Export Models"))
        {
            output2File();
        }
    }


    bool tIs2Export;
    void drawToggleTree()
    {
        string text;

        for (int i = 0; i < myModels.Count; ++i)
        {
            EditorGUILayout.BeginHorizontal(GUILayout.Width(200 + 50 * myModels[i].layer));
            EditorGUILayout.Space();

            text = string.Empty;
            if(!myModels[i].transform.gameObject.activeInHierarchy)
            {
                text += "(Inactive) ";
            }
            if (myModels[i].mesh == null)
            {
                text += "(No Mesh) ";
            }

            tIs2Export = EditorGUILayout.ToggleLeft(text + myModels[i].objName, myModels[i].is2Export);

            if (tIs2Export != myModels[i].is2Export)
            {
                myModels[i].is2Export = tIs2Export;

                //cameraC.transform.position = myModels[i].transform.TransformPoint(myModels[i].mesh.bounds.center) + Vector3.back * 10;
                //cameraC.orthographicSize = Mathf.Max(myModels[i].mesh.bounds.size.x, Mathf.Max(myModels[i].mesh.bounds.size.y, myModels[i].mesh.bounds.size.z));

                if(myModels[i].MR != null)
                {
                    myModels[i].MR.enabled = tIs2Export;
                }
                else if(myModels[i].SMR != null)
                {
                    myModels[i].SMR.enabled = tIs2Export;
                }
            }

            EditorGUILayout.EndHorizontal();
        }
    }
    
    static void findModels()
    {
        myModels = new List<MyModel>();

        GameObject[] GOs = SceneManager.GetActiveScene().GetRootGameObjects();
        for (int i = 0; i < GOs.Length; ++i)
        {
            traverse(GOs[i].transform, 0);
        }

        for (int i = 0; i < myModels.Count; ++i)
        {
            myModels[i].print();
        }
    }

    static bool traverse(Transform transform, int layer)
    {
        MeshFilter[] MFs;
        MeshRenderer[] MRs;
        SkinnedMeshRenderer[] SMRs;
        bool hasMesh = false;
        
        MFs = transform.GetComponents<MeshFilter>();
        MRs = transform.GetComponents<MeshRenderer>();
        SMRs = transform.GetComponents<SkinnedMeshRenderer>();
        if (MFs.Length > 0 || SMRs.Length > 0)
        {
            hasMesh = true;

            for (int j = 0; j < MRs.Length; ++j)
            {
                myModels.Add(new MyModel(transform, MFs[j].sharedMesh, MRs[j], transform.name, layer, MRs[j].enabled));
            }
            for (int j = 0; j < SMRs.Length; ++j)
            {
                myModels.Add(new MyModel(transform, SMRs[j].sharedMesh, SMRs[j], transform.name, layer, SMRs[j].enabled));
            }
        }
        else
        {
            myModels.Add(new MyModel(transform, transform.name, layer));
        }
        layer = layer + 1;

        Transform childTransform;
        for (int i = 0; i < transform.childCount; ++i)
        {
            childTransform = transform.GetChild(i);

            if(traverse(childTransform, layer))
            {
                hasMesh = true;
            }
        }

        if(!hasMesh)
        {
            myModels.RemoveAt(myModels.Count - 1);
        }

        return hasMesh;
    }

    static void output2File()
    {
        List<string> objNameList = new List<string>();
        List<List<Vector3[]>> verticesLL = new List<List<Vector3[]>>();
        int tLayer = int.MaxValue;

        for (int i = 0; i < myModels.Count; ++i)
        {
            if (!myModels[i].is2Export)
            {
                if (tLayer >= myModels[i].layer)
                {
                    tLayer = int.MaxValue;
                }

                continue;
            }

            if (tLayer >= myModels[i].layer)
            {
                objNameList.Add(myModels[i].objName);
                verticesLL.Add(new List<Vector3[]>());
                tLayer = myModels[i].layer;
            }

            if(myModels[i].mesh != null)
            {
                verticesLL[verticesLL.Count - 1].Add(myModels[i].mesh.vertices);
            }
        }

        for (int i = 0; i < verticesLL.Count; ++i)
        {
            output2File(objNameList[i], verticesLL[i]);
        }
    }

    void reset()
    {
        for (int i = 0; i < myModels.Count; ++i)
        {
            if(myModels[i].mesh == null)
            {
                myModels[i].is2Export = false;
            }
            else if (myModels[i].MR != null)
            {
                myModels[i].MR.enabled = myModels[i].isEnabled;
                myModels[i].is2Export = myModels[i].isEnabled;
            }
            else if (myModels[i].SMR != null)
            {
                myModels[i].SMR.enabled = myModels[i].isEnabled;
                myModels[i].is2Export = myModels[i].isEnabled;
            }
        }
    }

    void OnDidOpenScene()
    {
        findModels();
    }

    void OnInspectorUpdate()
    {
        Repaint();
    }
    
    void OnDestroy()
    {
        for (int i = 0; i < myModels.Count; ++i)
        {
            if(myModels[i].MR != null)
            {
                myModels[i].MR.enabled = myModels[i].isEnabled;
            }
            if(myModels[i].SMR != null)
            {
                myModels[i].SMR.enabled = myModels[i].isEnabled;
            }
        }

        //if(cameraC != null)
        //{
        //    DestroyImmediate(cameraC.gameObject);
        //}
    }

    //[MenuItem("Gizmos/ModelHierarchyExporter", false, 30)]
    //static void hierarchyExportModels()
    //{
    //    GameObject[] GOs = Selection.gameObjects;

    //    List<Vector3[]> verticesList = new List<Vector3[]>();
    //    MeshFilter[] MFs;
    //    SkinnedMeshRenderer[] SMRs;
    //    for (int i = 0; i < GOs.Length; ++i)
    //    {
    //        MFs = GOs[i].GetComponentsInChildren<MeshFilter>();
    //        SMRs = GOs[i].GetComponentsInChildren<SkinnedMeshRenderer>();

    //        Debug.Log("GameObject(" + GOs[i].name + ")");

    //        for (int j = 0; j < MFs.Length; ++j)
    //        {
    //            Debug.Log("Model(" + MFs[j].sharedMesh.name + ")");
    //            Debug.Log(MFs[j].sharedMesh.vertexCount);

    //            verticesList.Add(MFs[j].sharedMesh.vertices);
    //        }

    //        for (int j = 0; j < SMRs.Length; ++j)
    //        {
    //            Debug.Log("Model(" + SMRs[j].sharedMesh.name + ")");
    //            Debug.Log(SMRs[j].sharedMesh.vertexCount);

    //            verticesList.Add(SMRs[j].sharedMesh.vertices);
    //        }

    //        if (verticesList.Count > 0)
    //        {
    //            output2File(GOs[i].name, verticesList);
    //            verticesList.Clear();
    //        }
    //    }
    //}

    static void output2File(string name, List<Vector3[]> verticesList)
    {
        if (!Directory.Exists("ExportedModels"))
        {
            Directory.CreateDirectory("ExportedModels");
        }

        string fileHead = "ExportedModels/" + name;
        string fileAddr = fileHead + ".txt";

        for (int i = 0; File.Exists(fileAddr); ++i)
        {
            fileAddr = fileHead + "_" + i + ".txt";
        }

        FileStream fs = new FileStream(fileAddr, FileMode.Create);
        StreamWriter sw = new StreamWriter(fs);

        for (int i = 0; i < verticesList.Count; ++i)
        {
            for (int j = 0; j < verticesList[i].Length; ++j)
            {
                sw.WriteLine(verticesList[i][j].ToString());
            }
        }

        sw.Flush();
        fs.Flush();
        sw.Close();
        fs.Close();
    }
}
