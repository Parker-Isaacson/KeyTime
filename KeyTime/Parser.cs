using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using Timeline;

namespace KeyTime
{
    public class ParseException : Exception
    {
        public ParseException() : base() { }
        public ParseException(string msg) : base(msg) { }
        public ParseException(string msg, Exception inner) : base(msg, inner) { }
    }

    public class Parser
    {
        public Dictionary<String, Action> macros { get; } = new Dictionary<String, Action>();
        private Dictionary<String, String> vars = new Dictionary<String, String>();
        private Stack<String> tokens = new Stack<String>();
        private HashSet<byte> pressed = new HashSet<byte>();
        private String currentMacro = String.Empty;

        public Parser(Data formData)
        {
            String? text = formData.GetFileText();
            if (String.IsNullOrEmpty(text))
            {
                throw new ParseException("Error, no text passed in.");
            }
            ParseText(text);
        }

        private void ParseText(String text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                CheckTokens();
                // Trim
                if (Char.IsWhiteSpace(text[i]))
                {
                    continue;
                }

                // String Constant, in [a-zA-z\_\-\:]
                if (Char.IsLetter(text[i]) || text[i] == '_' || text[i] == '-' || text[i] == ':' || text[i] == '$')
                {
                    String buffer = String.Empty;
                    while (i < text.Length && (Char.IsLetter(text[i]) || text[i] == '_' || text[i] == '-' || text[i] == ':' || text[i] == '$'))
                    {
                        buffer += text[i];
                        i++;
                    }
                    i--;
                    tokens.Push(buffer.ToLower());
                    continue;
                }

                // Numeric Constant, in [0-9\.]
                if (Char.IsDigit(text[i]))
                {
                    String buffer = String.Empty;
                    while (i < text.Length && Char.IsDigit(text[i]))
                    {
                        buffer += text[i];
                        i++;
                    }
                    i--;
                    tokens.Push(buffer.ToLower());
                    continue;
                }

                // Consume Comments
                if (text[i] == ';')
                {
                    while (i < text.Length && text[i] != '\n')
                    {
                        i++;
                    }
                    continue;
                }

                // User gave something bad
                throw new ParseException(@$"Error, bad char \'{text[i]}\'.");
            }
            CheckTokens();
            CleanKeys();
            if (tokens.Count != 0)
            {
                throw new ParseException($"Error, ended with extra tokens.");
            }
        }

        private void CheckTokens()
        {
            // We can ignore, assume there will be more later!
            if (tokens.Count < 2)
            {
                return;
            }

            // We found too many valid tokens
            if (tokens.Count > 2)
            {
                throw new ParseException($"Error, to many tokens in the stream.");
            }

            // We found just enough valid tokens
            if (tokens.Count == 2)
            {
                String param = tokens.Pop().ToLower();
                String op = tokens.Pop().ToLower();
                
                // We need to do a var check on op and change it just in case
                if (op.EndsWith(':'))
                {
                    vars[op.Substring(0, op.Length - 1)] = param;
                    return;
                }

                if (param.StartsWith('$'))
                {
                    param = vars[param.Substring(1)];
                }

                switch (op)
                {
                    case "press":
                        foreach (var c in param)
                        {
                            byte key = Keyboard.CharToVirtualKey(c);
                            pressed.Add(key);
                            macros[currentMacro] += () => {
                                Debug.WriteLine($"Presing {c}");
                                Keyboard.PressKey(key);
                            };
                        }
                        break;
                    case "unpress":
                        foreach (var c in param)
                        {
                            byte key = Keyboard.CharToVirtualKey(c);
                            pressed.Remove(key);
                            macros[currentMacro] += () => {
                                Debug.WriteLine($"Unpressing {c}");
                                Keyboard.ReleaseKey(key);
                            };
                        }
                        break;
                    case "tap":
                        foreach (var c in param)
                        {
                            byte key = Keyboard.CharToVirtualKey(c);
                            pressed.Remove(key);
                            macros[currentMacro] += () => {
                                Debug.WriteLine($"Tapping {c}");
                                Keyboard.TapKey(key);
                            };
                        }
                        break;
                    case "wait":
                    case "sleep":
                        if ( int.TryParse(param, out int time) )
                        {
                            macros[currentMacro] += () => { PercisionTimer.AccurateDelay(time); };
                        }
                        else
                        {
                            throw new ParseException($"Error, {param} cannot be cast to Int.");
                        }
                        break;
                    case "macro":
                        CleanKeys();
                        if ( macros.Keys.Contains(param) )
                        {
                            throw new ParseException($"Error, {param} was already used as a macro.");
                        }
                        currentMacro = param;
                        macros[currentMacro] = () => { }; // Initialize action with nothing
                        break;
                    default:
                        throw new ParseException($"Error, bad token {op} was found.");
                }
            }
        }

