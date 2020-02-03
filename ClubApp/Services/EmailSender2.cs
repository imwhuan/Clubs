using ClubApp.Models;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mail;

namespace ClubApp.Services
{
    public class EmailSender2
    {
        [Obsolete]
        public Task SendEmailAsync(string ToEmail, string Subject, string BodyMsg, SendEmailType type = SendEmailType.其他)
        {
            if (type == SendEmailType.其他)
            {

            }
            else if (type == SendEmailType.注册验证码)
            {
                string[] s1 = ToEmail.Split('@');
                if (s1.Length > 2)
                {
                    throw new Exception("邮箱" + ToEmail + "格式非法");
                }
                string s10 = s1[0];
                string email2 = "";
                if (s10.Length > 4)
                {
                    email2 = s10.Substring(0, 2);
                    email2 += "*****";
                    email2 += s10.Substring(s10.Length - 2, 2);
                }
                else
                {
                    email2 = s10;
                }
                email2 += "@" + s1[1];
                BodyMsg = CreateEmailBody(email2, BodyMsg);
            }
            //MailMessage mailobj = new MailMessage();
            MailMessage mailobj = new MailMessage();
            //{
            //    From = "imwhuan@qq.com",
            //    //From = new MailAddress("imwhuan@qq.com", "一人之下"),//源邮件地址和发件人
            //    Subject = Subject,//发送邮件的标题
            //    Body = BodyMsg,//发送邮件的内容
            //    BodyFormat=MailFormat.Html
            //};//实例化对象
            //mailMsg.To = ToEmail;
            //指定smtp服务地址（根据发件人邮箱指定对应SMTP服务器地址）
            //SmtpMail.Send(mailMsg);
            //return client.SendMailAsync(mailMsg);

            ////QQ邮箱
            //mailobj.From = new MailAddress("imwhuan@qq.com");
            //mailobj.To.Add(new MailAddress("Ivan.wang@searching-info.com"));
            //mailobj.Priority = MailPriority.Normal;
            //mailobj.Subject = "测试邮件发送";
            //mailobj.Body = "正文发送的地方";
            //mailobj.IsBodyHtml = false;
            //SmtpClient sc = new SmtpClient();
            //sc.Host = "smtp.qq.com";
            //sc.Port = 465;
            //sc.EnableSsl = true;
            //sc.UseDefaultCredentials = false;
            //sc.Credentials = new NetworkCredential("imwhuan@qq.com", "hkefwggvoagwgigj");
            //sc.Send(mailobj);
            //ViewBag.Res = "发送成功";

            mailobj.To = ToEmail;
            mailobj.From = "1335322586@qq.com";
            mailobj.Subject = Subject;
            mailobj.BodyFormat = MailFormat.Html;
            mailobj.Body = BodyMsg;

            mailobj.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpauthenticate", "1"); //身份验证
            mailobj.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendusername", mailobj.From); //邮箱登录账号，这里跟前面的发送账号一样就行
            mailobj.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendpassword", "xhqwqsvywgxtjaee"); //这个密码要注意：如果是一般账号，要用授权码；企业账号用登录密码
            mailobj.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpserverport", 465);//端口
            mailobj.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpusessl", "true");//SSL加密
            SmtpMail.SmtpServer = "smtp.qq.com";    //企业账号用smtp.exmail.qq.com
            //SmtpMail.Send(mailobj);

            return Task.Run(() => SmtpMail.Send(mailobj));


        }
        private string CreateEmailBody(string email, string code)
        {
            StringBuilder codestr = new StringBuilder();
            codestr.Append("<div id='qm_con_body'><div id='mailContentContainer' class='qmbox qm_con_body_content qqmail_webmail_only'><style>.qmbox body {line-height: 1.5;}.qmbox body {font-size: 10.5pt;font-family: 微软雅黑;color: rgb(0, 0, 0);line-height: 1.5;}");
            codestr.Append(".t1{padding:0; font-family:'Segoe UI Semibold', 'Segoe UI Bold', 'Segoe UI', 'Helvetica Neue Medium', Arial, sans-serif; font-size:17px; color:#707070;}.t2{ padding:0; font-family:'Segoe UI Light', 'Segoe UI', 'Helvetica Neue Medium', Arial, sans-serif; font-size:41px; color:#2672ec;}");
            codestr.Append(".t3{padding:0; padding-top:25px; font-family:'Segoe UI', Tahoma, Verdana, Arial, sans-serif; font-size:14px; color:#2a2a2a;}.s1{font-family:'Segoe UI Bold', 'Segoe UI Semibold', 'Segoe UI', 'Helvetica Neue Medium', Arial, sans-serif; font-size:14px; font-weight:bold; color:#2a2a2a;}</style>");
            codestr.Append("<table dir='ltr'><tbody><tr><td id='i1' class='t1'>Times 帐户</td></tr><tr><td id='i2' class='t2'>邮箱验证码</td></tr><tr><td id='i3' class='t3'>你的 Times 帐户 <a dir='ltr' id='iAccount' class='link' style='color: rgb(38,114,236); text-decoration: none;' href='mailto:");
            codestr.Append(email);
            codestr.Append("' rel='noopener' target='_blank'>");
            codestr.Append(email);
            codestr.Append("</a> 邮箱验证码为：</td></tr><tr><td id='i4' class='t3'><span class='s1'><span style='border-bottom:1px dashed #ccc;z-index:1' t='7' onclick='return false;' data='");
            codestr.Append(code);
            codestr.Append("'>");
            codestr.Append(code);
            codestr.Append("</span></span></td></tr><tr><td id='i5' class='t3'><br></td></tr><tr><td id='i6' class='t3'>谢谢您的使用!</td></tr></tbody></table><style type='text/css'>.qmbox style, .qmbox script, .qmbox head, .qmbox link, .qmbox meta {display: none !important;}</style></div></div>");
            return codestr.ToString();
        }
    }
}