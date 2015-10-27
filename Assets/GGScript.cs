using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

public class GGScript : MonoBehaviour
{

    public Text ScoreGold;
    public Text ScoreSilver;
    public Text ScoreCopper;
    public Text ScoreWood;
    public Text Number1;
    public Text Number2;
    public InputField AnswerField;
    public Button OkButton;
    public Button PassButton;
    public Button ButtonGold;
    public Button ButtonSilver;
    public Button ButtonCopper;
    public Button ButtonWood;
    public SpriteRenderer HighlighterGold;
    public SpriteRenderer HighlighterSilver;
    public SpriteRenderer HighlighterCopper;
    public SpriteRenderer HighlighterWood;

    string score;
    string url = "YOURURL/api/values/";
    Question activeQuestion;
    string requestedQuestionType;
    SpriteRenderer toHighlight;
    DateTime QuestionReceivedAt;

    // Use this for initialization
    void Start()
    {
        score = PlayerPrefs.GetString("score");
        UpdateScoreBoard(score);
        OkButton.interactable = false;
        PassButton.interactable = false;
    }

    public void RequestQuestion(string id)
    {
        ToggleButtons();
        requestedQuestionType = id;
        StartCoroutine(GetQuestion(url + id));
    }

    void ToggleButtons()
    {
        if (ButtonGold.interactable)
        {
            ButtonGold.interactable = false;
            ButtonSilver.interactable = false;
            ButtonCopper.interactable = false;
            ButtonWood.interactable = false;
        }
        else
        {
            ButtonGold.interactable = true;
            ButtonSilver.interactable = true;
            ButtonCopper.interactable = true;
            ButtonWood.interactable = true;
        }
    }

    IEnumerator GetQuestion(string url)
    {
        Handheld.SetActivityIndicatorStyle(AndroidActivityIndicatorStyle.Large);
        Handheld.StartActivityIndicator();
        WWW www = new WWW(url);
        while (!www.isDone)
        {
            yield return null;
        }

        activeQuestion = JsonConvert.DeserializeObject<Question>(www.text);
        Number1.text = activeQuestion.Number1.ToString();
        Number2.text = activeQuestion.Number2.ToString();
        Handheld.StopActivityIndicator();
        QuestionReceivedAt = DateTime.Now;
        OkButton.interactable = true;
        PassButton.interactable = true;
    }

    public void CheckAnswer()
    {
        if (AnswerField.text == "")
        {
            return;
        }
        if (Convert.ToInt32(AnswerField.text) != activeQuestion.Result)
        {
            AnswerField.image.color = Color.red;
            Invoke("FixInputColor", 0.5f);
        }
        else
        {
            OkButton.interactable = false;
            PassButton.interactable = false;
            AnswerField.image.color = Color.white;
            activeQuestion.SolveDate = DateTime.Now;
            activeQuestion.SolveSeconds = Convert.ToInt32((DateTime.Now - QuestionReceivedAt).TotalSeconds);

            StartCoroutine(SendAnswer(url + activeQuestion.Id));

        }
    }

    void FixInputColor()
    {
        AnswerField.image.color = Color.white;
        AnswerField.text = "";
    }

    IEnumerator SendAnswer(string url)
    {
        Handheld.SetActivityIndicatorStyle(AndroidActivityIndicatorStyle.Large);
        Handheld.StartActivityIndicator();

        var encoding = new System.Text.UTF8Encoding();
        var header = new Dictionary<string, string>();
        header.Add("Content-Type", "application/json");
        var jsonString = JsonConvert.SerializeObject(activeQuestion);

        WWW request = new WWW(url, encoding.GetBytes(jsonString), header);

        while (!request.isDone)
        {
            yield return null;
        }

        UpdateScoreBoard(request.text);
        HighlightScoreboard();
        AnswerField.image.color = Color.green;
        Invoke("FixInputColor", 0.5f);
        Number1.text = "00";
        Number2.text = "00";
        ToggleButtons();
        Handheld.StopActivityIndicator();
    }

    void UpdateScoreBoard(string score)
    {
        Score coin = JsonConvert.DeserializeObject<Score>(score);
        ScoreGold.text = coin.Gold.ToString();
        ScoreSilver.text = coin.Silver.ToString();
        ScoreCopper.text = coin.Copper.ToString();
        ScoreWood.text = coin.Wood.ToString();        
    }

    void HighlightScoreboard()
    {
        if (requestedQuestionType == "1")
        {
            toHighlight = HighlighterWood;
        }
        else if (requestedQuestionType == "2")
        {
            toHighlight = HighlighterCopper;
        }
        else if (requestedQuestionType == "3")
        {
            toHighlight = HighlighterSilver;
        }
        else if (requestedQuestionType == "4")
        {
            toHighlight = HighlighterGold;
        }
        toHighlight.enabled = true;
        Invoke("DisableHighlight", 1f);
    }

    void DisableHighlight()
    {
        toHighlight.enabled = false;
    }

    public void Pass()
    {
        Number1.text = "00";
        Number2.text = "00";
        activeQuestion = null;
        ToggleButtons();
    }
}

public class Score
{
    public int Id { get; set; }
    public int Gold { get; set; }
    public int Silver { get; set; }
    public int Copper { get; set; }
    public int Wood { get; set; }
}

public class Question
{
    public int Id { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime? SolveDate { get; set; }
    public int Number1 { get; set; }
    public int Number2 { get; set; }
    public int Result { get; set; }
    public int Max { get; set; }
    public int? SolveSeconds { get; set; }
    public bool IsChecked { get; set; }
}