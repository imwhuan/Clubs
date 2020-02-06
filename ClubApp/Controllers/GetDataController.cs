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
        // GET: GetData
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
    }
}