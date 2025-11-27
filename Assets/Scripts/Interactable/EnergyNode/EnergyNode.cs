using System.Collections.Generic;
using UnityEngine;

public class EnergyNode : MonoBehaviour
{
    [Header("Tipo de nodo")]
    [Tooltip("Marca este nodo como el origen del flujo de energía")]
    public bool isSource = false;

    [Tooltip("Marca este nodo como el destino (árbol central)")]
    public bool isTarget = false;

    [Header("Conexiones fijas")]
    [Tooltip("Direcciones a las que conecta este nodo")]
    public List<RootSegment.Direction> connections = new List<RootSegment.Direction>();

    /// <summary>
    /// Devuelve las direcciones a las que este nodo envía/recibe energía.
    /// </summary>
    public List<RootSegment.Direction> GetConnections()
    {
        return connections;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        // Color distinto según rol
        if (isSource)
            Gizmos.color = Color.cyan;
        else if (isTarget)
            Gizmos.color = Color.magenta;
        else
            Gizmos.color = Color.yellow;

        Gizmos.DrawSphere(transform.position, 0.3f);

        // Dibujar las conexiones
        Gizmos.color = Color.green;
        Vector3 origin = transform.position;

        foreach (var dir in connections)
        {
            Vector3 offset = Vector3.zero;

            switch (dir)
            {
                case RootSegment.Direction.North: offset = Vector3.forward; break;
                case RootSegment.Direction.East: offset = Vector3.right; break;
                case RootSegment.Direction.South: offset = Vector3.back; break;
                case RootSegment.Direction.West: offset = Vector3.left; break;
            }

            Gizmos.DrawLine(origin, origin + offset);
            Gizmos.DrawSphere(origin + offset, 0.1f);
        }
    }
#endif
}
