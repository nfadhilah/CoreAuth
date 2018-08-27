using System.ComponentModel.DataAnnotations;

namespace CoreAuth.Dtos
{
  public class UserDto
  {
    [Required]
    public string Name { get; set; }
    [Required]
    [StringLength(8, MinimumLength = 4, ErrorMessage = "Anda harus memasukkan password dengan panjang antara 4 sd 8 karakter")]
    public string Password { get; set; }
  }
}
