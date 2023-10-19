using BeatSaberMarkupLanguage.Components;

namespace BetterBeatSaber.Core.UI.StringList; 

public sealed class StringList : ListData<string, StringListCell> {

    public StringList() { }
    public StringList(CustomListTableData table, bool reload = true) : base(table, reload) { }

}