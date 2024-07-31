using UnityEditor;
using UnityEngine.UIElements;
using EditorAttributes.Editor.Utility;

namespace EditorAttributes.Editor
{
    [CustomPropertyDrawer(typeof(HideFieldAttribute))]
    public class HideFieldDrawer : PropertyDrawerBase
    {
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var hideAttribute = attribute as HideFieldAttribute;
			var conditionalProperty = ReflectionUtility.GetValidMemberInfo(hideAttribute.ConditionName, property);

			var root = new VisualElement();
			var errorBox = new HelpBox();

			var propertyField = DrawProperty(property);

			UpdateVisualElement(root, () =>
			{
				if (!GetConditionValue(conditionalProperty, hideAttribute, property, errorBox))
				{
					root.Add(propertyField);
				}
				else
				{
					RemoveElement(root, propertyField);
				}

				DisplayErrorBox(root, errorBox);
			});

			root.Add(propertyField);

			return root;
		}
	}
}
