using UnityEngine;

[ExecuteInEditMode]
public class CleanCabinetBuilder : MonoBehaviour
{
    [ContextMenu("Перебудувати щиток красиво")]
    public void BuildCabinet()
    {
        // 1. Очищення старих деталей
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }

        // --- КОРПУС ШАФИ (ПОРОЖНИСТИЙ КОРОБ) ---
        // Задня стінка
        GameObject backWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        backWall.name = "Cabinet_Back";
        backWall.transform.SetParent(transform, false);
        backWall.transform.localPosition = new Vector3(0, 0, 0.08f);
        backWall.transform.localScale = new Vector3(0.55f, 0.7f, 0.02f);
        SetColor(backWall, new Color(0.2f, 0.22f, 0.25f));

        // Ліва стінка
        GameObject leftWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        leftWall.name = "Wall_Left";
        leftWall.transform.SetParent(transform, false);
        leftWall.transform.localPosition = new Vector3(-0.265f, 0, 0);
        leftWall.transform.localScale = new Vector3(0.02f, 0.7f, 0.16f);
        SetColor(leftWall, new Color(0.2f, 0.22f, 0.25f));

        // Права стінка
        GameObject rightWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rightWall.name = "Wall_Right";
        rightWall.transform.SetParent(transform, false);
        rightWall.transform.localPosition = new Vector3(0.265f, 0, 0);
        rightWall.transform.localScale = new Vector3(0.02f, 0.7f, 0.16f);
        SetColor(rightWall, new Color(0.2f, 0.22f, 0.25f));

        // Верхня кришка
        GameObject topWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        topWall.name = "Wall_Top";
        topWall.transform.SetParent(transform, false);
        topWall.transform.localPosition = new Vector3(0, 0.34f, 0);
        topWall.transform.localScale = new Vector3(0.55f, 0.02f, 0.16f);
        SetColor(topWall, new Color(0.2f, 0.22f, 0.25f));

        // Нижня кришка
        GameObject bottomWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        bottomWall.name = "Wall_Bottom";
        bottomWall.transform.SetParent(transform, false);
        bottomWall.transform.localPosition = new Vector3(0, -0.34f, 0);
        bottomWall.transform.localScale = new Vector3(0.55f, 0.02f, 0.16f);
        SetColor(bottomWall, new Color(0.2f, 0.22f, 0.25f));

        // Монтажна панель всередині ніші
        GameObject innerPlate = GameObject.CreatePrimitive(PrimitiveType.Cube);
        innerPlate.name = "Inner_DIN_Plate";
        innerPlate.transform.SetParent(transform, false);
        innerPlate.transform.localPosition = new Vector3(0, 0, 0.065f);
        innerPlate.transform.localScale = new Vector3(0.48f, 0.62f, 0.01f);
        SetColor(innerPlate, new Color(0.12f, 0.13f, 0.15f));

        // DIN-рейка під запобіжники
        GameObject dinRail = GameObject.CreatePrimitive(PrimitiveType.Cube);
        dinRail.name = "DIN_Rail";
        dinRail.transform.SetParent(transform, false);
        dinRail.transform.localPosition = new Vector3(0, 0.1f, 0.05f);
        dinRail.transform.localScale = new Vector3(0.42f, 0.03f, 0.015f);
        SetColor(dinRail, new Color(0.6f, 0.62f, 0.65f));

        // --- ВНУТРІШНІ ОРГАНИ: 3 ЗАПОБІЖНИКИ ---
        Renderer[] fuseRenderers = new Renderer[3];
        Color[] phaseColors = {
            new Color(0.85f, 0.2f, 0.2f),   // Червоний (Фаза A)
            new Color(0.9f, 0.75f, 0.1f),   // Жовтий (Фаза B)
            new Color(0.2f, 0.75f, 0.3f)    // Зелений (Фаза C)
        };
        float[] xPositions = { -0.12f, 0f, 0.12f };

        for (int i = 0; i < 3; i++)
        {
            GameObject fuse = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            fuse.name = $"Fuse_Phase_{i + 1}";
            fuse.transform.SetParent(transform, false);
            fuse.transform.localPosition = new Vector3(xPositions[i], 0.1f, 0.02f);
            fuse.transform.localScale = new Vector3(0.045f, 0.06f, 0.045f);
            SetColor(fuse, phaseColors[i]);
            fuseRenderers[i] = fuse.GetComponent<Renderer>();
        }

        // --- ВНУТРІШНІ ОРГАНИ: РУБИЛЬНИК (LEVER) ---
        GameObject switchBase = GameObject.CreatePrimitive(PrimitiveType.Cube);
        switchBase.name = "Switch_Base";
        switchBase.transform.SetParent(transform, false);
        switchBase.transform.localPosition = new Vector3(0, -0.14f, 0.045f);
        switchBase.transform.localScale = new Vector3(0.11f, 0.14f, 0.03f);
        SetColor(switchBase, new Color(0.05f, 0.05f, 0.06f));

        // Петля важеля рубильника (Pivot)
        GameObject leverPivot = new GameObject("Switch_Lever_Pivot");
        leverPivot.transform.SetParent(transform, false);
        leverPivot.transform.localPosition = new Vector3(0, -0.14f, 0.025f);

        GameObject switchLever = GameObject.CreatePrimitive(PrimitiveType.Cube);
        switchLever.name = "Switch_Lever";
        switchLever.transform.SetParent(leverPivot.transform, false);
        switchLever.transform.localPosition = new Vector3(0, 0.04f, 0);
        switchLever.transform.localScale = new Vector3(0.03f, 0.1f, 0.025f);
        SetColor(switchLever, new Color(0.95f, 0.45f, 0.05f)); // Помаранчевий важіль

        // --- ДВЕРЦЯТА ШАФИ (DOOR) ---
        GameObject hinge = new GameObject("Door_Hinge");
        hinge.transform.SetParent(transform, false);
        hinge.transform.localPosition = new Vector3(-0.265f, 0, -0.08f);

        GameObject door = GameObject.CreatePrimitive(PrimitiveType.Cube);
        door.name = "Cabinet_Door";
        door.transform.SetParent(hinge.transform, false);
        door.transform.localPosition = new Vector3(0.26f, 0, 0);
        door.transform.localScale = new Vector3(0.52f, 0.68f, 0.015f);
        SetColor(door, new Color(0.32f, 0.35f, 0.38f));

        // Ручка дверцят
        GameObject handle = GameObject.CreatePrimitive(PrimitiveType.Cube);
        handle.name = "Door_Handle";
        handle.transform.SetParent(door.transform, false);
        handle.transform.localPosition = new Vector3(0.21f, 0, -0.02f);
        handle.transform.localScale = new Vector3(0.025f, 0.12f, 0.025f);
        SetColor(handle, Color.black);

        // Прив'язка до скрипта взаємодії
        CabinetInteractions interactions = GetComponent<CabinetInteractions>();
        if (interactions == null) interactions = gameObject.AddComponent<CabinetInteractions>();
        interactions.doorHinge = hinge.transform;
        interactions.leverPivot = leverPivot.transform;
        interactions.fuses = fuseRenderers;
        interactions.openAngle = 115f;

        Debug.Log("<color=green>✓ ЩИТОК СТВОРЕНО: Порожниста шафа, запобіжники на DIN-рейці та рубильник готові!</color>");
    }

    private void SetColor(GameObject obj, Color color)
    {
        Renderer rend = obj.GetComponent<Renderer>();
        if (rend != null)
        {
            Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.color = color;
            rend.material = mat;
        }
    }
}