using System;

namespace Meritocious.Common.DTOs.Auth
{
    public class AuthorDetailsDto
    {
        public string Bio { get; set; }
        public int TotalPosts { get; set; }
        public decimal AverageMeritScore { get; set; }
        public DateTime JoinDate { get; set; }
        public bool IsFollowing { get; set; }
    }
}