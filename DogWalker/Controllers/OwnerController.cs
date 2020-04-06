using DogWalker.Models;
using Microsoft.AspNetCore.Http;
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
    public class OwnerController : ControllerBase
    {
        private readonly IConfiguration _config;
          
        public OwnerController(IConfiguration config)
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

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    SELECT Id, OwnerName, OwnerAddress, PhoneNumber, NeighborhoodId
                    FROM DogOwner";

                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Owner> OwnerList = new List<Owner>();

                    while (reader.Read())
                    {
                        Owner owner = new Owner
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("OwnerName")),
                            PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                            Address = reader.GetString(reader.GetOrdinal("OwnerAddress")),
                            NeighborhoodId = reader.GetInt32(reader.GetOrdinal("NeighborhoodId"))
                        

                        };

                        OwnerList.Add(owner);
                    }

                    reader.Close();
                    return Ok(OwnerList);
                }
            }
        } //end get

        //get by id
        [HttpGet("{id}", Name = "GetOwner")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    SELECT o.Id, o.OwnerName, o.OwnerAddress, o.PhoneNumber, o.NeighborhoodId,
                    n.NeighborhoodName, n.Id AS NId
                    FROM DogOwner o
                    Join Neighborhood n On o.NeighborhoodId = n.Id
                    WHERE o.Id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    Owner owner = null;

                    if (reader.Read())
                    {
                        owner = new Owner
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("OwnerName")),
                            PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                            Address = reader.GetString(reader.GetOrdinal("OwnerAddress")),
                            NeighborhoodId = reader.GetInt32(reader.GetOrdinal("NId"))
                        };

                        owner.Neighborhood = new Neighborhood
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("NeighborhoodName"))

                        };

                    }
                    reader.Close();
                    return Ok(owner);
                }
            }
        }
        //end get by id


        // post new owner
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Owner owner)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    INSERT INTO DogOwner (OwnerName, NeighborhoodId, OwnerAddress, PhoneNumber)
                    OUTPUT INSERTED.Id
                    VALUES (@OwnerName,@NeighborhoodId, @OwnerAddress, @PhoneNumber)";
                    cmd.Parameters.Add(new SqlParameter("@OwnerName", owner.Name));
                    cmd.Parameters.Add(new SqlParameter("@NeighborhoodId", owner.NeighborhoodId));
                    cmd.Parameters.Add(new SqlParameter("@OwnerAddress", owner.Address));
                    cmd.Parameters.Add(new SqlParameter("@PhoneNumber", owner.PhoneNumber));
                   
                    int newId = (int)cmd.ExecuteScalar();
                    owner.Id = newId;
                    return CreatedAtRoute("GetOwner", new { id = newId }, owner);
                }
            }
        }
        //end post new owner

        //edit owner
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Owner owner)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                                            UPDATE DogOwner 
                                            SET OwnerName = @Name, 
                                            OwnerAddress = @Address,
                                            NeighborhoodId = @NeighborhoodId, 
                                            PhoneNumber = @PhoneNumber
                                            WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@Name", owner.Name));
                        cmd.Parameters.Add(new SqlParameter("@Address", owner.Address));
                        cmd.Parameters.Add(new SqlParameter("@NeighborhoodId", owner.NeighborhoodId));
                        cmd.Parameters.Add(new SqlParameter("@PhoneNumber", owner.PhoneNumber));
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }
                        throw new Exception("No rows affected");

                    }
                }
            }
            catch (Exception)
            {
                if (!OwnerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }
        //end edit owner

        private bool OwnerExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    SELECT Id, OwnerName
                    FROM DogOwner
                    WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }
        }

    } //end controller class
} //end controller
