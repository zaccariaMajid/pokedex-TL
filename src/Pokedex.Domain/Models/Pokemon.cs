using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pokedex.Domain.Models;

public class Pokemon
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Habitat { get; set; } = string.Empty;
    public bool IsLegendary { get; set; }
}
