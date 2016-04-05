using System;
using System.Collections.Generic;
using System.Text;
using SubstringFramework;
using SubstringFramework.Enums;
using SubstringFramework.Views;

namespace SubstringClient
{
    public static class Program
    {
        private static bool _isRunning = true;
        private const int MaxStringSize = 512;


        public static void Main(string[] args)
        {
            ExecuteMainLoop();
        }

        private static void ExecuteMainLoop()
        {
            Console.WriteLine("SubstringSearch Verion 1.0 (April 2016)");
            Console.WriteLine("Type 'help' for a listing of commands.");

            while (_isRunning)
            {
                var readLine = Console.ReadLine();
                if (!string.IsNullOrEmpty(readLine))
                {
                    var args = ParseCommand(readLine);

                    if (args.Count > 0)
                    {
                        switch (args[0])
                        {
                            case "textsearch":
                                HandleTextSearch(args);
                                break;
                            case "regexsearch":
                                HandleRegexSearch(args);
                                break;
                            case "help":
                                HandlePrintHelp(args);
                                break;
                            case "quit":
                                HandleQuit(args);
                                break;
                            default:
                                Console.WriteLine("Invalid command.");
                                break;
                        }
                    }
                }
            }
        }

        #region MESSY_STRING_PARSING

        private static void HandleTextSearch(List<string> args)
        {
            string path;
            string pattern;
            bool useBlocks;


            if (args.Count > 1)
            {
                if (args[1].Length <= MaxStringSize)
                {
                    path = args[1];
                }
                else
                {
                    Console.WriteLine("The path was too long. Please limit your search to 512 characters.");
                    return;
                }
            }
            else
            {
                Console.WriteLine("You must provide a path for the search");
                return;
            }

            if (args.Count > 2)
            {
                if (args[2].Length <= MaxStringSize)
                {
                    pattern = args[2];
                }
                else
                {
                    Console.WriteLine("The pattern was too long. Please limit your search to 512 characters.");
                    return;
                }
            }
            else
            {
                Console.WriteLine("Your must provide a pattern for your search.");
                return;
            }

            if (args.Count > 3)
            {
                var useBlocksString = args[3];
                useBlocks = useBlocksString != "false";
            }
            else
            {
                useBlocks = true;
            }

            var search = new PlainTextSearch(useBlocks, path, pattern);
            var msg = new OutgoingPacket(OpCode.PlainTextSearch, search);
            RequestManager.CreateRequest(msg);
        }

        private static void HandleRegexSearch(List<string> args)
        {
            string path;
            string pattern;

            if (args.Count > 1)
            {
                if (args[1].Length <= MaxStringSize)
                {
                    path = args[1];
                }
                else
                {
                    Console.WriteLine("The path was too long. Please limit your search to 512 characters.");
                    return;
                }
            }
            else
            {
                Console.WriteLine("You must provide a path for the search");
                return;
            }

            if (args.Count > 2)
            {
                if (args[2].Length <= MaxStringSize)
                {
                    pattern = args[2];
                }
                else
                {
                    Console.WriteLine("The pattern was too long. Please limit your search to 512 characters.");
                    return;
                }
            }
            else
            {
                Console.WriteLine("Your must provide a pattern for your search.");
                return;
            }

            var search = new RegexSearch(path, pattern);
            var msg = new OutgoingPacket(OpCode.RegexSearch, search);
            RequestManager.CreateRequest(msg);
        }

        private static void HandlePrintHelp(List<string> args)
        {
            Console.WriteLine("COMMAND LISTING");
            Console.WriteLine("textsearch filePath pattern useBlocks");
            Console.WriteLine("     filePath: Path of the file to be searched");
            Console.WriteLine("     pattern: Pattern to search for");
            Console.WriteLine("     useBlocks: Search by blocks instead of by lines (Default value: true)(Faster)");
            Console.WriteLine("regexsearch filePath pattern");
            Console.WriteLine("     filePath: Path of the file to be searched");
            Console.WriteLine("     pattern: Pattern to match");
            Console.WriteLine("quit");
        }

        private static void HandleQuit(List<string> args)
        {
            _isRunning = false;
        }

        private static List<string> ParseCommand(string text)
        {
            text = text.Trim();
            var components = new List<string>();

            if (!string.IsNullOrEmpty(text))
            {
                var builder = new StringBuilder();

                int pos = 0;
                while (pos < text.Length)
                {
                    if (text[pos] == '\'')
                    {
                        while (++pos < text.Length && text[pos] != '\'')
                        {
                            builder.Append(text[pos]);
                        }
                        if (builder.Length > 0)
                        {
                            components.Add(builder.ToString());
                            builder.Clear();
                        }
                        ++pos;
                    }
                    else if (text[pos] != ' ')
                    {
                        builder.Append(text[pos]);
                        ++pos;
                    }
                    else
                    {
                        if (builder.Length > 0)
                        {
                            components.Add(builder.ToString());
                            builder.Clear();
                        }
                        while (++pos < text.Length && text[pos] == ' ') ;
                    }
                }

                if (builder.Length > 0)
                {
                    components.Add(builder.ToString());
                }
            }
            else
            {
                Console.WriteLine("Command cannot be empty");
            }
            return components;
        }
        #endregion
    }
}
