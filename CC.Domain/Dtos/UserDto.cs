﻿namespace CC.Domain.Dtos;

public class UserDto
{
    public Guid Id { get; set; }
    public string DNI { get; set; }
    public string? Password { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? RolName { get; set; }
    public bool? IsActive { get; set; }
    public string NickName { get; set; }
    public DateTime? HireDate { get; set; }
    public Guid? HireTypeId { get; set; }
    public string? HireTypeName { get; set; }
}