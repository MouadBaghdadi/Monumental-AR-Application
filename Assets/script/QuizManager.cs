using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using ArabicSupport; // Make sure this library is in your project

[System.Serializable]
public class QuizQuestion
{
    public Dictionary<string, string> questionText = new Dictionary<string, string>();
    public Dictionary<string, string[]> options = new Dictionary<string, string[]>();
    public int correctAnswerIndex;
}

public class LanguageData
{
    public string correctFeedback;
    public string wrongFeedback;
    public string quizFinished;
    public string nextButtonText;
}

public class QuizManager : MonoBehaviour
{
    private Text questionText;
    private Button[] answerButtons;
    private Text feedbackText;
    private Button nextButton;
    private Text nextButtonText;
    private Button languageButton;
    private Text languageButtonText;
    
    public AudioSource audioSource;
    public AudioClip correctSound;
    public AudioClip wrongSound;

    private List<QuizQuestion> questions = new List<QuizQuestion>();
    private int currentQuestionIndex = 0;
    
    private string currentLanguage = "en";
    private string[] supportedLanguages = new string[] { "en", "fr", "ar" };
    private int currentLanguageIndex = 0;
    
    private Dictionary<string, LanguageData> languageData = new Dictionary<string, LanguageData>()
    {
        { "en", new LanguageData 
            { 
                correctFeedback = "✅ Correct!", 
                wrongFeedback = "❌ Wrong!", 
                quizFinished = "🎉 Quiz Finished!",
                nextButtonText = "Next"
            } 
        },
        { "fr", new LanguageData 
            { 
                correctFeedback = "✅ Correct !", 
                wrongFeedback = "❌ Incorrect !", 
                quizFinished = "🎉 Quiz Terminé !",
                nextButtonText = "Suivant"
            } 
        },
        { "ar", new LanguageData 
            { 
                correctFeedback = "✅ صحيح!", 
                wrongFeedback = "❌ خطأ!", 
                quizFinished = "🎉 انتهى الاختبار!",
                nextButtonText = "التالي"
            } 
        }
    };

