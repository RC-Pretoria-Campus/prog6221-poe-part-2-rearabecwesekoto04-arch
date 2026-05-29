using System.Speech.Synthesis;

namespace PROG_Part1
{
    // Handles all console display formatting, ASCII art, colours,
    // and visual structure for the Cybersecurity Awareness Chatbot.
    public static class DisplayHelper
    {
        // Colour palette used throughout the UI
        private static readonly ConsoleColor ColourAccent = ConsoleColor.Cyan;
        private static readonly ConsoleColor ColourBot = ConsoleColor.Yellow;
        private static readonly ConsoleColor ColourSuccess = ConsoleColor.Green;
        private static readonly ConsoleColor ColourWarning = ConsoleColor.Red;
        private static readonly ConsoleColor ColourSubtle = ConsoleColor.DarkGray;
        private static readonly ConsoleColor ColourTitle = ConsoleColor.White;

        // Displays the animated ASCII art banner on startup.
        
        public static void ShowBanner()
        {
            Console.Clear();

            Write("  ", ColourSubtle);
            WriteLine("===========================================================================", ColourSubtle);

            string[] logo = new string[]
            {
                @"    ██████╗██╗   ██╗██████╗ ███████╗██████╗ ██████╗  ██████╗ ████████╗",
                @"   ██╔════╝╚██╗ ██╔╝██╔══██╗██╔════╝██╔══██╗██╔══██╗██╔═══██╗╚══██╔══╝",
                @"   ██║      ╚████╔╝ ██████╔╝█████╗  ██████╔╝██████╔╝██║   ██║   ██║   ",
                @"   ██║       ╚██╔╝  ██╔══██╗██╔══╝  ██╔══██╗██╔══██╗██║   ██║   ██║   ",
                @"   ╚██████╗   ██║   ██████╔╝███████╗██║  ██║██████╔╝╚██████╔╝   ██║   ",
                @"    ╚═════╝   ╚═╝   ╚═════╝ ╚══════╝╚═╝  ╚═╝╚═════╝  ╚═════╝   ╚═╝   ",
            };

            foreach (string line in logo)
            {
                Write("  ", ColourSubtle);
                WriteLine(line, ColourAccent);
                Thread.Sleep(60);
            }

            Console.WriteLine();
            WriteLine("CYBERSECURITY AWARENESS CHATBOT", ColourTitle);
            Console.WriteLine();

            Write("  ", ColourSubtle);
            WriteLine("===========================================================================", ColourSubtle);
            Console.WriteLine();

            PlayVoiceGreeting();
        }

        // Simulates a voice greeting using a typewriter text effect.

        private static void PlayVoiceGreeting()
        {
            string greeting = "Welcome! I am your Cybersecurity Awareness Assistant.";
            string subline = "I am here to help you stay safe in the digital world.";

            try
            {
                using (var synthesizer = new SpeechSynthesizer())
                {
                    synthesizer.Rate = 0;
                    synthesizer.Volume = 100;
                    synthesizer.Speak(greeting);
                    Thread.Sleep(250);
                    synthesizer.Speak(subline);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Voice synthesis unavailable. Using text fallback.");
                TypewriterEffect(greeting, ColourTitle, 18);
                Console.WriteLine();
                TypewriterEffect(subline, ColourSubtle, 14);
            }

            Console.WriteLine("Welcome! I am your Cybersecurity Awareness Assistant.");
            Console.WriteLine("I am here to help you stay safe in the digital world.");
        }

        // Prints a typewriter character-by-character effect for a given message.

        private static void TypewriterEffect(string message, ConsoleColor colour, int delayMs = 20)
        {
            Console.ForegroundColor = colour;
            foreach (char c in message)
            {
                Console.Write(c);
                Thread.Sleep(delayMs);
            }
            Console.ResetColor();
        }

        // Prints a bot message with word-wrap for long single-line responses.
        
        public static void PrintBot(string message)
        {
            Console.WriteLine();
            Write("Bot: ", ColourBot);
            Console.ResetColor();

            string[] words = message.Split(' ');
            int lineLength = 0;
            int maxLineWidth = 70;

            foreach (string word in words)
            {
                if (lineLength + word.Length + 1 > maxLineWidth)
                {
                    Console.WriteLine();
                    Console.Write("       ");
                    lineLength = 7;
                }
                Console.Write(word + " ");
                lineLength += word.Length + 1;
            }
            Console.WriteLine();
        }

        // Prints a bot message that contains a list or multiple lines.
        // Skips word-wrap so numbered lists and topics display correctly.
        
        public static void PrintBotList(string message)
        {
            Console.WriteLine();
            Write("Bot: ", ColourBot);
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        // Prints a success message in green.

        public static void PrintSuccess(string message)
        {
            WriteLine(message, ColourSuccess);
        }

        // Prints a warning or error message in red.
        public static void PrintWarning(string message)
        {
            WriteLine(message, ColourWarning);
        }


        // Internal helpers

        private static void Write(string text, ConsoleColor colour)
        {
            Console.ForegroundColor = colour;
            Console.Write(text);
            Console.ResetColor();
        }

        private static void WriteLine(string text, ConsoleColor colour)
        {
            Console.ForegroundColor = colour;
            Console.WriteLine(text);
            Console.ResetColor();
        }
    }
}