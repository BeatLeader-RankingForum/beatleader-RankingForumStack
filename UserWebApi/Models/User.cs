using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserWebApi.Models
{
    [Table("User", Schema = "dbo")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("entry_id")]
        public int EntryId { get; set; }

        [Column("user_id")]
        public string UserId { get; set; }

        [Column("username")]
        public string UserName { get; set; }

    }
}
