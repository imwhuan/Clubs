namespace ClubApp.Models
{
    public class EnumData
    {
    }

    /// <summary>
    /// 社团职位
    /// </summary>
    public enum UCStatus
    {
        未知, 社长, 秘书, 优秀会员, 会员
    }
    public enum SQType
    {
        注册社团 = 1, 注销社团, 加入社团, 退出社团
    }
    public enum EnumState
    {
        未使用, 系统锁定, 待提交, 待审批, 正常, 冻结, 查封, 制裁, 已失效
    }
    public enum OnlineState
    {
        离线, 在线
    }
    public enum EnumAuditState
    {
        未知, 创建, 通过, 拒绝
    }
    public enum Gender
    {
        未知, 男, 女
    }
    public enum SendEmailType
    {
        注册验证码, 登陆验证码, 找回密码, 更改密码, 其他
    }
}