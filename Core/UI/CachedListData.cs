using System.Collections.Generic;

using BeatSaberMarkupLanguage.Components;

using HMUI;

using UnityEngine;

namespace BetterBeatSaber.Core.UI; 

public class CachedListData<E, C> : ListData<E, C> where C : ListCell<E> {

    public Dictionary<E, C> Cache { get; private set; } = new();

    public CachedListData(CustomListTableData table, bool reload = true) : base(table, reload) { }

    public override TableCell CellForIdx(TableView tableView, int index) {

        var item = Items[index];
        
        if (Cache.TryGetValue(item, out var cachedCell))
            return cachedCell;
        
        var cell = tableView.DequeueReusableCellForIdentifier(nameof(C));
        if(cell == null) {
            
            cell = new GameObject(nameof(C), typeof(Touchable)).AddComponent<C>();
            
            cell.interactable = true;
            cell.reuseIdentifier = nameof(C);

            BuildCell(cell);
            
        }

        ((C) cell).Populate(item);
        
        Cache.Add(item, (C) cell);
        
        return cell;
        
    }

}