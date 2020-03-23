using ClubApp.Models;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ClubApp.Controllers
{
    public class GetDataController : Controller
    {
        #region 初始化用户/角色/登陆管理器
        private AppSignInManager _signInManager;
        private AppUserManager _userManager;
        private AppRoleManager _roleManager;

        public GetDataController()
        {
        }

        public GetDataController(AppUserManager userManager, AppSignInManager signInManager, AppRoleManager roleManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            RoleManager = roleManager;

        }

        public AppSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<AppSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public AppUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        public AppRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<AppRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        AppDbContext db = new AppDbContext();
        #endregion
        // GET: GetData 获取已注册用户，权限控制页面调用
        public async Task<ActionResult> UserSimpleData(int page, int limit)
        {
            List<UserSimpleModel> datas = new List<UserSimpleModel>();
            foreach (AppUser us in db.Users.OrderBy(u=>u.Id).Skip((page - 1) * limit).Take(limit).ToList())
            {
                UserSimpleModel data = new UserSimpleModel()
                {
                    id=us.UserName,
                    name=us.Email,
                    ifup=true,
                };
                if (us.Roles.Count > 0)
                {
                    foreach(Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole userrole in us.Roles)
                    {
                        var role = await RoleManager.FindByIdAsync(userrole.RoleId);
                        data.roles += role.Name + "/";
                        if (role.Name == "BAdmin")
                        {
                            data.ifup = false;
                        }
                    }
                    data.roles = data.roles.Substring(0, data.roles.Length - 1);
                }
                else
                {
                    data.roles = "无";
                }
                datas.Add(data);
            }
            PageDataModel dataModel = new PageDataModel()
            {
                code = 0,
                msg = "",
                count = db.Users.Count(),
                data = datas.AsQueryable()
            };

            return Json(dataModel, JsonRequestBehavior.AllowGet);
        }
        //获取所有账号池
        public ActionResult UserNumberData(int page, int limit)
        {
            List<UserNumberModel> datas = new List<UserNumberModel>();
            foreach (UserNumber us in db.UserNumbers.OrderBy(u => u.UserId).Skip((page - 1) * limit).Take(limit).ToList())
            {
                UserNumberModel data = new UserNumberModel()
                {
                    id = us.UserId,
                    relname=us.RelName??"无",
                    state=Enum.GetName(typeof(EnumState),us.State)
                };
                datas.Add(data);
            }
            PageDataModel dataModel = new PageDataModel()
            {
                code = 0,
                msg = "",
                count = db.UserNumbers.Count(),
                data = datas.AsQueryable()
            };

            return Json(dataModel, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ClubsNumberData(int page, int limit)
        {
            List<ClubsNumberModel> models = new List<ClubsNumberModel>();
            foreach (ClubNumber club in db.ClubNumbers.OrderBy(c => c.ClubId).Skip((page - 1) * limit).Take(limit))
            {
                ClubsNumberModel model = new ClubsNumberModel();
                model.ClubId = club.ClubId;
                switch (club.State)
                {
                    case ((int)EnumState.未使用):
                        model.State = Enum.GetName(typeof(EnumState), club.State);
                        model.Style = "black";
                        break;
                    case ((int)EnumState.系统锁定):
                        model.State = Enum.GetName(typeof(EnumState), club.State);
                        model.Style = "gray";
                        break;
                    case ((int)EnumState.待提交):
                        model.State = Enum.GetName(typeof(EnumState), club.State);
                        model.Style = "red";
                        break;
                    case ((int)EnumState.正常):
                        model.State = Enum.GetName(typeof(EnumState), club.State);
                        model.Style = "green";
                        break;
                    case ((int)EnumState.待审批):
                        model.State = Enum.GetName(typeof(EnumState), club.State);
                        model.Style = "orange";
                        break;
                    case ((int)EnumState.查封):
                        model.State = Enum.GetName(typeof(EnumState), club.State);
                        model.Style = "pink";
                        break;
                    case ((int)EnumState.冻结):
                        model.State = Enum.GetName(typeof(EnumState), club.State);
                        model.Style = "bule";
                        break;
                    case ((int)EnumState.制裁):
                        model.State = Enum.GetName(typeof(EnumState), club.State);
                        model.Style = "red";
                        break;
                    default:
                        model.State = "未知";
                        model.Style = "yellow";
                        break;
                }
                model.CreateDate = club.CreateDate == null ? "无" : club.CreateDate.ToString();
                models.Add(model);
            }
            PageDataModel dataModel = new PageDataModel()
            {
                code = 0,
                msg = "",
                count = db.ClubNumbers.Count(),
                data = models.AsQueryable()
            };

            return Json(dataModel, JsonRequestBehavior.AllowGet);
        }
        //查询社团调用
        public ActionResult GetAllClubData(int page, int limit)
        {
            List<ClubListModel> datas = new List<ClubListModel>();
            foreach (ClubNumber club in db.ClubNumbers.Where(c=>c.State>(int)EnumState.待审批).OrderBy(c => c.ClubId).Skip((page - 1) * limit).Take(limit).ToList())
            {
                ClubListModel data = new ClubListModel()
                {
                    cid = club.ClubId,
                    name = club.Name ?? "无",
                    Desc = club.Desc??"无",
                    CreateDate=club.CreateDate2?.ToString(),
                    CreateUser=club.User?.UserId,
                    CreateUserName=club.User?.UserName
                };
                datas.Add(data);
            }
            PageDataModel dataModel = new PageDataModel()
            {
                code = 0,
                msg = "",
                count = db.Users.Count(),
                data = datas.AsQueryable()
            };

            return Json(dataModel, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetActiveListData(int page,string cid)
        {
            int count = 6;//单页显示数量
            List<ActiveListModel> models = new List<ActiveListModel>();
            IQueryable<Activities> actlist;
            if (string.IsNullOrEmpty(cid))
            {
                actlist = db.Activities.Where(a=>a.State>2).OrderByDescending(a => a.Id).Skip((page - 1) * count).Take(count);
            }
            else
            {
                actlist = db.Activities.Where(a=>a.Club.ClubId==cid).OrderByDescending(a => a.Id).Skip((page - 1) * count).Take(count);
            }
            foreach (Activities act in actlist)
            {
                ActiveListModel model = new ActiveListModel()
                {
                    Id = act.Id,
                    Labels = act.Label.Split(',').ToList(),
                    HeadImg=act.Club.HeadImg,
                    Title1 = act.Title1,
                    Title2 = act.Title2,
                    Content = act.Content,
                    MaxUser = act.MaxUser == null ? "无限制" : act.MaxUser.ToString() + "人",
                    Area = act.Area?.Name,
                    Time1 = act.Time1.ToString("yyyy/MM/dd hh:mm"),
                    Time2 = act.Time2.ToString("yyyy/MM/dd hh:mm"),
                    CreateDate = act.CreateDate.ToString(),
                    UserID = act.User.UserId,
                    UserName=act.User.UserName,
                    ClubID = act.Club.ClubId,
                    ClubName=act.Club.Name,
                    VoteCount=db.Votings.Where(v=>v.Active.Id==act.Id).Count().ToString(),
                   // State = Enum.GetName(typeof(ActiveState), act.State),
                    Votes0 = string.IsNullOrEmpty(act.Votes0)?"0":act.Votes0
                };
                if (act.Time1 > DateTime.Now)
                {
                    model.State = "未开始";
                }
                else if (act.Time2 > DateTime.Now)
                {
                    model.State = "进行中";
                }
                else
                {
                    model.State = "已结束";
                }
                if (act.Area == null)
                {
                    if (string.IsNullOrEmpty(act.Area0))
                    {
                        model.Area = "未知";
                    }
                    else
                    {
                        model.Area= act.Area0;
                    }
                }

                models.Add(model);
            }
            PageDataModel dataModel = new PageDataModel()
            {
                code = 0,
                msg = "",
                count = (int)Math.Ceiling((double)db.Activities.Count()/count),
                data = models.AsQueryable()
            };

            return Json(dataModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetActiveListData2(int page, int limit, string cid)
        {
            List<ActiveListModel> models = new List<ActiveListModel>();
            IQueryable<Activities> actlist;
            if (string.IsNullOrEmpty(cid))
            {
                actlist = db.Activities.OrderByDescending(a => a.Id).Skip((page - 1) * limit).Take(limit);
            }
            else
            {
                actlist = db.Activities.Where(a => a.Club.ClubId == cid).OrderByDescending(a => a.Id).Skip((page - 1) * limit).Take(limit);
            }
            foreach (Activities act in actlist)
            {
                ActiveListModel model = new ActiveListModel()
                {
                    Id = act.Id,
                    Title1 = act.Title1,
                    Title2 = act.Title2,
                    Time1 = act.Time1.ToString("yyyy/MM/dd hh:mm"),
                    Time2 = act.Time2.ToString("yyyy/MM/dd hh:mm"),
                    CreateDate = act.CreateDate.ToString(),
                    UserID = act.User.UserId,
                    UserName = act.User.UserName,
                    VoteCount = db.Votings.Where(v => v.Active.Id == act.Id).Count().ToString(),
                    State = Enum.GetName(typeof(ActiveState), act.State),
                    Votes0 = string.IsNullOrEmpty(act.Votes0) ? "0" : act.Votes0
                };
                if (act.State > (int)ActiveState.已取消)
                {
                    if (act.Time1 > DateTime.Now)
                    {
                        model.State = "未开始";
                    }
                    else if (act.Time2 > DateTime.Now)
                    {
                        model.State = "进行中";
                    }
                    else
                    {
                        model.State = "已结束";
                    }
                }                

                models.Add(model);
            }
            PageDataModel dataModel = new PageDataModel()
            {
                code = 0,
                msg = "",
                count = db.Activities.Count(),
                data = models.AsQueryable()
            };

            return Json(dataModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetApplyData(int page, int limit, string state)
        {
            List<ApplyAudit> models = new List<ApplyAudit>();
            int st = 0;
            if (!string.IsNullOrEmpty(state)&& int.TryParse(state,out st))
            {
                models = db.ApplyAudits.Where(a => a.Type.Id == (int)SQType.创建活动 && a.CheckState == st).OrderBy(a => a.Id).ToList();
            }
            else
            {
                models = db.ApplyAudits.Where(a => a.Type.Id == (int)SQType.创建活动).OrderBy(a => a.Id).ToList();
            }
            List<ApplyView> datas = new List<ApplyView>();
            foreach (ApplyAudit applys in models)
            {
                ApplyView apply = new ApplyView
                {
                    Id = applys.Id.ToString(),
                    Type = applys.Type.Name,
                    Club = applys.Club?.Name,
                    ApplyUser = applys.ApplyUser.UserName,
                    CheckState = Enum.GetName(typeof(EnumAuditState), applys.CheckState),
                    ApplyDate = applys.ApplyDate.ToString(),
                    AuditDate = applys.AuditDate.ToString(),
                    AuditTimes = applys.AuditTimes ?? 0
                };
                datas.Add(apply);
            }
            PageDataModel dataModel = new PageDataModel()
            {
                code = 0,
                msg = "",
                count = models.Count(),
                data = datas.AsQueryable()
            };

            return Json(dataModel, JsonRequestBehavior.AllowGet);
        }
    }
}