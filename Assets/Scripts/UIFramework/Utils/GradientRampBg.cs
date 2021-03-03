using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(Image))]
public class GradientRampBg : MonoBehaviour
{
    [SerializeField]
    Texture _BgTexture;
    [SerializeField]
    Color _Color1;
    [SerializeField]
    Color _Color2;
    [SerializeField]
    float _Angle;

    private Image _BgImg;
    private Material _Material;

    public Texture BgTexture
    {
        get
        {
            return _BgTexture;
        }

        set
        {
            _BgTexture = value;
            _Material.SetTexture("_Texture", _BgTexture);
            UpdateMaterial();
        }
    }

    public Color Color1
    {
        get
        {
            return _Color1;
        }

        set
        {
            _Color1 = value;
            _Material.SetColor("_Color1", _Color1);
            UpdateMaterial();
        }
    }

    public Color Color2
    {
        get
        {
            return _Color2;
        }

        set
        {
            _Color2 = value;
            _Material.SetColor("_Color2", _Color2);
            UpdateMaterial();
        }
    }

    public float Angle
    {
        get
        {
            return _Angle;
        }

        set
        {
            _Angle = value;
            _Material.SetFloat("_Angle", _Angle);
            UpdateMaterial();
        }
    }

    private void Awake()
    {
        _Material = new Material(Shader.Find("Custom/GradientRamp"));
        _BgImg = this.GetComponent<Image>();
        _BgImg.material = _Material;

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
        _Material.SetColor("_Color1", _Color1);
        _Material.SetColor("_Color2", _Color2);
        _Material.SetFloat("_Angle", _Angle);
        _Material.SetTexture("_MainTex", _BgTexture);
    }
}
