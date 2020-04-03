using DogWalker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

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
                    SELECT d.Id, d.DogName, d.DogOwnerId, d.Breed, d.Notes, o.Id As OwnerId, o.OwnerName, o.NeighborhoodId, o.PhoneNumber
                    FROM Dog d
                    Left Join DogOwner o On d.DogOwnerId = o.Id
                    Where 1 = 1"; //provides a default so adds work in query string  

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Dog> dogs = new List<Dog>();

                    while (reader.Read())
                    {
                        int id = reader.GetInt32(reader.GetOrdinal("Id"));
                        int OwnerId = reader.GetInt32(reader.GetOrdinal("OwnerId"));
                        string PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber"));
                        string Breed = reader.GetString(reader.GetOrdinal("Breed"));
                        string Notes = reader.GetString(reader.GetOrdinal("Notes"));
                        string DogName = reader.GetString(reader.GetOrdinal("DogName"));
                        string OwnerName = reader.GetString(reader.GetOrdinal("OwnerName"));
                        int NeighborhoodId = reader.GetInt32(reader.GetOrdinal("NeighborhoodId"));


                        Owner owner = new Owner
                        {
                            Id = OwnerId,
                            Name = OwnerName,
                            PhoneNumber = PhoneNumber,
                            NeighborhoodId = NeighborhoodId

                        };

                        Dog dog = new Dog
                        {
                            Id = id,
                            DogOwnerId = OwnerId,
                            Owner  = owner,
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

        //get by id
        [HttpGet("{id}", Name = "GetDog")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    SELECT d.Id, d.DogName, d.DogOwnerId, d.Breed, d.Notes, o.Id As OwnerId, o.OwnerName, o.NeighborhoodId, o.PhoneNumber
                    FROM Dog d
                    Join DogOwner o On d.DogOwnerId = o.Id
                    WHERE d.Id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    Dog dog = null;

                    if (reader.Read())
                    {
                        int dogId = reader.GetInt32(reader.GetOrdinal("Id"));
                        int OwnerId = reader.GetInt32(reader.GetOrdinal("OwnerId"));
                        string PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber"));
                        string Breed = reader.GetString(reader.GetOrdinal("Breed"));
                        string Notes = reader.GetString(reader.GetOrdinal("Notes"));
                        string DogName = reader.GetString(reader.GetOrdinal("DogName"));
                        string OwnerName = reader.GetString(reader.GetOrdinal("OwnerName"));
                        int NeighborhoodId = reader.GetInt32(reader.GetOrdinal("NeighborhoodId"));


                        Owner owner = new Owner
                        {
                            Id = OwnerId,
                            Name = OwnerName,
                            PhoneNumber = PhoneNumber,
                            NeighborhoodId = NeighborhoodId

                        };

                        dog = new Dog
                        {
                            Id = id,
                            DogOwnerId = OwnerId,
                            Owner = owner,
                            DogName = DogName,
                            Breed = Breed,
                            Notes = Notes
                        };

                    }
                    reader.Close();
                    return Ok(dog);
                }
            }
        }
        //end get by id

        // post new dog
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Dog dog)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    INSERT INTO Dog (DogName, Breed, DogOwnerId, Notes)
                    OUTPUT INSERTED.Id
                    VALUES (@DogName, @Breed, @DogOwnerId, @Notes)";
                    cmd.Parameters.Add(new SqlParameter("@DogName", dog.DogName));
                    cmd.Parameters.Add(new SqlParameter("@Breed", dog.Breed));
                    cmd.Parameters.Add(new SqlParameter("@DogOwnerId", dog.DogOwnerId));
                    cmd.Parameters.Add(new SqlParameter("@Notes", dog.Notes));

                    int newId = (int)cmd.ExecuteScalar();
                    dog.Id = newId;
                    return CreatedAtRoute("GetDog", new { id = newId }, dog);
                }
            }
        }

        //end post new dog

        //edit dog

        //end edit dog

        //delete dog
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                        DELETE FROM Dog
                        WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }
                        throw new Exception("No Rows Affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!DogExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

        }

        //end delete dog

        private bool DogExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    SELECT Id, DogName
                    FROM Dog
                    WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }
        }


    } //end of controller
}
