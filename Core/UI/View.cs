using BeatSaberMarkupLanguage.ViewControllers;

using BetterBeatSaber.Core.Extensions;

using UnityEngine;

namespace BetterBeatSaber.Core.UI;

public abstract class View
#if DEBUG
: BSMLAutomaticViewController
#else
: BSMLViewController
#endif
{

    protected const string PostParseEvent = "#post-parse";
    
    public override string Content => GetType().ReadViewDefinition();
    
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
    
    protected override void DidDeactivate(bool removedFromHierarchy, bool screenSystemDisabling) {
        
        base.DidDeactivate(removedFromHierarchy, screenSystemDisabling);

        if (!removedFromHierarchy)
            return;

        OnDeactivate();
        
    }
    
    protected virtual void OnDeactivate() {}

    public interface IPostParseEventHandler {

        public void PostParse();

    }

}

public abstract class View<T> : View where T : View<T> {

    public static T? Instance { get; private set; }
    
    public override string Content => typeof(T).ReadViewDefinition();

    protected View() {
        Instance = (T) this;
    }
    
    protected override void DidDeactivate(bool removedFromHierarchy, bool screenSystemDisabling) {
        
        base.DidDeactivate(removedFromHierarchy, screenSystemDisabling);

        if (!removedFromHierarchy)
            return;

        OnDeactivate();
        
        Instance = null;

    }

}