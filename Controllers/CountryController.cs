using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApi.Dto;
using PokemonReviewApi.Interfaces;
using PokemonReviewApi.Models;

namespace PokemonReviewApi.Controlelr;

[Route("api/[controller]")]
[ApiController]
public class CountryController : ControllerBase
{
    private readonly ICountryRepository _countryRepository;
    private readonly IMapper _mapper;

    public CountryController(ICountryRepository countryRepository, IMapper mapper)
    {
        _countryRepository = countryRepository;
        _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerator<Country>))]
    public IActionResult GetCountries()
    {
        var countries = _mapper.Map<List<CountryDto>>(_countryRepository.GetCountries());
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Ok(countries);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(200, Type = typeof(Country))]
    [ProducesResponseType(400)]
    public IActionResult GetCountry(int id)
    {
        if (!_countryRepository.CountryExists(id))
        {
            return NotFound();
        }

        var country = _mapper.Map<CountryDto>(_countryRepository.GetCountry(id));

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Ok(country);
    }

    [HttpGet("/owners/{ownerId}")]
    [ProducesResponseType(200, Type = typeof(Country))]
    [ProducesResponseType(400)]
    public IActionResult GetCountryOfAnOwner(int ownerId)
    {
        var country = _mapper.Map<CountryDto>(
            _countryRepository.GetCountryByOwner(ownerId));
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Ok(country);
    }

    [HttpGet("/countries/{countryId}")]
    [ProducesResponseType(200, Type = typeof(Owner))]
    [ProducesResponseType(400)]
    public IActionResult GetOwnersFromACountry(int countryId)
    {
        var owners = _mapper.Map<List<OwnerDto>>(
            _countryRepository.GetOwnersFromACountry(countryId));
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Ok(owners);
    }

    [HttpPost]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public IActionResult CreateCountry([FromBody] CountryDto countryCreate)
    {
        if (countryCreate == null)
        {
            return BadRequest(ModelState);
        }

        var country = _countryRepository.GetCountries()
        .Where(c => c.Name.Trim().ToUpper() == countryCreate.Name.TrimEnd().ToUpper())
        .FirstOrDefault();

        if (country != null)
        {
            ModelState.AddModelError("", "Country already exists");
            return StatusCode(422, ModelState);
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var countryMap = _mapper.Map<Country>(countryCreate);
        if (!_countryRepository.CreateCountry(countryMap))
        {
            ModelState.AddModelError("", "Something went wrong while saving");
            return StatusCode(500, ModelState);
        }

        return Ok("Successfully created");
    }

    [HttpPut("{countryId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public IActionResult UpdateCountry(int countryId, [FromBody] CountryDto updateCountry)
    {
        if (updateCountry == null)
        {
            return BadRequest(ModelState);
        }

        if (countryId != updateCountry.Id)
        {
            return BadRequest(ModelState);
        }

        if (!_countryRepository.CountryExists(countryId))
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var countryMap = _mapper.Map<Country>(updateCountry);
        if (!_countryRepository.UpdateCountry(countryMap))
        {
            ModelState.AddModelError("", "Something went wrong updating Country");
        }
        return Ok("Successfully Updated.");
    }

    [HttpDelete("{countryId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public IActionResult DeleteCountry(int countryId)
    {
        if (!_countryRepository.CountryExists(countryId))
        {
            return NotFound();
        }

        var countryToDelete = _countryRepository.GetCountry(countryId);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!_countryRepository.DeleteCountry(countryToDelete))
        {
            ModelState.AddModelError("", "Someting went wrong deleting Country");
        }

        return NoContent();
    }
}
