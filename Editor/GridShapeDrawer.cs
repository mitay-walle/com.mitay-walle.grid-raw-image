using mitaywalle.UI.Packages.GridImage.Runtime;
using UnityEditor;
using UnityEngine;

namespace mitaywalle.UI.Packages.GridImage.Editor
{
	[CustomPropertyDrawer(typeof(GridShape))]
	public class GridShapeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			SerializedProperty size = property.FindPropertyRelative("_size");
			SerializedProperty prop = property.FindPropertyRelative("_bitArray");
			EditorGUILayout.PropertyField(size);
			EditorGUILayout.PropertyField(property.FindPropertyRelative("_readable"));

			EditorGUI.BeginChangeCheck();

			BitArray256 bitArray = (BitArray256)prop.boxedValue;

			for (int j = size.vector2IntValue.y - 1; j >= 0; j--)
			{
				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				for (int i = 0; i < size.vector2IntValue.x; i++)
				{
					int index = j * size.vector2IntValue.x + i;
					bool value = bitArray[(uint)index];
					bitArray[(uint)index] = EditorGUILayout.ToggleLeft("", value, GUILayout.ExpandWidth(false),GUILayout.Width(75));
				}
				GUILayout.EndHorizontal();
			}

			if (EditorGUI.EndChangeCheck())
			{
				foreach (Object targetObject in property.serializedObject.targetObjects)
				{
					Undo.RecordObject(targetObject, "GridShape flags");
					prop.boxedValue = bitArray;
				}
			}
		}
	}
}