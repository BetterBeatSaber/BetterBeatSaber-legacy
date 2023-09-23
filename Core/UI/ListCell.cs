using HMUI;

using UnityEngine;

namespace BetterBeatSaber.Core.UI; 

public abstract class ListCell<E> : TableCell {

    public abstract void Populate(E data);

    protected static void SetActiveIfNot(GameObject obj) {
        if(!obj.activeSelf)
            obj.SetActive(true);
    }
    
    protected static void SetInactiveIfNot(GameObject obj) {
        if(obj.activeSelf)
            obj.SetActive(false);
    }

    protected static void SetActiveIf(GameObject obj, bool when, bool setInactiveIfNot = true) {
        switch (when) {
            case true when !obj.activeSelf:
                obj.SetActive(true);
                break;
            case false when obj.activeSelf && setInactiveIfNot:
                obj.SetActive(false);
                break;
        }
    }
    
}