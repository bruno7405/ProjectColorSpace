using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TaloGameServices;

public class LeaderboardManager : MonoBehaviour
{
    public const string BoardName = "prismboard";
    public GameObject RunSubmitUI;

    public Button Submit, Cancel;
    public TMP_InputField NameInput;
    public TMP_Text StatusText;

    double _time;
    bool _busy;

    public LeaderboardDisplay LeaderDisplay;

    public void Awake()
    {
        RunSubmitUI.SetActive(false);
    }
    
    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void AllowRunSubmission()
    {
        var timer = SpeedrunTimer.Instance;
        if (timer == null) return;

        _time = timer.RTA;
        
        if (StatusText) StatusText.text = $"Time: {timer.Format(_time)}";

        NameInput.text = "";
        RunSubmitUI.SetActive(true);

        Submit.onClick.AddListener(SubmitRun);
        Cancel.onClick.AddListener(CancelSubmit);
    }

    async void SubmitRun()
    {
        if (_busy) return;

        string username = NameInput.text.Trim();
        if (string.IsNullOrEmpty(username))
        {
            if (StatusText) StatusText.text = "Enter a name first.";
            return;
        }

        _busy = true;
        Submit.interactable = false;
        if (StatusText) StatusText.text = "Submitting...";

        try
        {
            await Talo.Players.Identify("username", username);

           var result = await Talo.Leaderboards.AddEntry(
                BoardName,
                (float)_time,
                ("version", Application.version)
            );

            if (!result.Success)
            {
                if (StatusText) StatusText.text = "Submission was rejected, try again.";
                return;
            }

            RunSubmitUI.SetActive(false);
            Reset();
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
            if (StatusText) StatusText.text = "Submission error, try again.";
        }
        finally
        {
            _busy = false;
            Submit.interactable = true;
            LeaderDisplay.Refresh();
        }
    }

    public void CancelSubmit()
    {
        RunSubmitUI.SetActive(false);
        Reset();
    }

    void Reset()
    {
        Submit.onClick.RemoveListener(SubmitRun);
        Cancel.onClick.RemoveListener(CancelSubmit);
    }
}
