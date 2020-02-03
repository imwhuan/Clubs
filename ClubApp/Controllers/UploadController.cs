using ClubApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace ClubApp.Controllers
{
    public class UploadController : Controller
    {
        AppDbContext db = new AppDbContext();
        // GET: Upload
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ClubHeadImg(string cid)
        {
            try
            {
                List<string> imgtype = new List<string>(new string[] { ".jpg", ".gif", ".png" });
                HttpFileCollectionBase file = Request.Files;
                JsonData data = new JsonData()
                {
                    code = 1,
                    src = "",
                    name = "",
                    msg = ""
                };
                ClubNumber club = db.ClubNumbers.Find(cid);
                if (club == null)
                {
                    data.msg += "未发现有效的社团";
                }
                else if (file == null || file.Count < 1)
                {
                    data.msg += "未成功接收文件";
                }
                else if (!imgtype.Contains(Path.GetExtension(file[0].FileName)))
                {
                    data.msg = "文件格式错误（jpg/gif/png）";
                }
                else if (file[0].ContentLength > 2048000)
                {
                    data.msg += "文件大小不允许超过2M";
                }
                else
                {
                    int a = club.State ?? 0;
                    if (a == (int)EnumState.待审批)
                    {
                        throw new Exception("正在审批中的社团不允许更新信息");
                    }
                    string filepath = Server.MapPath("~/Content/upload/clubimg/");
                    if (!Directory.Exists(filepath))
                    {
                        Directory.CreateDirectory(filepath);
                    }
                    string name = Path.GetFileName(file[0].FileName);
                    if (name.Length > 10)
                    {
                        name = name.Substring(name.Length - 10);
                    }
                    //string ext = Path.GetExtension(name);
                    file[0].SaveAs(filepath + cid + "_head_" + name);
                    club.HeadImg = "Content/upload/clubimg/" + cid + "_head_" + name;
                    db.Entry(club).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    data.code = 0;
                    data.src = "Content/upload/clubimg /" + cid + "_head_" + name;
                    data.msg += "保存成功";
                }
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                JsonData data = new JsonData()
                {
                    code = 4,
                    src = "",
                    name = "",
                    msg = ex.Message
                };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ApplyClubFile(string cid)
        {
            try
            {
                HttpFileCollectionBase file = Request.Files;
                JsonData data = new JsonData()
                {
                    code = 1,
                    src = "",
                    name = "",
                    msg = ""
                };
                if (file == null)
                {
                    data.msg += "未成功接收文件";
                }
                else if (file.Count > 0)
                {
                    if (file[0].ContentLength > 2048000)
                    {
                        data.msg += "上传文件大小超过2M！不允许上传";
                    }
                    else if (string.IsNullOrEmpty(cid))
                    {
                        data.msg += "未识别到正确的社团账号，文件不被接收！";
                    }
                    else
                    {
                        ClubNumber club = db.ClubNumbers.Find(cid);
                        if (club == null)
                        {
                            data.msg = "未识别到正确的社团账号，文件不被接收！";
                        }
                        else if (club.State != (int)EnumState.待提交)
                        {
                            data.msg = cid + "社团当前状态不允许上传审批文件";
                        }
                        else
                        {
                            string filepath = Server.MapPath("~/Content/upload/apply/");
                            if (!Directory.Exists(filepath))
                            {
                                Directory.CreateDirectory(filepath);
                            }
                            string name = Path.GetFileName(file[0].FileName);
                            //string ext = Path.GetExtension(name);
                            file[0].SaveAs(filepath + cid + "_" + name);
                            data.src = "Content/upload/apply/" + cid + "_" + name;
                            data.name = cid + "_" + name;
                            data.code = 0;
                            data.msg += "上传成功！";
                        }
                    }
                }
                else
                {
                    data.msg = "错误！服务器接收到上载文件的个数为：" + file.Count;
                }
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                JsonData data = new JsonData()
                {
                    code = 4,
                    src = "",
                    name = "",
                    msg = ex.Message
                };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }
        public class JsonData
        {
            public int code { get; set; }
            public string src { get; set; }
            public string name { get; set; }
            public string msg { get; set; }
        }
    }
}