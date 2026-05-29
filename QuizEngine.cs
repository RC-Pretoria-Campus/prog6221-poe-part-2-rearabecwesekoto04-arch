using CybersecurityChatbot.Data;
using PROG_Part1;

namespace CybersecurityChatbot.Core
{
    public class QuizEngine
    {
        private readonly string _userName;

        private readonly Dictionary<int, string> _userAnswers = new();

        // Clean exit command (centralised)
        private const string EXIT = "__EXIT__";

        private readonly Dictionary<string, string> _scoreFeedback = new()
        {
            { "Excellent", "Excellent! You're a cybersecurity pro!" },
            { "Good",      "Good job! A little more practice and you'll be an expert." },
            { "Average",   "Not bad, but consider brushing up on the topics you missed." },
            { "Poor",      "You may be at risk. Take time to learn the basics of cybersecurity." },
        };

        public QuizEngine(string userName)
        {
            _userName = userName;
        }

        public void Run()
        {
            Console.WriteLine();
            DisplayHelper.PrintBot(
                $"Starting the Cybersecurity Quiz! " +
                $"Answer with A, B, C, or D. Type 'exit' anytime to quit.\n"
            );

            var questions = QuizBank.Round1;
            int score = 0;

            foreach (var questionEntry in questions)
            {
                int questionNum = questionEntry.Key;
                QuizQuestion q = questionEntry.Value;

                // Question display
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"\n  Q{questionNum}: {q.Question}");
                Console.ResetColor();

                // Options
                foreach (var option in q.Options)
                {
                    Console.WriteLine($"     {option.Key}) {option.Value}");
                }

                // Get answer
                string answer = GetValidAnswer(q.Options);

                // Clean Exit 
                if (answer == EXIT)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("\nQuiz exited early. Returning to chatbot...");
                    Console.ResetColor();
                    return;
                }

                _userAnswers[questionNum] = answer;

                // Check answer
                if (answer.Equals(q.CorrectAnswer, StringComparison.OrdinalIgnoreCase))
                {
                    score++;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Correct! {q.Explanation}");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(
                        $"Incorrect. The correct answer is {q.CorrectAnswer}: {q.Options[q.CorrectAnswer]}"
                    );
                    Console.WriteLine($"     {q.Explanation}");
                }

                Console.ResetColor();
            }

            ShowResults(score, questions.Count);
        }

        private string GetValidAnswer(Dictionary<string, string> options)
        {
            while (true)
            {
                Console.Write("  Your answer (A, B, CD or exit): ");
                string input = Console.ReadLine()?.Trim().ToUpper() ?? string.Empty;

                // EXIT ANYTIME
                if (input == "EXIT" || input == "Q")
                {
                    return EXIT;
                }

                // Valid answer check
                if (options.ContainsKey(input))
                {
                    return input;
                }

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Invalid input. Please enter A, B, C, D or type 'exit'.");
                Console.ResetColor();
            }
        }

        private void ShowResults(int score, int total)
        {
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("=====================================");
            Console.WriteLine($"   Quiz Results for {_userName}");
            Console.WriteLine($"   Score: {score} / {total}");
            Console.WriteLine("=====================================");
            Console.ResetColor();

            string feedbackKey =
                score == total ? "Excellent"
                : score >= total * 0.7 ? "Good"
                : score >= total * 0.4 ? "Average"
                : "Poor";

            if (_scoreFeedback.TryGetValue(feedbackKey, out string? feedback))
            {
                DisplayHelper.PrintBot(feedback);
            }

            Console.WriteLine();
        }
    }
}