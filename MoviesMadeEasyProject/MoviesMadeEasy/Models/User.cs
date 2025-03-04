﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
namespace MoviesMadeEasy.Models;

public partial class User : IdentityUser
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public Guid? StreamingServicesId { get; set; }

    public Guid? RecentlyViewedShowId { get; set; }

    public virtual Title? RecentlyViewedShow { get; set; }

    public virtual StreamingService? StreamingServices { get; set; }
}
