#if UNITY_2018_1_OR_NEWER

using UnityEngine;

using System.IO;

namespace MG.MDV
{
    [UnityEditor.AssetImporters.ScriptedImporter( 1, "markdown" )]
    public class MarkdownAssetImporter : UnityEditor.AssetImporters.ScriptedImporter
    {
        public override void OnImportAsset( UnityEditor.AssetImporters.AssetImportContext ctx )
        {
            var md = new TextAsset( File.ReadAllText( ctx.assetPath ) );
            ctx.AddObjectToAsset( "main", md );
            ctx.SetMainObject( md );
        }
    }
}

#endif
