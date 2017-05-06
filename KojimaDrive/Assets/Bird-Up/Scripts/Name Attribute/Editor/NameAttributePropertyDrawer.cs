//========================= Kojima Drive - Bird-Up 2017 =========================//
//
// Author: Sam Morris (SpAMCAN)
// Purpose: Property drawer for the NameAttribute
// Namespace: Bird
//
//===============================================================================//

using UnityEditor;
using UnityEngine;

namespace Bird {

	/// <summary>
	/// Drawer for fields with NameAttribute assigned.
	/// </summary>
	[CustomPropertyDrawer(typeof(NameAttribute))]
	public class NameAttributeDrawer : PropertyDrawer {
		/// <summary>
		/// Raises the GUI event and draws the property.
		/// </summary>
		/// <param name="position">Position for the property.</param>
		/// <param name="property">Serialized property.</param>
		/// <param name="label">Label for the property.</param>
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			// Position is the DrawArea of the property to be rendered. Includes height from GetHeight()
			// BeginProperty used for objects that don't handle [SerializeProperty] attribute.
			EditorGUI.BeginProperty(position, label, property);

			// Calculate ObjectReference rect size
			Rect objectReferenceRect = position;

			// Use Unity's default height calculation for the reference rectangle
			float objectReferenceHeight = base.GetPropertyHeight(property, label);
			objectReferenceRect.height = objectReferenceHeight;
			this.BuildObjectField(objectReferenceRect, property, label);

			EditorGUI.EndProperty();
		}

		private void BuildObjectField(Rect drawArea, SerializedProperty property, GUIContent label) {
			NameAttribute myAttribute = (NameAttribute)this.attribute;
			label.text = myAttribute.Name;
			EditorGUI.ObjectField(drawArea, property, label);
			
		}
	}
}