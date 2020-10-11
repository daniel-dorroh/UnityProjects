using UnityEngine;

public class HealthMeter : MonoBehaviour
{
    [SerializeField]
    private float Intensity = 0.01f;

    private int _health;
    private int _lastHealth;
    private int _maxHealth;
    private Renderer _renderer;

    private Color GreenBlue = new Color(0, 1, 1);

    // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponent<Renderer>();
        _health = GetComponentInParent<RocketMotion>().Health;
        _maxHealth = _health;
        _lastHealth = _health;
        GreenBlue *= _renderer.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        _health = GetComponentInParent<RocketMotion>().Health;
        if (_lastHealth != _health)
        {
            float damage = (_maxHealth - _health) / (_maxHealth * 1.0f);
            var indicator = new Material(_renderer.material);
            indicator.color += (Color.red * damage - GreenBlue * damage);
            _renderer.material = indicator;
            _lastHealth = _health;
        }
    }
}
