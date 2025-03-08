using System.Collections.Generic;

namespace Endurance_Testing.Models
{
    public class GeminiRequest
    {
        public List<GeminiContent> contents { get; set; }
        public GenerationConfig generationConfig { get; set; }

        public GeminiRequest()
        {
            contents = new List<GeminiContent>();
            generationConfig = new GenerationConfig
            {
                temperature = 1.0f,
                topK = 40,
                topP = 0.95f,
                maxOutputTokens = 8192
            };
        }
    }

    public class GeminiContent
    {
        public string role { get; set; }
        public List<GeminiPart> parts { get; set; }

        public GeminiContent()
        {
            parts = new List<GeminiPart>();
        }
    }

    public class GeminiPart
    {
        public string text { get; set; }
    }

    public class GenerationConfig
    {
        public float temperature { get; set; }
        public int topK { get; set; }
        public float topP { get; set; }
        public int maxOutputTokens { get; set; }
    }

    public class GeminiResponse
    {
        public List<GeminiCandidate> candidates { get; set; }
    }

    public class GeminiCandidate
    {
        public GeminiContent content { get; set; }
    }
}