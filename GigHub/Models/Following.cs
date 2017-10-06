using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GigHub.Models
{
    //ALTERNATIVELY THIS CLASS COULD BE CALLED RELATIONSHIP
    public class Following
    {
        [Key]
        [Column(Order = 1)]
        public string FollowerId { get; set; }

        [Key]
        [Column(Order = 2)]
        public string FolloweeId { get; set; }

        public ApplicationUser Follower { get; set; }

        public ApplicationUser Followee { get; set; }
    }
}