using HarmonyLib;

using IPA.Utilities;

using UnityEngine;

using Zenject;

namespace BetterBeatSaber.MissTexts.Patches; 

[HarmonyPatch(typeof(MissedNoteEffectSpawner), "HandleNoteWasMissed", MethodType.Normal)]
public sealed class MissedNoteEffectSpawnerHandleNoteWasMissedPatch {

    internal static DiContainer Container = null!;

    private static FlyingTextSpawner _spawner = null!;
    private static FlyingTextSpawner SpawnerBase {
        get {
            
            if (_spawner != null)
                return _spawner;
            
            _spawner = new GameObject("CustomMissTextSpawner").AddComponent<FlyingTextSpawner>();

            var installers = Object.FindObjectsOfType<MonoInstallerBase>();
            foreach (MonoInstallerBase installer in installers) {
                Container = installer.GetProperty<DiContainer, MonoInstallerBase>("Container");
                if (Container != null && Container.HasBinding<FlyingTextEffect.Pool>()) {
                    Container.Inject(_spawner);
                    break;
                }
            }

            _spawner.SetField("_color", Color.white);

            return _spawner;
        }
    }

    // ReSharper disable once InconsistentNaming
    [HarmonyPrefix]
    public static bool Prefix(NoteController noteController, float ____spawnPosZ) {
        
        if (SpawnerBase == null)
            return true;

        var noteData = noteController.noteData;
        if (noteData.colorType == ColorType.None)
            return false;
        
        var pos = noteController.noteTransform.position;
        var rot = noteController.worldRotation;
        
        pos = noteController.inverseWorldRotation * pos;
        pos.z = ____spawnPosZ;
        pos = rot * pos;
        
        _spawner.SpawnText(pos, noteController.worldRotation, noteController.inverseWorldRotation, "MISS");

        return false;
        
    }

}