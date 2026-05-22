using UnityEditor;
using UnityEngine;
/// <summary>
/// Represents a rigidbody that buoys itself upwards when it reaches a certain height.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class BuoyantBody : MonoBehaviour
{
    [SerializeField] float m_waterHeight;
    [SerializeField] BuoyancyPoint[] m_buoyancyPoints;
    Rigidbody m_rigidbody;
    
    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
    }
    void FixedUpdate()
    {
        Vector3 totalAngularDrag = Vector3.zero;
        foreach (BuoyancyPoint point in m_buoyancyPoints)
        {
            Vector3 globalPointPosition = transform.TransformPoint(point.LocalPosition);
            float depth = m_waterHeight - globalPointPosition.y;
            if (depth <= 0) continue;
            float submersion = Mathf.Clamp01(depth / point.Radius);
            float buoyancyForce = submersion * point.PointBuoyancy;
            m_rigidbody.AddForceAtPosition(buoyancyForce * Vector3.up, globalPointPosition);
            Vector3 pointVelocity = m_rigidbody.GetPointVelocity(globalPointPosition);
            m_rigidbody.AddForceAtPosition(-pointVelocity * (submersion * point.LinearDrag), globalPointPosition);
            totalAngularDrag += -m_rigidbody.angularVelocity * (submersion * point.AngularDrag);
        }
        m_rigidbody.AddTorque(totalAngularDrag);
    }
}
/// <summary>
/// Allows the user to edit the buoyancy points of a <see cref="BuoyantBody"/> in the scene.
/// </summary>
[CustomEditor(typeof(BuoyantBody))]
public class BuoyantBodyEditor : Editor
{
    int m_selectedIndex = -1;

    public void OnSceneGUI()
    {
        serializedObject.Update();
        SerializedProperty buoyancyPoints = serializedObject.FindProperty("m_buoyancyPoints");
        Transform objectTransform = ((BuoyantBody)target).transform;
        for (int i = 0; i < buoyancyPoints.arraySize; i++)
        {
            SerializedProperty point = buoyancyPoints.GetArrayElementAtIndex(i).FindPropertyRelative("LocalPosition");
            Vector3 worldPosition = objectTransform.TransformPoint(point.vector3Value);
            Vector3 updatedPosition = worldPosition;
            bool isSelected = i == m_selectedIndex;
            float size = buoyancyPoints.GetArrayElementAtIndex(i).FindPropertyRelative("Radius").floatValue * 2;
            Handles.color = m_selectedIndex == i ? Color.yellow : Color.cyan;
            EditorGUI.BeginChangeCheck();
            if(isSelected)
            {
                updatedPosition = Handles.DoPositionHandle(worldPosition, Quaternion.identity);
                Handles.SphereHandleCap(0, worldPosition, Quaternion.identity, size, EventType.Repaint);
            }
            else if(Handles.Button(worldPosition, Quaternion.identity, size, size, Handles.SphereHandleCap)) m_selectedIndex = i;
            if (!EditorGUI.EndChangeCheck()) continue;
            point.vector3Value = objectTransform.InverseTransformPoint(updatedPosition);
            serializedObject.ApplyModifiedProperties();
        }
    }
}