using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Utils.SOAP.Editor
{
    [CustomEditor(typeof(ScriptableEventVoid), true)]
    public class ScriptableEventVoidEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Listener info is available in Play Mode.", MessageType.Info);
                return;
            }

            var evt = (ScriptableEventVoid)target;
            var listeners = evt.Listeners;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Registered Listeners", EditorStyles.boldLabel);

            if (listeners.Count == 0)
            {
                EditorGUILayout.LabelField("  (none)");
                return;
            }

            for (int i = 0; i < listeners.Count; i++)
            {
                var listener = listeners[i];
                if (listener == null) continue;

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.ObjectField("Listener", listener, listener.GetType(), true);
                DrawUnityEventTargets(listener.Response);
                EditorGUILayout.EndVertical();
            }

            Repaint();
        }

        internal static void DrawUnityEventTargets(UnityEventBase unityEvent)
        {
            if (unityEvent == null) return;

            int count = unityEvent.GetPersistentEventCount();
            for (int j = 0; j < count; j++)
            {
                var target = unityEvent.GetPersistentTarget(j);
                string methodName = unityEvent.GetPersistentMethodName(j);
                string targetType = target != null ? target.GetType().Name : "(null)";
                string targetName = target != null ? target.ToString() : "(null)";

                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField($"{targetType}.{methodName}", EditorStyles.miniLabel);
                if (target is Object obj)
                    EditorGUILayout.ObjectField("  Target", obj, obj.GetType(), true);
                EditorGUI.indentLevel--;
            }

            // Runtime listeners count via reflection
            var field = typeof(UnityEventBase).GetField("m_Calls", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
            {
                var calls = field.GetValue(unityEvent);
                var runtimeField = calls?.GetType().GetField("m_RuntimeCalls", BindingFlags.NonPublic | BindingFlags.Instance);
                if (runtimeField?.GetValue(calls) is System.Collections.IList runtimeCalls && runtimeCalls.Count > 0)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField($"+ {runtimeCalls.Count} runtime listener(s)", EditorStyles.miniLabel);
                    EditorGUI.indentLevel--;
                }
            }
        }
    }

    [CustomEditor(typeof(ScriptableEvent<>), true)]
    public class ScriptableEventGenericEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Listener info is available in Play Mode.", MessageType.Info);
                return;
            }

            // Use reflection to get Listeners property from the generic base
            var listenersProperty = target.GetType().GetProperty("Listeners");
            if (listenersProperty == null) return;

            var listeners = listenersProperty.GetValue(target) as System.Collections.IList;
            if (listeners == null) return;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Registered Listeners", EditorStyles.boldLabel);

            if (listeners.Count == 0)
            {
                EditorGUILayout.LabelField("  (none)");
                return;
            }

            for (int i = 0; i < listeners.Count; i++)
            {
                var listener = listeners[i] as MonoBehaviour;
                if (listener == null) continue;

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.ObjectField("Listener", listener, listener.GetType(), true);

                var responseProp = listener.GetType().GetProperty("Response");
                if (responseProp?.GetValue(listener) is UnityEventBase unityEvent)
                    ScriptableEventVoidEditor.DrawUnityEventTargets(unityEvent);

                EditorGUILayout.EndVertical();
            }

            Repaint();
        }
    }
}
