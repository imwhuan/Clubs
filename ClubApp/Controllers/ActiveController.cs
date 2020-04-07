using ClubApp.Models;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClubApp.Controllers
{
    public class ActiveController : Controller
    {
        AppDbContext db = new AppDbContext();

        private AppUserManager _userManager;

        public ActiveController()
        {
        }

        public ActiveController(AppUserManager userManager)
        {
            UserManager = userManager;
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
        // GET: Active
        public ActionResult Index(string cid)
        {
            if (cid != null)
            {
                ClubNumber club = db.ClubNumbers.Find(cid);
                if (club != null)
                {
                    ViewBag.ClubId = cid;
                }
            }
            return View();
        }
        [HttpGet]
        public ActionResult Add(string cid)
        {
            if (cid == null)
            {
                return RedirectToAction("Index");
            }
            ClubNumber club = db.ClubNumbers.Find(cid);
            if (club == null)
            {
                return HttpNotFound("未发现社团" + cid);
            }
            ViewBag.Club = club.Name;
            return View();
        }
        public ActionResult Detail(string aid)
        {
            if (aid == null)
            {
                return RedirectToAction("Index");
            }
            if (!int.TryParse(aid, out int id))
            {
                return HttpNotFound("未发现活动" + aid);
            }
            Activities act = db.Activities.Find(id);
            if (act == null)
            {
                return HttpNotFound("未发现活动" + aid);
            }
            ActDetail model = new ActDetail();
            model.Id = act.Id;
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
            model.time1 = act.Time1.ToString("yyyy年MM月dd日：HH时mm分ss秒");
            model.time2 = act.Time2.ToString("yyyy年MM月dd日：HH时mm分ss秒");
            if (act.Area == null)
            {
                if (string.IsNullOrEmpty(act.Area0))
                {
                    model.area = "未知";
                }
                else
                {
                    model.area = act.Area0;
                }
            }
            else
            {
                model.area = act.Area.Name;
            }
            model.Title1 = act.Title1;
            model.Title2 = act.Title2;
            model.MaxUser = act.MaxUser == null ? "无限制" : act.MaxUser.ToString()+"人";
            model.Labels = act.Label.Split(',').ToList();
            model.Content = act.Content;
            model.CreateDate = act.CreateDate.ToString("yyyy年MM月dd日：HH时mm分ss秒");
            //统计不同打分的评论个数
            List<Voting> votings = db.Votings.Where(v => v.Active.Id == act.Id).ToList();
            if (votings == null)
            {
                model.Votings = new List<Voting>();
            }
            else
            {
                model.Votings = votings;
            }
            
            int v1=0, v2=0, v3=0, v4=0, v5=0;
            foreach(Voting v in votings)
            {
                if (v.Votes < 1)
                {
                    //小于1分的为无效值
                }
                else if (v.Votes < 2)
                {
                    v1 += 1;
                }
                else if (v.Votes < 3)
                {
                    v2 += 1;
                }
                else if (v.Votes < 4)
                {
                    v3 += 1;
                }
                else if (v.Votes < 5)
                {
                    v4 += 1;
                }
                else if (v.Votes < 6)
                {
                    v5 += 1;
                }
            }
            double av1 = v1 * 1 + v2 * 2 + v3 * 3 + v4 * 4 + v5 * 5;//总分数
            int av2 = v1 + v2 + v3 + v4 + v5;//总评论数
            model.Vote0 =av2==0?0: Math.Round(av1/av2,1);//平均分数，保留一位小数
            model.Vote1 = v1.ToString() + "/" + av2.ToString();
            model.Vote2 = v2.ToString() + "/" + av2.ToString();
            model.Vote3 = v3.ToString() + "/" + av2.ToString();
            model.Vote4 = v4.ToString() + "/" + av2.ToString();
            model.Vote5 = v5.ToString() + "/" + av2.ToString();
            //下述代码为更新活动平均分
            //act.Votes0 = model.Vote0.ToString();
            //db.Entry(act).State = System.Data.Entity.EntityState.Modified;
            //db.SaveChanges();
            return View(model);
        }
        [HttpGet]
        public ActionResult AddAct(string cid)
        {
            if (cid == null)
            {
                return RedirectToAction("Index");
            }
            ClubNumber club = db.ClubNumbers.Find(cid);
            if (club == null)
            {
                Session["Error"] = "未发现社团" + cid;
                return RedirectToAction("Error404", "Home");
            }
            ActiveListModel model = new ActiveListModel();
            model.ClubID = club.ClubId;
            model.ClubName = club.Name;
            List<Area> areas = db.Areas.Where(a=>a.State==(int)EnumState.正常).ToList();
            model.Areas = new Dictionary<string, string>();
            foreach(Area a in areas)
            {
                model.Areas.Add(a.Id.ToString(), a.Name);
            }
            return View(model);
        }
        [HttpPost,ValidateInput(false),Authorize]
        public ActionResult AddAct(ActiveListModel model)
        {
            ClubNumber club = db.ClubNumbers.Find(model.ClubID);
            UserNumber u = db.UserNumbers.Find(User.Identity.Name);
            if (club == null)
            {
                Session["Error"] = "未发现社团" + model.ClubID;
                return RedirectToAction("Error404", "Home");
            }
            //保存活动信息
            Activities Act = new Activities()
            {
                Title1 = model.Title1,
                Title2 = model.Title2,
                Content = model.Content,
                Club = club,
                CreateDate = DateTime.Now,
                User = u,
                State = (int)ActiveState.待提交,
                Votes0="0"
            };
            int type;
            int.TryParse(model.Type, out type);
            Act.Type = type;
            if (model.LabelStr.Length > 1)
            {
                Act.Label = model.LabelStr.Substring(1, model.LabelStr.Length - 1);
            }
            DateTime t1 ;
            DateTime.TryParse(model.Time1, out t1);
            if (t1 < DateTime.Now)
            {
                ModelState.AddModelError("", "活动开始时间不能小于当前时间");
                return View(model);
            }
            Act.Time1 = t1;
            DateTime.TryParse(model.Time2, out t1);
            Act.Time2 = t1;
            db.Activities.Add(Act);
            db.SaveChanges();
            return View("Index");
        }
        [HttpGet]
        public ActionResult Submit(string aid)
        {
            if (aid == null||(int.TryParse(aid,out int intaid) ==false))
            {
                return RedirectToAction("Manage");
            }
            Activities act = db.Activities.Find(intaid);
            if (act == null)
            {
                Session["Error"] = "未发现活动" + aid;
                return RedirectToAction("Error404", "Home");
            }
            if (act.User.UserId != User.Identity.Name)
            {
                Session["Error"] = "访问被拒绝！编号为" + aid + "的活动非当前登陆用户创建";
                return RedirectToAction("Error404", "Home");
            }
            ActiveSubModel model = new ActiveSubModel()
            {
                Id = act.Id,
                Type = act.Type.ToString(),
                State = Enum.GetName(typeof(ActiveState), act.State),
                Title1 = act.Title1,
                Title2 = act.Title2,
                Content = act.Content,
                Area = act.Area == null ? act.Area0 : act.Area.Name,
                Time1 = act.Time1.ToString(),
                Time2 = act.Time2.ToString(),
                MaxUser = act.MaxUser == null ? "无限制" : act.MaxUser.ToString(),
                Labels = act.Label?.Split(',').ToList()
            };

            return View(model);
        }
        [HttpPost]
        public ActionResult Submit([Bind(Include = "Id,ApplyDesc,ApplyFile")]ActiveSubModel model)
        {
            Activities act = db.Activities.Find(model.Id);
            if (act == null)
            {
                Session["Error"] = "未发现活动" + model.Id;
                return RedirectToAction("Error404", "Home");
            }
            ActiveSubModel model1 = new ActiveSubModel()
            {
                Id = act.Id,
                Type = act.Type.ToString(),
                State = Enum.GetName(typeof(ActiveState), act.State),
                Title1 = act.Title1,
                Title2 = act.Title2,
                Content = act.Content,
                Area = act.Area == null ? act.Area0 : act.Area.Name,
                Time1 = act.Time1.ToString(),
                Time2 = act.Time2.ToString(),
                MaxUser = act.MaxUser == null ? "无限制" : act.MaxUser.ToString(),
                Labels = act.Label?.Split(',').ToList()
            };
            try
            {
                if (string.IsNullOrWhiteSpace(model.ApplyFile))
                {
                    ModelState.AddModelError("", "申请任务未上传审批文件！");
                    return View(model1);
                }
                if (act.User.UserId != User.Identity.Name)
                {
                    ModelState.AddModelError("", "非用户" + User.Identity.Name + "创建的活动不能由用户" + User.Identity.Name + "提交！");
                    return View(model1);
                }
                if (act.State != (int)ActiveState.待提交)
                {
                    ModelState.AddModelError("", "请求状态错误不允许提交审批");
                    return View(model1);
                }
                ApplyType type = db.ApplyTypes.Find((int)SQType.创建活动);
                if (type == null || type.Enable != 1)
                {
                    ModelState.AddModelError("", "活动创建申请通道未启用，请联系管理员");
                    return View(model1);
                }

                ApplyAudit apply = new ApplyAudit()
                {
                    Type = db.ApplyTypes.Find((int)SQType.创建活动),
                    ApplicationDesc = model.ApplyDesc,
                    ApplicationFiled = model.ApplyFile,
                    ApplyUser = act.User,
                    Club = act.Club,
                    ApplyDate = DateTime.Now,
                    CheckState = (int)EnumAuditState.创建,
                    AuditTimes = 0
                };
                db.ApplyAudits.Add(apply);
                db.SaveChanges();

                AuditDetail audit = new AuditDetail()
                {
                    ApplyId = apply.Id,
                    CheckState = (int)EnumAuditState.创建,
                    AuditUser = act.User,
                    AuditDate = DateTime.Now
                };
                db.AuditDetails.Add(audit);

                act.State = (int)ActiveState.待审批;
                act.AuditID = apply.Id;

                db.Entry(act).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Manage", "Clubs", new { Msg = "活动编号[" + act.Id + "]一个申请已提交，牢记并使用申请任务凭证[" + apply.Id + "]查看申请进度" });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model1);
            }
        }
        [HttpPost]
        public ActionResult Add(ActiveListModel model)
        {
            return View(model);
        }
        [HttpPost, ValidateInput(false), Authorize]
        public ActionResult AddVote(int aid,string Content,int vote)
        {
            try
            {
                Activities act = db.Activities.Find(aid);
                if (act == null)
                {
                    Session["Error"] = "未发现活动" + aid;
                    return RedirectToAction("Error404", "Home");
                }
                UserNumber u = db.UserNumbers.Find(User.Identity.Name);
                //Voting oldvote = db.Votings.Where(v => v.Active.Id == act.Id && v.User.UserId == u.UserId).FirstOrDefault();
                //if (oldvote != null)
                //{
                //    throw new Exception("超过评论次数限制——你已评论/打分过该活动");
                //}
                int votenum = db.Votings.Where(v => v.Active.Id == act.Id).Count();
                double v1 = 0.0;
                double.TryParse(act.Votes0, out v1);//当前平均分v1
                double v2 = v1 * votenum;//当前总分v2
                double v3 = Math.Round((v2 + vote) / (votenum + 1), 1);//当前总分+本次评分/当前总评论数+1，为更新后的平均分
                act.Votes0 = v3.ToString();
                Voting newvote = new Voting
                {
                    Active = act,
                    User = u,
                    Desc = Content,
                    Votes = vote,
                    CreateDate = DateTime.Now,
                    State = 1,
                    Good=0,
                    Bad=0,
                    ToId=0
                };
                db.Votings.Add(newvote);
                db.Entry(act).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Detail", new { aid = aid });
            }
            catch (Exception ex)
            {
                Session["Error"] = ex.Message;
                return RedirectToAction("Error404", "Home");
            }
            
        }
    }
}