using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using albergo.Models;

namespace Albergo.DAO
{
    public class ClienteDao : IClienteDao
    {
        private readonly string _connectionString;

        public ClienteDao(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IEnumerable<Cliente> GetAll()
        {
            var clienti = new List<Cliente>();

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var query = "SELECT * FROM Clienti";
                using (var cmd = new SqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var cliente = new Cliente
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            CodiceFiscale = reader.GetString(reader.GetOrdinal("codice_fiscale")),
                            Cognome = reader.GetString(reader.GetOrdinal("cognome")),
                            Nome = reader.GetString(reader.GetOrdinal("nome")),
                            Citta = reader.GetString(reader.GetOrdinal("città")),
                            Provincia = reader.GetString(reader.GetOrdinal("provincia")),
                            Email = reader.GetString(reader.GetOrdinal("email")),
                            Cellulare = reader.GetString(reader.GetOrdinal("cellulare"))
                        };

                        clienti.Add(cliente);
                    }
                }
            }

            return clienti;
        }

        public Cliente GetById(int id)
        {
            Cliente cliente = null;

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var query = "SELECT * FROM Clienti WHERE id = @id";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            cliente = new Cliente
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                CodiceFiscale = reader.GetString(reader.GetOrdinal("codice_fiscale")),
                                Cognome = reader.GetString(reader.GetOrdinal("cognome")),
                                Nome = reader.GetString(reader.GetOrdinal("nome")),
                                Citta = reader.GetString(reader.GetOrdinal("città")),
                                Provincia = reader.GetString(reader.GetOrdinal("provincia")),
                                Email = reader.GetString(reader.GetOrdinal("email")),
                                Cellulare = reader.GetString(reader.GetOrdinal("cellulare"))
                            };
                        }
                    }
                }
            }

            return cliente;
        }

        public void Add(Cliente cliente)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var query = @"
                    INSERT INTO Clienti (codice_fiscale, cognome, nome, città, provincia, email, cellulare)
                    VALUES (@codice_fiscale, @cognome, @nome, @città, @provincia, @email, @cellulare)";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@codice_fiscale", cliente.CodiceFiscale);
                    cmd.Parameters.AddWithValue("@cognome", cliente.Cognome);
                    cmd.Parameters.AddWithValue("@nome", cliente.Nome);
                    cmd.Parameters.AddWithValue("@città", cliente.Citta);
                    cmd.Parameters.AddWithValue("@provincia", cliente.Provincia);
                    cmd.Parameters.AddWithValue("@email", cliente.Email);
                    cmd.Parameters.AddWithValue("@cellulare", cliente.Cellulare);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Update(Cliente cliente)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var query = @"
                    UPDATE Clienti SET 
                    codice_fiscale = @codice_fiscale, 
                    cognome = @cognome, 
                    nome = @nome, 
                    città = @città, 
                    provincia = @provincia, 
                    email = @email, 
                    cellulare = @cellulare
                    WHERE id = @id";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@codice_fiscale", cliente.CodiceFiscale);
                    cmd.Parameters.AddWithValue("@cognome", cliente.Cognome);
                    cmd.Parameters.AddWithValue("@nome", cliente.Nome);
                    cmd.Parameters.AddWithValue("@città", cliente.Citta);
                    cmd.Parameters.AddWithValue("@provincia", cliente.Provincia);
                    cmd.Parameters.AddWithValue("@email", cliente.Email);
                    cmd.Parameters.AddWithValue("@cellulare", cliente.Cellulare);
                    cmd.Parameters.AddWithValue("@id", cliente.Id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var query = "DELETE FROM Clienti WHERE id = @id";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}