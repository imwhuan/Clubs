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
        [Display(Name="最新投票")]
        public List<Vote> votes { get; set; }
        [Display(Name="当前用户与社团关系")]
        public int status { get; set; }
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