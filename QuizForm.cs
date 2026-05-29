using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CybersecurityChatbot.Data;

namespace CybersecurityChatbot.GUI
{
    /// <summary>
    /// Quiz window with 3 rounds of 5 questions each.
    /// Uses dictionaries throughout for clean, optimised code.
    /// Features: coloured A/B/C/D buttons, progress bar, round results,
    /// Exit button with confirmation dialog, final score screen.
    /// </summary>
    public class QuizForm : Form
    {
        // UI Controls
        private Label _roundLabel = null!;
        private Label _questionNumLabel = null!;
        private Label _scoreLabel = null!;
        private Label _totalScoreLabel = null!;
        private Label _questionLabel = null!;
        private Panel _optionsPanel = null!;
        private Label _feedbackLabel = null!;
        private Button _actionButton = null!;
        private Button _exitButton = null!;
        private ProgressBar _progressBar = null!;

        // State 
        private readonly string _userName;
        private int _currentRound = 0;
        private int _currentQ = 0;
        private int _roundScore = 0;
        private int _totalScore = 0;
        private bool _answered = false;
        private bool _quizFinished = false;
        private bool _forceClose = false; // ← FIX: prevents infinite loop on exit

        private List<KeyValuePair<int, QuizQuestion>> _currentQuestions = new();

        // Colour Palette (Dictionary)
        private readonly Dictionary<string, Color> _colours = new Dictionary<string, Color>
        {
            { "bgDark",    Color.FromArgb(10,  14,  20)  },
            { "bgMid",     Color.FromArgb(16,  22,  32)  },
            { "bgPanel",   Color.FromArgb(20,  28,  42)  },
            { "accent",    Color.FromArgb(0,   220, 180) },
            { "accentAlt", Color.FromArgb(255, 180, 0)   },
            { "correct",   Color.FromArgb(80,  220, 100) },
            { "wrong",     Color.FromArgb(255, 80,  80)  },
            { "neutral",   Color.FromArgb(60,  80,  110) },
            { "exit",      Color.FromArgb(180, 40,  40)  },
            { "hover",     Color.FromArgb(0,   60,  55)  },
        };

        // Constructor
        public QuizForm(string userName)
        {
            _userName = userName;
            InitialiseComponent();
            LoadRound(_currentRound);
        }

        // UI INIT
        

