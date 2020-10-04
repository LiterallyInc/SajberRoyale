using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;

/// <summary>
/// The behaviour of this class is almost the same as the original except:
/// 1. It absorbs version differences.
/// 2. It corrects the calculation of vertex list capacity (Unity 5.3 or older).
/// </summary>
public class ModifiedShadow : Shadow
{
#if !UNITY_5_4_OR_NEWER
    protected new void ApplyShadow(List<UIVertex> verts, Color32 color, int start, int end, float x, float y)
    {
        UIVertex vt;

        // The capacity calculation of the original version seems wrong.
        var neededCpacity = verts.Count + (end - start);
        if (verts.Capacity < neededCpacity)
            verts.Capacity = neededCpacity;

        for (int i = start; i < end; ++i)
        {
            vt = verts[i];
            verts.Add(vt);

            Vector3 v = vt.position;
            v.x += x;
            v.y += y;
            vt.position = v;
            var newColor = color;
            if (useGraphicAlpha)
                newColor.a = (byte)((newColor.a * verts[i].color.a) / 255);
            vt.color = newColor;
            verts[i] = vt;
        }
    }
#endif

#if UNITY_5_2 && !UNITY_5_2_1pX
    public override void ModifyMesh(Mesh mesh)
    {
        if (!this.IsActive())
            return;

        using (var vh = new VertexHelper(mesh))
        {
            ModifyMesh(vh);
            vh.FillMesh(mesh);
        }
    }
#endif

#if !(UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1)
#if UNITY_5_2_1pX || UNITY_5_3_OR_NEWER
    public override void ModifyMesh(VertexHelper vh)
#else
    public void ModifyMesh(VertexHelper vh)
#endif
    {
        if (!this.IsActive())
            return;

        var list = ListPool<UIVertex>.Get();
        vh.GetUIVertexStream(list);

        ModifyVertices(list);

#if UNITY_5_2_1pX || UNITY_5_3_OR_NEWER
        vh.Clear();
#endif
        vh.AddUIVertexTriangleStream(list);
        ListPool<UIVertex>.Release(list);
    }

    public virtual void ModifyVertices(List<UIVertex> verts)
    {
    }
#endif
}
static class ListPool<T>
{
    // Object pool to avoid allocations.
    private static readonly ObjectPool<List<T>> s_ListPool = new ObjectPool<List<T>>(null, l => l.Clear());

    public static List<T> Get()
    {
        return s_ListPool.Get();
    }

    public static void Release(List<T> toRelease)
    {
        s_ListPool.Release(toRelease);
    }
}
public class ObjectPool<T> where T : new()
{
    private readonly Stack<T> m_Stack = new Stack<T>();
    private readonly UnityAction<T> m_ActionOnGet;
    private readonly UnityAction<T> m_ActionOnRelease;

    public int countAll { get; private set; }
    public int countActive { get { return countAll - countInactive; } }
    public int countInactive { get { return m_Stack.Count; } }

    public ObjectPool(UnityAction<T> actionOnGet, UnityAction<T> actionOnRelease)
    {
        m_ActionOnGet = actionOnGet;
        m_ActionOnRelease = actionOnRelease;
    }

    public T Get()
    {
        T element;
        if (m_Stack.Count == 0)
        {
            element = new T();
            countAll++;
        }
        else
        {
            element = m_Stack.Pop();
        }
        if (m_ActionOnGet != null)
            m_ActionOnGet(element);
        return element;
    }

    public void Release(T element)
    {
        if (m_Stack.Count > 0 && ReferenceEquals(m_Stack.Peek(), element))
            Debug.LogError("Internal error. Trying to destroy object that is already released to pool.");
        if (m_ActionOnRelease != null)
            m_ActionOnRelease(element);
        m_Stack.Push(element);
    }
}