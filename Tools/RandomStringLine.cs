using System;

namespace SysPayServer.Tools
{
    public class RandomStringLine
    {
        public RandomStringLine(int digits)
        {
            _digits = digits;
            Run();
        }


        public string Value
        {
            get
            {
                return _word;
            }
        }


        private int _digits = 0;
        private string _word = "";

        private void Run()
        {
            string _wordLine = "abcdefhijkmnprstwxyz2345678";
            int _wordNumLinue = _wordLine.Length;
            Random _random = new Random();
            for (int i = 0; i < _digits; i++)
            {
                char[] _arrChar = _wordLine.ToCharArray();
                _word += _arrChar[_random.Next(1, _wordNumLinue)];
            }
        }
    }
}
