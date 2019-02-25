#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Linq;

public class ItemsTest : ScriptableWizard
{
    [MenuItem("Tools/Testing")]
    static void CreateWizard()
    {
        DisplayWizard<ItemsTest>("Testing...");
    }
    private int _selected = -1;
    public string SelectedDisplay
    {
        get
        {
            if (_selected < 0 || _selected >= paths.Length)
            {
                return "NOTHING SELECTED";
            }
            return Selected.Split(new char[] { '/', '\\' }).Last();
        }
    }
    public string Selected
    {
        get
        {
            if (_selected < 0 || _selected >= paths.Length)
            {
                return string.Empty;
            }
            return paths[_selected];
        }
    }
    string[] paths;
    private void OnFocus()
    {
        paths = AssetDatabase.GetAllAssetPaths();
    }
    protected override bool DrawWizardGUI()
    {
        if (GUILayout.Button(SelectedDisplay))
        {
            GenericMenu dropdown = new GenericMenu();
            for (int i = 0; i < paths.Length; i++)
            {
                if (paths[i].StartsWith("Assets/"))
                {
                    dropdown.AddItem(
                        //Add the assetpath minus the "Asset/"-part
                        new GUIContent(paths[i].Remove(0, 7)),
                        //show the currently selected item as selected
                        i == _selected,
                        //lambda to set the selected item to the one being clicked
                        selectedIndex => _selected = (int)selectedIndex,
                        //index of this menu item, passed on to the lambda when pressed.
                        i
                   );
                }
            }
            dropdown.ShowAsContext(); //finally show the dropdown
        }
        if (_selected >= 0 && _selected < paths.Length)
        {
            Object selectedAsset = AssetDatabase.LoadAssetAtPath(Selected, typeof(Object));
            Texture t = AssetPreview.GetAssetPreview(selectedAsset);
            if (t != null)
                GUI.DrawTexture(GUILayoutUtility.GetRect(200, 200), t);
        }
        //Do stuff with the selected item
        return (_selected >= 0 && _selected < paths.Length);
    }
}
#endif