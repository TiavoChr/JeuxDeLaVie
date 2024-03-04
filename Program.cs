using System;
using System.Threading;
using MySql.Data.MySqlClient;

class Program
{
    static void Main(string[] args)
    {
        string connStr = "server=localhost;user=root;database=GameOfLifeDB;password=";
        MySqlConnection conn = new MySqlConnection(connStr);

        try
        {
            Console.WriteLine("Connecting to MySQL...");
            conn.Open();

            // Test de la connexion
            Console.WriteLine("Connected to MySQL.");
            Console.WriteLine("Login :");
            string login = Console.ReadLine();
            Console.WriteLine("Password :");
            string Passwd = Console.ReadLine();

            if(AuthenticateUser(conn, login, Passwd)) {
                Console.WriteLine("User connected");
                
                int width = 10;
                int height = 10;
                int[,] grid = new int[height, width];
                int[,] newGrid = new int[height, width];

                // Initialisation aléatoire de la grille
                Random rand = new Random();
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        grid[y, x] = rand.Next(2); // Remplit la grille aléatoirement avec des 0 et des 1
                    }
                }

                while (true)
                {
                    Console.Clear();
                    DrawGrid(grid);

                    // Appliquer les règles du jeu de la vie
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            int aliveNeighbors = CountAliveNeighbors(grid, x, y);

                            if (grid[y, x] == 1 && (aliveNeighbors < 2 || aliveNeighbors > 3))
                            {
                                newGrid[y, x] = 0; // La cellule meurt
                            }
                            else if (grid[y, x] == 0 && aliveNeighbors == 3)
                            {
                                newGrid[y, x] = 1; // La cellule naît
                            }
                            else
                            {
                                newGrid[y, x] = grid[y, x]; // La cellule reste dans son état actuel
                            }
                        }
                    }

                    // Mettre à jour la grille avec la nouvelle configuration
                    Array.Copy(newGrid, grid, newGrid.Length);

                    // Attente avant la prochaine itération
                    Thread.Sleep(500); // Modifier le délai selon votre préférence
                }
            }
            else
            {
                Console.WriteLine("Authentication failed!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        finally
        {
            conn.Close();
            Console.WriteLine("Connection closed.");
        }

        // int width = 10;
        // int height = 10;
        // int[,] grid = new int[height, width];
        // int[,] newGrid = new int[height, width];

        // // Initialisation aléatoire de la grille
        // Random rand = new Random();
        // for (int y = 0; y < height; y++)
        // {
        //     for (int x = 0; x < width; x++)
        //     {
        //         grid[y, x] = rand.Next(2); // Remplit la grille aléatoirement avec des 0 et des 1
        //     }
        // }

        // while (true)
        // {
        //     Console.Clear();
        //     DrawGrid(grid);

        //     // Appliquer les règles du jeu de la vie
        //     for (int y = 0; y < height; y++)
        //     {
        //         for (int x = 0; x < width; x++)
        //         {
        //             int aliveNeighbors = CountAliveNeighbors(grid, x, y);

        //             if (grid[y, x] == 1 && (aliveNeighbors < 2 || aliveNeighbors > 3))
        //             {
        //                 newGrid[y, x] = 0; // La cellule meurt
        //             }
        //             else if (grid[y, x] == 0 && aliveNeighbors == 3)
        //             {
        //                 newGrid[y, x] = 1; // La cellule naît
        //             }
        //             else
        //             {
        //                 newGrid[y, x] = grid[y, x]; // La cellule reste dans son état actuel
        //             }
        //         }
        //     }

        //     // Mettre à jour la grille avec la nouvelle configuration
        //     Array.Copy(newGrid, grid, newGrid.Length);

        //     // Attente avant la prochaine itération
        //     Thread.Sleep(500); // Modifier le délai selon votre préférence
        // }
    }

    static void DrawGrid(int[,] grid)
    {
        for (int y = 0; y < grid.GetLength(0); y++)
        {
            for (int x = 0; x < grid.GetLength(1); x++)
            {
                Console.Write(grid[y, x] == 1 ? "█" : " "); // Utilisation du caractère plein pour une cellule vivante
            }
            Console.WriteLine();
        }
    }

    static int CountAliveNeighbors(int[,] grid, int x, int y)
    {
        int count = 0;
        int width = grid.GetLength(1);
        int height = grid.GetLength(0);

        for (int yOffset = -1; yOffset <= 1; yOffset++)
        {
            for (int xOffset = -1; xOffset <= 1; xOffset++)
            {
                int neighborX = x + xOffset;
                int neighborY = y + yOffset;

                // Vérifier si le voisin est dans la grille et n'est pas la cellule en cours d'évaluation
                if (neighborX >= 0 && neighborX < width && neighborY >= 0 && neighborY < height
                    && !(xOffset == 0 && yOffset == 0))
                {
                    count += grid[neighborY, neighborX];
                }
            }
        }

        return count;
    }

    static void CreateUser(MySqlConnection conn, string username, string password)
    {
        string sql = $"INSERT INTO Users (Username, Password) VALUES ('{username}', '{password}')";
        MySqlCommand cmd = new MySqlCommand(sql, conn);
        cmd.ExecuteNonQuery();
    }

    static bool AuthenticateUser(MySqlConnection conn, string username, string password)
    {
        string sql = $"SELECT COUNT(*) FROM Users WHERE Username = '{username}' AND Password = '{password}'";
        MySqlCommand cmd = new MySqlCommand(sql, conn);
        int count = Convert.ToInt32(cmd.ExecuteScalar());
        return count == 1;
    }
}