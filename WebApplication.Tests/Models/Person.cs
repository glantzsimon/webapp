using K9.Base.DataAccessLayer.Models;
using K9.SharedLibrary.Attributes;
using System.ComponentModel.DataAnnotations.Schema;
using K9.SharedLibrary.Enums;
using K9.SharedLibrary.Models;

namespace K9.WebApplication.Tests.Models
{
    public class Person : ObjectBase
    {
        [ForeignKey("User")]
        public int UserId { get; set; }

        public virtual User User { get; set; }

        [LinkedColumn(LinkedTableName = "User", LinkedColumnName = "Username")]
        public string UserName { get; set; }

        [FileSourceInfo("images/photos", Filter = EFilesSourceFilter.Images)]
        public FileSource Photos { get; set; }
    }

}
