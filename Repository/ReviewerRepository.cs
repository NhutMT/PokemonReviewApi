using Microsoft.EntityFrameworkCore;
using PokemonReviewApi.Data;
using PokemonReviewApi.Interfaces;
using PokemonReviewApi.Models;

namespace PokemonReviewApi.Repositories;

public class ReviewerRepository : IReviewerRepository
{
    private readonly DataContext _context;

    public ReviewerRepository(DataContext context)
    {
        _context = context;
    }

    public bool CreateReviewer(Reviewer reviewer)
    {
        _context.Add(reviewer);
        return Save();
    }

    public bool DeleteReviewer(Reviewer reviewer)
    {
        _context.Remove(reviewer);
        return Save();
    }

    public ICollection<Review> GetReviewByReviewer(int reviewerId)
    {
        return _context.Reviews.Where(r => r.Reviewer.Id == reviewerId).ToList();
    }

    public Reviewer GetReviewer(int id)
    {
        return _context.Reviewers
            .Where(r => r.Id == id)
            .Include(e => e.Reviews)
            .FirstOrDefault();
    }

    public ICollection<Reviewer> GetReviewers()
    {
        return _context.Reviewers.ToList();
    }

    public bool ReviewerExists(int id)
    {
        return _context.Reviewers.Any(r => r.Id == id);
    }

    public bool Save()
    {
        var saved = _context.SaveChanges();
        return saved > 0 ? true : false;
    }

    public bool UpdateReviewer(Reviewer reviewer)
    {
        _context.Update(reviewer);
        return Save();
    }
}
