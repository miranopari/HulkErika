using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine.Rendering;

[InitializeOnLoad]
public static class AvatarReadyDefineSymbols
{
    private const string AvatarReadySymbol = "AVATARREADY";

    // Key-Value of symbol / classname
    // if the classname is present in the project, the symbol will be defined in the Player Scripting Symbols
    // and can thus be use with precondition: '#if <symbol>'
    private static readonly List<Tuple<string, string>> symbols = new List<Tuple<string, string>>()
    {
        new Tuple<string, string>("FINALIK", "VRIK"),
        new Tuple<string, string>("STEAMVR", "SteamVR"),
        new Tuple<string, string>("XSENS", "XsStreamReader"),
        new Tuple<string, string>("SAFBIK", "FullBodyIKBehaviour"),
        new Tuple<string, string>("AZUREKINECT", "AzureKinectMemoryManager")
    };

    private const string TAG_HDRP = "USING_HDRP";
    private const string TAG_URP = "USING_URP";

    static AvatarReadyDefineSymbols()
    {
        string symbolsString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

        List<string> symbolsList = symbolsString.Split(';').ToList();

        // AvatarReady
        if (!symbolsList.Contains(AvatarReadySymbol))
            symbolsList.Add(AvatarReadySymbol);

        // Render pipeline
        if (GraphicsSettings.currentRenderPipeline != null)
        {
            // HDRP
            if (GraphicsSettings.currentRenderPipeline.GetType().Name == "HDRenderPipelineAsset")
            {
                if (!symbolsList.Contains(TAG_HDRP))
                    symbolsList.Add(TAG_HDRP);
            }
            else
            {
                if (symbolsList.Contains(TAG_HDRP))
                    symbolsList.Remove(TAG_HDRP);
            }

            // URP
            if (GraphicsSettings.currentRenderPipeline.GetType().Name == "UniversalRenderPipelineAsset")
            {
                if (!symbolsList.Contains(TAG_URP))
                    symbolsList.Add(TAG_URP);
            }
            else
            {
                if (symbolsList.Contains(TAG_URP))
                    symbolsList.Remove(TAG_URP);
            }
        }
        else
        {
            // built-in
            if (symbolsList.Contains(TAG_HDRP))
                symbolsList.Remove(TAG_HDRP);
            if (symbolsList.Contains(TAG_URP))
                symbolsList.Remove(TAG_URP);
        }

        // Third Parties
        string[] everyTypesName = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes().Select(t => t.Name)).ToArray();

        foreach (Tuple<string, string> tuple in symbols)
        {
            if (everyTypesName.Contains(tuple.Item2))
            {
                if (!symbolsList.Contains(tuple.Item1))
                    symbolsList.Add(tuple.Item1);
            }
            else
            {
                if (symbolsList.Contains(tuple.Item1))
                    symbolsList.Remove(tuple.Item1);
            }
        }

        PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, string.Join(";", symbolsList));
    }
}