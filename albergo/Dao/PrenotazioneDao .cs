using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using albergo.Models;

namespace albergo.DAO
{
    public class PrenotazioneDao : IPrenotazioneDao
    {
        private readonly string _connectionString;

        public PrenotazioneDao(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IEnumerable<Prenotazione> GetAll()
        {
            var prenotazioni = new List<Prenotazione>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = @"
                        SELECT p.*, c.Cognome, c.Nome, cam.Descrizione, cam.Tipologia 
                        FROM Prenotazioni p
                        JOIN Clienti c ON p.ClienteCodiceFiscale = c.CodiceFiscale
                        JOIN Camere cam ON p.NumeroCamera = cam.Numero";

                    using (var command = new SqlCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            prenotazioni.Add(MapPrenotazione(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception (logging framework should be used)
                Console.WriteLine($"Error fetching all prenotazioni: {ex.Message}");
                throw; // Re-throw the exception or handle it as needed
            }

            return prenotazioni;
        }

        public Prenotazione GetById(int id)
        {
            Prenotazione prenotazione = null;

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = @"
                SELECT p.*, c.Cognome, c.Nome, cam.Descrizione, cam.Tipologia 
                FROM Prenotazioni p
                JOIN Clienti c ON p.ClienteCodiceFiscale = c.CodiceFiscale
                JOIN Camere cam ON p.NumeroCamera = cam.Numero
                WHERE p.Id = @Id";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                prenotazione = MapPrenotazione(reader);
                                prenotazione.ServiziPrenotazioni = GetServiziPrenotazioniByPrenotazioneId(id);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error fetching prenotazione by Id: {ex.Message}");
                throw;
            }

            return prenotazione;
        }

        public void Add(Prenotazione prenotazione)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = @"
                        INSERT INTO Prenotazioni (ClienteCodiceFiscale, NumeroCamera, DataPrenotazione, NumeroProgressivo, Anno, Dal, Al, CaparraConfirmatoria, Tariffa, TipoSoggiorno)
                        VALUES (@ClienteCodiceFiscale, @NumeroCamera, @DataPrenotazione, @NumeroProgressivo, @Anno, @Dal, @Al, @CaparraConfirmatoria, @Tariffa, @TipoSoggiorno);
                        SELECT SCOPE_IDENTITY();";

                    using (var command = new SqlCommand(query, connection))
                    {
                        MapParameters(command, prenotazione);
                        prenotazione.Id = Convert.ToInt32(command.ExecuteScalar());
                    }

                    if (prenotazione.ServiziPrenotazioni != null && prenotazione.ServiziPrenotazioni.Count > 0)
                    {
                        foreach (var servizioPrenotazione in prenotazione.ServiziPrenotazioni)
                        {
                            AddServizioPrenotazione(servizioPrenotazione);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error adding prenotazione: {ex.Message}");
                throw;
            }
        }

        public void Update(Prenotazione prenotazione)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = @"
                        UPDATE Prenotazioni
                        SET ClienteCodiceFiscale = @ClienteCodiceFiscale,
                            NumeroCamera = @NumeroCamera,
                            DataPrenotazione = @DataPrenotazione,
                            NumeroProgressivo = @NumeroProgressivo,
                            Anno = @Anno,
                            Dal = @Dal,
                            Al = @Al,
                            CaparraConfirmatoria = @CaparraConfirmatoria,
                            Tariffa = @Tariffa,
                            TipoSoggiorno = @TipoSoggiorno
                        WHERE Id = @Id";

                    using (var command = new SqlCommand(query, connection))
                    {
                        MapParameters(command, prenotazione);
                        command.Parameters.AddWithValue("@Id", prenotazione.Id);
                        command.ExecuteNonQuery();
                    }

                    // Update associated services
                    UpdateServiziPrenotazioni(prenotazione.Id, prenotazione.ServiziPrenotazioni);
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error updating prenotazione: {ex.Message}");
                throw;
            }
        }

