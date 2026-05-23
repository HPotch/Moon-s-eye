using System.Collections.Generic;

public static class Classes
{
    public class Talk
    {
        public string Text;
        public bool TalkNext;
        public List<string> NextQuestions = new List<string>();
        public string NextTalk;
        public bool End;
    }

    public class Question
    {
        public string Text;
        public string NextTalk;
        public bool End;
    }
}
