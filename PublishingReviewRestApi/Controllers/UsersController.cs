using Microsoft.AspNetCore.Mvc;
using PublishingReviewContracts.BindingModel;
using PublishingReviewContracts.BusinessLogicContracts;
using PublishingReviewContracts.SearchModels;
using PublishingReviewRestApi.Models.Dto;

namespace PublishingReviewRestApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserLogic _userLogic;

    public UsersController(IUserLogic userLogic)
    {
        _userLogic = userLogic;
    }

    [HttpGet]
    public IActionResult GetAll() => Ok(_userLogic.ReadList(null) ?? new());

    [HttpGet("{id:int}")]
    public IActionResult Get(int id)
    {
        var user = _userLogic.ReadElement(new UserSearchModel { Id = id });
        return user == null ? NotFound() : Ok(user);
    }

    [HttpPost]
    public IActionResult Create([FromBody] UserCreateDto dto)
    {
        var model = new UserBindingModel
        {
            FullName = dto.FullName,
            Username = dto.Username,
            Email = dto.Email,
            Password = dto.Password,
            Role = dto.Role
        };

        _userLogic.Create(model);

        var created = _userLogic.ReadElement(new UserSearchModel { Email = dto.Email });
        return created == null
            ? Ok()
            : CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] UserCreateDto dto)
    {
        var model = new UserBindingModel
        {
            Id = id,
            FullName = dto.FullName,
            Username = dto.Username,
            Email = dto.Email,
            Password = dto.Password,
            Role = dto.Role
        };

        _userLogic.Update(model);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        _userLogic.Delete(new UserBindingModel { Id = id });
        return NoContent();
    }
}
