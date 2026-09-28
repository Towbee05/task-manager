using Dapper;

namespace TaskManager.Repository;

public interface ITaskRepository
{
    Task<Models.Task> CreateTaskAsync(string taskName);
    Task<IEnumerable<Models.Task>> GetAllTasksAsync();
    Task<Models.Task?> GetTaskByIdAsync(Guid id);
    Task<int> MarkTaskAsDoneById(Guid id, DateTime updatedAt);
}

public class TaskRepository : ITaskRepository
{
    private readonly Database.IConnectionFactory _connectionFactory;

    public TaskRepository(Database.IConnectionFactory connection)
    {
        this._connectionFactory = connection;
    }

    public async Task<Models.Task> CreateTaskAsync(string taskName)
    {
        string sql = @"
        INSERT INTO task (name) 
        VALUES (@Name)
        RETURNING id, name, completed, created_at, updated_at;
        ";

        using var connection = this._connectionFactory.CreateConnection();
        return await connection.QuerySingleAsync<Models.Task>(sql, new
        {
            Name = taskName
        });
    }

    public async Task<IEnumerable<Models.Task>> GetAllTasksAsync()
    {
        string sql = @"
        SELECT * FROM task
        ORDER BY created_at;
        ";

        using var connection = this._connectionFactory.CreateConnection();
        return await connection.QueryAsync<Models.Task>(sql, new { });
    }

    public async Task<Models.Task?> GetTaskByIdAsync(Guid id)
    {
        string sql = @"
        SELECT * FROM task
        WHERE id=@Id
        LIMIT 1;
        ";

        using var connection = this._connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<Models.Task>(sql, new
        {
            Id = id
        });
    }

    public async Task<int> MarkTaskAsDoneById(Guid id, DateTime updatedAt)
    {
        string sql = @"
        UPDATE task
        SET completed = true, updated_at = @UpdatedAt
        WHERE id=@Id;
        ";

        using var connection = this._connectionFactory.CreateConnection();
        return await connection.ExecuteAsync(sql, new
        {
            Id = id,
            UpdatedAt = updatedAt
        });

    }
}