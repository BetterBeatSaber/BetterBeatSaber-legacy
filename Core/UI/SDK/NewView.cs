using BetterBeatSaber.Core.Extensions;

using UnityEngine;

namespace BetterBeatSaber.Core.UI.SDK; 

public abstract class NewView : HMUI.ViewController {

    protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling) {
        //base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
        Parser.Instance.Parse(GetType().ReadViewDefinition(), this.gameObject, this);
    }

    #region Set Active / Inactive

    protected static void SetActiveIfNot(GameObject? gameObject) => gameObject.SetActiveIfNot();
    protected static void SetInactiveIfNot(GameObject? gameObject) => gameObject.SetActiveIfNot();
    protected static void SetActiveIf(GameObject? gameObject, bool when, bool setInactiveIfNot = true) => gameObject.SetActiveIf(when, setInactiveIfNot);

    protected static void SetActiveIfNot(Component? component) => component?.GameObject.SetActiveIfNot();
    protected static void SetInactiveIfNot(Component? component) => component?.GameObject.SetActiveIfNot();
    protected static void SetActiveIf(Component component, bool when, bool setInactiveIfNot = true) => component?.GameObject.SetActiveIf(when, setInactiveIfNot);

    #endregion

}