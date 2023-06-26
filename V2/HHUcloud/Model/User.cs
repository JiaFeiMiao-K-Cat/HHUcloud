using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HHUcloud.Model;


public enum Role
{
    User = 1,
    Administrator = 2,
    Root = 4
}

[Table("USER"), Comment("用户表")]
public class User
{
    [Key, Comment("用户Id")]
    public long UserId { get; set; }

    [Comment("用户名")]
    public string Name { get; set; }

    [Comment("生日")]
    public DateTime Birthday { get; set; }

    [Comment("邮箱地址")]
    public string Email { get; set; }

    [Comment("密码")]
    public string UserPassword { get; set; }

    [Comment("安全码")]
    public string SecurityKey { get; set; }

    [Comment("用户角色")]
    public Role UserRole { get; set; }

    [Comment("创建日期")]
    public DateTime Created { get; set; }

    [Comment("头像编号")]
    public int ProfilePhoto { get; set; }
}