        public void Delete(int id)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Delete associated services
                    var deleteServiziQuery = "DELETE FROM ServiziPrenotazioni WHERE PrenotazioneId = @Id";
                    using (var command = new SqlCommand(deleteServiziQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        command.ExecuteNonQuery();
                    }

                    // Delete the prenotazione
                    var deleteQuery = "DELETE FROM Prenotazioni WHERE Id = @Id";
                    using (var command = new SqlCommand(deleteQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error deleting prenotazione: {ex.Message}");
                throw;
            }
        }

        public int GetLastId()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = "SELECT ISNULL(MAX(Id), 0) FROM Prenotazioni";

                    using (var command = new SqlCommand(query, connection))
                    {
                        return (int)command.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error fetching last prenotazione Id: {ex.Message}");
                throw;
            }
        }

        public IEnumerable<Prenotazione> GetPrenotazioniByCodiceFiscale(string codiceFiscale)
        {
            var prenotazioni = new List<Prenotazione>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = @"
                        SELECT p.*, c.Cognome, c.Nome, cam.Descrizione, cam.Tipologia 
                        FROM Prenotazioni p
                        JOIN Clienti c ON p.ClienteCodiceFiscale = c.CodiceFiscale
                        JOIN Camere cam ON p.NumeroCamera = cam.Numero
                        WHERE c.CodiceFiscale = @CodiceFiscale";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CodiceFiscale", codiceFiscale);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var prenotazione = MapPrenotazione(reader);
                                prenotazione.ServiziPrenotazioni = GetServiziPrenotazioniByPrenotazioneId(prenotazione.Id);
                                prenotazioni.Add(prenotazione);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error fetching prenotazioni by codice fiscale: {ex.Message}");
                throw;
            }

