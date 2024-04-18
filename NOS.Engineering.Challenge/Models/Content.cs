namespace NOS.Engineering.Challenge.Models
{
    public class Content
    {
        public Guid Id { get; }
        public string Title { get; }
        public string SubTitle { get; }
        public string Description { get; }
        public string ImageUrl { get; }
        public int Duration { get; }
        public DateTime StartTime { get; }
        public DateTime EndTime { get; }
        public List<string> GenreList { get; set; } // Setter adicionado aqui

        public Content(Guid id, string title, string subTitle, string description, string imageUrl, int duration, DateTime startTime, DateTime endTime, IEnumerable<string> genreList)
        {
            Id = id;
            Title = title;
            SubTitle = subTitle;
            Description = description;
            ImageUrl = imageUrl;
            Duration = duration;
            StartTime = startTime;
            EndTime = endTime;
            GenreList = genreList.ToList(); // Convertendo IEnumerable para List
        }
    }
}
