using Microsoft.AspNetCore.Mvc;
using DataExtractor.Services;
using DataExtractor.Models;

namespace DataExtractor.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DataProcessorController : ControllerBase
{
    private readonly IDataProcessorService _dataProcessorService;

    public DataProcessorController(IDataProcessorService dataProcessorService)
    {
        _dataProcessorService = dataProcessorService;
    }

    [HttpPost]
    public IActionResult ProcessData([FromBody] DataInput input)
    {
        if (input.FirstFormat == null || input.SecondFormat == null)
        {
            return BadRequest("Both FirstFormat and SecondFormat are required");
        }

        var result = _dataProcessorService.ProcessData(input.FirstFormat, input.SecondFormat);
        return Ok(result);
    }
}