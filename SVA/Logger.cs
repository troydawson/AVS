using System;

public class Logger
{
    public Logger()
    {
    }

    internal static void msg(string format, params object[] args)
    {
        Console.WriteLine(String.Format(format, args));
    }
}
