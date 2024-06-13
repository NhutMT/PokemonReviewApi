using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApi.Dto;
using PokemonReviewApi.Interfaces;
using PokemonReviewApi.Models;

namespace PokemonReviewApi.Controlelr;

[Route("api/[controller]")]
[ApiController]
public class PokemonController : ControllerBase
{
    private readonly IPokemonRepository _pokemonRepository;
    private readonly IOwnerRepository _ownerRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IReviewRepository _reviewRepository;
    private readonly IMapper _mapper;

    public PokemonController(IPokemonRepository pokemonRepository,
        IOwnerRepository ownerRepository,
        ICategoryRepository categoryRepository,
        IReviewRepository reviewRepository,
        IMapper mapper)
    {
        _pokemonRepository = pokemonRepository;
        _ownerRepository = ownerRepository;
        _categoryRepository = categoryRepository;
        _reviewRepository = reviewRepository;
        _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerator<Pokemon>))]
    public IActionResult GetPokemons()
    {
        var pokemons = _mapper.Map<List<PokemonDto>>(_pokemonRepository.GetPokemons());
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Ok(pokemons);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(200, Type = typeof(Pokemon))]
    [ProducesResponseType(400)]
    public IActionResult GetPokemon(int id)
    {
        if (!_pokemonRepository.PokemonExists(id))
        {
            return NotFound();
        }

        var pokemon = _mapper.Map<PokemonDto>(_pokemonRepository.GetPokemon(id));

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Ok(pokemon);
    }

    [HttpGet("{id}/rating")]
    [ProducesResponseType(200, Type = typeof(decimal))]
    [ProducesResponseType(400)]
    public IActionResult GetPokemonRating(int id)
    {
        if (!_pokemonRepository.PokemonExists(id))
        {
            return NotFound();
        }

        var rating = _pokemonRepository.GetPokemonRating(id);
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        return Ok(rating);
    }

    [HttpPost]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public IActionResult CreatePokemon([FromQuery] int ownerId, [FromQuery] int categoryId, [FromBody] PokemonDto pokemonCreate)
    {
        if (pokemonCreate == null)
        {
            return BadRequest(ModelState);
        }

        var pokemon = _pokemonRepository.GetPokemons()
        .Where(c => c.Name.Trim().ToUpper() == pokemonCreate.Name.TrimEnd().ToUpper())
        .FirstOrDefault();

        if (pokemon != null)
        {
            ModelState.AddModelError("", "Pokemon already exists");
            return StatusCode(422, ModelState);
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var pokemonMap = _mapper.Map<Pokemon>(pokemonCreate);


        if (!_pokemonRepository.CreatePokemon(ownerId, categoryId, pokemonMap))
        {
            ModelState.AddModelError("", "Something went wrong while saving");
            return StatusCode(500, ModelState);
        }

        return Ok("Successfully created");
    }

    [HttpPut("{pokemonId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public IActionResult UpdatePokemon(int pokemonId,
        [FromQuery] int ownerId,
        [FromQuery] int categoryId,
        [FromBody] PokemonDto updatePokemon)
    {
        if (updatePokemon == null)
        {
            return BadRequest(ModelState);
        }

        if (pokemonId != updatePokemon.Id)
        {
            return BadRequest(ModelState);
        }

        if (!_pokemonRepository.PokemonExists(pokemonId))
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var pokemonMap = _mapper.Map<Pokemon>(updatePokemon);
        if (!_pokemonRepository.UpdatePokemon(ownerId, categoryId, pokemonMap))
        {
            ModelState.AddModelError("", "Something went wrong updating Owner");
        }
        return Ok("Successfully Updated.");
    }

    [HttpDelete("{pokemonId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public IActionResult DeletePokemon(int pokemonId)
    {
        if (!_pokemonRepository.PokemonExists(pokemonId))
        {
            return NotFound();
        }

        var pokemonToDelete = _pokemonRepository.GetPokemon(pokemonId);
        var reviewsToDelete = _reviewRepository.GetReviewsOfAPokemon(pokemonId);
        
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        if (!_reviewRepository.DeleteReviews(reviewsToDelete.ToList()))
        {
            ModelState.AddModelError("", "Someting went wrong deleting Reviews");

        }
        if (!_pokemonRepository.DeletePokemon(pokemonToDelete))
        {
            ModelState.AddModelError("", "Someting went wrong deleting Pokemon");
        }

        return NoContent();
    }
}
