using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    static UIManager instance;

    [SerializeField] UIObject _startUIObject;
    [SerializeField] UIObject[] _uiObjs;
    UIObject _currentUIObject;

    readonly Stack<UIObject> _history = new Stack<UIObject>();
    private void Awake() => instance = this;
    
    public static T GetObject<T>() where T : UIObject
    {
        for (int i = 0; i < instance._uiObjs.Length; i++)
        {
            if(instance._uiObjs[i] is T tuiObj)
            {
                return tuiObj;
            }
        }

        return null;
    }

    public static void Show<T>(bool remember = true) where T : UIObject
    {
        for (int i = 0; i < instance._uiObjs.Length; i++)
        {
            if(instance._uiObjs[i] is T)
            {
                if (instance._currentUIObject != null)
                {
                    if (remember)
                    {
                        instance._history.Push(instance._currentUIObject);
                    }

                    instance._currentUIObject.Hide();
                }

                instance._uiObjs[i].Show();

                instance._currentUIObject = instance._uiObjs[i];
            }
        }
    }

    public static void Show(UIObject uiObj, bool remember = true)
    {
        if(instance._currentUIObject != null)
        {
            if (remember)
            {
                instance._history.Push(instance._currentUIObject);
            }
            instance._currentUIObject.Hide();
        }

        uiObj.Show();

        instance._currentUIObject = uiObj;
    }

    public static void ShowLast()
    {
        if(instance._history.Count != 0)
        {
            Show(instance._history.Pop(), false);
        }
    }
}
