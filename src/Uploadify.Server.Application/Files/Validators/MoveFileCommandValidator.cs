﻿using FluentValidation;
using Uploadify.Server.Application.Files.Commands;

namespace Uploadify.Server.Application.Files.Validators;

public class MoveFileCommandValidator : AbstractValidator<MoveFileCommand>;
