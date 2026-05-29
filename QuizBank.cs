namespace CybersecurityChatbot.Data
{
    /// <summary>
    /// Stores all quiz questions grouped into 3 rounds of 5 questions each.
    /// Uses Dictionary for clean, optimised data access.
    /// </summary>
    public static class QuizBank
    {
        // Round 1: Cybersecurity Basics
        public static readonly Dictionary<int, QuizQuestion> Round1 =
            new Dictionary<int, QuizQuestion>
        {
            { 1, new QuizQuestion
                {
                    Question = "What does HTTPS stand for?",
                    Options  = new Dictionary<string, string>
                    {
                        { "A", "HyperText Transfer Protocol Secure"      },
                        { "B", "High Transfer Text Protocol System"       },
                        { "C", "Hyperlink Text Transmission Protocol"     },
                        { "D", "HyperText Transmission Privacy Standard" },
                    },
                    CorrectAnswer = "A",
                    Explanation   = "HTTPS encrypts data between your browser and the server using SSL/TLS."
                }
            },
            { 2, new QuizQuestion
                {
                    Question = "Which of the following is a sign of a phishing email?",
                    Options  = new Dictionary<string, string>
                    {
                        { "A", "It comes from a known contact"                        },
                        { "B", "It asks you to verify your password urgently"         },
                        { "C", "It has a personalised greeting with your full name"   },
                        { "D", "It contains a company logo"                           },
                    },
                    CorrectAnswer = "B",
                    Explanation   = "Urgency and requests for credentials are classic phishing red flags."
                }
            },
            { 3, new QuizQuestion
                {
                    Question = "What is the purpose of Two-Factor Authentication (2FA)?",
                    Options  = new Dictionary<string, string>
                    {
                        { "A", "To create two different passwords"                    },
                        { "B", "To speed up the login process"                        },
                        { "C", "To add an extra verification step beyond a password"  },
                        { "D", "To store passwords securely"                          },
                    },
                    CorrectAnswer = "C",
                    Explanation   = "2FA requires a second form of verification, making account takeover much harder."
                }
            },
            { 4, new QuizQuestion
                {
                    Question = "What should you do if you receive a ransomware attack?",
                    Options  = new Dictionary<string, string>
                    {
                        { "A", "Pay the ransom immediately"                   },
                        { "B", "Disconnect from the network and report it"    },
                        { "C", "Delete all your files"                        },
                        { "D", "Restart your computer repeatedly"             },
                    },
                    CorrectAnswer = "B",
                    Explanation   = "Disconnecting limits the spread. Paying the ransom does not guarantee file recovery."
                }
            },
            { 5, new QuizQuestion
                {
                    Question = "Which password is the strongest?",
                    Options  = new Dictionary<string, string>
                    {
                        { "A", "password123"   },
                        { "B", "John1990"      },
                        { "C", "P@ssw0rd"      },
                        { "D", "X7!mQ#92vLpW"  },
                    },
                    CorrectAnswer = "D",
                    Explanation   = "'X7!mQ#92vLpW' is long, random, and mixes character types — very high entropy."
                }
            },
        };

        // Round 2: Threats & Attacks
        public static readonly Dictionary<int, QuizQuestion> Round2 =
            new Dictionary<int, QuizQuestion>
        {
            { 1, new QuizQuestion
                {
                    Question = "What is a VPN primarily used for?",
                    Options  = new Dictionary<string, string>
                    {
                        { "A", "Speeding up your internet connection"                           },
                        { "B", "Encrypting your internet traffic and hiding your IP address"    },
                        { "C", "Blocking ads on websites"                                       },
                        { "D", "Scanning for viruses on your device"                            },
                    },
                    CorrectAnswer = "B",
                    Explanation   = "A VPN encrypts your connection, protecting your activity from hackers and ISPs."
                }
            },
            { 2, new QuizQuestion
                {
                    Question = "What does malware stand for?",
                    Options  = new Dictionary<string, string>
                    {
                        { "A", "Malicious Software"          },
                        { "B", "Managed Learning Ware"       },
                        { "C", "Multiple Application Ware"   },
                        { "D", "Manual Learning Software"    },
                    },
                    CorrectAnswer = "A",
                    Explanation   = "Malware is short for malicious software — any software designed to harm your system."
                }
            },
            { 3, new QuizQuestion
                {
                    Question = "Which of the following is NOT a type of social engineering attack?",
                    Options  = new Dictionary<string, string>
                    {
                        { "A", "Phishing"    },
                        { "B", "Pretexting"  },
                        { "C", "Encryption"  },
                        { "D", "Baiting"     },
                    },
                    CorrectAnswer = "C",
                    Explanation   = "Encryption is a security tool, not an attack. Phishing, pretexting, and baiting are social engineering tactics."
                }
            },
            { 4, new QuizQuestion
                {
                    Question = "What is a firewall?",
                    Options  = new Dictionary<string, string>
                    {
                        { "A", "A physical wall that protects servers"                                       },
                        { "B", "Software that speeds up internet connections"                                },
                        { "C", "A system that monitors and controls network traffic based on security rules" },
                        { "D", "A tool used to crack passwords"                                              },
                    },
                    CorrectAnswer = "C",
                    Explanation   = "A firewall filters incoming and outgoing traffic to block unauthorised access."
                }
            },
            { 5, new QuizQuestion
                {
                    Question = "What is the 3-2-1 backup rule?",
                    Options  = new Dictionary<string, string>
                    {
                        { "A", "3 passwords, 2 devices, 1 cloud account"                      },
                        { "B", "3 copies of data, 2 different media types, 1 offsite backup"  },
                        { "C", "Back up every 3 days, 2 times a day, 1 hour each"             },
                        { "D", "3 users, 2 admins, 1 backup manager"                          },
                    },
                    CorrectAnswer = "B",
                    Explanation   = "The 3-2-1 rule ensures your data survives hardware failure, ransomware, or disasters."
                }
            },
        };

        // Round 3: Privacy & Best Practices 
        public static readonly Dictionary<int, QuizQuestion> Round3 =
            new Dictionary<int, QuizQuestion>
        {
            { 1, new QuizQuestion
                {
                    Question = "What does SQL Injection allow an attacker to do?",
                    Options  = new Dictionary<string, string>
                    {
                        { "A", "Steal Wi-Fi passwords"                                                },
                        { "B", "Insert malicious code into database queries to manipulate data"       },
                        { "C", "Slow down a website with traffic"                                     },
                        { "D", "Encrypt files on a server"                                            },
                    },
                    CorrectAnswer = "B",
                    Explanation   = "SQL Injection exploits poorly coded input fields to manipulate or expose database content."
                }
            },
            { 2, new QuizQuestion
                {
                    Question = "What is a zero-day vulnerability?",
                    Options  = new Dictionary<string, string>
                    {
                        { "A", "A vulnerability that has been patched for zero days"      },
                        { "B", "A flaw unknown to the software vendor with no available patch" },
                        { "C", "A virus that activates at midnight"                        },
                        { "D", "A security feature that expires after one day"             },
                    },
                    CorrectAnswer = "B",
                    Explanation   = "Zero-day flaws are unknown to vendors, making them extremely dangerous until patched."
                }
            },
            { 3, new QuizQuestion
                {
                    Question = "Which is the safest way to connect to public Wi-Fi?",
                    Options  = new Dictionary<string, string>
                    {
                        { "A", "Use it freely — public Wi-Fi is always safe"    },
                        { "B", "Only visit HTTP websites"                        },
                        { "C", "Use a VPN to encrypt your connection"            },
                        { "D", "Share your hotspot instead"                      },
                    },
                    CorrectAnswer = "C",
                    Explanation   = "A VPN encrypts your traffic on public Wi-Fi, preventing eavesdropping by hackers."
                }
            },
            { 4, new QuizQuestion
                {
                    Question = "What does the principle of least privilege mean?",
                    Options  = new Dictionary<string, string>
                    {
                        { "A", "Giving all users admin access for convenience"                    },
                        { "B", "Users only receive the minimum access needed for their job"       },
                        { "C", "Limiting internet access to management only"                      },
                        { "D", "Deleting unused accounts every month"                             },
                    },
                    CorrectAnswer = "B",
                    Explanation   = "Least privilege limits damage if an account is compromised by restricting unnecessary access."
                }
            },
            { 5, new QuizQuestion
                {
                    Question = "What should you do FIRST if you suspect your account has been hacked?",
                    Options  = new Dictionary<string, string>
                    {
                        { "A", "Wait and see if anything happens"                         },
                        { "B", "Delete your account immediately"                           },
                        { "C", "Change your password and enable 2FA immediately"           },
                        { "D", "Post about it on social media"                             },
                    },
                    CorrectAnswer = "C",
                    Explanation   = "Changing your password and enabling 2FA immediately cuts off the attacker's access."
                }
            },
        };

        // All Rounds as a List for iteration
        public static readonly List<Dictionary<int, QuizQuestion>> AllRounds =
            new List<Dictionary<int, QuizQuestion>> { Round1, Round2, Round3 };

        // Round Titles Dictionary
        public static readonly List<string> RoundTitles = new List<string>
        {
            "Round 1 — Cybersecurity Basics",
            "Round 2 — Threats & Attacks",
            "Round 3 — Privacy & Best Practices",
        };
    }

    /// <summary>Represents a single quiz question with options and the correct answer.</summary>
    public class QuizQuestion
    {
        public string Question { get; set; } = string.Empty;
        public Dictionary<string, string> Options { get; set; } = new();
        public string CorrectAnswer { get; set; } = string.Empty;
        public string Explanation { get; set; } = string.Empty;
    }
}
