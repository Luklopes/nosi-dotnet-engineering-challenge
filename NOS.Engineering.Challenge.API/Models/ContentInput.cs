using NOS.Engineering.Challenge.Models;

namespace NOS.Engineering.Challenge.API.Models
{
    public class ContentInput
    {
        public Guid Id { get; set; } // Adicionando a propriedade Id

        public string? Title { get; set; }
        public string? SubTitle { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public int? Duration { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public List<string>? Genres { get; set; }

        public ContentDto ToDto()
        {
            return new ContentDto(
                Id, // Passando o Id como parâmetro
                Title,
                SubTitle,
                Description,
                ImageUrl,
                Duration,
                Genres ?? new List<string>(),
                StartTime,
                EndTime,
                Genres ?? new List<string>()
            );
        }
    }
}
