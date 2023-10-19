using System;

namespace BetterBeatSaber.Core.UI.SDK; 

public sealed class ParseException : Exception {

    public ParseException(string message) : base(message) { }

}