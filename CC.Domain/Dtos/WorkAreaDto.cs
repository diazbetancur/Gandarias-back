﻿namespace CC.Domain.Dtos;

public class WorkAreaDto : BaseDto<Guid>
{
    public string Name { get; set; }
    public bool? IsActive { get; set; }
}