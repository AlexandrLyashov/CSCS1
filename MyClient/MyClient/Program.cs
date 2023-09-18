﻿using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.IO;
namespace CSCS1
{
    class SendProgram
    {
        static void Main(string[] args)
        {
            SendMessage();
        }
        private static void SendMessage()
        {
            string remoteAddress = "127.0.0.1";
            int port = 1001;
            Commands commands = new Commands();
            UdpClient sender = new UdpClient(0);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(remoteAddress),
            port);
            Int16 x0, y0;
            Int16 x1, y1;
            Int16 radius;
            string text;
            string hexcolor;
            try
            {
                Console.WriteLine("Type 'help' or '?' for command list");
                while (true)
                {
                    Console.Write("Enter command > ");
                    string commandText = Console.ReadLine();
                    byte[] commandbyte = new byte[1];
                    byte[] result = new byte[1] { 0 };
                    switch (commandText)
                    {
                        case "1":
                        case "clear display":
                            commandbyte[0] = 1;
                            hexcolor = ReadHexColor();
                            result = commands.ClearDisplayEncode(commandbyte[0],
                            hexcolor);
                            sender.Send(result, result.Length, endPoint);
                            break;
                        case "2":
                        case "draw pixel":
                            commandbyte[0] = 2;
                            x0 = ReadNumber("x", false);
                            y0 = ReadNumber("y", false);
                            hexcolor = ReadHexColor();
                            result = commands.PixelEncode(commandbyte[0], x0, y0,
                            hexcolor);
                            sender.Send(result, result.Length, endPoint);
                            break;
                        case "3":
                        case "draw line":
                            commandbyte[0] = 3;
                            x0 = ReadNumber("x0", false);
                            y0 = ReadNumber("y0", false);
                            x1 = ReadNumber("x1", false);
                            y1 = ReadNumber("y1", false);
                            hexcolor = ReadHexColor();
                            result = commands.FourNumbersEncode(commandbyte[0],
                            x0, y0, x1, y1, hexcolor);
                            sender.Send(result, result.Length, endPoint);
                            break;
                        case "4":
                        case "draw circle":
                            commandbyte[0] = 8;
                            x0 = ReadNumber("x", false);
                            y0 = ReadNumber("y", false);
                            radius = ReadNumber("radius", true);
                            hexcolor = ReadHexColor();
                            result = commands.CircleEncode(commandbyte[0], x0, y0,
                            radius, hexcolor);
                            sender.Send(result, result.Length, endPoint);
                            break;
                        case "5":
                        case "fill circle":
                            commandbyte[0] = 9;
                            x0 = ReadNumber("x", false);
                            y0 = ReadNumber("y", false);
                            radius = ReadNumber("radius", true);
                            hexcolor = ReadHexColor();
                            result = commands.CircleEncode(commandbyte[0], x0, y0,
                            radius, hexcolor);
                            sender.Send(result, result.Length, endPoint);
                            break;
                        case "6":
                        case "draw text":
                            commandbyte[0] = 12;
                            x0 = ReadNumber("x", false);
                            y0 = ReadNumber("y", false);
                            hexcolor = ReadHexColor();
                            x1 = ReadNumber("font number", true);
                            Console.Write("Enter text > ");
                            text = Console.ReadLine();
                            y1 = Convert.ToInt16(text.Length);
                            result = commands.TextEncode(commandbyte[0], x0, y0,
                            hexcolor, x1, y1, text);
                            sender.Send(result, result.Length, endPoint);
                            break;
                        case "7":
                        case "draw image":
                            commandbyte[0] = 13;
                            x0 = ReadNumber("x", false);
                            y0 = ReadNumber("y", false);
                            x1 = ReadNumber("width", true);
                            y1 = ReadNumber("height", true);
                            text = ReadPath(); result = commands.ImageEncode(commandbyte[0], x0, y0,
                             x1, y1, text);
                            sender.Send(result, result.Length, endPoint);
                            break;
                        case "help":
                        case "?":
                            Console.WriteLine("\nCommands:");
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine(" 1. clear display");
                            Console.WriteLine(" 2. draw pixel");
                            Console.WriteLine(" 3. draw line");
                            Console.WriteLine(" 4. draw circle");
                            Console.WriteLine(" 5. fill circle");
                            Console.WriteLine(" 6. draw text");
                            Console.WriteLine(" 7. draw image");
                            Console.ResetColor();
                            break;
                        default:
                            Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("Error! Unknown operation! Tryagain.");
                             Console.ResetColor();
                            break;
                    }
                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
            finally
            {
                sender.Close();
            }
        }
        public static bool IsStringInHex(string text)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(text, @"\A\b[0-9afA-F]+\b\Z");
        }
        private static string ReadHexColor()
        {
            string str;
            while (true)
            {
                Console.Write("Enter RGB565 color > ");
                str = Console.ReadLine();
                if (IsStringInHex(str) && str.Length <= 4)
                {
                    break;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error! Data is not hexadecimal! Try again.");
                    Console.ResetColor();
                }
            }
            return str;
        }
        private static Int16 ReadNumber(string text, bool onlyPositive = false)
        {
            string str;
            Int16 number;
            while (true)
            {
                Console.Write($"Enter {text} > ");
                str = Console.ReadLine();
                try
                {
                    number = Int16.Parse(str);
                    if (onlyPositive)
                    {
                        if (number < 0)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Error! Bad data! (range 0 to 32767) Try again.");Console.ResetColor();
                        }
                        else { break; }
                    }
                    else { break; }
                }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error! Bad data! (range -32768 to 32767) Try again.");
                    Console.ResetColor();
                }
            }
            return Convert.ToInt16(str);
        }
        private static string ReadPath()
        {
            string str;
            while (true)
            {
                Console.Write("Enter path > ");
                str = Console.ReadLine();
                if (File.Exists(str) && IsImage(str))
                {
                    break;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error! File does not exist! Try again.");
                    Console.ResetColor();
                }
            }
            return @"" + str;
        }
        public static bool IsImage(string path)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(path,
            @"^.*\.(jpg|JPG|gif|GIF|png|PNG)$");
        }
        public static void RecieveMessage(UdpClient sender, IPEndPoint endPoint)
        {
            byte[] data = sender.Receive(ref endPoint);
            Console.WriteLine($"Recieved value: {BitConverter.ToInt16(data, 0)}");
        }
    }
}