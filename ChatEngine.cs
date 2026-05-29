using CybersecurityChatbot.Core;
using CybersecurityChatbot.Data;


namespace PROG_Part1
{
    // Core engine that drives chatbot interactions using dictionary-based responses.
    
    public class ChatEngine
    {
        private string _userName = string.Empty;

        // Dictionary: timestamp > message (conversation history)
        private readonly Dictionary<string, string> _conversationHistory = new Dictionary<string, string>();
        private int _historyIndex = 1;

        // Dictionary: topic number > topic key (built when user types 'topics')
        private readonly Dictionary<int, string> _topicNumberMap = new Dictionary<int, string>();

        // Dictionary: sentiment keyword > response prefix
        private readonly Dictionary<string, string> _sentimentMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Worried", "I understand your concern. " },
            { "Scared", "It's completely okay to feel that way. " },
            { "Confused", "No worries, let me clarify. " },
            { "Frustrated", "I hear you. Let's work through this together. " },
            { "Curious", "Great question! " },
            { "Happy", "Glad you're in good spirits! " },
            { "Angry", "I understand your frustration. " },
        };

        public void Start()
        {
            GetUserName();
            RunMainLoop();
        }

        private void GetUserName()
        {
            DisplayHelper.PrintBot("Hello! Welcome to the Cybersecurity Awareness Chatbot.");
            DisplayHelper.PrintBot("Before we begin, what's your name?");
            Console.Write("\n  You: ");
            _userName = Console.ReadLine()?.Trim() ?? "User";
            if (string.IsNullOrWhiteSpace(_userName)) _userName = "User";
            DisplayHelper.PrintBot($"Nice to meet you, {_userName}! How are you? Press 'help' to see a list of commands.");
            Console.WriteLine();
        }

        private void RunMainLoop()
        {
            bool running = true;

            while (running)
            {
                Console.Write($"  {_userName}: ");
                string input = Console.ReadLine()?.Trim() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(input))
                {
                    DisplayHelper.PrintWarning("I didn't catch that. Type 'help' for a list of commands.");
                    continue;
                }

                // Log to history
                _conversationHistory[$"[{_historyIndex++}] You"] = input;

                string response = ProcessInput(input);

                if (response == "EXIT")
                {
                    running = false;
                    DisplayHelper.PrintBot($"Stay safe online, {_userName}! Goodbye!");
                }
                else
                {
                    // Use PrintBotList for multi-line responses (topics, help, history)
                    // Use PrintBot for single-line responses (word-wrap applied)
                    if (response.Contains("\n"))
                    {
                        DisplayHelper.PrintBotList(response);
                    }
                    else
                    {
                        DisplayHelper.PrintBot(response);
                    }

                    _conversationHistory[$"[{_historyIndex++}] Bot"] = response;
                }

                Console.WriteLine();
            }
        }

        private string ProcessInput(string input)
        {
            string lowerInput = input.ToLower().Trim();

            // Check if user typed a number to select a topic
            if (int.TryParse(input.Trim(), out int topicNumber))
            {
                if (_topicNumberMap.ContainsKey(topicNumber))
                {
                    string topicKey = _topicNumberMap[topicNumber];
                    return ResponseBank.KeywordResponses[topicKey];
                }
                else if (_topicNumberMap.Count == 0)
                {
                    return "Type 'topics' first to see the numbered list, then enter a number.";
                }
                else
                {
                    return $"Please enter a number between 1 and {_topicNumberMap.Count}.";
                }
            }

            // Check commands first
            if (ResponseBank.Commands.ContainsKey(lowerInput))
            {
                return HandleCommand(lowerInput);
            }

            // Check for sentiment keywords
            string sentimentPrefix = DetectSentiment(lowerInput);

            // Check conversational / small-talk responses first
            foreach (var entry in ResponseBank.ConversationalResponses)
            {
                if (lowerInput.Contains(entry.Key.ToLower()))
                {
                    return entry.Value;
                }
            }

            // Search keyword responses
            foreach (var entry in ResponseBank.KeywordResponses)
            {
                if (lowerInput.Contains(entry.Key.ToLower()))
                {
                    return sentimentPrefix + entry.Value;
                }
            }

            // Fallback
            return sentimentPrefix + $"I'm not sure about that, {_userName}. Try typing 'topics' to see what I can help with, or 'help' for commands.";
        }

        private string HandleCommand(string command)
        {
            // Dictionary: command > handler method (using delegates)
            Dictionary<string, Func<string>> commandHandlers = new Dictionary<string, Func<string>>(StringComparer.OrdinalIgnoreCase)
            {
                { "Help",  ShowHelp },
                { "Topics",  ShowTopics },
                { "Tip",  ShowRandomTip },
                { "Quiz",  RunQuiz },
                { "History",  ShowHistory },
                { "Clear",  ClearHistory },
                { "Exit",  () => "EXIT" },
            };

            if (commandHandlers.TryGetValue(command, out Func<string>? handler))
            {
                return handler();
            }

            return "Unknown command. Type 'help' to see available commands.";
        }

        private string ShowHelp()
        {
            var lines = new List<string> { "Here are the available commands:\n" };
            foreach (var entry in ResponseBank.Commands)
            {
                lines.Add($"  • {entry.Key,-12} - {entry.Value}");
            }
            return string.Join("\n", lines);
        }

        private string ShowTopics()
        {
            _topicNumberMap.Clear();
            var lines = new List<string> { "You can ask me about the following topics:\n" };
            int count = 1;
            foreach (var key in ResponseBank.KeywordResponses.Keys)
            {
                _topicNumberMap[count] = key;
                lines.Add($"  {count,3}. {key,-25}");
                count++;
            }
            lines.Add("\nType a number to learn about that topic, or just ask a question!");
            return string.Join("\n", lines);
        }

        private string ShowRandomTip()
        {
            var tips = ResponseBank.SecurityTips;
            int randomIndex = new Random().Next(1, tips.Count + 1);
            string tipKey = $"tip{randomIndex}";
            if (tips.TryGetValue(tipKey, out string? tip))
            {
                return $"Security Tip: {tip}";
            }
            return "Stay safe: always think before you click!";
        }

        private string RunQuiz()
        {
            QuizEngine quiz = new QuizEngine(_userName);
            quiz.Run();
            return "Quiz complete! Type 'help' to continue exploring.";
        }

        private string ShowHistory()
        {
            if (_conversationHistory.Count == 0)
            {
                return "No conversation history yet.";
            }

            var lines = new List<string> { "--- Conversation History ---\n" };
            foreach (var entry in _conversationHistory)
            {
                lines.Add($"  {entry.Key}: {entry.Value}");
            }
            lines.Add("\n--- End of History ---");
            return string.Join("\n", lines);
        }

        private string ClearHistory()
        {
            _conversationHistory.Clear();
            _historyIndex = 1;
            return "Conversation history cleared.";
        }

        private string DetectSentiment(string input)
        {
            foreach (var entry in _sentimentMap)
            {
                if (input.Contains(entry.Key.ToLower()))
                {
                    return entry.Value;
                }
            }
            return string.Empty;
        }
    }
}