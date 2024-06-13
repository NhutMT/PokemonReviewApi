using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApi.Dto;
using PokemonReviewApi.Interfaces;
using PokemonReviewApi.Models;

namespace PokemonReviewApi.Controlelr;

[Route("api/[controller]")]
[ApiController]
public class ReviewController : ControllerBase
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IPokemonRepository _pokemonRepository;
    private readonly IReviewerRepository _reviewerRepository;
    private readonly IMapper _mapper;

    public ReviewController(IReviewRepository reviewRepository, IPokemonRepository pokemonRepository, IReviewerRepository reviewerRepository, IMapper mapper)
    {
        _reviewRepository = reviewRepository;
        _pokemonRepository = pokemonRepository;
        _reviewerRepository = reviewerRepository;
        _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerator<Review>))]
    [ProducesResponseType(400)]
    public IActionResult GetReviews()
    {
        var reviews = _mapper.Map<List<ReviewDto>>(_reviewRepository.GetReviews());
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Ok(reviews);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(200, Type = typeof(Review))]
    [ProducesResponseType(400)]
    public IActionResult GetReview(int id)
    {
        if (!_reviewRepository.ReviewExists(id))
        {
            return NotFound();
        }

        var review = _mapper.Map<ReviewDto>(_reviewRepository.GetReview(id));

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Ok(review);
    }

    [HttpGet("pokemon/{pokemonId}")]
    [ProducesResponseType(200, Type = typeof(Review))]
    [ProducesResponseType(400)]
    public IActionResult GetReviewForAPokemon(int pokemonId)
    {
        var reviews = _mapper.Map<List<ReviewDto>>(
            _reviewRepository.GetReviewsOfAPokemon(pokemonId)
        );

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Ok(reviews);
    }

    [HttpPost]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public IActionResult CreateReview([FromQuery] int reviewerId, [FromQuery] int pokemonId, [FromBody] ReviewDto reviewCreate)
    {
        if (reviewCreate == null)
        {
            return BadRequest(ModelState);
        }

        var review = _reviewRepository.GetReviews()
        .Where(r => r.Title.Trim().ToUpper() == reviewCreate.Title.TrimEnd().ToUpper())
        .FirstOrDefault();

        if (review != null)
        {
            ModelState.AddModelError("", "Review already exists");
            return StatusCode(422, ModelState);
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var reviewMap = _mapper.Map<Review>(reviewCreate);

        reviewMap.Pokemon = _pokemonRepository.GetPokemon(pokemonId);
        reviewMap.Reviewer = _reviewerRepository.GetReviewer(reviewerId);

        if (!_reviewRepository.CreateReview(reviewMap))
        {
            ModelState.AddModelError("", "Something went wrong while saving");
            return StatusCode(500, ModelState);
        }

        return Ok("Successfully created");
    }

    [HttpPut("{reviewId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public IActionResult Updatereview(int reviewId, [FromBody] ReviewDto updateReview)
    {
        if (updateReview == null)
        {
            return BadRequest(ModelState);
        }

        if (reviewId != updateReview.Id)
        {
            return BadRequest(ModelState);
        }

        if (!_reviewRepository.ReviewExists(reviewId))
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var reviewMap = _mapper.Map<Review>(updateReview);
        if (!_reviewRepository.UpdateReview(reviewMap))
        {
            ModelState.AddModelError("", "Something went wrong updating Review");
        }
        return Ok("Successfully Updated.");
    }

    [HttpDelete("{reviewId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public IActionResult DeleteReview(int reviewId)
    {
        if (!_reviewRepository.ReviewExists(reviewId))
        {
            return NotFound();
        }

        var reviewToDelete = _reviewRepository.GetReview(reviewId);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!_reviewRepository.DeleteReview(reviewToDelete))
        {
            ModelState.AddModelError("", "Someting went wrong deleting Review");
        }

        return NoContent();
    }
}
