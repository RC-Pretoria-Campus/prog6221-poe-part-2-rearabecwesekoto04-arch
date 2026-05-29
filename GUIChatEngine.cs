using System;
using System.Collections.Generic;
using System.Linq;
using CybersecurityChatbot.Data;

namespace CybersecurityChatbot.Core
{
    /// <summary>
    /// GUI-aware chat engine for Part 2.
    /// Fully dictionary-driven for clean, optimised, readable code.
    /// Features: memory/recall, random responses, conversation flow,
    /// advanced mood-based sentiment detection, error handling.
    /// </summary>
    public class GUIChatEngine
    {
        // State Machine
        public enum ChatState { AwaitingName, AwaitingFavTopic, Active }
        public ChatState State { get; private set; } = ChatState.AwaitingName;

        // User Memory
        public string UserName { get; private set; } = string.Empty;
        public string FavouriteTopic { get; private set; } = string.Empty;

        // Conversation Context 
        private string _lastTopic = string.Empty;
        private string _lastResponse = string.Empty;
        private int _tipIndex = 0;

        // History (Dictionary: label → message) 
        private readonly Dictionary<string, string> _conversationHistory = new();
        private int _historyIndex = 1;

        // Topic Number Map 
        private readonly Dictionary<int, string> _topicNumberMap = new();

        // Callbacks to GUI 
        private readonly Action<string> _appendBot;
        private readonly Action<string> _appendSystem;

