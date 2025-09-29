// 放到有 Light 组件的物体上（URP/内置皆可）
using UnityEngine;

[RequireComponent(typeof(Light))]
public class GlitchyLight : MonoBehaviour
{
    private float baseIntensity;
    public float noiseAmplitude = 1f;     // 平时轻微抖动幅度
    public float noiseSpeed = 4f;           // 抖动速度
    public float spikeIntensity = 6f;       // 故障尖峰亮度
    public Vector2 spikeInterval = new Vector2(1.5f, 4f); // 尖峰出现间隔
    public float spikeDuration = 0.06f;     // 尖峰持续时间

    Light _l; float _t; float _spikeEndTime; float _nextSpikeTime;

    void Awake() 
    { 
        _l = GetComponent<Light>(); 
        baseIntensity = _l.intensity;
        ScheduleNextSpike(); 
    }

    void ScheduleNextSpike()
    {
        _nextSpikeTime = Time.time + Random.Range(spikeInterval.x, spikeInterval.y);
    }

    void Update()
    {
        _t += Time.deltaTime * noiseSpeed;
        // 基于 Perlin 的柔和抖动
        float n = Mathf.PerlinNoise(_t, 0f) * 2f - 1f; // -1~1
        float flicker = baseIntensity + n * noiseAmplitude;

        // 偶发尖峰/熄灭（也可把 spikeIntensity 设为 0 做“瞬灭”）
        if (Time.time >= _nextSpikeTime)
        {
            _spikeEndTime = Time.time + spikeDuration;
            ScheduleNextSpike();
        }
        if (Time.time < _spikeEndTime)
            _l.intensity = Mathf.Max(0f, spikeIntensity);
        else
            _l.intensity = Mathf.Max(0f, flicker);
    }
}

