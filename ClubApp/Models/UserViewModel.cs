using System.ComponentModel.DataAnnotations;

namespace ClubApp.Models
{
    public class CPassWorldModel
    {
        [Required, DataType(DataType.Password)]
        [Display(Name = "旧密码")]
        public string OldPwd { get; set; }
        [Required, StringLength(20, MinimumLength = 6)]
        [DataType(DataType.Password), Display(Name = "新密码")]
        public string NewPwd { get; set; }
        [DataType(DataType.Password), Display(Name = "重复新密码")]
        [Compare("OldPwd", ErrorMessage = "两次密码输入不一致")]
        public string RePwd { get; set; }
    }
}