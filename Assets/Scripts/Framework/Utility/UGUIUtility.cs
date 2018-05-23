using UnityEngine;

public static class UGUIUtility
{
    public static GameObject AddChild(this GameObject InParent)
    {
        return InParent == null ? new GameObject() : AddChild(InParent.transform);
    }

    public static GameObject AddChild(this Transform InParent)
    {
        GameObject go = new GameObject();

        if (InParent != null)
        {
            Transform trans = go.transform;
            trans.SetParent(InParent);
            trans.localPosition = Vector3.zero;
            trans.localRotation = Quaternion.identity;
            trans.localScale = Vector3.one;
        }

        return go;
    }

    public static GameObject AddChild(this GameObject InParent, GameObject InPrefab)
    {
        return InParent == null ? Object.Instantiate(InPrefab) : AddChild(InParent.transform, InPrefab);
    }

    public static GameObject AddChild(this Transform InParent, GameObject InPrefab)
    {
        GameObject go = Object.Instantiate(InPrefab);

        if (go != null && InParent != null)
        {
            Transform trans = go.transform;
            trans.SetParent(InParent);
            trans.localPosition = Vector3.zero;
            trans.localRotation = Quaternion.identity;
            trans.localScale = Vector3.one;
        }
        return go;
    }

    public static void ClearChild(this GameObject InParent)
    {
        if (null != InParent) ClearChild(InParent.transform);
    }

    public static void ClearChild(this Transform InParent)
    {
        if (null != InParent)
        {
            int count = InParent.childCount;
            for (int i = 0; i < count; i++)
            {
                Object.Destroy(InParent.GetChild(0).gameObject);
            }
        }
    }

    public static void clearPanel(Transform panel)
    {
        int childCount = panel.childCount;
        for (int i = 0; i < childCount; i++)
        {
            GameObject.DestroyImmediate(panel.GetChild(0).gameObject);
            //GameObject.Destroy(panel.GetChild(0).gameObject, 0.2f);
        }
        panel.DetachChildren();
    }

    public static T AddChild<T>(this GameObject InParent) where T : Component
    {
        GameObject go = AddChild(InParent);
        go.name = typeof(T).Name;
        return go.AddComponent<T>();
    }

    public static void SetCusParent(this Transform InTrans, Transform InParent)
    {
        InTrans.SetParent(InParent);
        InTrans.localPosition = Vector3.zero;
        InTrans.localRotation = Quaternion.identity;
        InTrans.localScale = Vector3.one;
    }

    public static void SetCusParent(this Transform InTrans, Transform InParent,
        int InSibling)
    {
        InTrans.SetParent(InParent);
        InTrans.localPosition = Vector3.zero;
        InTrans.localRotation = Quaternion.identity;
        InTrans.localScale = Vector3.one;
        InTrans.SetSiblingIndex(InSibling);
    }

    public static void SetCusParent(this Transform InTrans, Transform InParent,
        Vector3 InLocalPosition)
    {
        InTrans.SetParent(InParent);
        InTrans.localPosition = InLocalPosition;
        InTrans.localRotation = Quaternion.identity;
        InTrans.localScale = Vector3.one;
    }

    public static void SetCusParent(this Transform InTrans, Transform InParent,
        Vector3 InLocalPosition, Quaternion InLocalRotation)
    {
        InTrans.SetParent(InParent);
        InTrans.localPosition = InLocalPosition;
        InTrans.localRotation = InLocalRotation;
        InTrans.localScale = Vector3.one;
    }

    public static void SetCusParent(this Transform InTrans, Transform InParent,
        Vector3 InLocalPosition, Quaternion InLocalRotation, Vector3 InLocalScale)
    {
        InTrans.SetParent(InParent);
        InTrans.localPosition = InLocalPosition;
        InTrans.localRotation = InLocalRotation;
        InTrans.localScale = InLocalScale;
    }

    public static void SetCusParent(this Transform InTrans, Transform InParent,
        Vector3 InLocalPosition, Quaternion InLocalRotation, Vector3 InLocalScale,
        int InSibling)
    {
        InTrans.SetParent(InParent);
        InTrans.localPosition = InLocalPosition;
        InTrans.localRotation = InLocalRotation;
        InTrans.localScale = InLocalScale;
        InTrans.SetSiblingIndex(InSibling);
    }


    public static Vector3 MultVector3(Vector3 InVector3, float InMult)
    {
        InVector3.x *= InMult;
        InVector3.y *= InMult;
        InVector3.z *= InMult;

        return InVector3;
    }

    public static void ResetPivot(this RectTransform InRectTrans, Vector2 InPivot)
    {
        Vector2 pivot = InRectTrans.pivot;
        if (!InPivot.Equals(pivot))
        {
            InRectTrans.pivot = InPivot;

            Vector2 sizeDelta = InRectTrans.sizeDelta;
            Vector3 pos = InRectTrans.localPosition;
            pos.x += sizeDelta.x * (InPivot.x - pivot.x);
            pos.y += sizeDelta.y * (InPivot.y - pivot.y);

            InRectTrans.localPosition = pos;
        }
    }

    public static Vector2 TransformToCanvasLocalPosition(this Transform InTrans, Canvas InCanvas)
    {
        Vector2 localPos;
        return RectTransformUtility.ScreenPointToLocalPointInRectangle(InCanvas.GetComponent<RectTransform>(),
            InCanvas.worldCamera.WorldToScreenPoint(InTrans.position),
            InCanvas.worldCamera,
            out localPos) ? localPos : Vector2.zero;
    }

    public static Vector3 GetScreenFitFactor(float InOriginalWidth, float InOriginalHeight, bool InIsMatchWithWidth = true)
    {
        float scaleFactor;
        if (InIsMatchWithWidth) scaleFactor = Screen.height / (Screen.width / InOriginalWidth) / InOriginalHeight;
        else scaleFactor = Screen.width / (Screen.height / InOriginalHeight) / InOriginalWidth;
        return new Vector3(scaleFactor, scaleFactor, scaleFactor);
    }
}
