using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
//[RequireComponent(typeof(Image)) || RequireComponent(typeof(RawImage))]
public class RoundedCornerRect : MonoBehaviour
{ 
    [SerializeField]
    float _RoundedRadius;

    private RawImage _BgRawImg;
    private Image _BgImg;
    private Material _Material;

    public float RoundedRadius
    {
        get
        {
            return _RoundedRadius;
        }

        set
        {
            _RoundedRadius = value;
            _Material.SetFloat("_RoundedRadius", _RoundedRadius);
            UpdateMaterial();
        }
    }

    private void Awake()
    {
        _Material = new Material(Shader.Find("Custom/RoundedRect"));
        _BgRawImg = this.GetComponent<RawImage>();
        if (_BgRawImg == null)
        {
            _BgImg = this.GetComponent<Image>();
            _BgImg.material = _Material;
        }
        else
        {
            _BgRawImg.material = _Material;
        }


        UpdateMaterial();
    }

    private void Update()
    {
#if UNITY_EDITOR
        UpdateMaterial();
#endif
    }

    public void UpdateMaterial()
    {
        _Material.SetFloat("_RoundedRadius", _RoundedRadius);
    }
}
