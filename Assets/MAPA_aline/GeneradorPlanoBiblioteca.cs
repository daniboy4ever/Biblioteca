using UnityEngine;

public class GeneradorPlanoBiblioteca : MonoBehaviour
{
    [Header("Dimensiones Principales")]
    public float anchoNaveCentral = 12f;
    public float largoNaveCentral = 24f;
    public float altoPared = 5f;
    public float grosorPared = 0.2f;

    [Header("Techo Abovedado / Curvo")]
    [Range(8, 32)] public int suavizadoTecho = 20;
    public float alturaExtraBoveda = 0f; // Para hacer el techo más alto o chato

    [Header("Entrada Principal (Línea Azul)")]
    public float anchoPuertaDoble = 4f;

    [Header("Ábside Curvo / Área Infantil (Línea Naranja Superior)")]
    public float radioAbside = 6f;
    [Range(8, 32)] public int segmentosCurvaAbside = 16;

    [Header("Ventanas Laterales (Líneas Naranjas Laterales)")]
    public float anchoVentanaLateral = 5f;

    [Header("Salas / Alcobas Laterales")]
    public int salasPorLado = 4;
    public float profundidadSalas = 5f;

    [Header("Materiales (Opcional)")]
    public Material materialPiso;
    public Material materialParedes;
    public Material materialTecho;

    [ContextMenu("Generar Plano Biblioteca")]
    public void GenerarPlano()
    {
        // 1. Limpiar generación anterior
        Transform previa = transform.Find("Biblioteca_Plano");
        if (previa != null) DestroyImmediate(previa.gameObject);

        GameObject root = new GameObject("Biblioteca_Plano");
        root.transform.SetParent(this.transform);
        root.transform.localPosition = Vector3.zero;

        // 2. Piso de la Nave Central
        CrearCubo("Piso_NaveCentral", new Vector3(0, -grosorPared / 2f, 0), new Vector3(anchoNaveCentral, grosorPared, largoNaveCentral), root.transform, materialPiso);

        // 3. Pared Frontal con Hueco para Puerta Doble
        GenerarParedEntradaFrontal(root.transform);

        // 4. Paredes Exteriores, Salas y Ventanas Laterales
        GenerarSalasYVentanasLaterales(root.transform);

        // 5. Ábside Semicircular al Fondo (Área Infantil)
        GenerarAbsideInfantilCurvo(root.transform);

        // 6. TECHO ABOVEDADO CURVO DE BIBLIOTECA
        GenerarTechoBoveda(root.transform);

        // 7. Guías de Elementos Centrales (Recepción, Óvalo e Infantil)
        GenerarGuiasCentrales(root.transform);
    }

    private void GenerarTechoBoveda(Transform padre)
    {
        GameObject techo = new GameObject("Techo_Abovedado_Biblioteca");
        techo.transform.SetParent(padre);
        techo.transform.localPosition = new Vector3(0, altoPared + alturaExtraBoveda, 0);

        MeshFilter mf = techo.AddComponent<MeshFilter>();
        MeshRenderer mr = techo.AddComponent<MeshRenderer>();
        MeshCollider mc = techo.AddComponent<MeshCollider>();

        Mesh meshTecho = GenerarMeshBoveda(anchoNaveCentral, largoNaveCentral, suavizadoTecho);
        mf.sharedMesh = meshTecho;
        mc.sharedMesh = meshTecho;

        if (materialTecho != null)
        {
            mr.sharedMaterial = materialTecho;
        }
        else
        {
            mr.sharedMaterial = ObtenerMaterialPorDefecto();
        }
    }

