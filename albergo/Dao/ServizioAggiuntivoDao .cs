using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using albergo.Models;

namespace albergo.DAO
{
    public class ServizioDao : IServizioAggiuntivoDao
    {
        private readonly string _connectionString;

        public ServizioDao(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IEnumerable<ServizioAggiuntivo> GetAll()
        {
            var servizi = new List<ServizioAggiuntivo>();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    const string query = "SELECT * FROM Servizi";

                    using (var cmd = new SqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            servizi.Add(MapServizio(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception (use a logging framework)
                Console.WriteLine($"Error fetching all servizi: {ex.Message}");
                throw;
            }

            return servizi;
        }

        public ServizioAggiuntivo GetById(int id)
        {
            ServizioAggiuntivo servizio = null;

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    const string query = "SELECT * FROM Servizi WHERE id = @id";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@id", System.Data.SqlDbType.Int).Value = id;
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                servizio = MapServizio(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error fetching servizio by Id: {ex.Message}");
                throw;
            }

            return servizio;
        }

        public void Add(ServizioAggiuntivo servizio)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    const string query = @"
                        INSERT INTO Servizi (descrizione, prezzo)
                        VALUES (@descrizione, @prezzo)";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        MapParameters(cmd, servizio);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error adding servizio: {ex.Message}");
                throw;
            }
        }

        public void Update(ServizioAggiuntivo servizio)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    const string query = @"
                        UPDATE Servizi 
                        SET descrizione = @descrizione, prezzo = @prezzo
                        WHERE id = @id";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        MapParameters(cmd, servizio);
                        cmd.Parameters.Add("@id", System.Data.SqlDbType.Int).Value = servizio.Id;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error updating servizio: {ex.Message}");
                throw;
            }
        }

        public void Delete(int id)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    const string query = "DELETE FROM Servizi WHERE id = @id";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@id", System.Data.SqlDbType.Int).Value = id;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error deleting servizio: {ex.Message}");
                throw;
            }
        }

        public IEnumerable<ServizioAggiuntivo> GetByPrenotazioneId(int prenotazioneId)
        {
            var servizi = new List<ServizioAggiuntivo>();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    const string query = @"
                        SELECT s.*
                        FROM Servizi s
                        JOIN Prenotazioni_Servizi ps ON s.id = ps.servizio_id
                        WHERE ps.prenotazione_id = @prenotazione_id";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@prenotazione_id", System.Data.SqlDbType.Int).Value = prenotazioneId;
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                servizi.Add(MapServizio(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error fetching servizi by prenotazione Id: {ex.Message}");
                throw;
            }

            return servizi;
        }

        private static ServizioAggiuntivo MapServizio(SqlDataReader reader)
        {
            return new ServizioAggiuntivo
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Descrizione = reader.GetString(reader.GetOrdinal("descrizione")),
                Prezzo = reader.GetDecimal(reader.GetOrdinal("prezzo"))
            };
        }

        private static void MapParameters(SqlCommand cmd, ServizioAggiuntivo servizio)
        {
            cmd.Parameters.Add("@descrizione", System.Data.SqlDbType.NVarChar).Value = servizio.Descrizione;
            cmd.Parameters.Add("@prezzo", System.Data.SqlDbType.Decimal).Value = servizio.Prezzo;
        }
    }
}