    void ListChildrenRecursive(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Debug.Log("Child name: " + child.gameObject.name);
            ListChildrenRecursive(child);
        }
    }

    public void Awake()
    {
        Debug.Log("[QuizManager] Auto-detecting UI elements in the scene...");
        // ListChildrenRecursive(transform); // Optional: for debugging hierarchy

        GameObject qObj = GameObject.Find("QuestionText");
        if (qObj) questionText = qObj.GetComponent<Text>();
        else Debug.LogWarning("[QuizManager] Missing QuestionText in scene!");

        GameObject fbObj = GameObject.Find("FeedbackText");
        if (fbObj) feedbackText = fbObj.GetComponent<Text>();
        else Debug.LogWarning("[QuizManager] Missing FeedbackText in scene!");

        GameObject nextBtnObj = GameObject.Find("NextButton");
        if (nextBtnObj)
        {
            nextButton = nextBtnObj.GetComponent<Button>();
            nextButtonText = nextBtnObj.GetComponentInChildren<Text>();
        }
        else Debug.LogWarning("[QuizManager] Missing NextButton in scene!");

        GameObject langBtnObj = GameObject.Find("LanguageButton");
        if (langBtnObj)
        {
            languageButton = langBtnObj.GetComponent<Button>();
            languageButtonText = langBtnObj.GetComponentInChildren<Text>();
            if (languageButtonText != null)
            {
                languageButtonText.text = currentLanguage.ToUpper(); // Initial display
            }
            if (languageButton != null)
            {
                languageButton.onClick.AddListener(SwitchLanguage);
            }
        }
        else Debug.LogWarning("[QuizManager] Missing LanguageButton in scene!");

        List<Button> buttons = new List<Button>();
        for (int i = 1; i <= 4; i++)
        {
            GameObject btnObj = GameObject.Find("AnswerButton" + i);
            if (btnObj) buttons.Add(btnObj.GetComponent<Button>());
            else Debug.LogWarning($"[QuizManager] Missing AnswerButton{i} in scene!");
        }
        answerButtons = buttons.ToArray();

        // Initial UI state
        SafeSetActive(questionText, false);
        SafeSetActive(feedbackText, false);
        SafeSetActive(nextButton, false);
        foreach (var btn in answerButtons) SafeSetActive(btn, false);
        // Language button should remain active if found
        if(languageButton != null) SafeSetActive(languageButton, true);
    }
    
    private void SafeSetActive(Component component, bool active)
    {
        if (component != null && component.gameObject != null)
            component.gameObject.SetActive(active);
    }


    public void StartQuiz(string wonderName)
    {
        Debug.Log($"[QuizManager] Starting quiz for: {wonderName}");
        questions = LoadQuestionsFor(wonderName);
        if (questions == null || questions.Count == 0)
        {
            Debug.LogError($"[QuizManager] No questions loaded for {wonderName}. Aborting quiz start.");
            // Optionally, display a message to the user via UI
            if (questionText != null)
            {
                 questionText.gameObject.SetActive(true);
                 questionText.text = ArabicFixer.Fix($"No quiz available for {wonderName}.", true, false);
            }
            SafeSetActive(feedbackText, false);
            foreach (var btn in answerButtons) SafeSetActive(btn, false);
            SafeSetActive(nextButton, false);
            return;
        }
        currentQuestionIndex = 0;

        SafeSetActive(questionText, true);
        SafeSetActive(feedbackText, true); // It will be cleared in ShowQuestion
        foreach (var btn in answerButtons) SafeSetActive(btn, true); // They will be configured in ShowQuestion
        SafeSetActive(nextButton, false);
        
        // Apply initial language settings correctly
        ApplyLanguageSettings();
        ShowQuestion();
    }
    
    private void ApplyLanguageSettings()
    {
        string langDisplay = currentLanguage.ToUpper();
        if (languageButtonText != null)
        {
            // The language button itself usually shows "EN", "FR", "AR" which might not need ArabicFixer
            // unless you want the "AR" itself to be fixed, which is unlikely.
            // If you have full Arabic text for the button like "اللغة", then fix it.
            languageButtonText.text = (currentLanguage == "ar" && langDisplay == "AR") ? ArabicFixer.Fix("ع", true, false) : langDisplay; // Example: if langDisplay is "AR", show "ع"
        }

        if (nextButtonText != null && languageData.ContainsKey(currentLanguage))
        {
            string rawNextText = languageData[currentLanguage].nextButtonText;
            nextButtonText.text = currentLanguage == "ar" ? ArabicFixer.Fix(rawNextText, true, false) : rawNextText;
        }

        // Set text alignment
        bool isRTL = currentLanguage == "ar";
        if (questionText != null) questionText.alignment = isRTL ? TextAnchor.UpperRight : TextAnchor.UpperLeft;
        if (feedbackText != null) feedbackText.alignment = isRTL ? TextAnchor.MiddleRight : TextAnchor.MiddleLeft; // Or UpperRight

        // Answer button text alignment is set to MiddleCenter in ShowQuestion, 
        // but if you want RTL for Arabic options:
        /*
        foreach (var btn in answerButtons)
        {
            var btnTextComp = btn.GetComponentInChildren<Text>();
            if (btnTextComp != null) btnTextComp.alignment = isRTL ? TextAnchor.MiddleRight : TextAnchor.MiddleCenter;
        }
        */
    }


    public void SwitchLanguage()
    {
        currentLanguageIndex = (currentLanguageIndex + 1) % supportedLanguages.Length;
        currentLanguage = supportedLanguages[currentLanguageIndex];
        
        ApplyLanguageSettings(); // Apply all language-specific settings
        ShowQuestion(); // Refresh the current question with the new language
    }

    private void ShowQuestion()
    {
        if (questions == null || questions.Count == 0 || currentQuestionIndex >= questions.Count)
        {
            Debug.LogWarning("[QuizManager] No questions or invalid index. Cannot show question.");
            // Optionally, handle this by ending the quiz or showing an error.
            EndQuiz(); // Or some other appropriate action
            return;
        }
        var q = questions[currentQuestionIndex];

        if (questionText != null)
        {
            string rawQuestion = q.questionText.ContainsKey(currentLanguage) ? q.questionText[currentLanguage] : q.questionText["en"];
            questionText.text = currentLanguage == "ar" ? ArabicFixer.Fix(rawQuestion, true, false) : rawQuestion;
        }
        
        if (feedbackText != null)
        {
            feedbackText.text = "";
            feedbackText.color = Color.black;
        }

        string[] currentOptions = q.options.ContainsKey(currentLanguage) ? q.options[currentLanguage] : q.options["en"];
        
        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (answerButtons[i] == null) continue;

            if (i < currentOptions.Length)
            {
                answerButtons[i].gameObject.SetActive(true);
                var btnText = answerButtons[i].GetComponentInChildren<Text>();
                if (btnText != null)
                {
                    string rawOption = currentOptions[i];
                    btnText.text = currentLanguage == "ar" ? ArabicFixer.Fix(rawOption, true, false) : rawOption;
                    btnText.alignment = TextAnchor.MiddleCenter; // Centered by default
                    // If you want RTL for Arabic options:
                    // btnText.alignment = currentLanguage == "ar" ? TextAnchor.MiddleRight : TextAnchor.MiddleCenter;
                }
                answerButtons[i].interactable = true;
                int idx = i;
                answerButtons[i].onClick.RemoveAllListeners();
                answerButtons[i].onClick.AddListener(() => AnswerSelected(idx));
            }
            else
            {
                answerButtons[i].gameObject.SetActive(false);
            }
        }
        SafeSetActive(nextButton, false); // Hide next button until an answer is selected
    }

    private void AnswerSelected(int index)
    {
        if (questions == null || currentQuestionIndex >= questions.Count) return;

        bool correct = index == questions[currentQuestionIndex].correctAnswerIndex;

        if (feedbackText != null && languageData.ContainsKey(currentLanguage))
        {
            string rawFeedback = correct ? languageData[currentLanguage].correctFeedback : languageData[currentLanguage].wrongFeedback;
            feedbackText.text = currentLanguage == "ar" ? ArabicFixer.Fix(rawFeedback, true, false) : rawFeedback;
            feedbackText.color = correct ? Color.green : Color.red;
        }

        if (audioSource != null)
        {
            audioSource.PlayOneShot(correct ? correctSound : wrongSound);
        }

        foreach (var btn in answerButtons)
        {
            if(btn != null) btn.interactable = false;
        }
            
        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(true);
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(NextQuestion);
            // Next button text is already updated in ApplyLanguageSettings via SwitchLanguage or StartQuiz
        }
    }

    private void NextQuestion()
    {
        currentQuestionIndex++;
        if (currentQuestionIndex < questions.Count)
        {
            ShowQuestion();
        }
        else
        {
            EndQuiz();
        }
    }

    private void EndQuiz()
    {
        if (questionText != null && languageData.ContainsKey(currentLanguage))
        {
            string rawEndMessage = languageData[currentLanguage].quizFinished;
            questionText.text = currentLanguage == "ar" ? ArabicFixer.Fix(rawEndMessage, true, false) : rawEndMessage;
        }
        
        if (feedbackText != null)
        {
            feedbackText.text = "";
        }
        foreach (var btn in answerButtons)
        {
            if(btn != null) btn.gameObject.SetActive(false);
        }
        if (nextButton != null) nextButton.gameObject.SetActive(false);
    }

    private List<QuizQuestion> LoadQuestionsFor(string wonderName)
    {
        var list = new List<QuizQuestion>();
        switch (wonderName)
        {
            case "TajMahal":
                list.Add(new QuizQuestion { 
                    questionText = new Dictionary<string, string> {
                        { "en", "In which Indian city is the Taj Mahal located?" },
                        { "fr", "Dans quelle ville indienne se trouve le Taj Mahal ?" },
                        { "ar", "في أي مدينة هندية يقع تاج محل؟" }
                    },
                    options = new Dictionary<string, string[]> {
                        { "en", new[] { "Agra", "Delhi", "Jaipur", "Mumbai" } },
                        { "fr", new[] { "Agra", "Delhi", "Jaipur", "Mumbai" } },
                        { "ar", new[] { "أغرا", "دلهي", "جايبور", "مومباي" } }
                    },
                    correctAnswerIndex = 0
                });
                list.Add(new QuizQuestion { 
                    questionText = new Dictionary<string, string> {
                        { "en", "Who commissioned the Taj Mahal?" },
                        { "fr", "Qui a commandé la construction du Taj Mahal ?" },
                        { "ar", "من أمر ببناء تاج محل؟" }
                    },
                    options = new Dictionary<string, string[]> {
                        { "en", new[] { "Shah Jahan", "Akbar", "Aurangzeb", "Humayun" } },
                        { "fr", new[] { "Shah Jahan", "Akbar", "Aurangzeb", "Humayun" } },
                        { "ar", new[] { "شاه جهان", "أكبر", "أورانجزيب", "همايون" } }
                    },
                    correctAnswerIndex = 0 
                });
                list.Add(new QuizQuestion { 
                    questionText = new Dictionary<string, string> {
                        { "en", "What material is the Taj Mahal primarily built from?" },
                        { "fr", "De quel matériau le Taj Mahal est-il principalement construit ?" },
                        { "ar", "ما هي المادة الرئيسية التي بني منها تاج محل؟" }
                    },
                    options = new Dictionary<string, string[]> {
                        { "en", new[] { "White marble", "Red sandstone", "Granite", "Limestone" } },
                        { "fr", new[] { "Marbre blanc", "Grès rouge", "Granit", "Calcaire" } },
                        { "ar", new[] { "رخام أبيض", "حجر رملي أحمر", "جرانيت", "حجر جيري" } }
                    },
                    correctAnswerIndex = 0 
                });
                break;

            case "ChichenItza":
                list.Add(new QuizQuestion { 
                    questionText = new Dictionary<string, string> {
                        { "en", "In which country is Chichen Itza located?" },
                        { "fr", "Dans quel pays se trouve Chichen Itza ?" },
                        { "ar", "في أي بلد يقع تشيتشن إيتزا؟" }
                    },
                    options = new Dictionary<string, string[]> {
                        { "en", new[] { "Mexico", "Guatemala", "Peru", "Honduras" } },
                        { "fr", new[] { "Mexique", "Guatemala", "Pérou", "Honduras" } },
                        { "ar", new[] { "المكسيك", "غواتيمالا", "بيرو", "هندوراس" } }
                    },
                    correctAnswerIndex = 0 
                });
                list.Add(new QuizQuestion { 
                    questionText = new Dictionary<string, string> {
                        { "en", "Which ancient civilization built Chichen Itza?" },
                        { "fr", "Quelle civilisation ancienne a construit Chichen Itza ?" },
                        { "ar", "أي حضارة قديمة بنت تشيتشن إيتزا؟" }
                    },
                    options = new Dictionary<string, string[]> {
                        { "en", new[] { "Maya", "Aztec", "Inca", "Olmec" } },
                        { "fr", new[] { "Maya", "Aztèque", "Inca", "Olmèque" } },
                        { "ar", new[] { "المايا", "الأزتيك", "الإنكا", "الأولمك" } }
                    },
                    correctAnswerIndex = 0 
                });
                list.Add(new QuizQuestion { 
                    questionText = new Dictionary<string, string> {
                        { "en", "What is the main pyramid at Chichen Itza called?" },
                        { "fr", "Comment s'appelle la pyramide principale de Chichen Itza ?" },
                        { "ar", "ما هو اسم الهرم الرئيسي في تشيتشن إيتزا؟" }
                    },
                    options = new Dictionary<string, string[]> {
                        { "en", new[] { "El Castillo", "La Iglesia", "El Caracol", "El Mercado" } },
                        { "fr", new[] { "El Castillo", "La Iglesia", "El Caracol", "El Mercado" } },
                        { "ar", new[] { "إل كاستيو", "لا إغليسيا", "إل كاراكول", "إل ميركادو" } }
                    },
                    correctAnswerIndex = 0 
                });
                break;

            case "AbulHol": // Sphinx
                list.Add(new QuizQuestion { 
                    questionText = new Dictionary<string, string> {
                        { "en", "Where is the Great Sphinx (Abul-Hol) located?" },
                        { "fr", "Où se trouve le Grand Sphinx (Abul-Hol) ?" },
                        { "ar", "أين يقع أبو الهول؟" }
                    },
                    options = new Dictionary<string, string[]> {
                        { "en", new[] { "Giza, Egypt", "Cairo, Egypt", "Luxor, Egypt", "Alexandria, Egypt" } },
                        { "fr", new[] { "Gizeh, Égypte", "Le Caire, Égypte", "Louxor, Égypte", "Alexandrie, Égypte" } },
                        { "ar", new[] { "الجيزة، مصر", "القاهرة، مصر", "الأقصر، مصر", "الإسكندرية، مصر" } }
                    },
                    correctAnswerIndex = 0 
                });
                list.Add(new QuizQuestion { 
                    questionText = new Dictionary<string, string> {
                        { "en", "What is the Sphinx (Abul-Hol) a combination of?" },
                        { "fr", "Le Sphinx (Abul-Hol) est une combinaison de quoi ?" },
                        { "ar", "ما هي المكونات التي يجمع بينها أبو الهول؟" }
                    },
                    options = new Dictionary<string, string[]> {
                        { "en", new[] { "Human head and lion body", "Human head and eagle wings", "Lion head and human body", "Human head and bull body" } },
                        { "fr", new[] { "Tête humaine et corps de lion", "Tête humaine et ailes d'aigle", "Tête de lion et corps humain", "Tête humaine et corps de taureau" } },
                        { "ar", new[] { "رأس إنسان وجسم أسد", "رأس إنسان وأجنحة نسر", "رأس أسد وجسم إنسان", "رأس إنسان وجسم ثور" } }
                    },
                    correctAnswerIndex = 0 
                });
                list.Add(new QuizQuestion { 
                    questionText = new Dictionary<string, string> {
                        { "en", "During which pharaoh's reign is the Sphinx believed to have been built?" },
                        { "fr", "Sous le règne de quel pharaon pense-t-on que le Sphinx a été construit ?" },
                        { "ar", "في عهد أي فرعون يُعتقد أنه تم بناء أبو الهول؟" }
                    },
                    options = new Dictionary<string, string[]> {
                        { "en", new[] { "Khafre", "Khufu", "Tutankhamun", "Ramses II" } },
                        { "fr", new[] { "Khafrê", "Khéops", "Toutânkhamon", "Ramsès II" } }, // Corrected French spelling
                        { "ar", new[] { "خفرع", "خوفو", "توت عنخ آمون", "رمسيس الثاني" } }
                    },
                    correctAnswerIndex = 0 
                });
                break;

            case "Colosseum":
                list.Add(new QuizQuestion { 
                    questionText = new Dictionary<string, string> {
                        { "en", "In which city is the Colosseum located?" },
                        { "fr", "Dans quelle ville se trouve le Colisée ?" },
                        { "ar", "في أي مدينة يقع الكولوسيوم؟" }
                    },
                    options = new Dictionary<string, string[]> {
                        { "en", new[] { "Rome", "Athens", "Milan", "Naples" } },
                        { "fr", new[] { "Rome", "Athènes", "Milan", "Naples" } },
                        { "ar", new[] { "روما", "أثينا", "ميلانو", "نابولي" } }
                    },
                    correctAnswerIndex = 0 
                });
                list.Add(new QuizQuestion { 
                    questionText = new Dictionary<string, string> {
                        { "en", "When was the Colosseum completed?" },
                        { "fr", "Quand le Colisée a-t-il été achevé ?" },
                        { "ar", "متى تم الانتهاء من بناء الكولوسيوم؟" }
                    },
                    options = new Dictionary<string, string[]> {
                        { "en", new[] { "80 AD", "50 AD", "120 AD", "200 AD" } },
                        { "fr", new[] { "80 après J.-C.", "50 après J.-C.", "120 après J.-C.", "200 après J.-C." } },
                        { "ar", new[] { "80 ميلادي", "50 ميلادي", "120 ميلادي", "200 ميلادي" } }
                    },
                    correctAnswerIndex = 0 
                });
                list.Add(new QuizQuestion { 
                    questionText = new Dictionary<string, string> {
                        { "en", "What was the original name of the Colosseum?" },
                        { "fr", "Quel était le nom original du Colisée ?" },
                        { "ar", "ما هو الاسم الأصلي للكولوسيوم؟" }
                    },
                    options = new Dictionary<string, string[]> {
                        { "en", new[] { "Flavian Amphitheatre", "Roman Arena", "Imperial Stadium", "Julius Caesar Theatre" } },
                        { "fr", new[] { "Amphithéâtre Flavien", "Arène Romaine", "Stade Impérial", "Théâtre de Jules César" } },
                        { "ar", new[] { "مدرج فلافيان", "حلبة رومانية", "الملعب الإمبراطوري", "مسرح يوليوس قيصر" } }
                    },
                    correctAnswerIndex = 0 
                });
                break;

            case "GreatWallOfChina":
                 list.Add(new QuizQuestion { 
                    questionText = new Dictionary<string, string> {
                        { "en", "During which dynasty did the majority of the Great Wall's construction take place?" },
                        { "fr", "Pendant quelle dynastie la majeure partie de la construction de la Grande Muraille a-t-elle eu lieu ?" },
                        { "ar", "في أي سلالة حاكمة تم بناء الجزء الأكبر من سور الصين العظيم؟" }
                    },
                    options = new Dictionary<string, string[]> {
                        { "en", new[] { "Ming", "Han", "Tang", "Song" } },
                        { "fr", new[] { "Ming", "Han", "Tang", "Song" } },
                        { "ar", new[] { "مينغ", "هان", "تانغ", "سونغ" } }
                    },
                    correctAnswerIndex = 0 
                });
                list.Add(new QuizQuestion { 
                    questionText = new Dictionary<string, string> {
                        { "en", "What was the primary purpose of building the Great Wall of China?" },
                        { "fr", "Quel était l'objectif principal de la construction de la Grande Muraille de Chine ?" },
                        { "ar", "ما كان الغرض الأساسي من بناء سور الصين العظيم؟" }
                    },
                    options = new Dictionary<string, string[]> {
                        { "en", new[] { "Defense against invasions", "Trade route marker", "Religious boundary", "Imperial showcase" } },
                        { "fr", new[] { "Défense contre les invasions", "Marqueur de route commerciale", "Frontière religieuse", "Vitrine impériale" } },
                        { "ar", new[] { "الدفاع ضد الغزوات", "علامة للطرق التجارية", "حدود دينية", "واجهة إمبراطورية" } }
                    },
                    correctAnswerIndex = 0 
                });
                list.Add(new QuizQuestion { 
                    questionText = new Dictionary<string, string> {
                        { "en", "Approximately how long is the Great Wall of China?" },
                        { "fr", "Quelle est la longueur approximative de la Grande Muraille de Chine ?" },
                        { "ar", "ما هو الطول التقريبي لسور الصين العظيم؟" }
                    },
                    options = new Dictionary<string, string[]> {
                        { "en", new[] { "21,196 kilometers", "10,000 kilometers", "5,000 kilometers", "30,000 kilometers" } },
                        { "fr", new[] { "21 196 kilomètres", "10 000 kilomètres", "5 000 kilomètres", "30 000 kilomètres" } },
                        { "ar", new[] { "21,196 كيلومتر", "10,000 كيلومتر", "5,000 كيلومتر", "30,000 كيلومتر" } }
                    },
                    correctAnswerIndex = 0 
                });
                break;

            case "Pyramid": // Assuming Great Pyramid of Giza
                list.Add(new QuizQuestion { 
                    questionText = new Dictionary<string, string> {
                        { "en", "Which is the largest of the Egyptian pyramids?" },
                        { "fr", "Quelle est la plus grande des pyramides égyptiennes ?" },
                        { "ar", "ما هو أكبر الأهرامات المصرية؟" }
                    },
                    options = new Dictionary<string, string[]> {
                        { "en", new[] { "Great Pyramid of Giza", "Pyramid of Khafre", "Pyramid of Menkaure", "Red Pyramid" } },
                        { "fr", new[] { "Grande Pyramide de Gizeh", "Pyramide de Khéphren", "Pyramide de Mykérinos", "Pyramide Rouge" } },
                        { "ar", new[] { "هرم خوفو الأكبر", "هرم خفرع", "هرم منقرع", "الهرم الأحمر" } }
                    },
                    correctAnswerIndex = 0 
                });
                list.Add(new QuizQuestion { 
                    questionText = new Dictionary<string, string> {
                        { "en", "What were Egyptian pyramids primarily built as?" },
                        { "fr", "À quoi servaient principalement les pyramides égyptiennes ?" },
                        { "ar", "ما هو الغرض الرئيسي من بناء الأهرامات المصرية؟" }
                    },
                    options = new Dictionary<string, string[]> {
                        { "en", new[] { "Tombs", "Temples", "Observatories", "Palaces" } },
                        { "fr", new[] { "Tombeaux", "Temples", "Observatoires", "Palais" } },
                        { "ar", new[] { "مقابر", "معابد", "مراصد", "قصور" } }
                    },
                    correctAnswerIndex = 0 
                });
                list.Add(new QuizQuestion { 
                    questionText = new Dictionary<string, string> {
                        { "en", "Who is believed to have built the Great Pyramid of Giza?" },
                        { "fr", "Qui pense-t-on avoir construit la Grande Pyramide de Gizeh ?" },
                        { "ar", "من يُعتقد أنه بنى الهرم الأكبر في الجيزة؟" }
                    },
                    options = new Dictionary<string, string[]> {
                        { "en", new[] { "Khufu", "Khafre", "Menkaure", "Djoser" } },
                        { "fr", new[] { "Khéops", "Khéphren", "Mykérinos", "Djéser" } },
                        { "ar", new[] { "خوفو", "خفرع", "منقرع", "زوسر" } }
                    },
                    correctAnswerIndex = 0 
                });
                break;
            default:
                Debug.LogWarning($"[QuizManager] No questions found for wonder: {wonderName}");
                break;
        }
        return list;
    }
}