        // Mood Keywords (Dictionary: keyword > mood label) 
        private readonly Dictionary<string, string> _moodKeywords =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "worried",     "worried"     },
            { "scared",      "scared"      },
            { "confused",    "confused"    },
            { "frustrated",  "frustrated"  },
            { "curious",     "curious"     },
            { "happy",       "happy"       },
            { "excited",     "excited"     },
            { "angry",       "angry"       },
            { "nervous",     "nervous"     },
            { "anxious",     "anxious"     },
            { "unsure",      "unsure"      },
            { "bored",       "bored"       },
            { "overwhelmed", "overwhelmed" },
            { "confident",   "confident"   },
            { "stressed",    "stressed"    },
        };

        //  Mood-Only Responses (Dictionary: mood > full response)
        // Used when user expresses ONLY a mood with no cybersecurity topic.
        private readonly Dictionary<string, string> _moodOnlyResponses =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "worried",
              "I can hear that you're worried, {name}, and that's completely understandable. " +
              "Cybersecurity threats are real, but knowledge is your greatest shield. " +
              "Start with strong unique passwords and enable Two-Factor Authentication. " +
              "Would you like me to guide you through any specific concern?" },

            { "scared",
              "It's okay to feel scared, {name} — the digital world can feel overwhelming. " +
              "With a few simple habits you can protect yourself very effectively. " +
              "Start small: update your software, use a password manager, and never click suspicious links. " +
              "You've already taken a great step just by being here!" },

            { "confused",
              "No worries at all, {name}! Cybersecurity can feel like a lot to take in. " +
              "Let's keep things simple — type 'topics' to see everything I can explain, " +
              "or just ask me something like 'what is phishing?' and I'll break it down in plain language!" },

            { "frustrated",
              "I hear you, {name}, and I'm sorry you're feeling frustrated. " +
              "Cybersecurity can feel complicated, but I'm here to make it easier. " +
              "Let's slow things down — tell me what's confusing you and we'll work through it together." },

            { "curious",
              "I love the curiosity, {name}! That's exactly the right mindset for staying safe online. " +
              "Curious people ask questions before clicking suspicious links — and that makes all the difference. " +
              "Type 'topics' to see everything I cover, or fire away with your questions!" },

            { "happy",
              "That's wonderful to hear, {name}! A positive mindset is great for learning. " +
              "Since you're in a good mood, why not level up your cybersecurity knowledge? " +
              "Type 'quiz' to test yourself, or ask me about any topic you're curious about!" },

            { "excited",
              "Love the energy, {name}! Let's channel that excitement into something useful — " +
              "type 'quiz' to test your cybersecurity knowledge, " +
              "or ask me about phishing, encryption, VPNs, and more!" },

            { "angry",
              "I'm sorry something has upset you, {name}. " +
              "If it's related to a cybersecurity incident, the most important thing right now is to " +
              "change your passwords immediately and enable Two-Factor Authentication. " +
              "Take a breath — let's figure this out together. What happened?" },

            { "nervous",
              "It's natural to feel nervous about online safety, {name}. " +
              "Most cyberattacks can be prevented with simple habits — " +
              "strong passwords, keeping software updated, and thinking before clicking links. " +
              "You're already doing great just by learning!" },

            { "anxious",
              "Take a breath, {name} — you're in the right place. " +
              "Anxiety about cybersecurity usually comes from uncertainty, " +
              "and the best cure is knowledge. Let's tackle one topic at a time. " +
              "What's worrying you most right now?" },

            { "unsure",
              "That's perfectly okay, {name} — nobody knows everything about cybersecurity! " +
              "That's exactly why I'm here. Type 'topics' to see all the subjects I can explain, " +
              "or just describe what you're trying to understand and I'll help." },

            { "bored",
              "Bored? Let's fix that, {name}! Type 'quiz' to test your cybersecurity knowledge — " +
              "it's actually quite challenging! Or ask me about zero-day vulnerabilities, " +
              "how ransomware works, or what a VPN really does. More fascinating than it sounds!" },

            { "overwhelmed",
              "I completely understand, {name} — there's a lot of information out there. " +
              "Let's simplify. Focus on just three things for now: " +
              "strong passwords, Two-Factor Authentication, and being careful with emails. " +
              "Master those three and you'll already be safer than most people online." },

            { "confident",
              "That's the spirit, {name}! Even the most confident people benefit from staying sharp. " +
              "Type 'quiz' to put your knowledge to the test, " +
              "or ask me about advanced topics like penetration testing or zero-day vulnerabilities!" },

            { "stressed",
              "I'm sorry you're feeling stressed, {name}. " +
              "If it's about a cybersecurity situation — don't panic. " +
              "Change your passwords, check your accounts for suspicious activity, " +
              "and enable 2FA if you haven't already. Tell me what's going on and I'll help." },
        };

        // Mood Prefixes (Dictionary: mood > short prefix for topic responses) 
        private readonly Dictionary<string, string> _moodPrefixes =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "worried",     "I understand you're worried — here's what you need to know: "    },
            { "scared",      "It's okay to feel that way. Here's some reassurance: "           },
            { "confused",    "No worries, let me break this down simply: "                     },
            { "frustrated",  "I hear you — let's work through this together: "                 },
            { "curious",     "Great question! Here's what you need to know: "                  },
            { "happy",       "Love the positive energy! Here's some info: "                    },
            { "excited",     "Awesome! Here's what you're looking for: "                       },
            { "angry",       "Let's address this calmly. Here's the information: "             },
            { "nervous",     "Take a breath — here's some helpful information: "               },
            { "anxious",     "You're in safe hands. Here's what to know: "                     },
            { "unsure",      "Let me guide you through this: "                                 },
            { "bored",       "Let's make this interesting! Here's something useful: "          },
            { "overwhelmed", "Let's keep it simple. Here's the key point: "                    },
            { "confident",   "Great mindset! Here's the detail you're after: "                 },
            { "stressed",    "Stay calm — here's exactly what you need: "                      },
        };

        // Random Response Pools (Dictionary: topic > List of responses)
        private readonly Dictionary<string, List<string>> _randomResponses =
            new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
        {
            { "phishing", new List<string>
                {
                    "Be cautious of emails asking for personal information — scammers disguise themselves as trusted organisations.",
                    "Always check the sender's email address carefully — scammers use addresses that look almost right but have small differences.",
                    "Hover over links before clicking to see the real URL. If it looks suspicious, don't click!",
                    "Phishing emails often create urgency — 'Your account will be closed!' Pause and verify before acting.",
                    "When in doubt, contact the organisation directly using a number from their official website."
                }
            },
            { "malware", new List<string>
                {
                    "Keep your antivirus updated and run regular scans to catch malware before it causes damage.",
                    "Never download software from untrusted sources — always use the official website or app store.",
                    "Malware often hides in email attachments. Don't open files from people you don't know.",
                    "Pop-ups saying 'Your computer is infected!' are often scams themselves. Close them immediately.",
                    "Enable real-time protection on your antivirus for continuous monitoring of your system."
                }
            },
            { "password", new List<string>
                {
                    "Use at least 12 characters mixing uppercase, lowercase, numbers, and symbols for strong passwords.",
                    "Never reuse the same password across multiple sites — a breach on one site shouldn't compromise others.",
                    "Consider using a passphrase: three random words combined are memorable and hard to crack.",
                    "A password manager generates and stores strong unique passwords so you only need to remember one.",
                    "Change passwords immediately if you suspect a breach or receive a suspicious activity notification."
                }
            },
            { "scam", new List<string>
                {
                    "If an offer sounds too good to be true, it almost certainly is. Trust your instincts.",
                    "Scammers often pretend to be tech support or government agencies. Legitimate organisations don't ask for gift card payments.",
                    "Never share OTPs or verification codes with anyone — not even someone claiming to be from your bank.",
                    "Romance scams are on the rise — be wary of online contacts who quickly ask for money.",
                    "Check for scam reports at websites like Scamwatch before sending money or personal information."
                }
            },
            { "privacy", new List<string>
                {
                    "Review your social media privacy settings regularly — platforms often change defaults after updates.",
                    "Limit the personal information you share publicly online. Scammers use this to craft targeted attacks.",
                    "Read privacy policies — especially for apps that access your camera, microphone, or location.",
                    "Use private browsing mode when you don't want your history stored locally.",
                    "Consider a VPN to encrypt your browsing and prevent your ISP from tracking your activity."
                }
            },
            { "ransomware", new List<string>
                {
                    "Back up your data regularly using the 3-2-1 rule: 3 copies, 2 different media, 1 offsite backup.",
                    "Never pay the ransom — it does not guarantee your files will be returned and funds criminal activity.",
                    "Disconnect from the internet immediately if you suspect ransomware to limit its spread.",
                    "Keep your OS and software updated — ransomware often exploits known, patchable vulnerabilities.",
                    "Email is the most common delivery method. Think before opening attachments or clicking links."
                }
            },
        };

        // Follow-up Phrases (List)
        private readonly List<string> _followUpPhrases = new List<string>
        {
            "tell me more", "more", "another tip", "give me another tip",
            "explain more", "go on", "continue", "what else", "keep going",
            "and then", "more info", "elaborate",
        };

        // Fallback Responses (List — rotated for variety) 
        private readonly List<string> _fallbacks;
        private int _fallbackIndex = 0;

        // Constructor 
        public GUIChatEngine(Action<string> appendBot, Action<string> appendSystem)
        {
            _appendBot = appendBot;
            _appendSystem = appendSystem;

            // Initialise fallbacks as a list for easy rotation
            _fallbacks = new List<string>
            {
                "I'm not sure about that, {name}. Try typing 'topics' to see what I can help with.",
                "Hmm, I didn't quite catch that. Could you rephrase? Type 'help' for commands.",
                "I'm not sure I understand. Can you try rephrasing? I know about passwords, phishing, malware, and more!",
                "That's outside my knowledge base. Type 'topics' to browse what I cover, {name}.",
            };
        }

        public void SetState(ChatState state) => State = state;

        // MAIN PROCESS
       

        public string Process(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "I didn't catch that. Try typing 'help' for a list of commands.";

            // State: Name collection
            if (State == ChatState.AwaitingName)
            {
                UserName = input.Trim().Length > 0 ? CapitaliseName(input.Trim()) : "User";
                State = ChatState.AwaitingFavTopic;
                return $"Nice to meet you, {UserName}! 😊\n\n" +
                       $"What is your favourite cybersecurity topic? " +
                       $"(e.g. phishing, passwords, privacy)\n" +
                       $"This helps me personalise your experience!";
            }

            // State: Favourite topic collection
            if (State == ChatState.AwaitingFavTopic)
            {
                FavouriteTopic = input.Trim();
                State = ChatState.Active;
                LogHistory(UserName, input);
                string topicResponse = GetRandomResponse(FavouriteTopic);
                string reply =
                    $"Great choice, {UserName}! I'll remember that you're interested in {FavouriteTopic}.\n\n" +
                    $"Here's something about {FavouriteTopic} to get us started:\n{topicResponse}\n\n" +
                    $"Type 'help' to see all commands, or just ask me anything!";
                LogHistory("Bot", reply);
                return reply;
            }

            // Active chat
            LogHistory(UserName, input);
            string response = ProcessActive(input);
            LogHistory("Bot", response);
            return response;
        }

        
        // ACTIVE PROCESSING

        private string ProcessActive(string input)
        {
            string lower = input.ToLower().Trim();

            // ── Topic number selection ──
            if (int.TryParse(input.Trim(), out int topicNum))
            {
                if (_topicNumberMap.TryGetValue(topicNum, out string? key))
                {
                    _lastTopic = key;
                    return ResponseBank.KeywordResponses[key];
                }
                return _topicNumberMap.Count == 0
                    ? "Type 'topics' first to see the numbered list, then enter a number."
                    : $"Please enter a number between 1 and {_topicNumberMap.Count}.";
            }

            // Commands (via dictionary of delegates) 
            if (ResponseBank.Commands.ContainsKey(lower))
                return HandleCommand(lower);

            // Follow-up phrases 
            if (_followUpPhrases.Any(p => lower.Contains(p)))
                return HandleFollowUp();

            // Detect mood
            string mood = DetectMood(lower);

            // Memory recall triggers 
            if (lower.Contains("my favourite") || lower.Contains("what do i like") ||
                lower.Contains("remember me"))
                return HandleMemoryRecall();

            // Conversational responses
            foreach (var entry in ResponseBank.ConversationalResponses)
                if (lower.Contains(entry.Key.ToLower()))
                    return entry.Value;

            // Pure mood (no topic detected)
            bool hasTopic = HasTopicKeyword(lower);
            if (!string.IsNullOrEmpty(mood) && !hasTopic)
                return GetMoodOnlyResponse(mood);

            // Mood prefix for topic responses
            string prefix = !string.IsNullOrEmpty(mood) ? GetMoodPrefix(mood) : string.Empty;

            // Random response pool (checked FIRST) 
            foreach (var topic in _randomResponses.Keys)
            {
                if (lower.Contains(topic.ToLower()))
                {
                    _lastTopic = topic;
                    string rand = GetRandomResponse(topic);
                    _lastResponse = rand;
                    return prefix + rand + GetPersonalisedSuffix(topic);
                }
            }

            // Keyword bank (checked AFTER random pool) 
            foreach (var entry in ResponseBank.KeywordResponses)
            {
                if (lower.Contains(entry.Key.ToLower()))
                {
                    _lastTopic = entry.Key;
                    _lastResponse = entry.Value;
                    return prefix + entry.Value + GetPersonalisedSuffix(entry.Key);
                }
            }

            // Fallback
            return string.IsNullOrEmpty(mood) ? GetFallback() : GetMoodOnlyResponse(mood);
        }

        // COMMAND HANDLERS (Dictionary of Delegates)

        private string HandleCommand(string command)
        {
            // Dictionary maps command strings to handler methods (Func<string> delegates)
            var handlers = new Dictionary<string, Func<string>>(StringComparer.OrdinalIgnoreCase)
            {
                { "help",    ShowHelp     },
                { "topics",  ShowTopics   },
                { "tip",     ShowNextTip  },
                { "quiz",    () => "LAUNCH_QUIZ" },
                { "history", ShowHistory  },
                { "clear",   ClearHistory },
                { "exit",    () => "EXIT" },
            };

            return handlers.TryGetValue(command, out Func<string>? handler)
                ? handler()
                : "Unknown command. Type 'help' to see available commands.";
        }

        private string ShowHelp()
        {
            var lines = new List<string> { "Here are all available commands:\n" };
            foreach (var entry in ResponseBank.Commands)
                lines.Add($"  • {entry.Key,-14} — {entry.Value}");
            lines.Add("\nYou can also just type any cybersecurity question!");
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
                lines.Add($"  {count,3}. {key,-28}");
                count++;
            }
            lines.Add("\nType a number to select a topic, or just ask a question!");
            return string.Join("\n", lines);
        }

        /// <summary>Cycles through tips in order to avoid repeats.</summary>
        private string ShowNextTip()
        {
            var tips = ResponseBank.SecurityTips.Values.ToList();
            string tip = tips[_tipIndex % tips.Count];
            _tipIndex++;
            return $"Security Tip #{_tipIndex}:\n{tip}";
        }

        private string ShowHistory()
        {
            if (_conversationHistory.Count == 0)
                return "No conversation history yet.";

            var lines = new List<string> { "─── Conversation History ───\n" };
            foreach (var entry in _conversationHistory)
                lines.Add($"  {entry.Key}: {entry.Value}");
            lines.Add("\n─── End of History ───");
            return string.Join("\n", lines);
        }

        private string ClearHistory()
        {
            _conversationHistory.Clear();
            _historyIndex = 1;
            return "Conversation history cleared.";
        }

        // FOLLOW-UP / CONVERSATION FLOW

        private string HandleFollowUp()
        {
            if (string.IsNullOrEmpty(_lastTopic))
                return $"Sure, {UserName}! What topic would you like to know more about? " +
                       $"Type 'topics' to see all options.";

            if (_randomResponses.ContainsKey(_lastTopic))
            {
                string next = GetRandomResponse(_lastTopic);
                _lastResponse = next;
                return $"Here's another tip about {_lastTopic}, {UserName}:\n\n{next}";
            }

            if (ResponseBank.KeywordResponses.TryGetValue(_lastTopic, out string? kw))
                return $"Here's more on {_lastTopic}:\n\n{kw}";

            return $"I've shared everything I know about '{_lastTopic}'. " +
                   $"Type 'topics' to explore another subject!";
        }

        // MEMORY RECALL

        private string HandleMemoryRecall()
        {
            if (string.IsNullOrEmpty(FavouriteTopic))
                return $"I know your name is {UserName}, but I don't have a favourite topic stored yet. " +
                       $"What cybersecurity topic interests you most?";

            string rand = GetRandomResponse(FavouriteTopic);
            return $"As someone interested in {FavouriteTopic}, {UserName}, " +
                   $"here's something relevant for you:\n\n{rand}\n\n" +
                   $"You might also want to review the security settings on your " +
                   $"accounts related to {FavouriteTopic}.";
        }

        // MOOD HELPERS

        /// <summary>Returns detected mood label or empty string.</summary>
        private string DetectMood(string input)
        {
            foreach (var entry in _moodKeywords)
                if (input.Contains(entry.Key.ToLower()))
                    return entry.Value;
            return string.Empty;
        }

        /// <summary>Returns full personalised mood-only response.</summary>
        private string GetMoodOnlyResponse(string mood)
        {
            if (_moodOnlyResponses.TryGetValue(mood, out string? response))
                return response.Replace("{name}", UserName);
            return $"I can sense how you're feeling, {UserName}. " +
                   $"I'm here to help — what would you like to know about cybersecurity today?";
        }

        /// <summary>Returns short mood prefix to prepend before topic info.</summary>
        private string GetMoodPrefix(string mood)
        {
            return _moodPrefixes.TryGetValue(mood, out string? prefix) ? prefix : string.Empty;
        }

        /// <summary>Checks if input contains any known topic keyword.</summary>
        private bool HasTopicKeyword(string lower)
        {
            foreach (var topic in _randomResponses.Keys)
                if (lower.Contains(topic.ToLower())) return true;
            foreach (var entry in ResponseBank.KeywordResponses)
                if (lower.Contains(entry.Key.ToLower())) return true;
            return false;
        }

        // GENERAL HELPERS

        /// <summary>Returns a random response from the pool for the given topic.</summary>
        private string GetRandomResponse(string topic)
        {
            foreach (var key in _randomResponses.Keys)
            {
                if (topic.ToLower().Contains(key.ToLower()) ||
                    key.ToLower().Contains(topic.ToLower()))
                {
                    var pool = _randomResponses[key];
                    return pool[new Random().Next(pool.Count)];
                }
            }
            foreach (var entry in ResponseBank.KeywordResponses)
                if (topic.ToLower().Contains(entry.Key.ToLower()))
                    return entry.Value;

            return $"There's a lot to know about {topic}! Type 'topics' to browse all categories.";
        }

        /// <summary>Adds personalised suffix when topic matches user's favourite.</summary>
        private string GetPersonalisedSuffix(string topic)
        {
            if (!string.IsNullOrEmpty(FavouriteTopic) &&
                topic.ToLower().Contains(FavouriteTopic.ToLower()))
                return $"\n\nSince {FavouriteTopic} is your favourite topic, {UserName}, " +
                       $"you might want to explore this further!";
            return string.Empty;
        }

        /// <summary>Rotates through fallback messages for variety.</summary>
        private string GetFallback()
        {
            string msg = _fallbacks[_fallbackIndex % _fallbacks.Count]
                         .Replace("{name}", UserName);
            _fallbackIndex++;
            return msg;
        }

        private void LogHistory(string speaker, string message)
        {
            _conversationHistory[$"[{_historyIndex++}] {speaker}"] = message;
        }

        private static string CapitaliseName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return name;
            return char.ToUpper(name[0]) + name.Substring(1).ToLower();
        }
    }
}
