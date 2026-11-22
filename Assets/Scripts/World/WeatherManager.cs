using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // 新Input System用

// 天候ごとの設定をまとめるクラス
[Serializable]
public class WeatherPreset
{
    public string weatherName = "Sunny"; // 天候名

    [Header("太陽光設定")]
    public Color lightColor = Color.white; // 太陽光の色
    public float lightIntensity = 1.0f;  // 太陽光の強さ

    [Header("フォグ設定")]
    public bool enableFog = false; // フォグを有効化するか
    public Color fogColor = Color.gray; // フォグの色
    public float fogDensity = 0.01f;

    [Header("パーティクル設定")]
    public ParticleSystem weatherParticle; // 該当するパーティクル（なければNone）
    public Vector3 particleOffset = Vector3.zero; // ターゲットからの相対位置オフセット（例: 雨なら上空に y = 15）
}

public class WeatherManager : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private Light directionalLight;

    [Header("パーティクル追従設定")]
    [Tooltip("パーティクルが追従するターゲット（未設定の場合はこのオブジェクト自身）")]
    [SerializeField] private Transform followTarget;

    [Header("遷移設定")]
    [Tooltip("天候が切り替わるまでの時間（秒）")]
    [SerializeField] private float transitionDuration = 2.0f;

    [Header("天候一覧")]
    [SerializeField] private List<WeatherPreset> weatherPresets = new List<WeatherPreset>();

    [Header("InputSystem")]
    [Tooltip("天候を切り替えるキーやボタンをバインドしてください")]
    [SerializeField] private InputAction changeWeatherAction;

    private int currentWeatherIndex = 0;
    private Coroutine transitionCoroutine;

    private void OnEnable()
    {
        changeWeatherAction.Enable();
        changeWeatherAction.performed += OnChangeWeatherPerformed;
    }

    private void OnDisable()
    {
        changeWeatherAction.performed -= OnChangeWeatherPerformed;
        changeWeatherAction.Disable();
    }

    private void Start()
    {
        if (weatherPresets.Count > 0)
        {
            ApplyWeatherImmediate(0);
        }
    }

    private void LateUpdate()
    {
        FollowTargetWithParticles();
    }

    private void FollowTargetWithParticles()
    {
        Vector3 basePosition = followTarget != null ? followTarget.position : transform.position;

        foreach (var preset in weatherPresets)
        {
            if (preset.weatherParticle == null) continue;

            // 再生中のパーティクル、または余韻が残っているパーティクルをターゲットに追従させる
            if (preset.weatherParticle.isPlaying || preset.weatherParticle.particleCount > 0)
            {
                preset.weatherParticle.transform.position = basePosition + preset.particleOffset;
            }
        }
    }

    private void OnChangeWeatherPerformed(InputAction.CallbackContext context)
    {
        NextWeather();
    }

    public void NextWeather()
    {
        if (weatherPresets.Count == 0) return;

        currentWeatherIndex = (currentWeatherIndex + 1) % weatherPresets.Count;
        StartWeatherTransition(currentWeatherIndex);
    }

    public void SetWeatherByName(string name)
    {
        int index = weatherPresets.FindIndex(w => w.weatherName == name);
        if (index != -1)
        {
            currentWeatherIndex = index;
            StartWeatherTransition(currentWeatherIndex);
        }
        else
        {
            Debug.LogWarning($"天候 '{name}' がリストに見つかりません。");
        }
    }

    private void StartWeatherTransition(int targetIndex)
    {
        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }
        transitionCoroutine = StartCoroutine(TransitionWeatherRoutine(weatherPresets[targetIndex]));
    }

    private void ApplyWeatherImmediate(int index)
    {
        WeatherPreset preset = weatherPresets[index];

        if (directionalLight != null)
        {
            directionalLight.color = preset.lightColor;
            directionalLight.intensity = preset.lightIntensity;
        }

        RenderSettings.fog = preset.enableFog;
        if (preset.enableFog)
        {
            RenderSettings.fogColor = preset.fogColor;
            RenderSettings.fogDensity = preset.fogDensity;
        }
        else
        {
            RenderSettings.fogDensity = 0f;
        }

        UpdateParticles(preset);
        FollowTargetWithParticles();
        Debug.Log($"天候を即時反映しました: {preset.weatherName}");
    }

    private IEnumerator TransitionWeatherRoutine(WeatherPreset target)
    {
        UpdateParticles(target);

        Color startLightColor = directionalLight != null ? directionalLight.color : Color.white;
        float startLightIntensity = directionalLight != null ? directionalLight.intensity : 1.0f;

        bool wasFogEnabled = RenderSettings.fog;
        Color startFogColor = RenderSettings.fogColor;
        float startFogDensity = wasFogEnabled ? RenderSettings.fogDensity : 0f;

        float targetFogDensity = target.enableFog ? target.fogDensity : 0f;
        Color targetFogColor = target.enableFog ? target.fogColor : startFogColor;

        if (target.enableFog)
        {
            RenderSettings.fog = true;
        }

        float elapsed = 0f;
        float duration = Mathf.Max(0.01f, transitionDuration);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            if (directionalLight != null)
            {
                directionalLight.color = Color.Lerp(startLightColor, target.lightColor, smoothT);
                directionalLight.intensity = Mathf.Lerp(startLightIntensity, target.lightIntensity, smoothT);
            }

            if (RenderSettings.fog)
            {
                RenderSettings.fogColor = Color.Lerp(startFogColor, targetFogColor, smoothT);
                RenderSettings.fogDensity = Mathf.Lerp(startFogDensity, targetFogDensity, smoothT);
            }

            yield return null;
        }

        if (directionalLight != null)
        {
            directionalLight.color = target.lightColor;
            directionalLight.intensity = target.lightIntensity;
        }

        if (target.enableFog)
        {
            RenderSettings.fog = true;
            RenderSettings.fogColor = target.fogColor;
            RenderSettings.fogDensity = target.fogDensity;
        }
        else
        {
            RenderSettings.fogDensity = 0f;
            RenderSettings.fog = false;
        }

        transitionCoroutine = null;
        Debug.Log($"天候を変更しました: {target.weatherName}");
    }

    private void UpdateParticles(WeatherPreset target)
    {
        foreach (var p in weatherPresets)
        {
            if (p.weatherParticle != null && p.weatherParticle != target.weatherParticle)
            {
                p.weatherParticle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
        }

        if (target.weatherParticle != null && !target.weatherParticle.isPlaying)
        {
            target.weatherParticle.Play();
        }
    }
}