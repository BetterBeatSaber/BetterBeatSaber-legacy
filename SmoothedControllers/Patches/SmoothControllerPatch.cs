using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

using BetterBeatSaber.Core.Harmomy.Dynamic;
using BetterBeatSaber.SmoothedControllers.Config;

using HarmonyLib;

using UnityEngine;
using UnityEngine.XR;

namespace BetterBeatSaber.SmoothedControllers.Patches; 

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

// https://github.com/kinsi55/BeatSaber_SmoothedController
//[DynamicPatch(typeof(VRController), "Update")]
[HarmonyPatch(typeof(VRController), "Update")]
public sealed class SmoothControllerPatch : DynamicPatch {

    public SmoothControllerPatch(bool enabled) : base(enabled) { }

    private static readonly float PositionSmoothing = 20f - Mathf.Clamp(SmoothedControllersConfig.Instance.PositionSmoothing, 0f, 20f);
    private static readonly float RotationSmoothing = 20f - Mathf.Clamp(SmoothedControllersConfig.Instance.RotationSmoothing, 0f, 20f);

    private static readonly Dictionary<int, OpCode> E = new() { 
	    { 50, OpCodes.Ldarg_0 },
	    { 51, OpCodes.Ldfld },
	    { 68, OpCodes.Ret }
    };

	private static readonly Dictionary<XRNode, XRNodeData> NodeData = new();

	private static VRController? instance;
	
	// ReSharper disable PossibleMultipleEnumeration
	private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
			
		if(!CheckIL(instructions, E))
			return instructions;

		InsertFunction(50, ref instructions, AccessTools.Method(typeof(SmoothControllerPatch), nameof(YES)));

		return instructions;
		
	}

	private static void Prefix(VRController __instance) {
		// Check if the VRController's gameObject name starts with "C" (Controller) so that sabers (LeftHand / RightHand) are not smoothed lmao
		if(__instance.gameObject.name[0] != 'C') {
			instance = null;
			return;
		}
		instance = __instance;
	}

	private static void YES() {
			
		if(instance == null)
			return;

		if(!NodeData.TryGetValue(instance.node, out var data))
			NodeData.Add(instance.node, data = new XRNodeData());

		var angDiff = Quaternion.Angle(data.smoothedRotation, instance.transform.localRotation);
		data.angleVelocitySnap = Math.Min(data.angleVelocitySnap + angDiff, 90f);

		var snapMulti = Mathf.Clamp(data.angleVelocitySnap / SmoothedControllersConfig.Instance.SmallMovementThresholdAngle, .1f, 2.5f);

		if(data.angleVelocitySnap > 0.1) {
			data.angleVelocitySnap -= Math.Max(0.4f, data.angleVelocitySnap / 1.7f);
		}

		if(SmoothedControllersConfig.Instance.PositionSmoothing > 0f) {
			data.smoothedPosition = Vector3.Lerp(data.smoothedPosition, instance.transform.localPosition, PositionSmoothing * Time.deltaTime * snapMulti);
			instance.transform.localPosition = data.smoothedPosition;
		}

		if(!(SmoothedControllersConfig.Instance.RotationSmoothing > 0f))
			return;
		
		data.smoothedRotation = Quaternion.Lerp(data.smoothedRotation, instance.transform.localRotation, RotationSmoothing * Time.deltaTime * snapMulti);
		instance.transform.localRotation = data.smoothedRotation;

	}
	
	#region Utilities

	public static bool CheckIL(IEnumerable<CodeInstruction> instructions, Dictionary<int, OpCode> confirmations) {
		foreach(var c in confirmations) {
			if(instructions.ElementAt(c.Key).opcode != c.Value)
				return false;
		}
		return true;
	}

	public static void InsertFunction(int index, ref IEnumerable<CodeInstruction> instructions, MethodInfo function) {
		
		if(function.ReturnType != typeof(void) || !function.IsStatic)
			throw new Exception("Function must be static void");

		var codeInstructions = instructions.ToList();

		codeInstructions.Insert(index, new CodeInstruction(OpCodes.Call, function));

		instructions = codeInstructions;
		
	}
	
	#endregion
	
	private class XRNodeData {
		
		public Vector3 smoothedPosition = Vector3.zero;
		public Quaternion smoothedRotation = Quaternion.identity;
		public float angleVelocitySnap = 1f;
		
	}
	
}