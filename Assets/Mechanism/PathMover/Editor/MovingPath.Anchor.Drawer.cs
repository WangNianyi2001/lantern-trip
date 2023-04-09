using UnityEngine;
using UnityEditor;

namespace LanternTrip {
	[CustomPropertyDrawer(typeof(MovingPath.Anchor))]
	public class MovingPathAnchorDrawer : PropertyDrawer {
		MovingPath path;

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			var target = property.serializedObject.targetObject;
			if(!(target is MovingPath)) {
				path = null;
				return base.GetPropertyHeight(property, label);
			}
			path = target as MovingPath;
			if(!path.controlRotation)
				return EditorGUIUtility.singleLineHeight;
			else
				return 2 * EditorGUIUtility.singleLineHeight;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			if(path == null) {
				base.OnGUI(position, property, label);
				return;
			}
			
			EditorGUI.BeginChangeCheck();

			position.height = EditorGUIUtility.singleLineHeight;
			var pPosition = property.FindPropertyRelative("position");
			EditorGUI.PropertyField(position, pPosition);
			position.y += EditorGUIUtility.singleLineHeight;

			if(path.controlRotation) {
				position.height = EditorGUIUtility.singleLineHeight;
				var pEuler = property.FindPropertyRelative("rulerAngles");
				EditorGUI.PropertyField(position, pEuler);
			}

			if(EditorGUI.EndChangeCheck())
				property.serializedObject.ApplyModifiedProperties();
		}
	}
}