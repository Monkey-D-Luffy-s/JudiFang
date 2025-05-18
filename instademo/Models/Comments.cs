namespace instademo.Models
{
    public class Comments
    {
        public Guid Id { get; set; }

        public Guid VideoId { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public string Date { get; set; }

    }
}
