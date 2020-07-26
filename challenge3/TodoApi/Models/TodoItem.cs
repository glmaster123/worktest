using System.ComponentModel.DataAnnotations;
public class TodoItem
{
    public long Id { get; set; }
    [Required]
    public string Name { get; set; }
    public bool IsComplete { get; set; }
    [Timestamp]
    public byte[] Version { get; set; }
}