   private Mesh GenerarMeshBoveda(float ancho, float largo, int segmentos)
{
    Mesh mesh = new Mesh();
    
    int numVertsPorCara = (segmentos + 1) * 2;
    Vector3[] vertices = new Vector3[numVertsPorCara * 2];
    Vector2[] uvs = new Vector2[numVertsPorCara * 2];
    int[] triangulos = new int[segmentos * 6 * 2];

    float radio = ancho / 2f;

    // 1. Vértices para la cara INTERIOR
    for (int i = 0; i <= segmentos; i++)
    {
        float pct = (float)i / segmentos;
        float angulo = pct * Mathf.PI;

        float x = -Mathf.Cos(angulo) * radio;
        float y = Mathf.Sin(angulo) * radio;

        int vIdx = i * 2;
        vertices[vIdx] = new Vector3(x, y, -largo / 2f);
        uvs[vIdx] = new Vector2(pct, 0);

        vertices[vIdx + 1] = new Vector3(x, y, largo / 2f);
        uvs[vIdx + 1] = new Vector2(pct, 1);
    }

    // 2. Vértices para la cara EXTERIOR (Independientes para corregir la iluminación)
    for (int i = 0; i <= segmentos; i++)
    {
        int vIdx = numVertsPorCara + (i * 2);
        vertices[vIdx] = vertices[i * 2];
        uvs[vIdx] = uvs[i * 2];

        vertices[vIdx + 1] = vertices[(i * 2) + 1];
        uvs[vIdx + 1] = uvs[(i * 2) + 1];
    }

    int tIdx = 0;

    // Triángulos de la cara Interior
    for (int i = 0; i < segmentos; i++)
    {
        int c = i * 2;
        int n = (i + 1) * 2;

        triangulos[tIdx++] = c;
        triangulos[tIdx++] = c + 1;
        triangulos[tIdx++] = n + 1;

        triangulos[tIdx++] = c;
        triangulos[tIdx++] = n + 1;
        triangulos[tIdx++] = n;
    }

    // Triángulos de la cara Exterior
    for (int i = 0; i < segmentos; i++)
    {
        int c = numVertsPorCara + (i * 2);
        int n = numVertsPorCara + ((i + 1) * 2);

        triangulos[tIdx++] = c;
        triangulos[tIdx++] = n + 1;
        triangulos[tIdx++] = c + 1;

        triangulos[tIdx++] = c;
        triangulos[tIdx++] = n;
        triangulos[tIdx++] = n + 1;
    }

    mesh.vertices = vertices;
    mesh.uv = uvs;
    mesh.triangles = triangulos;
    mesh.RecalculateNormals();
    mesh.RecalculateBounds();

    return mesh;
}
    private void GenerarParedEntradaFrontal(Transform padre)
    {
        float zFrontal = -largoNaveCentral / 2f;
        float anchoMuro = (anchoNaveCentral - anchoPuertaDoble) / 2f;

        if (anchoMuro > 0)
        {
            float posXIzq = -anchoNaveCentral / 2f + anchoMuro / 2f;
            CrearCubo("Pared_Frontal_Izq", new Vector3(posXIzq, altoPared / 2f, zFrontal), new Vector3(anchoMuro, altoPared, grosorPared), padre, materialParedes);

            float posXDer = anchoNaveCentral / 2f - anchoMuro / 2f;
            CrearCubo("Pared_Frontal_Der", new Vector3(posXDer, altoPared / 2f, zFrontal), new Vector3(anchoMuro, altoPared, grosorPared), padre, materialParedes);
        }
    }

    private void GenerarSalasYVentanasLaterales(Transform padre)
    {
        float largoPaso = largoNaveCentral / salasPorLado;

        for (int lado = -1; lado <= 1; lado += 2)
        {
            float posXExt = (anchoNaveCentral / 2f + profundidadSalas) * lado;

            for (int i = 0; i < salasPorLado; i++)
            {
                float zCentro = -largoNaveCentral / 2f + largoPaso * (i + 0.5f);
                float largoSala = largoPaso - grosorPared;

                CrearCubo($"Piso_Sala_{lado}_{i}", new Vector3((anchoNaveCentral / 2f + profundidadSalas / 2f) * lado, -grosorPared / 2f, zCentro), new Vector3(profundidadSalas, grosorPared, largoSala), padre, materialPiso);

                CrearCubo($"Muro_Divisorio_{lado}_{i}", new Vector3((anchoNaveCentral / 2f + profundidadSalas / 2f) * lado, altoPared / 2f, zCentro + largoSala / 2f), new Vector3(profundidadSalas, altoPared, grosorPared), padre, materialParedes);

                // Techo plano para las salas laterales
                CrearCubo($"Techo_Sala_{lado}_{i}", new Vector3((anchoNaveCentral / 2f + profundidadSalas / 2f) * lado, altoPared + grosorPared / 2f, zCentro), new Vector3(profundidadSalas, grosorPared, largoSala), padre, materialTecho);

                bool esVentanaLateral = (i == 2);

                if (esVentanaLateral)
                {
                    float altoVentana = altoPared * 0.6f;
                    float altoAntepecho = 1f;

                    CrearCubo($"Antepecho_Ventana_{lado}", new Vector3(posXExt, altoAntepecho / 2f, zCentro), new Vector3(grosorPared, altoAntepecho, largoSala), padre, materialParedes);
                    CrearCubo($"Dintel_Ventana_{lado}", new Vector3(posXExt, altoPared - (altoPared - altoVentana - altoAntepecho) / 2f, zCentro), new Vector3(grosorPared, altoPared - altoVentana - altoAntepecho, largoSala), padre, materialParedes);
                }
                else
                {
                    CrearCubo($"Pared_Exterior_{lado}_{i}", new Vector3(posXExt, altoPared / 2f, zCentro), new Vector3(grosorPared, altoPared, largoSala), padre, materialParedes);
                }
            }
        }
    }

