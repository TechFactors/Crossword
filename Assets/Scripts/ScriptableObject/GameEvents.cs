using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameEvents", menuName = "Quiz/new GameEvents")]
public class GameEvents : ScriptableObject {

    public delegate void    UpdateQuestionUICallback            (Question question);
    public                  UpdateQuestionUICallback            UpdateQuestionUI                = null;

    public delegate void    UpdateQuestionAnswerCallback        (AnswerData pickedAnswer);
    public                  UpdateQuestionAnswerCallback        UpdateQuestionAnswer            = null;

    public delegate void    DisplayResolutionScreenCallback     (UIManager.ResolutionScreenType type, int score);
    public                  DisplayResolutionScreenCallback     DisplayResolutionScreen         = null;

    public delegate void    ScoreUpdatedCallback();
    public                  ScoreUpdatedCallback                ScoreUpdated                    = null;

    [HideInInspector]
    public                  int                                 level                           = 1;
    public const            int                                 maxLevel                        = 1;

    [HideInInspector]
    public                  int                                 CurrentFinalScore               = 0;
    [HideInInspector]
    public                  int                                 StartupHighscore                = 0;
    public                  List<answeredQuestion>              answeredQuestions               = new List<answeredQuestion>();
    public                  int                                 currentQuestion                 = 0;
    public                  bool                                IsOnReview                      = false;

}
[System.Serializable]
public class answeredQuestion
{
    public int questionIndex;
    public List<int> answers;
    public List<int> correctAnswers;
    public bool noAnswer = false;
    public bool isCorrect = false;
}