using ClubApp.Models;
using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ClubApp.Services
{
    public class EmailSender
    {
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
            MailMessage mailMsg = new MailMessage
            {
                From = new MailAddress("Ivan.wang@searching-info.com", "一人之下"),
                //From = new MailAddress("imwhuan@qq.com", "一人之下"),//源邮件地址和发件人
                Subject = Subject,//发送邮件的标题
                Body = BodyMsg,//发送邮件的内容
                IsBodyHtml = true,
            };//实例化对象
            mailMsg.To.Add(new MailAddress(ToEmail));//收件人地址
                                                     //指定smtp服务地址（根据发件人邮箱指定对应SMTP服务器地址）
                                                     //SmtpClient client = new SmtpClient
                                                     //{
                                                     //    UseDefaultCredentials=false,
                                                     //    Host = "smtp.qq.com",
                                                     //    //要用587端口
                                                     //    Port = 587,
                                                     //    //加密
                                                     //    EnableSsl = true,
                                                     //    //通过用户名和密码验证发件人身份
                                                     //    Credentials = new NetworkCredential("imwhuan@qq.com", "uxtktpyxkdwrhihc")
                                                     //};
            SmtpClient client = new SmtpClient
            {
                UseDefaultCredentials = false,
                Host = "smtp.searching-info.com",
                Port = 25,
                EnableSsl = true,
                Credentials = new NetworkCredential("Ivan.wang@searching-info.com", "init123456")
            };
            return client.SendMailAsync(mailMsg);

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