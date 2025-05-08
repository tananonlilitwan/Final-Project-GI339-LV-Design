using UnityEngine;

public class RandomLightController : MonoBehaviour
{
    [System.Serializable]
    public class LightToggleData
    {
        public Light light;              // ดวงไฟ
        public float minInterval = 1f;   // ช่วงเวลาต่ำสุด
        public float maxInterval = 5f;   // ช่วงเวลาสูงสุด

        [HideInInspector] public float timer;
        [HideInInspector] public float nextToggleTime;
    }

    public LightToggleData[] lights;

    void Start()
    {
        // ตั้งเวลาสุ่มเริ่มต้นสำหรับแต่ละดวง
        foreach (var data in lights)
        {
            data.nextToggleTime = Random.Range(data.minInterval, data.maxInterval);
            data.timer = 0f;
        }
    }

    void Update()
    {
        foreach (var data in lights)
        {
            data.timer += Time.deltaTime;

            if (data.timer >= data.nextToggleTime)
            {
                if (data.light != null)
                {
                    data.light.enabled = !data.light.enabled;
                }

                // ตั้งเวลาสุ่มใหม่
                data.nextToggleTime = Random.Range(data.minInterval, data.maxInterval);
                data.timer = 0f;
            }
        }
    }
}