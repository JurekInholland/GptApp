namespace Models;

public abstract class ApiResponse
{
    public string Id { get; set; }
    public string Object { get; set; }
    public string Created { get; set; }
    public string Model { get; set; }

    public ApiUsage Usage { get; set; }

    public Choice[] Choices { get; set; }

    public abstract class ApiUsage
    {
        public int PromptTokens { get; set; }
        public int CompletionTokens { get; set; }
        public int TotalTokens { get; set; }
    }

    public abstract class Choice
    {
        public abstract class DeltaObj
        {
            public string Content { get; set; }
        }

        public DeltaObj Delta { get; set; }
        public int Index { get; set; }
        public string FinishReason { get; set; }
    }
}
