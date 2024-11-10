# Spawn Prefab

Adds a command to spawn a prefab.

## Syntax:
`spawn_prefab "<prefab_key>" <pos_x> <pos_y> <pos_z> <rotation_x> <rotation_y> <rotation_z>`

Use the `~` prefix before a position value to make it relative to the player's position. Similarly, use the `~` prefix before a rotation value to make it relative to the prefab's base rotation. 

For a list of available prefabs, refer to [this link](https://xiaoxiao921.github.io/GithubActionCacheTest/assetPathsDump.html). You may want to filter by `UnityEngine.GameObject` to see only the spawnable objects.

### Examples:
- `spawn_prefab "RoR2/Base/ExplosivePotDestructible/ExplosivePotDestructibleBody.prefab"`
- `spawn_prefab "RoR2/Base/Teleporters/LunarTeleporter Variant.prefab" ~20 ~5 ~0`
- `spawn_prefab "RoR2/Base/Teleporters/Teleporter1.prefab" ~0 ~0 ~0 ~0 ~0 ~90`