using System;

namespace BetterBeatSaber.Shared.Enums; 

[Flags]
public enum PlayerFlag : ushort {

    IsSteven = 0x001,
    
    HasCustomName = 0x101,
    
    HasScoreSaber = 0x210,
    HasBeatLeader = 0x211

}