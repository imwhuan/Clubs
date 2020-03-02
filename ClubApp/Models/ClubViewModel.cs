using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ClubApp.Models
{
    public class ClubListModel
    {
        [Display(Name ="编号")]
        public string cid { get; set; }
        [Display(Name="名称")]
        public string name { get; set; }
        [Display(Name="描述")]
        public string Desc { get; set; }
        [Display(Name ="创建时间")]
        public string CreateDate { get; set; }
        [Display(Name ="创建人")]
        public string CreateUser { get; set; }
        [Display(Name = "创建人")]
        public string CreateUserName { get; set; }
    }
    public class ClubViewModel
    {
        [Display(Name = "社团账号")]
        public string ClubId { get; set; }
        [Display(Name = "社团类型")]
        public string ClubType { get; set; }
        [Display(Name = "标签"), MaxLength(3)]
        public List<string> Labels { get; set; }
        [MaxLength(50), Display(Name = "社团名称")]
        public string Name { get; set; }
        [Display(Name = "头像"), MaxLength(500)]
        public string HeadImg { get; set; }
        [MaxLength(100), Display(Name = "签名")]
        public string ShortDesc { get; set; }
        [MaxLength(500), Display(Name = "描述")]
        public string Desc { get; set; }
        [Display(Name = "状态")]
        public string State { get; set; }
        [Display(Name = "创建日期")]
        public string CreateDate { get; set; }
        [Display(Name = "创建人")]
        public virtual UserNumber User { get; set; }
        [Display(Name="成员人数")]
        public int UserCount { get; set; }
        [Display(Name="最新活动")]
        public List<Activities> Activities { get; set; }
        [Display(Name="最新公告")]
        public List<AnnounceMent> announceMents { get; set; }
        [Display(Name="当前用户与社团关系")]
        public string status { get; set; }
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
    public class JoinClubSubModel
    {
        [Key, Display(Name = "社团账号"), MaxLength(10)]
        public string ClubId { get; set; }
        [Display(Name = "社团类型")]
        public string Type { get; set; }
        [MaxLength(50), Display(Name = "社团名称")]
        public string Name { get; set; }
        [Display(Name="社团人数")]
        public int UserCount { get; set; }
        [Display(Name = "头像"), MaxLength(500)]
        public string HeadImg { get; set; }
        [MaxLength(100), Display(Name = "签名")]
        public string ShortDesc { get; set; }
        [MaxLength(500), Display(Name = "社团描述")]
        public string Desc { get; set; }
        [Display(Name = "账号状态")]
        public string State { get; set; }
        [Display(Name = "创建日期")]
        [DataType(DataType.DateTime)]
        public string CreateDate { get; set; }
        [Display(Name = "创建人")]
        public string User { get; set; }
        [Display(Name = "申请理由"), MaxLength(500)]
        public string ApplyDesc { get; set; }
        [Display(Name = "附件")]
        public string ApplyFile { get; set; }
        [Display(Name = "是否可以申请加入")]
        public bool CanJoin { get; set; }
        [Display(Name = "上次处理日期")]
        public string AuditDate { get; set; }
        [Display(Name = "审批次数")]
        public int AuditTime { get; set; }
        [Display(Name = "审批编号")]
        public int AuditId { get; set; }
    }
    public class AuditJoinClubModel
    {
        [Display(Name ="社团")]
        public string ClubName { get; set; }
        public UserNumber UserInfo { get; set; }
        [Display(Name = "申请日期")]
        public string ApplyDate { get; set; }
        [Display(Name = "申请理由"), MaxLength(500)]
        public string ApplyDesc { get; set; }
        [Display(Name = "附件")]
        public string ApplyFile { get; set; }
        [Display(Name = "申请状态")]
        public string State { get; set; }
        [Display(Name = "上次处理日期")]
        public string AuditDate { get; set; }
        [Display(Name = "审批次数")]
        public int AuditTime { get; set; }
        [Display(Name = "审批编号")]
        public int AuditId { get; set; }
    }
}