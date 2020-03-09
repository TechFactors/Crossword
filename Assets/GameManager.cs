using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace WordSearch
{
    public class GameManager : MonoBehaviour
    {
        public List<answeredQuestion> Question = new List<answeredQuestion>();

        public void CheckAnswer()
        {
            var Count = Question.Count;
            var Correct = 0;
            foreach (var question in Question)
            {
                for (int i = 0; i < question.letter.Count; i++)
                {
                    if (question.letter[i] == question.Answers[i].text)
                    {
                        Correct += 1;
                    }
                    if (Correct == Count) print(question.letter[i] + "Correct");
                }
            }
        }

    }
    [System.Serializable]
    public class answeredQuestion
    {
        public List<string> letter = new List<string>();
        public List<TMP_InputField> Answers = new List<TMP_InputField>();
    }
}