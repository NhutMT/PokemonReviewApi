using PokemonReviewApi.Data;
using PokemonReviewApi.Interfaces;
using PokemonReviewApi.Models;

namespace PokemonReviewApi.Repositories;

public class OwnerRepository : IOwnerRepository
{
    private readonly DataContext _context;

    public OwnerRepository(DataContext context)
    {
        _context = context;
    }

    public bool CreateOwner(Owner owner)
    {
        _context.Add(owner);
        return Save();
    }

    public bool DeleteOwner(Owner owner)
    {
        _context.Remove(owner);
        return Save();
    }

    public Owner GetOwner(int ownerId)
    {
        return _context.Owners.FirstOrDefault(o => o.Id == ownerId);
    }

    public ICollection<Owner> GetOwnerOfAPokemon(int pokemonId)
    {
        return _context.PokemonOwners
            .Where(p => p.Pokemon.Id == pokemonId)
            .Select(o => o.Owner)
            .ToList();
    }

    public ICollection<Owner> GetOwners()
    {
        return _context.Owners.ToList();
    }

    public ICollection<Pokemon> GetPokemonByOwner(int ownerId)
    {
        return _context.PokemonOwners
            .Where(p => p.Owner.Id == ownerId)
            .Select(p => p.Pokemon)
            .ToList();
    }

    public bool OwnerExists(int ownerId)
    {
        return _context.Owners.Any(o => o.Id == ownerId);
    }

    public bool Save()
    {
        var saved = _context.SaveChanges();
        return saved > 0 ? true : false;
    }

    public bool UpdateOwner(Owner owner)
    {
        _context.Update(owner);
        return Save();
    }
}
