using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pokedex.Domain.Models;

public sealed record Pokemon(
    string Name,
    string Description,
    string Habitat,
    bool IsLegendary);