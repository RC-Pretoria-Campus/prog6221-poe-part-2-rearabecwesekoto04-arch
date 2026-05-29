using System;
using System.Windows.Forms;
using CybersecurityChatbot.GUI;

namespace CybersecurityChatbot
{
    /// <summary>
    /// Entry point for Part 2 — launches the WinForms GUI chatbot.
    /// </summary>
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ChatForm());
        }
    }
}