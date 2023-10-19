using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.ViewControllers;

using BetterBeatSaber.Core.Extensions;

using UnityEngine;

namespace BetterBeatSaber.Core.UI;

public abstract class View : BSMLViewController {

    protected const string PostParseEvent = "#post-parse";
    
    public override string Content => UIManager.ReadViewDefinition(GetType());
    
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

    #if DEBUG // To catch any exceptions
    protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling) {
        BSMLParser.instance.Parse(Content, gameObject, this);
    }
    #endif

    public interface IPostParseEventHandler {

        public void PostParse();

    }

}

public abstract class View<T> : View where T : View<T> {

    public static T? Instance { get; private set; }
    
    public override string Content => UIManager.ReadViewDefinition<T>();

    protected View() {
        Instance = (T) this;
    }
    
    protected override void DidDeactivate(bool removedFromHierarchy, bool screenSystemDisabling) {
        
        base.DidDeactivate(removedFromHierarchy, screenSystemDisabling);

        if (!removedFromHierarchy)
            return;

        Instance = null;

    }

}