using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using tick_tack_toe.Models;

namespace tick_tack_toe.Controllers
{
    [Route("api/v1/[controller]")]
    public class GameController : ControllerBase
    {
        private readonly IMongoCollection<Game> db;
        private string[] field = new string[9];
        private int move;
        public GameController()
        {
            var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
            var dbName = Environment.GetEnvironmentVariable("DB_NAME");
            var connectionString = $"mongodb://{dbHost}:27017/{dbName}";

            var mongoUrl = MongoUrl.Create(connectionString);
            var mongoClient = new MongoClient(mongoUrl);
            var database = mongoClient.GetDatabase(mongoUrl.DatabaseName);
            db = database.GetCollection<Game>("game");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Game>>> GetAllGames()
        {
            return await db.Find(Builders<Game>.Filter.Empty).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Game>> GetGameById(string id)
        {
            var game = Builders<Game>.Filter.Eq(g => g.GameId, id);
            return await db.Find(game).SingleOrDefaultAsync();
        }

        [HttpPost]
        [Route("create")]
        public async Task<ActionResult> CreateGame()
        {
            Game game = new();
            field = new string[9];
            move = 0;
            game.Move = move;
            game.Field = field;
            game.Status = Game_Status.In_Game.ToString();
            await db.InsertOneAsync(game);
            return Ok();
        }

        [HttpPut]
        [Route("move")]
        public async Task<ActionResult> MakeMove(string gameId, int fieldNum)
        {
            if (gameId.Length != 24)
            {
                return BadRequest("Игра не найдена");
            }

            var gameFilter = Builders<Game>.Filter.Eq(g => g.GameId, gameId);
            var game = await db.Find(gameFilter).SingleOrDefaultAsync();

            if(game == null)
            {
                return BadRequest("Игра не найдена");
            }

            if(game.Status == Game_Status.X_Win.ToString() || game.Status == Game_Status.O_Win.ToString() || game.Status == Game_Status.Draw.ToString())
            {
                return BadRequest("Игра закончена");
            }

            if (fieldNum < 0 || fieldNum > 8)
            {
                return BadRequest("Вы вышли за диапазон");
            }

            field = game.Field;

            if (field[fieldNum] != null)
            {
                return BadRequest("Клетка занята");
            }
            else
            {
                field = game.Field;
                move = game.Move;
                if(move%2 == 0)
                {
                    field[fieldNum] = "X";
                }
                else
                {
                    field[fieldNum] = "O";
                }

                move++;
                if(move == 9)
                {
                    game.Status = Game_Status.Draw.ToString();
                }
                else
                {
                    CheckWinCondition(field, out string result);
                    if(result == "X win")
                    {
                        game.Status = Game_Status.X_Win.ToString();
                    }
                    if(result == "O win")
                    {
                        game.Status = Game_Status.O_Win.ToString();
                    }
                }
                game.Move = move;
                game.Field = field;
                await db.ReplaceOneAsync(gameFilter, game);
                return Ok(); 
            }
        }
        private void CheckWinCondition(string[] field, out string result)
        {
            result = "In game";
            // проверка по горизонтали
            if ((field[0] != null) && ((field[0] == field[1]) && (field[1] == field[2])))
            {
                result = $"{field[0]} win";
            }
            if ((field[3] != null) && ((field[3] == field[4]) && (field[4] == field[5])))
            {
                result = $"{field[3]} win";
            }
            if ((field[6] != null) && ((field[6] == field[7]) && (field[7] == field[8])))
            {
                result = $"{field[6]} win";
            }

            //проверка по вертикали
            if ((field[0] != null) && ((field[0] == field[3]) && (field[3] == field[6])))
            {
                result = $"{field[0]} win";
            }
            if ((field[1] != null) && ((field[1] == field[4]) && (field[4] == field[7])))
            {
                result = $"{field[1]} win";
            }
            if ((field[2] != null) && ((field[2] == field[5]) && (field[5] == field[8])))
            {
                result = $"{field[2]} win";
            }

            // проверка по диагонали
            if ((field[0] != null) && ((field[0] == field[4]) && (field[4] == field[8])))
            {
                result = $"{field[0]} win";
            }
            if ((field[2] != null) && ((field[2] == field[4]) && (field[4] == field[6])))
            {
                result = $"{field[2]} win";
            }
        }
    }
}
public enum Game_Status
{
    In_Game,
    X_Win,
    O_Win,
    Draw
}



