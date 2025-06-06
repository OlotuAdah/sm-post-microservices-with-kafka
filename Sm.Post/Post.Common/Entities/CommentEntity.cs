
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Post.Common.Entities;

[Table("Comment", Schema = "dbo")]
public class CommentEntity
{
    [Key]
    public Guid CommentId { get; set; } //PK
    public Guid PostId { get; set; } //FK to Post entity
    public string Username { get; set; }
    public string Comment { get; set; }
    public DateTime CommentDate { get; set; }
    public bool Edited { get; set; }

    [System.Text.Json.Serialization.JsonIgnore] // Prevent circular reference when serializing to JSON
    public virtual PostEntity Post { get; set; }
}