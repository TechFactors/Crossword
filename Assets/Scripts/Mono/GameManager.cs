using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FMMultipleChoice
{

    [System.Serializable]
    public class GameManager : MonoBehaviour
    {

        #region Variables

        private Data data = new Data();

        [SerializeField] GameEvents events = null;

        [SerializeField] Animator timerAnimtor = null;
        [SerializeField] TextMeshProUGUI timerText = null;
        [SerializeField] Color timerHalfWayOutColor = Color.yellow;
        [SerializeField] Color timerAlmostOutColor = Color.red;
        private Color timerDefaultColor = Color.white;

        private List<AnswerData> PickedAnswers = new List<AnswerData>();
        private List<int> FinishedQuestions = new List<int>();

        private int timerStateParaHash = 0;

        private IEnumerator IE_WaitTillNextRound = null;
        private IEnumerator IE_StartTimer = null;
        private bool IsFinished
        {
            get
            {
                return (events.currentQuestion == data.Questions.Length) ? true : false;
            }
        }
        private bool IsOnSubmit
        {
            get
            {
                return (events.currentQuestion + 1 == data.Questions.Length) ? true : false;
            }
        }
        #endregion

        #region Default Unity methods

        /// <summary>
        /// Function that is called when the object becomes enabled and active
        /// </summary>
        private void OnEnable()
        {
            events.UpdateQuestionAnswer += UpdateAnswers;
        }
        /// <summary>
        /// Function that is called when the behaviour becomes disabled
        /// </summary>
        private void OnDisable()
        {
            events.UpdateQuestionAnswer -= UpdateAnswers;
        }

        /// <summary>
        /// Function that is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            //If current level is a first level, reset the final score back to zero.
            if (events.level == 1) { events.CurrentFinalScore = 0; }
        }

        /// <summary>
        /// Function that is called when the script instance is being loaded.
        /// </summary>
        private void Start()
        {
            events.currentQuestion = 0;
            events.CurrentFinalScore = 0;
            events.IsOnReview = false;
            events.answeredQuestions = new List<answeredQuestion>();
            events.StartupHighscore = PlayerPrefs.GetInt(GameUtility.SavePrefKey);
            timerDefaultColor = timerText.color;
            LoadData();

            timerStateParaHash = Animator.StringToHash("TimerState");

            var seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            UnityEngine.Random.InitState(seed);

            Display();
        }
        #endregion

        /// <summary>
        /// Function that is called to update new selected answer.
        /// </summary>

        void retrieveAnswer()
        {
            List<int> previousAnswer = events.answeredQuestions[events.currentQuestion].answers;

            foreach (var answer in PickedAnswers)
            {
                answer.Reset();
            }
        }
        public void UpdateAnswers(AnswerData newAnswer)
        {

            if (data.Questions[events.currentQuestion].Type == AnswerType.Single)
            {
                foreach (var answer in PickedAnswers)
                {
                    if (answer != newAnswer)
                    {
                        answer.Reset();
                    }
                }
                PickedAnswers.Clear();
                PickedAnswers.Add(newAnswer);
            }
            else
            {
                bool alreadyPicked = PickedAnswers.Exists(x => x == newAnswer);
                if (alreadyPicked)
                {
                    PickedAnswers.Remove(newAnswer);
                }
                else
                {
                    PickedAnswers.Add(newAnswer);
                }
            }
        }

        /// <summary>
        /// Function that is called to clear PickedAnswers list.
        /// </summary>
        public void EraseAnswers()
        {
            PickedAnswers = new List<AnswerData>();
        }
        /// <summary>
        /// Function that is called to display new question.
        /// </summary>
        void Display()
        {

            var question = data.Questions[0];

            if (events.UpdateQuestionUI != null)
            {
                events.UpdateQuestionUI(question);
            }
            else { Debug.LogWarning("Ups! Something went wrong while trying to display new Question UI Data. GameEvents.UpdateQuestionUI is null. Issue occured in GameManager.Display() method."); }

        }
        public void DisplayNext()
        {
            if (!events.IsOnReview)
            {
                selectedAnswers();
                EraseAnswers();
                displaySubmitButton();
            }
            events.currentQuestion += 1;

            if (IsFinished)
            {
                getResult();
                if (events.IsOnReview)
                {
                    events.IsOnReview = false;
                }
            }
            else
            {
                displaySubmitButton();
                var question = data.Questions[events.currentQuestion];

                if (events.UpdateQuestionUI != null)
                {
                    events.UpdateQuestionUI(question);
                }
                else { Debug.LogWarning("Ups! Something went wrong while trying to display new Question UI Data. GameEvents.UpdateQuestionUI is null. Issue occured in GameManager.Display() method."); }
            }
        }
        public void SubmitAnswer()
        {
            if (IsFinished)
            {
                getResult();
                if (events.IsOnReview)
                {
                    events.IsOnReview = false;
                }
            }
        }
        void displaySubmitButton()
        {
            if (IsOnSubmit)
            {
                events.DisplayResolutionScreen?.Invoke(UIManager.ResolutionScreenType.onSubmit, 0);
            }
        }
        public void previousQuestion()
        {
            if (!events.IsOnReview)
            {
                selectedAnswers();
                EraseAnswers();
                displaySubmitButton();
            }

            if (events.currentQuestion > 0) events.currentQuestion -= 1;

            var question = data.Questions[events.currentQuestion];

            if (events.UpdateQuestionUI != null)
            {
                events.UpdateQuestionUI(question);
            }
            else { Debug.LogWarning("Ups! Something went wrong while trying to display new Question UI Data. GameEvents.UpdateQuestionUI is null. Issue occured in GameManager.Display() method."); }

        }
        /// <summary>
        /// Function that is called to accept picked answers and check/display the result.
        /// </summary>

        void getResult()
        {
            if (!events.IsOnReview)
            {
                for (int i = 0; i < data.Questions.Length; i++)
                {
                    addCorrectAnswer(i);

                    bool isCorrect = CheckAnswers(i);

                    events.answeredQuestions[i].isCorrect = isCorrect;
                    UpdateScore(isCorrect ? 0 : 1);
                }
                events.level++;
                if (events.level > GameEvents.maxLevel)
                {
                    events.level = 1;
                }
                SetHighscore();
                events.CurrentFinalScore = data.Questions.Length - events.CurrentFinalScore;

                print(data.Questions.Length + "cFS" + events.CurrentFinalScore);
            }
            events.DisplayResolutionScreen?.Invoke(UIManager.ResolutionScreenType.Finish, 0);
        }
        void addCorrectAnswer(int i)
        {
            List<int> c = data.Questions[i].GetCorrectAnswers();
            events.answeredQuestions[i].correctAnswers = c;
        }
        public void reviewAnswer()
        {
            events.IsOnReview = true;
            events.DisplayResolutionScreen?.Invoke(UIManager.ResolutionScreenType.onReview, 0);
            previousQuestion();
        }

        #region Timer Methods

        IEnumerator WaitTillNextRound()
        {
            yield return new WaitForSeconds(GameUtility.ResolutionDelayTime);
            DisplayNext();
        }

        #endregion
        bool isAnswered()
        {
            bool isAnswered = false;
            for (int i = 0; i < events.answeredQuestions.Count; i++)
            {
                if (events.answeredQuestions[i].questionIndex == events.currentQuestion) isAnswered = true;
            }
            return isAnswered;
        }
        void selectedAnswers()
        {
            if (!isAnswered())
            {
                answeredQuestion answers = new answeredQuestion();

                if (PickedAnswers.Count > 0)
                {
                    List<int> p = PickedAnswers.Select(x => x.AnswerIndex).ToList();
                    answers.answers = p;
                    answers.noAnswer = false;
                }
                else
                {
                    answers.noAnswer = true;
                }
                answers.questionIndex = events.currentQuestion;
                events.answeredQuestions.Add(answers);
            }
            else if (isAnswered())
            {
                if (PickedAnswers.Count > 0)
                {
                    List<int> p = PickedAnswers.Select(x => x.AnswerIndex).ToList();
                    events.answeredQuestions[events.currentQuestion].answers = p;
                    events.answeredQuestions[events.currentQuestion].noAnswer = false;
                }
                else
                {
                    events.answeredQuestions[events.currentQuestion].noAnswer = true;
                }
            }

        }
        /// <summary>
        /// Function that is called to check currently picked answers and return the result.
        /// </summary>
        bool CheckAnswers(int i)
        {
            if (!CompareAnswers(i))
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// Function that is called to compare picked answers with question correct answers.
        /// </summary>
        bool CompareAnswers(int i)
        {
            if (events.answeredQuestions[i].answers != null && events.answeredQuestions[i].answers.Count > 0)
            {
                List<int> c = data.Questions[i].GetCorrectAnswers();
                List<int> p = events.answeredQuestions[i].answers;

                var f = c.Except(p).ToList();
                var s = p.Except(c).ToList();

                return !f.Any() && !s.Any();
            }
            return false;
        }
        void answeredQuestions(List<int> s)
        {
            answeredQuestion wrongAnswer = new answeredQuestion();
            wrongAnswer.answers = s;
            wrongAnswer.questionIndex = events.currentQuestion;

            events.answeredQuestions.Add(wrongAnswer);

        }
        void noAnswer()
        {
            answeredQuestion wrongAnswer = new answeredQuestion();
            wrongAnswer.noAnswer = true;
            wrongAnswer.questionIndex = events.currentQuestion;

            events.answeredQuestions.Add(wrongAnswer);
        }
        /// <summary>
        /// Function that is called to load data from the xml file.
        /// </summary>
        void LoadData()
        {
            var path = Path.Combine(GameUtility.FileDir, GameUtility.FileName + events.level + ".xml");
            data = Data.Fetch(path);
        }

        /// <summary>
        /// Function that is called restart the game.
        /// </summary>
        public void RestartGame()
        {
            //If next level is the first level, meaning that we start playing a game again, reset the final score.
            if (events.level == 1) { events.CurrentFinalScore = 0; }

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        /// <summary>
        /// Function that is called to quit the application.
        /// </summary>
        public void QuitGame()
        {
            //On quit reset the current level back to the first level.
            events.level = 1;

            Application.Quit();
        }

        /// <summary>
        /// Function that is called to set new highscore if game score is higher.
        /// </summary>
        private void SetHighscore()
        {
            var highscore = PlayerPrefs.GetInt(GameUtility.SavePrefKey);
            if (highscore < events.CurrentFinalScore)
            {
                PlayerPrefs.SetInt(GameUtility.SavePrefKey, events.CurrentFinalScore);
            }
        }
        /// <summary>
        /// Function that is called update the score and update the UI.
        /// </summary>
        private void UpdateScore(int add)
        {
            events.CurrentFinalScore += add;
            events.ScoreUpdated?.Invoke();
        }

        #region Getters

        Question GetRandomQuestion()
        {
            var randomIndex = GetRandomQuestionIndex();
            events.currentQuestion = randomIndex;

            return data.Questions[events.currentQuestion];
        }
        int GetRandomQuestionIndex()
        {
            var random = 0;
            if (FinishedQuestions.Count < data.Questions.Length)
            {
                do
                {
                    random = Random.Range(0, data.Questions.Length);
                } while (FinishedQuestions.Contains(random) || random == events.currentQuestion);
            }
            return random;
        }

        #endregion
    }
}