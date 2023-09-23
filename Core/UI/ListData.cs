using System;
using System.Collections.Generic;
using System.Reflection;

using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Components;

using BetterBeatSaber.Core.Extensions;

using HMUI;

using UnityEngine;

namespace BetterBeatSaber.Core.UI; 

public class ListData<E, C> : TableView.IDataSource where C : ListCell<E> {

    private static FieldInfo? CanSelectSelectedCellField => typeof(TableView).GetField("_canSelectSelectedCell", BindingFlags.Instance | BindingFlags.NonPublic);
    
    public virtual List<E> Items { get; set; } = new();

    public CustomListTableData? Table { get; private set; }

    public virtual float ItemCellSize { get; set; } = 14f;

    // ReSharper disable once MemberCanBeProtected.Global
    public ListData() { }

    public ListData(CustomListTableData table, bool reload = true) {
        Table = table;
        if(table.tableView != null)
            table.tableView.SetDataSource(this, reload);
    }

    public float CellSize() => ItemCellSize;

    public int NumberOfCells() => Items.Count;
    
    public virtual TableCell CellForIdx(TableView tableView, int index) {
        
        var cell = tableView.DequeueReusableCellForIdentifier(nameof(C));
        if(cell == null) {
            
            cell = new GameObject(nameof(C), typeof(Touchable)).AddComponent<C>();
            
            cell.interactable = true;
            cell.reuseIdentifier = nameof(C);
            
            BuildCell(cell);
            
        }

        ((C) cell).Populate(Items[index]);
        
        return cell;
        
    }

    public void Reload() {
        if(Table != null)
            Table.tableView.ReloadData();
    }
    
    public void SetItems(List<E> items, bool reload = true) {
        Items = items;
        if(reload)
            Reload();
    }

    public void SetReselection(bool enabled) {
        if (Table != null)
            CanSelectSelectedCellField?.SetValue(Table.tableView, enabled);
    }
    
    public void ScrollToItem(int index, TableView.ScrollPositionType scrollPositionType = TableView.ScrollPositionType.Beginning, bool animated = false) {
        // ReSharper disable once Unity.NoNullPropagation
        Table?.tableView.ScrollToCellWithIdx(index, scrollPositionType, animated);
    }

    [Obsolete]
    public C? GetItem(E item) {
        if (Table == null || Table.tableView == null)
            return null;
        return CellForIdx(Table.tableView, Items.IndexOf(item)).GetComponent<C>();
    }
    
    protected virtual void BuildCell(TableCell tableCell) =>
        BSMLParser.instance.Parse(typeof(C).ReadViewDefinition(), tableCell.gameObject, tableCell);

}