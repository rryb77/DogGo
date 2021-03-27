using DogGo.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DogGo.Repositories
{
    public class WalkRepository : IWalkRepository
    {
        private readonly IConfiguration _config;

        public WalkRepository(IConfiguration config)
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

        public List<Walk> GetWalksByWalkerId(int walkerId)
        {
                        
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                                       SELECT wk.Id, w.Id AS WalkerId, d.Id AS DogId, w.[Name], wk.[Date], wk.Duration, o.[Name] AS Owner
                                        FROM Walker w
                                        LEFT JOIN Walks wk on wk.WalkerId = w.Id
                                        LEFT JOIN Dog d on wk.DogId = d.Id
                                        LEFT JOIN [Owner] o on o.Id = d.OwnerId
                                        WHERE w.id = @id
                                        ORDER BY o.name ASC
                                       ";
                    cmd.Parameters.AddWithValue("@id", walkerId);

                    SqlDataReader reader = cmd.ExecuteReader();
                    Walk walk = null;
                    List<Walk> walks = new List<Walk>();

                    while (reader.Read())
                    {
                        walk = new Walk
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                            Duration = reader.GetInt32(reader.GetOrdinal("Duration")) / 60,
                            WalkerId = reader.GetInt32(reader.GetOrdinal("WalkerId")),
                            DogId = reader.GetInt32(reader.GetOrdinal("DogId")),
                            Owner = reader.GetString(reader.GetOrdinal("Owner"))
                        };

                        walks.Add(walk);
                    }

                    reader.Close();
                    return walks;
                }
            }
        }
    }
}
