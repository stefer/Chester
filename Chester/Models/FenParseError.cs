﻿using System;

namespace Chester.Models;

public class FenParseError : Exception
{
    public FenParseError(string message) : base(message) { }
}
