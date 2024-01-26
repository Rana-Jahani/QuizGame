using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Quiz : MonoBehaviour
{
[Header("Questions")]
    [SerializeField] TextMeshProUGUI questionText;
    [SerializeField] List<QuestionSO> questions = new List<QuestionSO>();
    QuestionSO currentQuestion;

    [Header("Answers")]
    [SerializeField] GameObject[] answerButtons;
    int correctAnswerIndex;
    bool hasAnsweredEarly = true;

    [Header("Button Colors")]
    [SerializeField] Sprite defaultAnswerSprite;
    [SerializeField] Sprite correctAnswerSprite;

    [Header("Timer")]
    [SerializeField] Image timerImage;
    Timer timer;

    [Header("Scoring")]
    [SerializeField] TextMeshProUGUI scoreText;
    ScoreKeeper scoreKeeper;

    [Header("ProgressBar")]
    [SerializeField] Slider progressBar;

    public bool isComplete;



    void Awake()
    {
        timer = FindObjectOfType<Timer>();
        scoreKeeper = FindObjectOfType<ScoreKeeper>();
        progressBar.maxValue = questions.Count;
        progressBar.value = 0;

    }

    void Update()
    {
        timerImage.fillAmount = timer.fillFraction;

        if (timer.loadNextQuestion)
        {
            if(progressBar.value == progressBar.maxValue)
        {
            isComplete = true;
            return;
        }
            hasAnsweredEarly = false;
            GetNextQuestion();
            timer.loadNextQuestion = false;
        }
         else if(!hasAnsweredEarly && !timer.isAnsweringQuestion)
        {
            DisplayAnswer(-1);
            SetButtonState(false);
        }
    }

     public void OnAnswerSelected(int index)
    {

        hasAnsweredEarly = true;
        DisplayAnswer(index);
        SetButtonState(false);
        timer.CancelTimer();
        scoreText.text = "Score: " + scoreKeeper.CalculateScore() + "%";
    }
    
    void DisplayAnswer(int index)
    {
        Image buttonImage;

        if(index == currentQuestion.GetCorrectAnswerIndex())
        {
            questionText.text = "Correct!";
            buttonImage = answerButtons[index].GetComponent<Image>();
            buttonImage.sprite = correctAnswerSprite;
            scoreKeeper.IncrementCorrectAnswers();
        }
        else
        {
            // get the text for the correct answer (first index and then storing it in a strng)
            correctAnswerIndex = currentQuestion.GetCorrectAnswerIndex();
            string correctAnswer = currentQuestion.GetAnswer(correctAnswerIndex);
            // change the text of the questionText
            questionText.text = "Sorry, the correct answer was;\n" + correctAnswer;
            // instead of the changing the sprite of the clicked (wrong) button we want to change the sprite of the correct answer
            buttonImage = answerButtons[correctAnswerIndex].GetComponent<Image>(); 
            buttonImage.sprite = correctAnswerSprite;
        }

    }

    void GetNextQuestion()
    {
        if (questions.Count > 0) // to avoid the bug when we answer all the questions and the list is empty, we put an if-check
        {
            SetButtonState(true); // buttons are intractable at the beginning of getting a question
            SetDefaultButtonSprites(); // sprites of the buttons are set to default at the beginning of a question
            GetRandomQuestion();
            DisplayQuestion(); // question is being displayed after every setting being reset (above lines) 
            progressBar.value++;
            scoreKeeper.IncrementQuestionsSeen();
        }
    }

     void GetRandomQuestion()
    {
        int index = Random.Range(0, questions.Count); // getting the random number between the first number in our list and the last number
        currentQuestion = questions[index];  // the current question would be the question in our list at that index

        //Then we want to remove that question from the list
        if (questions.Contains(currentQuestion))
        {
            questions.Remove(currentQuestion);
        }
    }



    void DisplayQuestion()
    {
        questionText.text = currentQuestion.GetQuestion();

        for (int i = 0; i < answerButtons.Length; i++)
        {
            TextMeshProUGUI buttonText = answerButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = currentQuestion.GetAnswer(i);
        }
    }

    void SetButtonState(bool state) // check if the iteractable state of a button is active or not active
    {
        for(int i = 0; i < answerButtons.Length; i++)
        {
            Button button = answerButtons[i].GetComponent<Button>();
            button.interactable = state;
        }
    }
    void SetDefaultButtonSprites()
    {
        for(int i = 0; i < answerButtons.Length; i++)
        {
            Image buttonImage = answerButtons[i].GetComponent<Image>();
            buttonImage.sprite = defaultAnswerSprite;
        }
    }    
}
