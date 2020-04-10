using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ClubApp.Models
{
    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }
    public class ViewModels
    {
    }
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "账号")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [Display(Name = "记住我?")]
        public bool RememberMe { get; set; }
    }
    public class EmailRegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "电子邮件")]
        [System.Web.Mvc.Remote("EmailRegisterValidate", "Account", HttpMethod = "post", ErrorMessage = "该邮箱已被注册")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "确认密码")]
        [Compare("Password", ErrorMessage = "密码和确认密码不匹配。")]
        public string ConfirmPassword { get; set; }
        [Display(Name ="性别")]
        public int Gender { get; set; }
        [Required, Display(Name = "图片验证码")]
        [System.Web.Mvc.Remote("CheckImgCode", "Account", HttpMethod = "post", ErrorMessage = "验证码 错误")]
        public string ImgCode { get; set; }
        [Required, Display(Name = "邮箱验证码")]
        [System.Web.Mvc.Remote("CheckEmailCode", "Account", HttpMethod = "post", ErrorMessage = "邮箱验证码 错误")]
        public string EmailCode { get; set; }
    }

    public class RelNameRegisterViewModel
    {
        [Required]
        [StringLength(maximumLength:12,MinimumLength =6, ErrorMessage = "{0} 长度应为{2}至{1}个字符。")]
        [Display(Name = "学号")]
        [System.Web.Mvc.Remote("RelNameRegisterValidate", "Account", HttpMethod = "post", ErrorMessage = "该学号已被注册")]
        public string RelName { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "确认密码")]
        [Compare("Password", ErrorMessage = "密码和确认密码不匹配。")]
        public string ConfirmPassword { get; set; }
        [Display(Name = "性别")]
        public int Gender { get; set; }
        [Required, Display(Name = "图片验证码")]
        [System.Web.Mvc.Remote("CheckImgCode", "Account", HttpMethod = "post", ErrorMessage = "验证码 错误")]
        public string ImgCode { get; set; }
    }

    public class PhoneRegisterViewModel
    {
        [Required]
        [Phone]
        [Display(Name = "电话号码")]
        [System.Web.Mvc.Remote("PhoneRegisterValidate", "Account", HttpMethod = "post", ErrorMessage = "该电话号码已被注册")]
        public string Phone { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "确认密码")]
        [Compare("Password", ErrorMessage = "密码和确认密码不匹配。")]
        public string ConfirmPassword { get; set; }
        [Required, Display(Name = "图片验证码")]
        [System.Web.Mvc.Remote("CheckImgCode", "Account", HttpMethod = "post", ErrorMessage = "验证码 错误")]
        public string ImgCode { get; set; }
        [Required, Display(Name = "手机验证码")]
        [System.Web.Mvc.Remote("CheckPhoneCode", "Account", HttpMethod = "post", ErrorMessage = "手机验证码 错误")]
        public string PhoneCode { get; set; }
    }
    public class RegisterConfirmModel
    {
        [Display(Name = "名次")]
        public int Index { get; set; }
        [Display(Name = "账号")]
        public string Num { get; set; }
        [Display(Name = "注册时间")]
        public string Date { get; set; }
    }
    public class UserIndexModel
    {
        [Key, Display(Name = "账号"), MaxLength(10)]
        public string UserId { get; set; }
        [Display(Name = "用户名")]
        public string UserName { get; set; }
        [Display(Name = "用户头像")]
        public string HeadImg { get; set; }
        [Display(Name = "账号状态")]
        public string State { get; set; }
        [Display(Name = "在线状态")]
        public string Online { get; set; }
        [Display(Name = "学院")]
        public string Coloege { get; set; }
        [Display(Name = "班级")]
        public string Class { get; set; }
        [Display(Name = "真实姓名")]
        public string RelName { get; set; }
        [Display(Name = "性别")]
        public string Gender { get; set; }
        [Display(Name = "出生日期")]
        [DataType(DataType.Date)]
        public DateTime? Birthday { get; set; }
        [Display(Name = "签名")]
        public string ShortDesc { get; set; }
        [Display(Name = "描述")]
        public string Desc { get; set; }
        [Display(Name = "创建日期")]
        [DataType(DataType.DateTime)]
        public DateTime? CreateDate { get; set; }
        [Display(Name = "最后登陆时间")]
        [DataType(DataType.DateTime)]
        public DateTime? LoginDate { get; set; }
        public string SysAge { get; set; }
        public string Label { get; set; }
        public string Role { get; set; }
    }
    public class ApplyClubModel
    {
        [Display(Name = "社团类型")]
        public List<ClubType> clubTypes { get; set; }
        [Display(Name = "社团账号")]
        public string ClubId { get; set; }
        [Display(Name = "社团类型")]
        public int Type { get; set; }
        [Display(Name = "社团标签")]
        public string Label { get; set; }
        [MaxLength(50), Display(Name = "社团名称")]
        public string Name { get; set; }
        [Display(Name = "头像"), MaxLength(500)]
        public string HeadImg { get; set; }
        [MaxLength(100), Display(Name = "签名")]
        public string ShortDesc { get; set; }
        [MaxLength(500), Display(Name = "社团描述")]
        public string Desc { get; set; }
        [Display(Name = "账号状态")]
        public int? State { get; set; }
        [Display(Name = "创建日期")]
        [DataType(DataType.DateTime)]
        public DateTime? CreateDate { get; set; }
        [Display(Name = "创建人")]
        public virtual UserNumber User { get; set; }
        [Display(Name = "申请理由"), MaxLength(500)]
        public string ApplyDesc { get; set; }
        [Display(Name = "附件")]
        public string ApplyFile { get; set; }
    }
    public class ApplyClubSubModel
    {
        [Key, Display(Name = "社团账号"), MaxLength(10)]
        public string ClubId { get; set; }
        [Display(Name = "社团类型")]
        public string Type { get; set; }
        [MaxLength(50), Display(Name = "社团名称")]
        public string Name { get; set; }
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
        [Display(Name = "审批备注"), MaxLength(500)]
        public string AuditDesc { get; set; }
        [Display(Name = "上次处理日期")]
        public string AuditDate { get; set; }
        [Display(Name = "审批次数")]
        public int AuditTime { get; set; }
        [Display(Name = "审批编号")]
        public int AuditId { get; set; }
    }
    public class UserClubDataModel
    {
        public int Id { get; set; }
        [Display(Name = "用户")]
        public virtual UserNumber User { get; set; }
        [Display(Name = "社团编号")]
        public string ClubId { get; set; }
        [Display(Name = "社团")]
        public string Club { get; set; }
        [Display(Name = "职位/状态")]
        public string Status { get; set; }
        public string StatusT { get; set; }
        [Display(Name = "入社时间")]
        public string CreateDate { get; set; }
        [Display(Name = "备注"), MaxLength(500)]
        public string Desc { get; set; }
    }
    public class ApplyView
    {
        [Display(Name = "申请编号")]
        public string Id { get; set; }
        [Display(Name = "申请类型"), Required]
        public string Type { get; set; }
        [Display(Name = "申请社团")]
        public string Club { get; set; }
        [Display(Name = "申请人")]
        public string ApplyUser { get; set; }
        [Display(Name = "申请日期")]
        public string ApplyDate { get; set; }
        [Display(Name = "审批进度")]
        public string CheckState { get; set; }
        [Display(Name = "处理日期")]
        public string AuditDate { get; set; }
        [Display(Name = "审批次数")]
        public int AuditTimes { get; set; }
    }
    public class AreaView
    {
        [Key, Display(Name = "编号")]
        public int Id { get; set; }
        [MaxLength(30), Display(Name = "地址")]
        public string Name { get; set; }
        [MaxLength(150), Display(Name = "地址描述")]
        public string Desc { get; set; }
        [MaxLength(50), Display(Name = "联系方式")]
        public string Owner { get; set; }
        [Display(Name = "状态")]
        public string State { get; set; }
    }
    public class NoticeView
    {
        [Display(Name = "公告ID")]
        public int Id { get; set; }
        [Display(Name = "公告类型")]
        public int type { get; set; }
        [Display(Name = "主标题"), MaxLength(100)]
        public string Title1 { get; set; }
        [Display(Name = "副标题"), MaxLength(100)]
        public string Title2 { get; set; }
        [Display(Name = "内容"), MaxLength(1000)]
        public string Content { get; set; }
        [Display(Name = "发布时间")]
        public string CreateDate { get; set; }
        [Display(Name = "发布用户")]
        public string User { get; set; }
        [Display(Name = "状态")]
        public string state { get; set; }
    }
}