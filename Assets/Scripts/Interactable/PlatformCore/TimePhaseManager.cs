using System.Collections.Generic;
using UnityEngine;

public class TimePhaseManager : MonoBehaviour
{
    public static TimePhaseManager Instance { get; private set; }

    [SerializeField] private int maxPhase = 3; // 0..maxPhase-1
    [SerializeField] public int CurrentPhase = 0;

    private readonly List<TemporalPlatform> platforms = new List<TemporalPlatform>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (maxPhase < 1) maxPhase = 1;
        CurrentPhase = Mathf.Clamp(CurrentPhase, 0, maxPhase - 1);
    }

    public void RegisterPlatform(TemporalPlatform platform)
    {
        if (!platforms.Contains(platform))
        {
            platforms.Add(platform);
            platform.ApplyPhase(CurrentPhase, GetNextPhase());
        }
    }

    public void UnregisterPlatform(TemporalPlatform platform)
    {
        platforms.Remove(platform);
    }

    public void NextPhase()
    {
        CurrentPhase = (CurrentPhase + 1) % maxPhase;
        Debug.Log($"[TimePhaseManager] Phase -> {CurrentPhase}");
        ApplyPhaseToAll();
    }

    public void SetPhase(int phase)
    {
        int clamped = Mathf.Clamp(phase, 0, maxPhase - 1);
        if (clamped == CurrentPhase) return;

        CurrentPhase = clamped;
        Debug.Log($"[TimePhaseManager] Phase set -> {CurrentPhase}");
        ApplyPhaseToAll();
    }

    private int GetNextPhase()
    {
        if (maxPhase <= 1) return CurrentPhase;
        return (CurrentPhase + 1) % maxPhase;
    }

    private void ApplyPhaseToAll()
    {
        int next = GetNextPhase();
        foreach (var p in platforms)
        {
            if (p != null)
                p.ApplyPhase(CurrentPhase, next);
        }
    }
}
