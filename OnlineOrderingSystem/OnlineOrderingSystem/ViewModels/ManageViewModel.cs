using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;


public class ManageViewModel
{


    public string NewUsername { get; set; }

    public string CurrentUserName { get; set; }
    // Email fields
    [Display(Name = "Current Email")]
    public string CurrentEmail { get; set; }

    [Display(Name = "New Email")]
    [EmailAddress]
    public string NewEmail { get; set; }

    // Password fields
    [Display(Name = "Current Password")]
    [DataType(DataType.Password)]
    public string CurrentPassword { get; set; }

    [Display(Name = "New Password")]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; }

    [Display(Name = "Confirm New Password")]
    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
    public string ConfirmNewPassword { get; set; }

    // Avatar fields
    [Display(Name = "Current Avatar")]
    public string CurrentAvatar { get; set; } 

    [Display(Name = "New Avatar")]
    public IFormFile NewAvatar { get; set; }
}

