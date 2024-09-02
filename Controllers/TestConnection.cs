using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using testProd.Model;

namespace testProd
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestCon : ControllerBase
    {
        private readonly DataContext _dataContext;
        public TestCon(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet("TestConnection")]
        public ActionResult<List<User>> TestConnection()
        {
            
                var users = _dataContext.Users.FromSqlRaw("SELECT * FROM dbo.Users").ToList();
                return Ok(users); 
           
        }

    }
}