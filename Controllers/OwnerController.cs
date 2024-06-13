using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApi.Dto;
using PokemonReviewApi.Interfaces;
using PokemonReviewApi.Models;

namespace PokemonReviewApi.Controller;

[Route("api/[controller]")]
[ApiController]
public class OwnerController : ControllerBase
{
    private readonly IOwnerRepository _ownerRepository;
    private readonly ICountryRepository _countryRepository;
    private readonly IMapper _mapper;

    public OwnerController(IOwnerRepository ownerRepository, ICountryRepository countryRepository, IMapper mapper)
    {
        _ownerRepository = ownerRepository;
        _countryRepository = countryRepository;
        _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerator<Owner>))]
    [ProducesResponseType(400)]
    public IActionResult GetOwners()
    {
        var owners = _mapper.Map<List<OwnerDto>>(_ownerRepository.GetOwners());
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Ok(owners);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(200, Type = typeof(Owner))]
    [ProducesResponseType(400)]
    public IActionResult GetOwner(int id)
    {
        if (!_ownerRepository.OwnerExists(id))
        {
            return NotFound();
        }

        var owner = _mapper.Map<OwnerDto>(_ownerRepository.GetOwner(id));

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Ok(owner);
    }

    [HttpGet("{ownerId}/pokemon")]
    [ProducesResponseType(200, Type = typeof(Owner))]
    [ProducesResponseType(400)]
    public IActionResult GetPokemonByOwner(int ownerId)
    {
        if (!_ownerRepository.OwnerExists(ownerId))
        {
            return NotFound();
        }
        var pokemons = _mapper.Map<List<PokemonDto>>(
                _ownerRepository.GetPokemonByOwner(ownerId));
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Ok(pokemons);
    }
    [HttpPost]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public IActionResult CreateOwner([FromQuery] int countryId, [FromBody] OwnerDto ownerCreate)
    {
        if (ownerCreate == null)
        {
            return BadRequest(ModelState);
        }

        var owner = _ownerRepository.GetOwners()
        .Where(o => (o.FirstName + o.LastName).Trim().ToUpper() == (ownerCreate.FirstName + ownerCreate.LastName).TrimEnd().ToUpper())
        .FirstOrDefault();

        if (owner != null)
        {
            ModelState.AddModelError("", "Owner already exists");
            return StatusCode(422, ModelState);
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var ownerMap = _mapper.Map<Owner>(ownerCreate);
        ownerMap.Country = _countryRepository.GetCountry(countryId);
        if (!_ownerRepository.CreateOwner(ownerMap))
        {
            ModelState.AddModelError("", "Something went wrong while saving");
            return StatusCode(500, ModelState);
        }

        return Ok("Successfully created");
    }

    [HttpPut("{ownerId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public IActionResult UpdateOwner(int ownerId, [FromBody] OwnerDto updateOwner)
    {
        if (updateOwner == null)
        {
            return BadRequest(ModelState);
        }

        if (ownerId != updateOwner.Id)
        {
            return BadRequest(ModelState);
        }

        if (!_ownerRepository.OwnerExists(ownerId))
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var ownerMap = _mapper.Map<Owner>(updateOwner);
        if (!_ownerRepository.UpdateOwner(ownerMap))
        {
            ModelState.AddModelError("", "Something went wrong updating Owner");
        }
        return Ok("Successfully Updated.");
    }

    [HttpDelete("{ownerId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public IActionResult DeleteOwner(int ownerId)
    {
        if (!_ownerRepository.OwnerExists(ownerId))
        {
            return NotFound();
        }

        var ownerToDelete = _ownerRepository.GetOwner(ownerId);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!_ownerRepository.DeleteOwner(ownerToDelete))
        {
            ModelState.AddModelError("", "Someting went wrong deleting Owner");
        }

        return NoContent();
    }
}
