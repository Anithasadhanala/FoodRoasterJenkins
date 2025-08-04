using FoodRoasterServer.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class UserMealRegistration
{
    [Key]
    public int Id { get; set; }
    public int UserId { get; set; }
    public int FoodMenuId { get; set; }
    [Required]
    public DateTime RegisteredAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    [Required]
    public bool IsVegChoice { get; set; }
    public User User { get; set; }
    public FoodMenu FoodMenu { get; set; }
}
