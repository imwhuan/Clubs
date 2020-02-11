using System.Collections.Generic;
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
    public class UserSetModel
    {
        [Display(Name ="邮箱"),DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Display(Name ="手机号码"),DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }
        [Display(Name ="学号/职工号"),MaxLength(15)]
        public string RealName { get; set; }
        [Display(Name ="学校")]
        public string School { get; set; }
        [Display(Name ="学院")]
        public string Cologe { get; set; }
        public List<Coloege> Cologes { get; set; }
        [Display(Name ="班级")]
        public string Grade { get; set; }
        public string Grade2 { get; set; }
        [Display(Name ="兴趣标签"),MaxLength(3)]
        public List<string> Labels { get; set; }
    }
}