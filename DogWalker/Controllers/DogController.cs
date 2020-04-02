using DogWalker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DogWalker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DogController : ControllerBase
    {
        private readonly IConfiguration _config;

        public DogController(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        public async Task<IActionResult> Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    SELECT d.Id, d.DogName, d.DogOwnerId, d.Breed, d.Notes, o.Id As OwnerId, o.OwnerName, o.NeighborhoodId
                    FROM Dog d
                    Left Join DogOwner o On d.DogOwnerId = o.Id
                    Where 1 = 1"; //provides a default so adds work in query string  

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Dog> dogs = new List<Dog>();

                    while (reader.Read())
                    {
                        int id = reader.GetInt32(reader.GetOrdinal("Id"));
                        int OwnerId = reader.GetInt32(reader.GetOrdinal("OwnerId"));
                        string Breed = reader.GetString(reader.GetOrdinal("Breed"));
                        string Notes = reader.GetString(reader.GetOrdinal("Notes"));
                        string DogName = reader.GetString(reader.GetOrdinal("DogName"));

                        Dog dog = new Dog
                        {
                            Id = id,
                            DogOwnerId = OwnerId,
                            DogName = DogName,
                            Breed = Breed,
                            Notes = Notes
                        };

                        dogs.Add(dog);
                    }
                    reader.Close();
                    return Ok(dogs);
                }
            }
        }


        } //end of controller
}
