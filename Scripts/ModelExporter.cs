//Gong Xueyuan
//2016/9/7
using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Runtime.InteropServices;

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

        //cameraPrefab = Camera.main;

        //if(cameraPrefab != null)
        //{
        //    cameraC = Instantiate<Camera>(cameraPrefab);
        //    RT = new RenderTexture(640, 360, 24);
        //    cameraC.targetTexture = RT;
        //}

        GameObject GO = GameObject.Find("heliu_DiMian");
        Mesh mesh = GO.GetComponent<MeshFilter>().sharedMesh;
        int[] triangles = mesh.triangles;
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;

        Debug.Log(triangles[0] + " " + triangles[1] + " " + triangles[2] + " " + triangles[3] + " " + triangles[4] + " " + triangles[5]);

        for (int i = 0; i < vertices.Length; ++i)
        {
            vertices[i].x = vertices[i].x * GO.transform.lossyScale.x;
            vertices[i].y = vertices[i].y * GO.transform.lossyScale.y;
            vertices[i].z = vertices[i].z * GO.transform.lossyScale.z;
        }

        Debug.Log(vertices[0] + " " + vertices[1] + " " + vertices[2] + " " + vertices[3]);

        for (int i = 0; i < normals.Length; ++i)
        {

        }

        Debug.Log(normals[0] + " " + normals[1] + " " + normals[2] + " " + normals[3]);
    }

    void OnEnable()
    {
        findModels();
    }

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
            if (myModels[i].MR != null)
            {
                myModels[i].MR.enabled = myModels[i].isEnabled;
            }
            if (myModels[i].SMR != null)
            {
                myModels[i].SMR.enabled = myModels[i].isEnabled;
            }
        }

        //if(cameraC != null)
        //{
        //    DestroyImmediate(cameraC.gameObject);
        //}
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
        List<List<MyModel>> myModelLL = new List<List<MyModel>>();
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
                myModelLL.Add(new List<MyModel>());
                tLayer = myModels[i].layer;
            }

            if(myModels[i].mesh != null)
            {
                myModelLL[myModelLL.Count - 1].Add(myModels[i]);
            }
        }

        for (int i = 0; i < myModelLL.Count; ++i)
        {
            output2File(objNameList[i], myModelLL[i]);
            input4File(objNameList[i]); //TODO
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

    [MenuItem("Assets/ExportSceneModel", false, 1500)]
    static void exportScene()
    {
        string crtScene = SceneManager.GetActiveScene().path;

        UnityEngine.Object[] Os = Selection.GetFiltered(typeof(SceneAsset), SelectionMode.Assets);
        SceneAsset SA;

        List<MyModel> _myModels;
        GameObject[] GOs;

        for (int i = 0; i < Os.Length; ++i)
        {
            SA = Os[i] as SceneAsset;
            Debug.Log(AssetDatabase.GetAssetPath(SA));

            EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(SA));

            _myModels = new List<MyModel>();
            GOs = FindObjectsOfType<GameObject>();

            MeshFilter[] MFs;
            MeshRenderer[] MRs;
            SkinnedMeshRenderer[] SMRs;

            for (int k = 0; k < GOs.Length; ++k)
            {
                MFs = GOs[k].GetComponents<MeshFilter>();
                MRs = GOs[k].GetComponents<MeshRenderer>();
                SMRs = GOs[k].GetComponents<SkinnedMeshRenderer>();

                for (int j = 0; j < MRs.Length; ++j)
                {
                    _myModels.Add(new MyModel(GOs[k].transform, MFs[j].sharedMesh, MRs[j], GOs[k].transform.name, GOs[k].layer, MRs[j].enabled));
                }

                for (int j = 0; j < SMRs.Length; ++j)
                {
                    _myModels.Add(new MyModel(GOs[k].transform, SMRs[j].sharedMesh, SMRs[j], GOs[k].transform.name, GOs[k].layer, SMRs[j].enabled));
                }
            }

            if (_myModels.Count > 0)
            {
                output2File(SA.name, _myModels);
                //input4File(SA.name); //TODO
            }
        }

        EditorSceneManager.OpenScene(crtScene);
    }

    public static byte[] Ints2Bytes(int[] aryInt)
    {
        int nLen = aryInt.Length;
        if (nLen <= 0)
        {
            return null;
        }

        int nByteMulti = Marshal.SizeOf(aryInt[0]);
        byte[] ary = new byte[nLen * nByteMulti];

        for (int i = 0; i < nLen; i++)
        {
            byte[] tmp = BitConverter.GetBytes(aryInt[i]);
            Buffer.BlockCopy(tmp, 0, ary, i * nByteMulti, nByteMulti);
        }
        return ary;
    }

    public static byte[] Vecs2Bytes(Vector3[] aryVec)
    {
        int nLen = aryVec.Length;
        if (nLen <= 0)
        {
            return null;
        }

        int nByteMulti = Marshal.SizeOf(aryVec[0].x);
        byte[] ary = new byte[nLen * nByteMulti * 3];

        for (int i = 0; i < nLen; i++)
        {
            byte[] tmpX = BitConverter.GetBytes(aryVec[i].x);
            byte[] tmpY = BitConverter.GetBytes(aryVec[i].y);
            byte[] tmpZ = BitConverter.GetBytes(aryVec[i].z);

            int nOffsetBase = i * nByteMulti * 3;
            Buffer.BlockCopy(tmpX, 0, ary, nOffsetBase, nByteMulti);
            Buffer.BlockCopy(tmpY, 0, ary, nOffsetBase + nByteMulti, nByteMulti);
            Buffer.BlockCopy(tmpZ, 0, ary, nOffsetBase + nByteMulti * 2, nByteMulti);
        }
        return ary;
    }

    public static byte[] Vec2Bytes(Vector3 vec)
    {
        int nByteMulti = Marshal.SizeOf(vec.x);
        byte[] ary = new byte[Marshal.SizeOf(vec)];

        byte[] tmpX = BitConverter.GetBytes(vec.x);
        byte[] tmpY = BitConverter.GetBytes(vec.y);
        byte[] tmpZ = BitConverter.GetBytes(vec.z);

        Buffer.BlockCopy(tmpX, 0, ary, 0, nByteMulti);
        Buffer.BlockCopy(tmpY, 0, ary, nByteMulti, nByteMulti);
        Buffer.BlockCopy(tmpZ, 0, ary, nByteMulti * 2, nByteMulti);

        return ary;
    }

    public static byte[] Quaternion2Bytes(Quaternion qua)
    {
        int nByteMulti = Marshal.SizeOf(qua.x);
        byte[] ary = new byte[Marshal.SizeOf(qua)];

        byte[] tmpX = BitConverter.GetBytes(qua.x);
        byte[] tmpY = BitConverter.GetBytes(qua.y);
        byte[] tmpZ = BitConverter.GetBytes(qua.z);
        byte[] tmpW = BitConverter.GetBytes(qua.w);

        Buffer.BlockCopy(tmpX, 0, ary, 0, nByteMulti);
        Buffer.BlockCopy(tmpY, 0, ary, nByteMulti, nByteMulti);
        Buffer.BlockCopy(tmpZ, 0, ary, nByteMulti * 2, nByteMulti);
        Buffer.BlockCopy(tmpW, 0, ary, nByteMulti * 3, nByteMulti);

        return ary;
    }

    public static string getFileAddr(string name, string postfix)
    {
        if (!Directory.Exists("ExportedModels"))
        {
            Directory.CreateDirectory("ExportedModels");
        }

        string fileHead = "ExportedModels/" + name;
        string fileAddr = fileHead + postfix;

        for (int i = 0; File.Exists(fileAddr); ++i)
        {
            fileAddr = fileHead + "_" + i + postfix;
        }

        return fileAddr;
    }

    static void output2File(string name, List<MyModel> myModelList)
    {
        string fileAddr = getFileAddr(name, ".myM");

        FileStream fs = new FileStream(fileAddr, FileMode.Create);

        byte[] bytes;
        int[] triangles;
        Vector3[] vertices;
        Vector3[] normals;
        for (int i = 0; i < myModelList.Count; ++i)
        {
            bytes = Vec2Bytes(myModelList[i].transform.position);
            fs.Write(bytes, 0, bytes.Length);
            
            bytes = Quaternion2Bytes(myModelList[i].transform.rotation);
            fs.Write(bytes, 0, bytes.Length);

            bytes = Vec2Bytes(myModelList[i].transform.lossyScale);
            fs.Write(bytes, 0, bytes.Length);

            triangles = myModelList[i].mesh.triangles;
            bytes = BitConverter.GetBytes(triangles.Length);
            fs.Write(bytes, 0, bytes.Length);
            bytes = Ints2Bytes(triangles);
            fs.Write(bytes, 0, bytes.Length);

            vertices = myModelList[i].mesh.vertices;
            bytes = BitConverter.GetBytes(vertices.Length);
            fs.Write(bytes, 0, bytes.Length);
            bytes = Vecs2Bytes(vertices);
            fs.Write(bytes, 0, bytes.Length);

            normals = myModelList[i].mesh.normals;
            bytes = BitConverter.GetBytes(normals.Length);
            fs.Write(bytes, 0, bytes.Length);
            bytes = Vecs2Bytes(normals);
            fs.Write(bytes, 0, bytes.Length);
        }

        fs.Flush();
        fs.Close();
    }

    static List<MyModel> input4File(string name)
    {
        List<MyModel> myModelList = new List<MyModel>();
        string fileAddr = "ExportedModels/" + name + ".myM";

        FileStream fs = new FileStream(fileAddr, FileMode.Open);

        int cnt;
        int hasRead = 0;
        int toRead = (int)fs.Length;
        byte[] bytes = new byte[toRead];
        while((cnt = fs.Read(bytes, hasRead, toRead)) > 0)
        {
            hasRead += cnt;
            toRead -= cnt;
        }

        for (int index = 0; index < bytes.Length;)
        {
            Debug.Log(BitConverter.ToSingle(bytes, index) + " " + BitConverter.ToSingle(bytes, index + 4) + " " + BitConverter.ToSingle(bytes, index + 8));
            Debug.Log(BitConverter.ToSingle(bytes, index + 12) + " " + BitConverter.ToSingle(bytes, index + 16) + " " + BitConverter.ToSingle(bytes, index + 20) + " " + BitConverter.ToSingle(bytes, index + 24));
            Debug.Log(BitConverter.ToSingle(bytes, index + 28) + " " + BitConverter.ToSingle(bytes, index + 32) + " " + BitConverter.ToSingle(bytes, index + 36));

            index += 40;
            int triCnt = BitConverter.ToInt32(bytes, index);
            Debug.Log("Triangles: " + triCnt);
            index += 4;
            for (int i = 0; i < triCnt; ++i)
            {
                Debug.Log(BitConverter.ToInt32(bytes, index));
                index += 4;
            }

            int verCnt = BitConverter.ToInt32(bytes, index);
            Debug.Log("Vertices: " + verCnt);
            index += 4;
            for (int i = 0; i < verCnt; ++i)
            {
                Debug.Log(BitConverter.ToSingle(bytes, index) + " " + BitConverter.ToSingle(bytes, index + 4) + " " + BitConverter.ToSingle(bytes, index + 8));
                index += 12;
            }

            int norCnt = BitConverter.ToInt32(bytes, index);
            Debug.Log("Normals: " + norCnt);
            index += 4;
            for (int i = 0; i < norCnt; ++i)
            {
                Debug.Log(BitConverter.ToSingle(bytes, index) + " " + BitConverter.ToSingle(bytes, index + 4) + " " + BitConverter.ToSingle(bytes, index + 8));
                index += 12;
            }
        }

        fs.Flush();
        fs.Close();

        return myModelList;
    }
}
