using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Regex r = new Regex(@"\[.*?\]");
            string d = "我迷你[表情]版的，你的小宝贝突然出啦";
            string result = r.Replace(d,"b");
            Console.WriteLine(result);
            

            for(int i = 0; i < 3; i++)
            {
                Random random = new Random();
                int a =random.Next(1, 10);
                Console.WriteLine(a);
            }
            Console.ReadLine();
        }
    }
}
