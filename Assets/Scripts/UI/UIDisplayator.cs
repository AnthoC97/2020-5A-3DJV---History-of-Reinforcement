using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDisplayator : MonoBehaviour
{
    private static UIDisplayator instance = null;

    public GameObject ButtonCategoryPrefab = null;
    public Transform ButtonCategoryContent = null;

    public GameObject ColumnFieldPrefab = null;
    public Transform RowPanelColumn = null;
    public Transform RowPanel1 = null;

    List<float[]> VFs;

    void Awake()
    {
        instance = this;
        VFs = new List<float[]>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void AddVF(string name, ref float[] V)
    {
        instance.VFs.Add(V);
        GameObject buttonGO = GameObject.Instantiate(instance.ButtonCategoryPrefab, instance.ButtonCategoryContent);
        UnityEngine.UI.Button button = buttonGO.GetComponent<UnityEngine.UI.Button>();
        int i = instance.VFs.Count - 1;
        button.onClick.AddListener(()=>{
            Show(i);
        });
        buttonGO.GetComponentInChildren<UnityEngine.UI.Text>().text = name;
    }

    public static void Show(int index)
    {
        foreach (Transform tr in instance.RowPanelColumn)
            Destroy(tr.gameObject);
        foreach (Transform tr in instance.RowPanel1)
            Destroy(tr.gameObject);

        for (int i=0; i<instance.VFs[index].Length; ++i)
        {
            GameObject columnFieldGO = GameObject.Instantiate(instance.ColumnFieldPrefab, instance.RowPanelColumn);
            columnFieldGO.GetComponentInChildren<UnityEngine.UI.Text>().text = i.ToString();

            GameObject fieldGO = GameObject.Instantiate(instance.ColumnFieldPrefab, instance.RowPanel1);
            fieldGO.GetComponentInChildren<UnityEngine.UI.Text>().text = instance.VFs[index][i].ToString();
        }

    }
}
