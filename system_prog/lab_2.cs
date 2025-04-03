using System;



public class Lab2
{
    public static void Lab_2()
    {
        HANDLE file_lab_2 = CreateFile("lab_2.txt", GENERIC_READ | GENERIC_WRITE, 0, null, OPEN_EXISTING, 0, 0);
        Console.WriteLine("descriptor: {0}", file_lab_2);
    }

}