using System.Diagnostics;
using TMPro;
using UnityEngine;

public class SpeedrunTimer : MonoBehaviour
{
    public TMP_Text TimerText;
    public static SpeedrunTimer Instance {get; private set;}

    Stopwatch _timer = new Stopwatch();

    bool _running;
    bool _finished;

    public bool FinishedRun => _finished;

    public double RTA => _timer.Elapsed.TotalSeconds;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        gameObject.SetActive(false);
    }

    public void StartRun()
    {
        gameObject.SetActive(true);
        
        _timer.Restart();
        _running = true;
        _finished = false;
    }

    public void FinishRun()
    {
        if (!_running || _finished) return;
        _finished = true;
        _timer.Stop();
    }

    void Update()
    {
        if (TimerText == null || !_running) return;
        TimerText.text = Format(RTA);
    }

    public string Format(double seconds)
    {
        var t = System.TimeSpan.FromSeconds(seconds);
        return $"{(int)t.TotalMinutes:00}:{t.Seconds:00}.{t.Milliseconds:000}";
    }
}