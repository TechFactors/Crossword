using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
// Script inserted to prefab of Checkbox - 
public class AnswerData : MonoBehaviour {

    #region Variables

    [Header("UI Elements")]
    [SerializeField]    TextMeshProUGUI infoTextObject      = null;
    [SerializeField]    Image           toggle              = null;

    [Header("Textures")]
    [SerializeField]    Sprite          uncheckedToggle     = null;
    [SerializeField]    Sprite          checkedToggle       = null;

    [Header("References")]
    [SerializeField]    GameEvents      events              = null;

    private             RectTransform   _rect               = null;
    public              RectTransform   Rect
    {
        get
        {
            if (_rect == null)
            {
                _rect = GetComponent<RectTransform>() ?? gameObject.AddComponent<RectTransform>();
            }
            return _rect;
        }
    }

    private             int             _answerIndex        = -1;
    public              int             AnswerIndex         { get { return _answerIndex; } }

    private             bool            Checked             = false;

    #endregion

    void Start()
    {
        if (events.IsOnReview)
        {
            var answersData = events.answeredQuestions[events.currentQuestion];

            foreach (var correctAnswers in answersData.correctAnswers)
            {
                if (correctAnswers == this.AnswerIndex)
                {
                    this.GetComponent<Image>().color = new Color32(62, 175, 65, 255);
                }
            }
        }
        if (events.answeredQuestions.Count > events.currentQuestion)
        {
            var answersData = events.answeredQuestions[events.currentQuestion];
            if (!answersData.noAnswer) 
            { 
                foreach (var answers in answersData.answers)
                {
                    if (answers == this.AnswerIndex)
                    {
                        if (events.IsOnReview && answersData.isCorrect)
                        {
                            this.GetComponent<Image>().color = new Color32(62, 175, 65, 255);
                        }
                        else if(events.IsOnReview)
                        {
                            this.GetComponent<Image>().color = new Color32(244, 58, 39, 255);
                        }
                        Checked = !Checked;
                        UpdateUI();
                        if (events.UpdateQuestionAnswer != null)
                        {
                            events.UpdateQuestionAnswer(this);
                        }
                    }
                }
            }
        }
    }
    /// <summary>
    /// Function that is called to update the answer data. - Choices Data
    /// </summary>
    public void UpdateData (string info, int index)
    {
        infoTextObject.text = info;
        _answerIndex = index;
    }
    /// <summary>
    /// Function that is called to reset values back to default. - Uncheck all Toggle
    /// </summary>
    public void Reset ()
    {
        Checked = false;
        UpdateUI();
    }
    /// <summary>
    /// Function that is called to switch the state.
    /// </summary>
    public void SwitchState ()
    {
        Checked = !Checked;
        UpdateUI();
        if (events.UpdateQuestionAnswer != null)
        {
            events.UpdateQuestionAnswer(this);
        }
    }

    /// <summary>
    /// Function that is called to update UI.
    /// </summary>
    void UpdateUI ()
    {
        if (toggle == null) return;

        toggle.sprite = (Checked) ? checkedToggle : uncheckedToggle;


    }
}