            return prenotazioni;
        }

        public int GetTotalePrenotazioniPerTipologia(string tipologiaSoggiorno)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = @"
                        SELECT COUNT(*)
                        FROM Prenotazioni
                        WHERE TipoSoggiorno = @TipoSoggiorno";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@TipoSoggiorno", tipologiaSoggiorno);
                        return (int)command.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error fetching total prenotazioni per tipologia: {ex.Message}");
                throw;
            }
        }

        private void AddServizioPrenotazione(ServizioPrenotazione servizioPrenotazione)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = @"
                        INSERT INTO ServiziPrenotazioni (PrenotazioneId, ServizioId, Data, Quantita, Prezzo)
                        VALUES (@PrenotazioneId, @ServizioId, @Data, @Quantita, @Prezzo)";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@PrenotazioneId", servizioPrenotazione.PrenotazioneId);
                        command.Parameters.AddWithValue("@ServizioId", servizioPrenotazione.ServizioId);
                        command.Parameters.AddWithValue("@Data", servizioPrenotazione.Data);
                        command.Parameters.AddWithValue("@Quantita", servizioPrenotazione.Quantita);
                        command.Parameters.AddWithValue("@Prezzo", servizioPrenotazione.Prezzo);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error adding servizio prenotazione: {ex.Message}");
                throw;
            }
        }

        private void UpdateServiziPrenotazioni(int prenotazioneId, List<ServizioPrenotazione> serviziPrenotazioni)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Elimina i servizi esistenti
                    var deleteQuery = "DELETE FROM ServiziPrenotazioni WHERE PrenotazioneId = @PrenotazioneId";
                    using (var command = new SqlCommand(deleteQuery, connection))
                    {
                        command.Parameters.AddWithValue("@PrenotazioneId", prenotazioneId);
                        command.ExecuteNonQuery();
                    }

                    // Aggiungi nuovi servizi
                    if (serviziPrenotazioni != null && serviziPrenotazioni.Count > 0)
                    {
                        foreach (var servizioPrenotazione in serviziPrenotazioni)
                        {
                            AddServizioPrenotazione(servizioPrenotazione);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error updating servizi prenotazioni: {ex.Message}");
                throw;
            }
        }

        private List<ServizioPrenotazione> GetServiziPrenotazioniByPrenotazioneId(int prenotazioneId)
        {
            var serviziPrenotazioni = new List<ServizioPrenotazione>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = @"
                        SELECT sp.*, s.Nome, s.PrezzoUnitario 
                        FROM ServiziPrenotazioni sp
                        JOIN Servizi s ON sp.ServizioId = s.Id
                        WHERE sp.PrenotazioneId = @PrenotazioneId";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@PrenotazioneId", prenotazioneId);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var servizioPrenotazione = new ServizioPrenotazione
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    PrenotazioneId = reader.GetInt32(reader.GetOrdinal("PrenotazioneId")),
                                    ServizioId = reader.GetInt32(reader.GetOrdinal("ServizioId")),
                                    Data = reader.GetDateTime(reader.GetOrdinal("Data")),
                                    Quantita = reader.GetInt32(reader.GetOrdinal("Quantita")),
                                    Prezzo = reader.GetDecimal(reader.GetOrdinal("Prezzo")),
                                    Servizio = new Servizio
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("ServizioId")),
                                        Nome = reader.GetString(reader.GetOrdinal("Nome")),
                                        PrezzoUnitario = reader.GetDecimal(reader.GetOrdinal("PrezzoUnitario"))
                                    }
                                };
                                serviziPrenotazioni.Add(servizioPrenotazione);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error fetching servizi prenotazioni by prenotazione Id: {ex.Message}");
                throw;
            }

            return serviziPrenotazioni;
        }

        private Prenotazione MapPrenotazione(SqlDataReader reader)
        {
            return new Prenotazione
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                ClienteCodiceFiscale = reader.GetString(reader.GetOrdinal("ClienteCodiceFiscale")),
                NumeroCamera = reader.GetInt32(reader.GetOrdinal("NumeroCamera")),
                DataPrenotazione = reader.GetDateTime(reader.GetOrdinal("DataPrenotazione")),
                NumeroProgressivo = reader.GetInt32(reader.GetOrdinal("NumeroProgressivo")),
                Anno = reader.GetInt32(reader.GetOrdinal("Anno")),
                Dal = reader.GetDateTime(reader.GetOrdinal("Dal")),
                Al = reader.GetDateTime(reader.GetOrdinal("Al")),
                CaparraConfirmatoria = reader.IsDBNull(reader.GetOrdinal("CaparraConfirmatoria")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("CaparraConfirmatoria")),
                Tariffa = reader.GetDecimal(reader.GetOrdinal("Tariffa")),
                TipoSoggiorno = reader.GetString(reader.GetOrdinal("TipoSoggiorno")),
                Cliente = new Cliente
                {
                    CodiceFiscale = reader.GetString(reader.GetOrdinal("ClienteCodiceFiscale")),
                    Cognome = reader.GetString(reader.GetOrdinal("Cognome")),
                    Nome = reader.GetString(reader.GetOrdinal("Nome"))
                },
                Camera = new Camera
                {
                    Numero = reader.GetInt32(reader.GetOrdinal("NumeroCamera")),
                    Descrizione = reader.GetString(reader.GetOrdinal("Descrizione")),
                    Tipologia = reader.GetString(reader.GetOrdinal("Tipologia"))
                }
            };
        }

        private void MapParameters(SqlCommand command, Prenotazione prenotazione)
        {
            command.Parameters.AddWithValue("@ClienteCodiceFiscale", prenotazione.ClienteCodiceFiscale);
            command.Parameters.AddWithValue("@NumeroCamera", prenotazione.NumeroCamera);
            command.Parameters.AddWithValue("@DataPrenotazione", prenotazione.DataPrenotazione);
            command.Parameters.AddWithValue("@NumeroProgressivo", prenotazione.NumeroProgressivo);
            command.Parameters.AddWithValue("@Anno", prenotazione.Anno);
            command.Parameters.AddWithValue("@Dal", prenotazione.Dal);
            command.Parameters.AddWithValue("@Al", prenotazione.Al);
            command.Parameters.AddWithValue("@CaparraConfirmatoria", (object)prenotazione.CaparraConfirmatoria ?? DBNull.Value);
            command.Parameters.AddWithValue("@Tariffa", prenotazione.Tariffa);
            command.Parameters.AddWithValue("@TipoSoggiorno", prenotazione.TipoSoggiorno);
        }
    }
}
