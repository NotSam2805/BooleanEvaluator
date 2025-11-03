using System;
using System.Collections.Generic;
using System.Text;

namespace BooleanEvaluator
{
    public class Input
    {
        private char name;
        public bool state;

        public Input(char name_)
        {
            name = name_;
            state = false;
        }

        public char GetName() { return name; }
    }
}
