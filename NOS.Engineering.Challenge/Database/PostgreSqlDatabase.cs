using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;
using NOS.Engineering.Challenge.Models;

namespace NOS.Engineering.Challenge.Database
{
    public class PostgreSqlDatabase : IDatabase<ContentDto, ContentDto>
    {
        private readonly string _connectionString;

        public PostgreSqlDatabase(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<ContentDto?> Create(ContentDto item)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "INSERT INTO Contents (Title, SubTitle, Description, ImageUrl, Duration, StartTime, EndTime, GenreList) " +
                        "VALUES (@Title, @SubTitle, @Description, @ImageUrl, @Duration, @StartTime, @EndTime, @GenreList) " +
                        "RETURNING Id";
            using var cmd = new NpgsqlCommand(query, connection);
            cmd.Parameters.AddWithValue("Title", item.Title);
            cmd.Parameters.AddWithValue("SubTitle", item.SubTitle);
            cmd.Parameters.AddWithValue("Description", item.Description);
            cmd.Parameters.AddWithValue("ImageUrl", item.ImageUrl);
            cmd.Parameters.AddWithValue("Duration", item.Duration);
            cmd.Parameters.AddWithValue("StartTime", item.StartTime);
            cmd.Parameters.AddWithValue("EndTime", item.EndTime);
            cmd.Parameters.AddWithValue("GenreList", item.GenreList);

            var id = await cmd.ExecuteScalarAsync();
            if (id != null)
            {
                item.Id = (Guid)id;
                return item;
            }
            return null;
        }

        public async Task<ContentDto?> Read(Guid id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "SELECT Title, SubTitle, Description, ImageUrl, Duration, StartTime, EndTime, GenreList FROM Contents WHERE Id = @Id";
            using var cmd = new NpgsqlCommand(query, connection);
            cmd.Parameters.AddWithValue("Id", id);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new ContentDto
                {
                    Id = id,
                    Title = reader.GetString(0),
                    SubTitle = reader.GetString(1),
                    Description = reader.GetString(2),
                    ImageUrl = reader.GetString(3),
                    Duration = reader.GetInt32(4),
                    StartTime = reader.GetDateTime(5),
                    EndTime = reader.GetDateTime(6),
                    GenreList = reader.GetFieldValue<List<string>>(7)
                };
            }
            return null;
        }

        public async Task<IEnumerable<ContentDto?>> ReadAll()
        {
            var contents = new List<ContentDto?>();

            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "SELECT Id, Title, SubTitle, Description, ImageUrl, Duration, StartTime, EndTime, GenreList FROM Contents";
            using var cmd = new NpgsqlCommand(query, connection);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                contents.Add(new ContentDto
                {
                    Id = reader.GetGuid(0),
                    Title = reader.GetString(1),
                    SubTitle = reader.GetString(2),
                    Description = reader.GetString(3),
                    ImageUrl = reader.GetString(4),
                    Duration = reader.GetInt32(5),
                    StartTime = reader.GetDateTime(6),
                    EndTime = reader.GetDateTime(7),
                    GenreList = reader.GetFieldValue<List<string>>(8)
                });
            }

            return contents;
        }

        public async Task<ContentDto?> Update(Guid id, ContentDto item)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "UPDATE Contents SET Title = @Title, SubTitle = @SubTitle, Description = @Description, " +
                        "ImageUrl = @ImageUrl, Duration = @Duration, StartTime = @StartTime, EndTime = @EndTime, GenreList = @GenreList " +
                        "WHERE Id = @Id";
            using var cmd = new NpgsqlCommand(query, connection);
            cmd.Parameters.AddWithValue("Id", id);
            cmd.Parameters.AddWithValue("Title", item.Title);
            cmd.Parameters.AddWithValue("SubTitle", item.SubTitle);
            cmd.Parameters.AddWithValue("Description", item.Description);
            cmd.Parameters.AddWithValue("ImageUrl", item.ImageUrl);
            cmd.Parameters.AddWithValue("Duration", item.Duration);
            cmd.Parameters.AddWithValue("StartTime", item.StartTime);
            cmd.Parameters.AddWithValue("EndTime", item.EndTime);
            cmd.Parameters.AddWithValue("GenreList", item.GenreList);

            var rowsAffected = await cmd.ExecuteNonQueryAsync();
            if (rowsAffected > 0)
            {
                return await Read(id);
            }
            return null;
        }

        public async Task<Guid> Delete(Guid id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "DELETE FROM Contents WHERE Id = @Id";
            using var cmd = new NpgsqlCommand(query, connection);
            cmd.Parameters.AddWithValue("Id", id);

            var rowsAffected = await cmd.ExecuteNonQueryAsync();
            return rowsAffected > 0 ? id : Guid.Empty;
        }
    }
}