    private void GenerarAbsideInfantilCurvo(Transform padre)
    {
        float zInicioAbside = largoNaveCentral / 2f;
        float pasoAngulo = Mathf.PI / segmentosCurvaAbside;

        GameObject absideGroup = new GameObject("Abside_Infantil");
        absideGroup.transform.SetParent(padre);

        for (int i = 0; i < segmentosCurvaAbside; i++)
        {
            float angulo1 = i * pasoAngulo;
            float angulo2 = (i + 1) * pasoAngulo;

            float x1 = -Mathf.Cos(angulo1) * radioAbside;
            float z1 = zInicioAbside + Mathf.Sin(angulo1) * radioAbside;
            float x2 = -Mathf.Cos(angulo2) * radioAbside;
            float z2 = zInicioAbside + Mathf.Sin(angulo2) * radioAbside;

            Vector3 posMedia = new Vector3((x1 + x2) / 2f, altoPared / 2f, (z1 + z2) / 2f);
            float anchoSegmento = Vector3.Distance(new Vector3(x1, 0, z1), new Vector3(x2, 0, z2));
            float anguloGrados = Mathf.Atan2(x2 - x1, z2 - z1) * Mathf.Rad2Deg;

            bool esHuecoVentana = (i >= segmentosCurvaAbside / 4 && i < (segmentosCurvaAbside * 3) / 4);

            if (!esHuecoVentana)
            {
                GameObject segMuro = GameObject.CreatePrimitive(PrimitiveType.Cube);
                segMuro.name = $"Muro_Curvo_{i}";
                segMuro.transform.SetParent(absideGroup.transform);
                segMuro.transform.position = posMedia;
                segMuro.transform.rotation = Quaternion.Euler(0, anguloGrados, 0);
                segMuro.transform.localScale = new Vector3(grosorPared, altoPared, anchoSegmento + 0.05f);
                if (materialParedes != null) segMuro.GetComponent<Renderer>().sharedMaterial = materialParedes;
            }
            else
            {
                GameObject segAntepecho = GameObject.CreatePrimitive(PrimitiveType.Cube);
                segAntepecho.name = $"Antepecho_Curvo_{i}";
                segAntepecho.transform.SetParent(absideGroup.transform);
                segAntepecho.transform.position = new Vector3(posMedia.x, 0.5f, posMedia.z);
                segAntepecho.transform.rotation = Quaternion.Euler(0, anguloGrados, 0);
                segAntepecho.transform.localScale = new Vector3(grosorPared, 1f, anchoSegmento + 0.05f);
                if (materialParedes != null) segAntepecho.GetComponent<Renderer>().sharedMaterial = materialParedes;
            }
        }
    }

    private void GenerarGuiasCentrales(Transform padre)
    {
        GameObject recepcion = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        recepcion.name = "Zona_Recepcion_Guia";
        recepcion.transform.SetParent(padre);
        recepcion.transform.localPosition = new Vector3(0, 0.5f, -largoNaveCentral / 3f);
        recepcion.transform.localScale = new Vector3(4f, 0.5f, 2f);

        GameObject ovalo = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        ovalo.name = "Ovalo_Central_Guia";
        ovalo.transform.SetParent(padre);
        ovalo.transform.localPosition = new Vector3(0, 0.05f, 0);
        ovalo.transform.localScale = new Vector3(6f, 0.05f, 4f);

        GameObject circuloInfantil = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        circuloInfantil.name = "Area_Infantil_Guia";
        circuloInfantil.transform.SetParent(padre);
        circuloInfantil.transform.localPosition = new Vector3(0, 0.05f, largoNaveCentral / 3f);
        circuloInfantil.transform.localScale = new Vector3(4f, 0.05f, 4f);
    }

    private void CrearCubo(string nombre, Vector3 pos, Vector3 escala, Transform padre, Material mat)
    {
        GameObject cubo = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cubo.name = nombre;
        cubo.transform.SetParent(padre);
        cubo.transform.localPosition = pos;
        cubo.transform.localScale = escala;
        if (mat != null) cubo.GetComponent<Renderer>().sharedMaterial = mat;
    }

    private Material ObtenerMaterialPorDefecto()
    {
        return UnityEngine.Rendering.GraphicsSettings.defaultRenderPipeline != null 
            ? UnityEngine.Rendering.GraphicsSettings.defaultRenderPipeline.defaultMaterial 
            : new Material(Shader.Find("Standard"));
    }
}