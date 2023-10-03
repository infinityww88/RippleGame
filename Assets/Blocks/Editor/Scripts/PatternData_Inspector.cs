using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[CustomEditor(typeof(PatternData))]
public class PatternData_Inspector : Editor 
{
    public string markClass = "mark";
    public VisualTreeAsset m_InspectorXML;
    public override UnityEngine.UIElements.VisualElement CreateInspectorGUI()
    {
	    SerializedProperty property = this.serializedObject.FindProperty("data");
	    ulong data = property.ulongValue;
        var root = m_InspectorXML.CloneTree();
        int i = 0;
        root.Query(classes: "cell").ForEach(e =>
        {
            int row = i / 5, col = i % 5;
	        if (PatternData.GetMask(row, col, data))
            {
                e.AddToClassList(markClass);
            }
            else
            {
                e.RemoveFromClassList(markClass);
            }
            e.userData = i++;
            e.RegisterCallback<ClickEvent>(OnCellClick);
        });
        var defaultInspector = root.Q("Default_Inspector");
        InspectorElement.FillDefaultInspector(defaultInspector, serializedObject, this);
        return root;
    }

    private void OnCellClick(ClickEvent evt)
    {
        SerializedProperty property = this.serializedObject.FindProperty("data");
	    ulong data = property.ulongValue;
	    var pRow = serializedObject.FindProperty("row");
	    var pCol = serializedObject.FindProperty("col");

        var cell = evt.target as VisualElement;
        int idx = (int)cell.userData;
        int row = idx / 5, col = idx % 5;
	    cell.ToggleInClassList("mark");

	    data = PatternData.SetMask(row, col, data, cell.ClassListContains(markClass));
        
	    var size = PatternData.GetSize(data);

	    property.ulongValue = data;
	    pRow.intValue = size.Item1;
	    pCol.intValue = size.Item2;
	    
        serializedObject.ApplyModifiedProperties();
    }
}
