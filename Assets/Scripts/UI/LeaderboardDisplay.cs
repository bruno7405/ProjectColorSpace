using TMPro;
using UnityEngine;
using TaloGameServices;
using System.Collections.Generic;

public class LeaderboardDisplay : MonoBehaviour
{
    public LeaderboardRow EntryPrefab;
    public Transform LeftContainer;
    public Transform RightContainer;

    public TMP_Text StatusText;
    private const int Count = 10;
    List<GameObject> _rows = new();
    
    
    void OnEnable()
    {
        Refresh();
    }

    public async void Refresh()
    {
        Clear();
        if (StatusText) StatusText.text = "Loading...";

        try
        {
            var top = new List<TaloGameServices.LeaderboardEntry>();
            TaloGameServices.LeaderboardEntry[] mine = null;

            if (Talo.HasIdentity())
            {
                var res = await Talo.Leaderboards.GetTopEntries(LeaderboardManager.BoardName, Count);
                top.AddRange(res.topEntries);
                mine = res.playerEntries;
            }
            else
            {
                var res = await Talo.Leaderboards.GetEntries(
                    LeaderboardManager.BoardName,
                    new GetEntriesOptions() { page = 0 });

                for (int i = 0; i < res.entries.Length && i < Count; i++)
                    top.Add(res.entries[i]);
            }

            Clear();

            int index = 0;
            foreach (var entry in top)
            {
                var parent = index < 5 ? LeftContainer : RightContainer;
                var row = Instantiate(EntryPrefab, parent);
                row.Name.text = $"{entry.position + 1}. {entry.playerAlias.displayName}";
                row.Time.text = FormatTime(entry.score);
                _rows.Add(row.gameObject);
                index++;
            }

            if (StatusText) StatusText.text = top.Count == 0 ? "No runs found." : "";
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
            if (StatusText) StatusText.text = "Error loading leaderboard.";
        }
    }

    void Clear()
    {
        foreach (var r in _rows) Destroy(r.gameObject);
        _rows.Clear();
    }

    static string FormatTime(float seconds)
    {
        var t = System.TimeSpan.FromSeconds(seconds);
        return $"{(int)t.TotalMinutes:00}:{t.Seconds:00}.{t.Milliseconds:000}";
    }
}
