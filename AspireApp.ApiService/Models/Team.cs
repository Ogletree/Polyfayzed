﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace AspireApp.ApiService.Models;

public partial class Team
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string League { get; set; }

    public string Record { get; set; }

    public string Logo { get; set; }

    public string Abbreviation { get; set; }

    public string Alias { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}