        private void CleanKeys()
        {
            foreach (byte key in pressed)
            {
                macros[currentMacro] += () => {
                    Debug.WriteLine($"Unhandled key: {key}, automatically unpressing.");
                    Keyboard.ReleaseKey(key);
                };
            }
        }
    }

    public class TimelineParser
    {
        public List<TimelineControl.TimelineData> timelines { get; } = new();
        private Dictionary<String, String> vars = new Dictionary<String, String>();
        private Stack<String> tokens = new Stack<String>();

        private int currentTrack = -1;
        private int currentClip = -1;
        private int currentTime = 0;

        public TimelineParser(Data formData)
        {
            String? text = formData.GetFileText();
            if (String.IsNullOrEmpty(text))
            {
                throw new ParseException("Error, no text passed in.");
            }
            ParseText(text);
        }

        private void ParseText(String text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                CheckTokens();
                // Trim
                if (Char.IsWhiteSpace(text[i]))
                {
                    continue;
                }

                // String Constant, in [a-zA-z\_\-\:]
                if (Char.IsLetter(text[i]) || text[i] == '_' || text[i] == '-' || text[i] == ':' || text[i] == '$')
                {
                    String buffer = String.Empty;
                    while (i < text.Length && (Char.IsLetter(text[i]) || text[i] == '_' || text[i] == '-' || text[i] == ':' || text[i] == '$'))
                    {
                        buffer += text[i];
                        i++;
                    }
                    i--;
                    tokens.Push(buffer.ToLower());
                    continue;
                }

                // Numeric Constant, in [0-9\.]
                if (Char.IsDigit(text[i]))
                {
                    String buffer = String.Empty;
                    while (i < text.Length && Char.IsDigit(text[i]))
                    {
                        buffer += text[i];
                        i++;
                    }
                    i--;
                    tokens.Push(buffer.ToLower());
                    continue;
                }

                // Consume Comments
                if (text[i] == ';')
                {
                    while (i < text.Length && text[i] != '\n')
                    {
                        i++;
                    }
                    continue;
                }

                // User gave something bad
                throw new ParseException(@$"Error, bad char \'{text[i]}\'.");
            }
            CheckTokens();

            if (tokens.Count != 0)
            {
                throw new ParseException($"Error, ended with extra tokens.");
            }
        }

        private void CheckTokens()
        {
            // We can ignore, assume there will be more later!
            if (tokens.Count < 2)
            {
                return;
            }

            // We found too many valid tokens
            if (tokens.Count > 2)
            {
                throw new ParseException($"Error, to many tokens in the stream.");
            }

            // We found just enough valid tokens
            if (tokens.Count == 2)
            {
                String param = tokens.Pop().ToLower();
                String op = tokens.Pop().ToLower();

                // We need to do a var check on op and change it just in case
                if (op.EndsWith(':'))
                {
                    vars[op.Substring(0, op.Length - 1)] = param;
                    return;
                }

                if (param.StartsWith('$'))
                {
                    param = vars[param.Substring(1)];
                }

                switch (op)
                {
                    /*
                     * Since the macros must be `press` -> `sleep` -> `unpress`, we must assume that they exist
                     */
                    case "press":
                        timelines[currentTrack].Clips.Add(new TimelineControl.ClipData
                        {
                            Character = param,
                            StartTime = currentTime,
                            EndTime = currentTime // Default as needed
                        });
                        currentClip += 1;
                        break;
                    case "unpress":
                        timelines[currentTrack].Clips[currentClip].EndTime = currentTime;
                        break;
                    case "tap":
                        timelines[currentTrack].Clips.Add(new TimelineControl.ClipData
                        {
                            Character = param,
                            StartTime = currentTime,
                            EndTime = currentTime + 50
                        });
                        currentClip += 1;
                        currentTime += 50;
                        break;
                    case "wait":
                    case "sleep":
                        if (int.TryParse(param, out int time))
                        {
                            currentTime += time;
                        }
                        else
                        {
                            throw new ParseException($"Error, {param} cannot be cast to Int.");
                        }
                        break;
                    case "macro":
                        timelines.Add(new TimelineControl.TimelineData { });
                        currentTrack += 1;
                        currentClip = -1;
                        currentTime = 0;
                        break;
                    default:
                        throw new ParseException($"Error, bad token {op} was found.");
                }
            }
        }
    }
}
