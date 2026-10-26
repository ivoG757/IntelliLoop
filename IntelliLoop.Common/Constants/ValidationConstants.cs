namespace IntelliLoop.Common.Constants
{
    public static class ValidationConstants
    {
        public static class Lecture
        {
            public const int TitleMaxLength = 150;
            public const int SummaryMaxLength = 10000;
            public const int TranscriptMaxLength = 60000;
            public const int MaxConceptCount = 30;
            public const int ConceptTextMaxLength = 200;
        }

        public static class NoteSection
        {
            public const int TitleMaxLength = 150;
            public const int ContentMaxLength = 20000;
        }
    }
}