using System.ComponentModel.DataAnnotations;
namespace Web.Api.Dtos
{
   
    public class MyDto
    {
        [Required(ErrorMessage = "eeeeeeeeeee")]
        public string x { get; set; }
    }
}
