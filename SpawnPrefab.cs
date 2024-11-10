using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using System.IO;
using RoR2;
using UnityEngine.AddressableAssets;
using System.Linq;
using UnityEngine.Networking;

[assembly: HG.Reflection.SearchableAttribute.OptIn]

namespace SpawnPrefab;

[BepInPlugin(PluginGUID, PluginName, PluginVersion)]
public class SpawnPrefabPlugin : BaseUnityPlugin
{
    public const string PluginGUID = "Lawlzee.SpawnPrefab";
    public const string PluginAuthor = "Lawlzee";
    public const string PluginName = "SpawnPrefab";
    public const string PluginVersion = "1.1.0";

    private const string _description = "`spawn_prefab \"<prefab_key>\" <pos_x> <pos_y> <pos_z> <rotation_x> <rotation_y> <rotation_z>`: Use the '~' prefix before a position value to make it relative to the player's position. Similarly, use the '~' prefix before a rotation value to make it relative to the prefab's base rotation. For a list of available prefabs, refer to https://xiaoxiao921.github.io/GithubActionCacheTest/assetPathsDump.html";

    public void Awake()
    {
        Log.Init(Logger);
        On.RoR2.Console.Lexer.IsIdentifierCharacter += Lexer_IsIdentifierCharacter;
    }

    private bool Lexer_IsIdentifierCharacter(On.RoR2.Console.Lexer.orig_IsIdentifierCharacter orig, char character)
    {
        return character == '~' || orig(character);
    }

    [ConCommand(commandName = "spawn_prefab", flags = ConVarFlags.None, helpText = _description)]
    public static void SpawnPrefab(ConCommandArgs args)
    {
        var playerPosition = args.GetSenderBody().transform.position;

        if (args.Count == 0)
        {
            Debug.Log($"spawn_prefab must have at least 1 argument. {_description}");
            return;
        }

        string prefabKey = args.GetArgString(0);
        var prefab = Addressables.LoadAssetAsync<GameObject>(prefabKey).WaitForCompletion();

        if (prefab == null)
        {
            Debug.Log($"Prefab '{prefabKey}' not found");
            return;
        }

        Vector3 position = new Vector3(
            GetValue(1).Apply(playerPosition.x),
            GetValue(2).Apply(playerPosition.y),
            GetValue(3).Apply(playerPosition.z));

        Vector3 rotation = new Vector3(
            GetValue(4).Apply(prefab.transform.rotation.x),
            GetValue(5).Apply(prefab.transform.rotation.y),
            GetValue(6).Apply(prefab.transform.rotation.z));

        GameObject gameObject = Instantiate(prefab, position, Quaternion.Euler(rotation));
        if (gameObject.GetComponent<NetworkIdentity>() != null)
        {
            NetworkServer.Spawn(gameObject);
        }

        Debug.Log($"Prefab '{prefabKey}' spawned at {position}");

        RelativeFloat GetValue(int index)
        {
            if (index < args.Count)
            {
                string strValue = args.GetArgString(index);

                if (strValue.StartsWith("~") && TextSerialization.TryParseInvariant(strValue.Substring(1), out float value))
                {
                    return new RelativeFloat
                    {
                        relative = true,
                        value = value
                    };
                }

                float? floatValue = args.TryGetArgFloat(index);
                if (floatValue != null)
                {
                    return new RelativeFloat
                    {
                        relative = false,
                        value = floatValue.Value
                    };
                }
            }

            return new RelativeFloat
            {
                relative = true,
                value = 0
            };
        }
    }
}

public struct RelativeFloat
{
    public bool relative;
    public float value;

    public float Apply(float number)
    {
        return relative
            ? number + value
            : value;
    }
}
