using UnityEditor;
using UnityEngine;

namespace SnowIsland.Scripts
{
    /// <summary>
    /// 只读特性功能类
    /// </summary>
  
    /// <summary>
    /// 面板绘制
    /// </summary>
    [CustomPropertyDrawer(typeof(ReadOnly ))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        /// <summary>
        /// 用来保持原有高度
        /// </summary>
        /// <param name="property"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
 
        /// <summary>
        /// 只读
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
    }
}