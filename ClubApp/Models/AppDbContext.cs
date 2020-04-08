using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ClubApp.Models
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext() : base("DefaultConnection", throwIfV1Schema: false)
        { }
        public static AppDbContext Create()
        {
            return new AppDbContext();
        }

        /// <summary>
        /// 用户信息
        /// </summary>
        public virtual DbSet<UserNumber> UserNumbers { get; set; }
        /// <summary>
        /// 学院
        /// </summary>
        public virtual DbSet<Coloege> Coloeges { get; set; }
        /// <summary>
        /// 申请审批
        /// </summary>
        public virtual DbSet<ApplyAudit> ApplyAudits { get; set; }
        /// <summary>
        /// 申请类型
        /// </summary>
        public virtual DbSet<ApplyType> ApplyTypes { get; set; }
        /// <summary>
        /// 审批明细
        /// </summary>
        public virtual DbSet<AuditDetail> AuditDetails { get; set; }
        /// <summary>
        /// 社团信息
        /// </summary>
        public virtual DbSet<ClubNumber> ClubNumbers { get; set; }
        /// <summary>
        /// 社团类型
        /// </summary>
        public virtual DbSet<ClubType> ClubTypes { get; set; }
        /// <summary>
        /// 关系:用户-社团
        /// </summary>
        public virtual DbSet<UserClubs> UserClubs { get; set; }
        /// <summary>
        /// 关系:用户-门票
        /// </summary>
        public virtual DbSet<UserTicket> UserTickets { get; set; }
        /// <summary>
        /// 系统公告
        /// </summary>
        public virtual DbSet<Notice> Notices { get; set; }
        /// <summary>
        /// 社团公告
        /// </summary>
        public virtual DbSet<AnnounceMent> AnnounceMents { get; set; }
        /// <summary>
        /// 图片
        /// </summary>
        public virtual DbSet<ImgList> ImgLists { get; set; }
        /// <summary>
        /// 投票记录
        /// </summary>
        public virtual DbSet<Voting> Votings { get; set; }
        /// <summary>
        /// 活动
        /// </summary>
        public virtual  DbSet<Activities> Activities { get; set; }
        /// <summary>
        /// 区域地点
        /// </summary>
        public virtual  DbSet<Area> Areas { get; set; }
        /// <summary>
        /// 地点使用记录
        /// </summary>
        public virtual DbSet<UseArea> UseAreas { get; set; }
    }
    public class AppUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<AppUser> manager)
        {
            // 请注意，authenticationType 必须与 CookieAuthenticationOptions.AuthenticationType 中定义的相应项匹配
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // 在此处添加自定义用户声明
            AppDbContext db = new AppDbContext();
            var userinfo = (from u in db.UserNumbers where u.UserId == this.UserName select u).FirstOrDefault();
            if (userinfo != null)
            {
                if (userinfo.HeadImg == null)
                {
                    userinfo.HeadImg = "/Content/layui/images/face/0.gif";
                }
                Claim claim1 = new Claim(ClaimTypes.UserData, userinfo.HeadImg);
                Claim claim2 = new Claim(ClaimTypes.GivenName, userinfo.UserName);
                userIdentity.AddClaim(claim1);
                userIdentity.AddClaim(claim2);
            }
            return userIdentity;
        }
    }
    public class Coloege
    {
        [Key, Display(Name = "学院编号")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Display(Name = "学院"), MaxLength(50)]
        public string Name { get; set; }
        [Display(Name = "状态")]
        public int? State { get; set; }
    }
    //用户账号表 
    public class UserNumber
    {
        [Key, Display(Name = "账号"), MaxLength(10)]
        public string UserId { get; set; }
        [MaxLength(30), Display(Name = "用户名")]
        public string UserName { get; set; }
        [MaxLength(500), Display(Name = "用户头像")]
        public string HeadImg { get; set; }
        [Display(Name = "账号状态")]
        public int? State { get; set; }
        [Display(Name = "在线状态")]
        public int? OnlineState { get; set; }
        [Display(Name = "学院")]
        public virtual Coloege Coloege { get; set; }
        [Display(Name = "班级"), MaxLength(30)]
        public string Class { get; set; }
        [MaxLength(20), Display(Name = "学号")]
        public string RelName { get; set; }
        [Display(Name = "性别")]
        public int? Gender { get; set; }
        [Display(Name = "出生日期")]
        [DataType(DataType.Date)]
        public DateTime? Birthday { get; set; }
        [MaxLength(100), Display(Name = "签名")]
        public string ShortDesc { get; set; }
        [MaxLength(500), Display(Name = "描述")]
        public string Desc { get; set; }
        [MaxLength(100),Display(Name ="兴趣标签")]
        public string Labels { get; set; }
        [Display(Name = "创建日期")]
        [DataType(DataType.DateTime)]
        public DateTime? CreateDate { get; set; }
        [Display(Name = "最后登陆时间")]
        [DataType(DataType.DateTime)]
        public DateTime? LoginDate { get; set; }
    }
    //申请及审核
    public class ApplyAudit
    {
        [Key, Display(Name = "申请编号")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Display(Name = "申请类型"), Required]
        public virtual ApplyType Type { get; set; }
        [Display(Name = "申请备注"), MaxLength(500)]
        public string ApplicationDesc { get; set; }
        [Display(Name = "申请材料"), MaxLength(500)]
        public string ApplicationFiled { get; set; }
        [Display(Name = "申请社团")]
        public virtual ClubNumber Club { get; set; }
        [Display(Name = "申请人")]
        public virtual UserNumber ApplyUser { get; set; }
        [Display(Name = "申请日期")]
        [DataType(DataType.DateTime)]
        public DateTime? ApplyDate { get; set; }
        [Display(Name = "审批进度")]
        public int? CheckState { get; set; }
        [Display(Name = "处理日期")]
        [DataType(DataType.DateTime)]
        public DateTime? AuditDate { get; set; }
        [Display(Name = "审批次数")]
        public int? AuditTimes { get; set; }
    }
    public class AuditDetail
    {
        [Key, Display(Name = "审核明细编号")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Display(Name = "申请审核编号")]
        public int ApplyId { get; set; }
        [Display(Name = "请求来源")]
        public virtual UserNumber FromUser { get; set; }
        [Display(Name = "审批状态")]
        public int? CheckState { get; set; }
        [Display(Name = "审批人")]
        public virtual UserNumber AuditUser { get; set; }
        [Display(Name = "审批备注"), MaxLength(500)]
        public string AuditDesc { get; set; }
        [Display(Name = "审批日期")]
        [DataType(DataType.DateTime)]
        public DateTime? AuditDate { get; set; }
    }
    //申请审核类型
    public class ApplyType
    {
        [Key, Display(Name = "申请类型ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(20), Display(Name = "申请类型名称")]
        public string Name { get; set; }
        [Display(Name = "是否启用")]
        public int Enable { get; set; }
    }
    //社团头表
    public class ClubNumber
    {
        [Key, Display(Name = "社团账号"), MaxLength(10)]
        public string ClubId { get; set; }
        [Display(Name = "社团类型")]
        public virtual ClubType Type { get; set; }
        [Display(Name = "标签"), MaxLength(100)]
        public string Label { get; set; }
        [MaxLength(50), Display(Name = "社团名称")]
        public string Name { get; set; }
        [Display(Name = "头像"), MaxLength(500)]
        public string HeadImg { get; set; }
        [MaxLength(100), Display(Name = "签名")]
        public string ShortDesc { get; set; }
        [MaxLength(500), Display(Name = "描述")]
        public string Desc { get; set; }
        [Display(Name = "状态")]
        public int? State { get; set; }
        [Display(Name = "创建日期")]
        [DataType(DataType.DateTime)]
        public DateTime? CreateDate { get; set; }
        [Display(Name = "正式创建日期")]
        [DataType(DataType.DateTime)]
        public DateTime? CreateDate2 { get; set; }
        [Display(Name = "创建人")]
        public virtual UserNumber User { get; set; }
        [Display(Name = "审批编号")]
        public int AuditID { get; set; }
    }
    public class AnnounceMent
    {
        [Key, Display(Name = "公告ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Display(Name = "标签"), MaxLength(500)]
        public string Label { get; set; }
        [Display(Name = "主标题"), MaxLength(100)]
        public string Title1 { get; set; }
        [Display(Name = "副标题"), MaxLength(100)]
        public string Title2 { get; set; }
        [Display(Name = "内容"), MaxLength(1000)]
        public string Content { get; set; }
        [MaxLength(50)]
        public string ImgList { get; set; }
        [Display(Name = "发布时间"), DataType(DataType.DateTime)]
        public DateTime? CreateDate { get; set; }
        [Display(Name = "发布用户")]
        public virtual UserNumber User { get; set; }
        [Display(Name = "发布社团")]
        public virtual ClubNumber Club { get; set; }
        [Display(Name ="状态")]
        public int state { get; set; }
    }
    public class Activities
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Display(Name = "活动类型")]
        public int Type { get; set; }
        [Display(Name = "标签"), MaxLength(100)]
        public string Label { get; set; }
        [Display(Name = "主标题"), MaxLength(100)]
        public string Title1 { get; set; }
        [Display(Name = "副标题"), MaxLength(100)]
        public string Title2 { get; set; }
        [Display(Name = "内容")]
        public string Content { get; set; }
        [Display(Name = "人数限制")]
        public int? MaxUser { get; set; }
        [Display(Name ="活动地点")]
        public virtual Area Area { get; set; }
        [Display(Name = "活动地点"),MaxLength(100)]
        public string Area0 { get; set; }
        [Display(Name = "开始时间")]
        public DateTime Time1 { get; set; }
        [Display(Name = "结束时间")]
        public DateTime Time2 { get; set; }
        [Display(Name = "活动时长")]
        public int? Time3 { get; set; }
        [Display(Name = "发布时间"), DataType(DataType.DateTime)]
        public DateTime CreateDate { get; set; }
        [Display(Name = "发布用户")]
        public virtual UserNumber User { get; set; }
        [Display(Name = "发布社团")]
        public virtual ClubNumber Club { get; set; }
        [Display(Name = "状态")]
        public int State { get; set; }
        [Display(Name = "审批编号")]
        public int AuditID { get; set; }
        [Display(Name = "评分"),MaxLength(4)]
        public string Votes0 { get; set; }
    }
    //用户-社团
    public class UserClubs
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Display(Name = "用户")]
        public virtual UserNumber User { get; set; }
        [Display(Name = "社团")]
        public virtual ClubNumber Club { get; set; }
        [Display(Name = "职位/状态")]
        public int Status { get; set; }
        [Display(Name = "创建时间")]
        public DateTime? CreateDate { get; set; }
        [Display(Name = "备注"), MaxLength(500)]
        public string Desc { get; set; }
        [Display(Name = "状态")]
        public int State { get; set; }
        [Display(Name = "审批编号")]
        public int AuditID { get; set; }
    }
    //社团类型
    public class ClubType
    {
        [Key, Display(Name = "编号")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required, MaxLength(50), Display(Name = "名称")]
        public string Name { get; set; }
        [Display(Name = "是否启用")]
        public int? Enable { get; set; }
    }
    //public class UserStatus
    //{
    //    [Key, Display(Name = "职位ID")]
    //    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    //    public int Id { get; set; }
    //    [MaxLength(50), Display(Name = "职位名称")]
    //    public string Name { get; set; }
    //    [Display(Name = "是否启用")]
    //    public int? Enable { get; set; }
    //}
    //public class Vote
    //{
    //    [Key, Display(Name = "编号"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    //    public int Id { get; set; }
    //    [MaxLength(100), Display(Name = "标题")]
    //    public string Title { get; set; }
    //    [MaxLength(200), Display(Name = "简介")]
    //    public string ShortDesc { get; set; }
    //    [MaxLength(500), Display(Name = "说明")]
    //    public string LongDesc { get; set; }
    //    [Display(Name = "一星")]
    //    public int Votes1 { get; set; }
    //    [Display(Name = "二星")]
    //    public int Votes2 { get; set; }
    //    [Display(Name = "三星")]
    //    public int Votes3 { get; set; }
    //    [Display(Name = "四星")]
    //    public int Votes4 { get; set; }
    //    [Display(Name = "五星")]
    //    public int Votes5 { get; set; }
    //    [Display(Name = "开始日期")]
    //    public DateTime BeginDate { get; set; }
    //    [Display(Name = "截至日期")]
    //    public DateTime EndDate { get; set; }
    //    public virtual UserNumber User { get; set; }
    //    public virtual ClubNumber Club { get; set; }
    //    [Display(Name ="状态")]
    //    public int State { get; set; }
    //}
    public class Voting
    {
        [Key,Display(Name ="编号"),DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Display(Name = "回复对象")]
        public int ToId { get; set; }
        [Display(Name ="活动")]
        public virtual Activities Active { get; set; }
        [Display(Name ="用户")]
        public virtual UserNumber User { get; set; }
        [Display(Name = "分数")]
        public int Votes { get; set; }
        [Display(Name = "评论"), MaxLength(1000)]
        public string Desc { get; set; }
        [Display(Name = "赞同")]
        public int Good { get; set; }
        [Display(Name = "反对")]
        public int Bad { get; set; }
        [Display(Name = "状态")]
        public int State { get; set; }
        [Display(Name ="创建日期")]
        public DateTime CreateDate { get; set; }
    }
    public class Area
    {
        [Key, Display(Name = "编号"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(30),Required,Display(Name="地址")]
        public string Name { get; set; }
        [MaxLength(150), Display(Name = "地址描述")]
        public string Desc { get; set; }
        [MaxLength(50),Display(Name="联系方式")]
        public string Owner { get; set; }
        [Display(Name="状态")]
        public int? State { get; set; }
    }
    public class UseArea
    {
        [Key, Display(Name = "编号"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Display(Name = "地址")]
        public virtual Area Area { get; set; }
        [MaxLength(150), Display(Name = "备注")]
        public string Desc { get; set; }
        [Display(Name = "活动")]
        public virtual Activities Act { get; set; }
        [Display(Name = "占用时间")]
        public DateTime? Time1 { get; set; }
        [Display(Name = "占用结束时间")]
        public DateTime? Time2 { get; set; }
        [Display(Name ="占用时长")]
        public int? Time3 { get; set; }
        [Display(Name = "状态")]
        public int? State { get; set; }
    }
    public class UserTicket
    {
        [Key, Display(Name = "编号"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Display(Name = "活动")]
        public virtual Activities Active { get; set; }
        [Display(Name = "用户")]
        public virtual UserNumber User { get; set; }
        [Display(Name = "描述"),MaxLength(100)]
        public string Desc { get; set; }
        [Display(Name = "状态")]
        public int State { get; set; }
        [Display(Name = "创建日期")]
        public DateTime CreateDate { get; set; }
    }
    public class ImgList
    {
        [Key, Display(Name = "编号"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Display(Name = "相对路径")]
        public string ImgUrl { get; set; }
        [Display(Name = "绝对路径")]
        public string Path { get; set; }
        [Display(Name = "图片类型")]
        public int Type { get; set; }
        [Display(Name = "图片名称"),MaxLength(50)]
        public string Name { get; set; }
        [Display(Name = "状态")]
        public int State { get; set; }
        [Display(Name = "创建时间")]
        public DateTime CreateDate { get; set; }
        [Display(Name = "创建人"),MaxLength(10)]
        public string CreateUser { get; set; }
        [Display(Name = "创建社团"), MaxLength(10)]
        public string CreateClub { get; set; }
    }
    public class Notice
    {
        [Key, Display(Name = "公告ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Display(Name = "公告类型")]
        public int type { get; set; }
        [Display(Name = "主标题"), MaxLength(100)]
        public string Title1 { get; set; }
        [Display(Name = "副标题"), MaxLength(100)]
        public string Title2 { get; set; }
        [Display(Name = "内容"), MaxLength(1000)]
        public string Content { get; set; }
        [Display(Name = "发布时间"), DataType(DataType.DateTime)]
        public DateTime CreateDate { get; set; }
        [Display(Name = "发布用户")]
        public virtual UserNumber User { get; set; }
        [Display(Name = "状态")]
        public int state { get; set; }
    }
}