using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CoreAuth.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class ValuesController : ControllerBase
  {
    private readonly IDbConnection _connection;

    public ValuesController(IConfiguration configuration)
    {
      _connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
    }
    // GET api/values
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery]string search)
    {
      using (_connection)
      {
        var users = await _connection.QueryAsync("SELECT * FROM Users ORDER BY Id");

        return Ok(users);
      }
    }

    // GET api/values/5
    [HttpGet("{id}")]
    public ActionResult<string> Get(int id)
    {
      if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
        return Unauthorized();

      return "value";
    }

    // POST api/values
    [HttpPost]
    public string Post([FromBody] string value)
    {
      return value;
    }

    // PUT api/values/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
    }

    // DELETE api/values/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
  }
}
