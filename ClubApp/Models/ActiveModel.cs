using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ClubApp.Models
{
    public class ActiveModel
    {

    }
    public class ActiveListModel
    {
        public int Id { get; set; }
        [Display(Name = "标签")]
        public List<string> Labels { get; set; }
        [Display(Name = "标签")]
        public string LabelStr { get; set; }
        [Display(Name ="活动类型")]
        public string Type { get; set; }
        [Display(Name ="头像")]
        public string HeadImg { get; set; }
        [Display(Name = "主标题"), MaxLength(100)]
        public string Title1 { get; set; }
        [Display(Name = "简介"), MaxLength(100)]
        public string Title2 { get; set; }
        [Display(Name = "内容"), MaxLength(1000)]
        public string Content { get; set; }
        [Display(Name = "活动图片")]
        public List<string> ImgList { get; set; }
        [Display(Name = "人数限制")]
        public string MaxUser { get; set; }
        public Dictionary<string,string> Areas { get; set; }
        [Display(Name = "活动地点")]
        public string Area { get; set; }
        [Display(Name = "开始时间")]
        public string Time1 { get; set; }
        [Display(Name = "结束时间")]
        public string Time2 { get; set; }
        [Display(Name = "发布时间")]
        public string CreateDate { get; set; }
        [Display(Name = "发布用户")]
        public string UserID { get; set; }
        [Display(Name = "发布用户")]
        public string UserName { get; set; }
        [Display(Name = "发布社团")]
        public string ClubID { get; set; }
        [Display(Name = "发布社团")]
        public string ClubName { get; set; }
        [Display(Name = "状态")]
        public string State { get; set; }
        [Display(Name = "评分")]
        public string Votes0 { get; set; }
        [Display(Name = "评论数量")]
        public string VoteCount { get; set; }
    }

    public class ActiveSubModel
    {
        public int Id { get; set; }
        [Display(Name = "标签")]
        public List<string> Labels { get; set; }
        [Display(Name = "活动类型")]
        public string Type { get; set; }
        [Display(Name = "主标题"), MaxLength(100)]
        public string Title1 { get; set; }
        [Display(Name = "简介"), MaxLength(100)]
        public string Title2 { get; set; }
        [Display(Name = "内容"), MaxLength(1000)]
        public string Content { get; set; }
        [Display(Name = "人数限制")]
        public string MaxUser { get; set; }
        [Display(Name = "活动地点")]
        public string Area { get; set; }
        [Display(Name = "开始时间")]
        public string Time1 { get; set; }
        [Display(Name = "结束时间")]
        public string Time2 { get; set; }
        [Display(Name = "发布时间")]
        public string CreateDate { get; set; }
        [Display(Name = "发布用户")]
        public string UserID { get; set; }
        [Display(Name = "发布用户")]
        public string UserName { get; set; }
        [Display(Name = "发布社团")]
        public string ClubID { get; set; }
        [Display(Name = "发布社团")]
        public string ClubName { get; set; }
        [Display(Name = "状态")]
        public string State { get; set; }

        [Display(Name = "申请理由"), MaxLength(500)]
        public string ApplyDesc { get; set; }
        [Display(Name = "附件")]
        public string ApplyFile { get; set; }
        [Display(Name = "审批备注"), MaxLength(500)]
        public string AuditDesc { get; set; }
        [Display(Name = "上次处理日期")]
        public string AuditDate { get; set; }
        [Display(Name = "审批次数")]
        public int AuditTime { get; set; }
        [Display(Name = "审批编号")]
        public int AuditId { get; set; }
    }
    public class AddVote
    {
        [Display(Name = "活动")]
        public int Active { get; set; }
        [Display(Name = "分数")]
        public int Votes { get; set; }
        [Display(Name = "评论"), MaxLength(100)]
        public string Desc { get; set; }
    }
    public class ActDetail
    {
        public int Id { get; set; }
        public string State { get; set; }
        public string time1 { get; set; }
        public string time2 { get; set; }
        public string area { get; set; }
        public string Title1 { get; set; }
        public string Title2 { get; set; }
        public string MaxUser { get; set; }
        public string Content { get; set; }
        public string CreateDate { get; set; }
        public double Vote0 { get; set; }
        public string Vote1 { get; set; }
        public string Vote2 { get; set; }
        public string Vote3 { get; set; }
        public string Vote4 { get; set; }
        public string Vote5 { get; set; }
        public List<string> Labels { get; set; }
        public List<Voting> Votings { get; set; }
    }
}