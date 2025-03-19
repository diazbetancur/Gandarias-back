﻿namespace CC.Domain.Dtos;

public class BaseDto<T>
{
    /// <summary>
    /// Id
    /// </summary>
    public T? Id { get; set; }

    /// <summary>
    /// Date created
    /// </summary>
    public DateTime? DateCreated { get; set; }
}