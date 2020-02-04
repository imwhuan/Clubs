using System.ComponentModel.DataAnnotations;

namespace ClubApp.Models
{
    public class ClubViewModel
    {
        
    }
    public class UserClubModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        [Display(Name = "用户")]
        public string User { get; set; }
        public string ClubId { get; set; }
        [Display(Name = "社团")]
        public string Club { get; set; }
        [Display(Name = "职位/状态")]
        public string Status { get; set; }
        public int state { get; set; }
        [Display(Name = "社团状态")]
        public string CState { get; set; }
        [Display(Name = "创建时间")]
        public string CreateDate { get; set; }
        [Display(Name = "备注"), MaxLength(500)]
        public string Desc { get; set; }
        public int Enable { get; set; }
    }
}