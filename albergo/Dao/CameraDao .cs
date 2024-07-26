using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using albergo.DAO;
using albergo.Models;

namespace Albergo.DAO
{
    public class CameraDao : ICameraDao
    {
        private readonly string _connectionString;

        public CameraDao(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IEnumerable<Camera> GetAll()
        {
            var camere = new List<Camera>();

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var query = "SELECT * FROM Camere";
                using (var cmd = new SqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var camera = new Camera
                        {
                            Numero = reader.GetInt32(reader.GetOrdinal("Numero")),
                            Descrizione = reader.IsDBNull(reader.GetOrdinal("Descrizione")) ? null : reader.GetString(reader.GetOrdinal("Descrizione")),
                            Tipologia = reader.IsDBNull(reader.GetOrdinal("Tipologia")) ? null : reader.GetString(reader.GetOrdinal("Tipologia"))
                        };
                        camere.Add(camera);
                    }
                }
            }

            return camere;
        }

        public Camera GetById(int numero)
        {
            Camera camera = null;

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var query = "SELECT * FROM Camere WHERE Numero = @Numero";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Numero", numero);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            camera = new Camera
                            {
                                Numero = reader.GetInt32(reader.GetOrdinal("Numero")),
                                Descrizione = reader.IsDBNull(reader.GetOrdinal("Descrizione")) ? null : reader.GetString(reader.GetOrdinal("Descrizione")),
                                Tipologia = reader.IsDBNull(reader.GetOrdinal("Tipologia")) ? null : reader.GetString(reader.GetOrdinal("Tipologia"))
                            };
                        }
                    }
                }
            }

            return camera;
        }

        public void Add(Camera camera)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var query = @"
                    INSERT INTO Camere (Numero, Descrizione, Tipologia)
                    VALUES (@Numero, @Descrizione, @Tipologia)";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Numero", camera.Numero);
                    cmd.Parameters.AddWithValue("@Descrizione", (object)camera.Descrizione ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Tipologia", (object)camera.Tipologia ?? DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Update(Camera camera)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var query = @"
                    UPDATE Camere SET 
                    Descrizione = @Descrizione, 
                    Tipologia = @Tipologia
                    WHERE Numero = @Numero";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Descrizione", (object)camera.Descrizione ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Tipologia", (object)camera.Tipologia ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Numero", camera.Numero);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int numero)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var query = "DELETE FROM Camere WHERE Numero = @Numero";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Numero", numero);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}