        private void InitialiseComponent()
        {
            Text = "CyberBot — Quiz";
            Size = new Size(700, 580);
            BackColor = _colours["bgDark"];
            ForeColor = Color.White;
            Font = new Font("Consolas", 10f);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            // Intercept X button
            FormClosing += OnFormClosing;

            // Header 
            var header = new Label
            {
                Text = $"  CYBERSECURITY QUIZ  —  {_userName.ToUpper()}",
                ForeColor = _colours["accent"],
                Font = new Font("Consolas", 11f, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 40,
                BackColor = _colours["bgPanel"],
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Progress bar
            _progressBar = new ProgressBar
            {
                Dock = DockStyle.Top,
                Height = 6,
                Minimum = 0,
                Maximum = 5,
                Value = 0,
                Style = ProgressBarStyle.Continuous,
                BackColor = _colours["bgMid"],
                ForeColor = _colours["accent"]
            };

            // Labels using dictionary: key → (text, location, size, colour, font)
            var labelDefs = new Dictionary<string, (string text, Point loc, Size size, string colour, float fontSize)>
            {
                { "round",    ( "",                  new Point(20, 58),  new Size(500, 20), "accentAlt", 9.5f ) },
                { "qNum",     ( "",                  new Point(20, 80),  new Size(300, 18), "accent",    8.5f ) },
                { "score",    ( "Round Score: 0/5",  new Point(510, 80), new Size(160, 18), "accent",    8.5f ) },
                { "total",    ( "Total: 0/15",       new Point(510, 58), new Size(160, 18), "accentAlt", 8.5f ) },
                { "question", ( "",                  new Point(20, 108), new Size(650, 60), "white",     10.5f) },
                { "feedback", ( "",                  new Point(20, 405), new Size(650, 55), "correct",   8.5f ) },
            };

            foreach (var def in labelDefs)
            {
                Color colour = def.Value.colour == "white" ? Color.White : _colours[def.Value.colour];
                var lbl = new Label
                {
                    Text = def.Value.text,
                    ForeColor = colour,
                    Font = new Font("Consolas", def.Value.fontSize),
                    Location = def.Value.loc,
                    Size = def.Value.size,
                    BackColor = Color.Transparent,
                    AutoSize = false
                };

                // Assign references by key
                switch (def.Key)
                {
                    case "round": _roundLabel = lbl; break;
                    case "qNum": _questionNumLabel = lbl; break;
                    case "score": _scoreLabel = lbl; break;
                    case "total": _totalScoreLabel = lbl; break;
                    case "question": _questionLabel = lbl; break;
                    case "feedback": _feedbackLabel = lbl; break;
                }

                Controls.Add(lbl);
            }

            // Options panel 
            _optionsPanel = new Panel
            {
                Location = new Point(20, 175),
                Size = new Size(650, 220),
                BackColor = Color.Transparent
            };

            // Action button (Next / See Results / Next Quiz / Close) 
            _actionButton = new Button
            {
                Text = "Next Question ▶",
                Location = new Point(500, 505),
                Size = new Size(175, 36),
                BackColor = _colours["accent"],
                ForeColor = _colours["bgDark"],
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Consolas", 8.5f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Enabled = false,
                FlatAppearance = { BorderSize = 0 }
            };
            _actionButton.Click += OnActionButton;

            // Exit button 
            _exitButton = new Button
            {
                Text = "EXIT ✕",
                Location = new Point(20, 505),
                Size = new Size(100, 36),
                BackColor = _colours["exit"],
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Consolas", 8.5f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                FlatAppearance = { BorderSize = 0 }
            };
            _exitButton.Click += OnExitClicked;
            _exitButton.MouseEnter += (s, e) => _exitButton.BackColor = Color.FromArgb(220, 60, 60);
            _exitButton.MouseLeave += (s, e) => _exitButton.BackColor = _colours["exit"];

            Controls.AddRange(new Control[]
            {
                header, _progressBar, _optionsPanel,
                _actionButton, _exitButton
            });
        }

        // ROUND LOADING
        

        private void LoadRound(int roundIndex)
        {
            _currentQ = 0;
            _roundScore = 0;
            _answered = false;

            _currentQuestions = new List<KeyValuePair<int, QuizQuestion>>(QuizBank.AllRounds[roundIndex]);
            _progressBar.Value = 0;
            _progressBar.Maximum = _currentQuestions.Count;
            _roundLabel.Text = QuizBank.RoundTitles[roundIndex];
            _scoreLabel.Text = $"Round Score: 0/{_currentQuestions.Count}";

            LoadQuestion();
        }

        // QUESTION LOADING

        private void LoadQuestion()
        {
            if (_currentQ >= _currentQuestions.Count)
            {
                ShowRoundResults();
                return;
            }

            _answered = false;
            _feedbackLabel.Text = "";
            _actionButton.Enabled = false;
            _actionButton.Text = "Next Question ▶";
            _progressBar.Value = _currentQ;

            var q = _currentQuestions[_currentQ].Value;
            _questionNumLabel.Text = $"Question {_currentQ + 1} of {_currentQuestions.Count}";
            _questionLabel.Text = q.Question;

            // Build A/B/C/D buttons using dictionary: optKey → optText
            _optionsPanel.Controls.Clear();
            int y = 0;

            foreach (var opt in q.Options)
            {
                string optKey = opt.Key;
                string optText = opt.Value;

                var btn = new Button
                {
                    Text = $"  {optKey})  {optText}",
                    Location = new Point(0, y),
                    Size = new Size(650, 46),
                    BackColor = _colours["neutral"],
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Consolas", 9f),
                    TextAlign = ContentAlignment.MiddleLeft,
                    Cursor = Cursors.Hand,
                    Tag = optKey,
                    FlatAppearance =
                    {
                        BorderColor = Color.FromArgb(60, 80, 120),
                        BorderSize  = 1
                    }
                };

                btn.Click += OnAnswerSelected;
                btn.MouseEnter += (s, e) => { if (!_answered) ((Button)s!).BackColor = _colours["hover"]; };
                btn.MouseLeave += (s, e) => { if (!_answered) ((Button)s!).BackColor = _colours["neutral"]; };

                _optionsPanel.Controls.Add(btn);
                y += 50;
            }
        }

        // ANSWER SELECTION

        private void OnAnswerSelected(object? sender, EventArgs e)
        {
            if (_answered) return;
            _answered = true;

            var clicked = (Button)sender!;
            string selected = clicked.Tag?.ToString() ?? "";
            var q = _currentQuestions[_currentQ].Value;
            bool isCorrect = selected.Equals(q.CorrectAnswer, StringComparison.OrdinalIgnoreCase);

            // Colour buttons using dictionary: optKey → colour key
            var buttonColours = new Dictionary<string, string>();
            foreach (Button btn in _optionsPanel.Controls)
            {
                string key = btn.Tag?.ToString() ?? "";
                buttonColours[key] = key == q.CorrectAnswer ? "correct"
                                   : key == selected ? "wrong"
                                   : "neutral";
                btn.BackColor = _colours[buttonColours[key]];
                btn.Enabled = false;
            }

            if (isCorrect)
            {
                _roundScore++;
                _totalScore++;
                _feedbackLabel.ForeColor = _colours["correct"];
                _feedbackLabel.Text = $"✔  Correct!  {q.Explanation}";
            }
            else
            {
                _feedbackLabel.ForeColor = _colours["wrong"];
                _feedbackLabel.Text =
                    $"✘  Incorrect. Answer: {q.CorrectAnswer}) " +
                    $"{q.Options[q.CorrectAnswer]}\n   {q.Explanation}";
            }

            _scoreLabel.Text = $"Round Score: {_roundScore}/{_currentQuestions.Count}";
            _totalScoreLabel.Text = $"Total: {_totalScore}/15";
            _actionButton.Enabled = true;

            bool isLastQ = _currentQ == _currentQuestions.Count - 1;
            bool isLastRound = _currentRound == QuizBank.AllRounds.Count - 1;

            if (isLastQ)
                _actionButton.Text = isLastRound ? "See Final Results ▶" : "See Round Results ▶";
        }

        // ACTION BUTTON

        private void OnActionButton(object? sender, EventArgs e)
        {
            _currentQ++;
            if (_currentQ >= _currentQuestions.Count)
            {
                bool isLastRound = _currentRound == QuizBank.AllRounds.Count - 1;
                if (isLastRound) ShowFinalResults();
                else ShowRoundResults();
            }
            else
            {
                LoadQuestion();
            }
        }

        // ROUND RESULTS

        private void ShowRoundResults()
        {
            _optionsPanel.Controls.Clear();
            _progressBar.Value = _currentQuestions.Count;
            _questionNumLabel.Text = "Round Complete!";

            // Grade using dictionary: min score → grade message
            var grades = new Dictionary<int, string>
            {
                { 5, "Perfect round! Outstanding!"          },
                { 4, "Excellent! Nearly perfect!"           },
                { 3, "Good job! Keep it up!"                },
                { 2, "Not bad — review what you missed."    },
                { 0, "Keep studying — you'll get there!"    },
            };

            string grade = "Keep studying — you'll get there!";
            foreach (var g in grades)
                if (_roundScore >= g.Key) { grade = g.Value; break; }

            _questionLabel.Font = new Font("Consolas", 11f, FontStyle.Bold);
            _questionLabel.Text = $"Round {_currentRound + 1} Results";

            _feedbackLabel.Font = new Font("Consolas", 10f);
            _feedbackLabel.ForeColor = _colours["accentAlt"];
            _feedbackLabel.Text =
                $"Round Score: {_roundScore}/{_currentQuestions.Count}\n{grade}\n\n" +
                $"Total so far: {_totalScore}/{(_currentRound + 1) * 5}";

            _actionButton.Text = "Next Quiz ▶";
            _actionButton.Enabled = true;
            _actionButton.Click -= OnActionButton;
            _actionButton.Click += OnNextQuiz;
        }

        // NEXT QUIZ

        private void OnNextQuiz(object? sender, EventArgs e)
        {
            _currentRound++;
            _questionLabel.Font = new Font("Consolas", 10.5f);
            _feedbackLabel.Font = new Font("Consolas", 8.5f, FontStyle.Italic);
            _actionButton.Text = "Next Question ▶";
            _actionButton.Click -= OnNextQuiz;
            _actionButton.Click += OnActionButton;
            LoadRound(_currentRound);
        }

        // FINAL RESULTS

        private void ShowFinalResults()
        {
            _quizFinished = true;
            _optionsPanel.Controls.Clear();
            _progressBar.Value = _currentQuestions.Count;
            _questionNumLabel.Text = "Quiz Complete!";
            _roundLabel.Text = "All 3 Rounds Finished!";

            // Final grade using dictionary: min score → grade message
            var finalGrades = new Dictionary<int, string>
            {
                { 15, "PERFECT SCORE! You are a Cybersecurity Expert!"          },
                { 12, "Outstanding! You really know your stuff!"                 },
                { 9,  "Great work! Keep building your knowledge."                },
                { 6,  "Not bad — consider brushing up on the topics you missed." },
                { 0,  "Keep learning! Cybersecurity knowledge keeps you safe."   },
            };

            string finalGrade = "Keep learning! Cybersecurity knowledge keeps you safe.";
            foreach (var g in finalGrades)
                if (_totalScore >= g.Key) { finalGrade = g.Value; break; }

            _questionLabel.Font = new Font("Consolas", 11f, FontStyle.Bold);
            _questionLabel.Text = $"Final Results — {_userName}";

            _feedbackLabel.Font = new Font("Consolas", 10f);
            _feedbackLabel.ForeColor = _colours["accentAlt"];
            _feedbackLabel.Text = $"Total Score: {_totalScore} / 15\n\n{finalGrade}";

            _scoreLabel.Text = $"Round Score: {_roundScore}/5";
            _totalScoreLabel.Text = $"Total: {_totalScore}/15";

            _actionButton.Text = "Close Quiz";
            _actionButton.Enabled = true;
            _actionButton.Click -= OnActionButton;
            _actionButton.Click += (s, e) =>
            {
                _forceClose = true;
                Close();
            };

            // Hide exit button — quiz is done
            _exitButton.Visible = false;
        }

        // EXIT CONFIRMATION

        private void OnExitClicked(object? sender, EventArgs e)
        {
            ConfirmExit();
        }

        /// <summary>
        /// Intercepts the window X button. If already confirmed (_forceClose),
        /// lets it close. Otherwise shows confirmation dialog.
        /// </summary>
        private void OnFormClosing(object? sender, FormClosingEventArgs e)
        {
            if (_forceClose) return; // ← Already confirmed, let it close normally

            if (e.CloseReason == CloseReason.UserClosing && !_quizFinished)
            {
                e.Cancel = true;
                ConfirmExit();
            }
        }

        /// <summary>
        /// Styled confirmation dialog before exiting mid-quiz.
        /// Uses a dictionary to build Yes/No buttons cleanly.
        /// Sets _forceClose = true before calling Close() to prevent
        /// OnFormClosing from cancelling the close again.
        /// </summary>
        private void ConfirmExit()
        {
            using Form dialog = new Form
            {
                Text = "Exit Quiz?",
                Size = new Size(400, 190),
                BackColor = _colours["bgPanel"],
                ForeColor = Color.White,
                Font = new Font("Consolas", 9.5f),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            dialog.Controls.Add(new Label
            {
                Text = "Are you sure you want to exit the quiz?\n\n" +
                       "Your progress will not be saved.",
                ForeColor = Color.White,
                Font = new Font("Consolas", 9.5f),
                Location = new Point(20, 20),
                Size = new Size(350, 60),
                BackColor = Color.Transparent
            });

            // Dictionary of dialog buttons: label → (x position, colour key, dialog result)
            var dialogButtons = new Dictionary<string, (int x, string colourKey, DialogResult result)>
            {
                { "Yes, Exit", (220, "exit",   DialogResult.Yes) },
                { "No, Stay",  (100, "accent", DialogResult.No)  },
            };

            foreach (var def in dialogButtons)
            {
                var btn = new Button
                {
                    Text = def.Key,
                    Location = new Point(def.Value.x, 110),
                    Size = new Size(110, 34),
                    BackColor = _colours[def.Value.colourKey],
                    ForeColor = def.Key == "No, Stay" ? _colours["bgDark"] : Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Consolas", 9f, FontStyle.Bold),
                    Cursor = Cursors.Hand,
                    DialogResult = def.Value.result,
                    FlatAppearance = { BorderSize = 0 }
                };
                dialog.Controls.Add(btn);
            }

            if (dialog.ShowDialog(this) == DialogResult.Yes)
            {
                _forceClose = true; // ← Allow OnFormClosing to pass through
                Close();
            }
            // If No — do nothing, return to quiz
        }
    }
}
