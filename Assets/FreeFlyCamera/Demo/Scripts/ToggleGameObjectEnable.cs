using UnityEngine;
using UnityEngine.Serialization;

public class ToggleGameObjectEnable : MonoBehaviour
{
    [FormerlySerializedAs("_go")] [SerializeField]
    private GameObject go;

    private bool _enabled = true;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            _enabled = !_enabled;
            go.SetActive(_enabled);
        }
    }
}
