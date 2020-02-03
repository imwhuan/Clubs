using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ClubApp.Models
{
    public class AdminViewModels
    {
    }
    public class AddUserNumModel
    {
        [Required]
        [Display(Name = "起始序号")]
        public int MinNum { get; set; }
        [Required, Display(Name = "最大序号")]
        public int MaxNum { get; set; }
        [Display(Name = "当前最大值")]
        public int NowNum { get; set; }
    }
    public class ClubsNumberModel
    {
        public string ClubId { get; set; }
        public string ClubName { get; set; }
        public string State { get; set; }
        public string Style { get; set; }
        public string CreateDate { get; set; }
    }
    public class DelClubTypeModel
    {
        public string TypeName { get; set; }
        public int Enable { get; set; }
        public List<ClubsNumberModel> clubs { get; set; }
    }
    public class ApplyAuditModel
    {
        [Display(Name = "申请审核编号")]
        public int Id { get; set; }
        [Display(Name = "申请类型"), Required]
        public string Type { get; set; }
        [Display(Name = "申请备注")]
        public string ApplicationDesc { get; set; }
        [Display(Name = "申请材料")]
        public string ApplicationFiled { get; set; }
        [Display(Name = "申请社团")]
        public string Club { get; set; }
        [Display(Name = "申请人")]
        public string ApplyUser { get; set; }
        [Display(Name = "申请日期")]
        public string ApplyDate { get; set; }
        [Display(Name = "审批进度")]
        public string CheckState { get; set; }
    }
    public class AllTypeModel
    {
        public string TypeName { get; set; }
        public int Enable { get; set; }
        public List<ApplyAuditModel> Data { get; set; }
    }
}