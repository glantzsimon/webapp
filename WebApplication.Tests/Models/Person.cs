using K9.Base.DataAccessLayer.Models;
using K9.SharedLibrary.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace K9.WebApplication.Tests.Models
{
    public class Person : ObjectBase
    {
        [ForeignKey("User")]
        public int UserId { get; set; }

        public virtual User User { get; set; }

        [LinkedColumn(LinkedTableName = "User", LinkedColumnName = "Username")]
        public string UserName { get; set; }
    }

}
