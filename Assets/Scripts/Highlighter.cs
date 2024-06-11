using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Highlighter : MonoBehaviour
{
    [SerializeField] private Board board;
    [SerializeField] private Material highlightMaterial;
    [SerializeField] private GameObject highlightPrefab;
    private List<GameObject> currentHighlightsList = new List<GameObject>();

    public void Highlight(List<Vector2Int> positions2d){
        // Destroys current highlights
        foreach (var x in currentHighlightsList)
            Destroy(x.gameObject);
        currentHighlightsList.Clear();

        // If null, then don't highlight anything.
        if (positions2d == null) return;

        // Creates new highlights from data
        foreach (var position2d in positions2d){
            Vector3 position3D = board.GetRealCoordsFromGamePos(position2d);
            position3D = position3D - new Vector3(0, 0.2f, 0);
            GameObject highlight = Instantiate(highlightPrefab, position3D, Quaternion.identity);
            currentHighlightsList.Add(highlight);
            MeshRenderer mesh = highlight.GetComponentInChildren<MeshRenderer>();
            List<Material> matList = new List<Material>();
            matList.Add(highlightMaterial);
            mesh.SetMaterials(matList);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
