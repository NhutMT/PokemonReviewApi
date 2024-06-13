using PokemonReviewApi.Models;

namespace PokemonReviewApi.Interfaces;

public interface IReviewRepository
{
    ICollection<Review> GetReviews();
    Review GetReview(int id);
    ICollection<Review> GetReviewsOfAPokemon(int pokemonId);
    bool ReviewExists(int id);
    bool CreateReview(Review review);
    bool UpdateReview(Review review);
    bool Save();
    bool DeleteReviews(List<Review> reviews);
    bool DeleteReview(Review review);
}
