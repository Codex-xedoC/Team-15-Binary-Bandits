using UnityEngine;

public class SkyTimeChanger : MonoBehaviour
{
    [SerializeField] private Material skybox;
    private float _elapsedTime = 2f;
    private float _timeScale = 0.5f;

    private static readonly int Rotation = Shader.PropertyToID("_Rotation");
    private static readonly int Exposure = Shader.PropertyToID("_Exposure");

    void Update() {
        _elapsedTime += _timeScale * Time.deltaTime;
        skybox.SetFloat(Rotation, _elapsedTime * _timeScale);
        skybox.SetFloat(Exposure, Mathf.Clamp(Mathf.Sin(_elapsedTime), 0.4f, 1.5f));
    }
}
