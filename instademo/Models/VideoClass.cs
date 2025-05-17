using System.ComponentModel.DataAnnotations;

namespace instademo.Models
{
    public class VideoClass
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string VideoName { get; set; }

        [Required]
        public string VideoPath { get; set; }

        [Required]
        public int Likes { get; set; }

    }
}
