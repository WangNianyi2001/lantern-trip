using UnityEditor;
using UnityEngine;

namespace LanternTrip {
	[CustomPropertyDrawer(typeof(InstanceAttribute))]
	public class InstanceDrawer : PropertyDrawer {
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			return EditorGUI.GetPropertyHeight(property, label);
		}
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			if(property.propertyType == SerializedPropertyType.ObjectReference) {
				if(property.objectReferenceValue != null) {
					EditorGUI.PropertyField(position, property, label);
					return;
				}
				if(GUI.Button(position, $"Create {label.text}")) {
					var type = fieldInfo.FieldType;
					Object value;
					if(typeof(ScriptableObject).IsAssignableFrom(type))
						value = ScriptableObject.CreateInstance(type);
					else
						value = System.Activator.CreateInstance(type) as Object;
					property.objectReferenceValue = value;
					property.serializedObject.ApplyModifiedProperties();
				}
			}
		}
	}
}