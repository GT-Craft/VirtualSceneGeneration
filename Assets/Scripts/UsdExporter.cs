using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using USD.NET;
using Unity.Formats.USD;

public class UsdExporter : MonoBehaviour
{
    public bool exportMaterials = true;

    public BasisTransformation convertHandedness = BasisTransformation.SlowAndSafe;
    public ActiveExportPolicy activePolicy = ActiveExportPolicy.ExportAsActive;

    [SerializeField] string usdFileName = "GeneratedScene.usda";
    [SerializeField] public GameObject usdRoot;

    public Scene usdScene {private set; get;}
    ExportContext usdExportContext = new ExportContext();
    public bool isRecording {private set; get;}

    void Awake()
    {
        InitUsd.Initialize();

        // if not editor, set usd file path with application's data path
        if (!Application.isEditor)
        {
            usdFileName = System.IO.Path.Combine(Application.dataPath, usdFileName);
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            StartRecording();
            // usdScene.Write("/dt_snapshot", snapshotAttributes);
        }
    }

    void LateUpdate()
    {
        if (!isRecording) return;

        usdExportContext.exportMaterials = true;
        // usdExportContext.scene.Time = Time.timeSinceLevelLoad; // testing...
        usdExportContext.activePolicy = activePolicy;
        usdExportContext.exportNative = true;
        SceneExporter.SyncExportContext(usdRoot, usdExportContext);
        SceneExporter.Export(usdRoot, usdExportContext, zeroRootTransform: true);

        // Export the scene
        StopRecording();
    }

    public void StartRecording()
    {
        if(isRecording) return;
        if(usdRoot == null) return;
        if(usdScene != null)
        {
            usdScene.Close();
            usdScene = null;
        }

        try
        {
            if (string.IsNullOrEmpty(usdFileName)) usdScene = Scene.Create();
            else usdScene = Scene.Create(usdFileName);

            usdExportContext = new ExportContext();
            usdExportContext.scene = usdScene;
            usdExportContext.basisTransform = convertHandedness;

            isRecording = true;
        }
        catch
        {
            if (usdScene != null)
            {
                usdScene.Close();
                usdScene = null;
            }
            throw;
        }
    }

    public void StopRecording()
    {
        if(!isRecording) return;

        usdExportContext = new ExportContext();
        if(!string.IsNullOrEmpty(usdFileName)) usdScene.Save();

        usdScene.Close();
        usdScene = null;
        isRecording = false;
    }

    void Export()
    {
        SceneExporter.Export(usdRoot, usdScene, convertHandedness, exportUnvarying: true, zeroRootTransform: true);
    }
}
